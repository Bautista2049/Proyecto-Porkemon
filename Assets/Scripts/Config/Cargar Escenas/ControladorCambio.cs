﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections; // Necesario para la corrutina si deseas animación de retorno

public class ControladorCambio : MonoBehaviour
{
    [Header("UI")]
    public List<Button> botonesPokemon;
    public TextMeshProUGUI tituloTexto;

    private List<Porkemon> equipoJugador;
    private const string EscenaCombate = "Escena de combate"; // Nombre de tu escena de combate

    void Start()
    {
        if (GestorDeBatalla.instance == null)
        {
            Debug.LogError("GestorDeBatalla.instance es null");
            return;
        }
        equipoJugador = GestorDeBatalla.instance.equipoJugador;
        ActualizarBotones();
        tituloTexto.text = $"Tienes {equipoJugador.Count} Pokémon";
    }
     
    // ... (Métodos ActualizarBotones y OnButtonClicked sin cambios) ...

    void ActualizarBotones()
    {
        Porkemon activo = GestorDeBatalla.instance.porkemonJugador;

        for (int i = 0; i < botonesPokemon.Count; i++)
        {
            if (i < equipoJugador.Count)
            {
                Porkemon p = equipoJugador[i];
                botonesPokemon[i].gameObject.SetActive(true);

                TextMeshProUGUI texto = botonesPokemon[i].GetComponentInChildren<TextMeshProUGUI>();
                if (texto != null)
                    texto.text = $"{p.BaseData.nombre} ({p.VidaActual}/{p.VidaMaxima})";

                if (p == activo)
                {
                    botonesPokemon[i].interactable = false;
                    botonesPokemon[i].onClick.RemoveAllListeners();
                }
                else
                {
                    botonesPokemon[i].interactable = p.VidaActual > 0;
                    botonesPokemon[i].onClick.RemoveAllListeners();
                    int index = i; // Capture for closure
                    botonesPokemon[i].onClick.AddListener(() => OnButtonClicked(index));
                }
            }
            else
            {
                botonesPokemon[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnButtonClicked(int index)
    {
        if (index < 0 || index >= equipoJugador.Count)
        {
            Debug.LogWarning($"Index {index} is out of range. Equipo count: {equipoJugador.Count}");
            return;
        }

        Porkemon selected = equipoJugador[index];
        SeleccionarPorkemon(selected);
    }

    public void SeleccionarPorkemon(Porkemon nuevo)
    {
        if (nuevo == null || nuevo.VidaActual <= 0)
        {
            Debug.LogWarning("Porkemon is null or has no health");
            return;
        }

        // 1. Actualizar el estado global
        GestorDeBatalla.instance.porkemonJugador = nuevo;
        GameState.porkemonDelJugador = nuevo;

        Debug.Log($"Cambiaste a {nuevo.BaseData.nombre}. Turno gastado.");

        // 2. Ceder el turno
        GameState.player1Turn = false;

        // 3. ACTUALIZAR MODELO/UI Y VOLVER A LA ESCENA DE COMBATE SIN RECARGAR
        // *** CAMBIO CLAVE A IMPLEMENTAR POR TI ***
        // Necesitas un script global que maneje la descarga/activación de escenas.
        
        // --- Lógica de Retorno ---
        
        // Simulación: Llamar a la función de retorno (esto depende de tu SceneTransitionManager)
        StartCoroutine(RetornarYActualizar());
    }
    
    // Función para manejar la actualización y la vuelta a la escena de combate
    private IEnumerator RetornarYActualizar()
    {
        // 1. Volver a la escena de combate
        // Asumiendo que esta escena (la de UI de cambio) se llama "Escena de Cambio" o similar
        // Y que la escena de combate sigue cargada aditivamente o está desactivada.
        
        // Opción 1: Si usas SceneManager.LoadScene(string, LoadSceneMode.Single)
        // SceneManager.LoadScene(EscenaCombate); 
        
        // Opción 2: Si usas SceneTransitionManager para cargar/descargar aditivamente.
        // Asumiremos que el Transicion_Combate.cs ya ha cargado Escena de Combate.
        
        // Desactivar la escena actual o descargarla
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name); 
        
        // Nota: Si usas UnloadScene, asegúrate de que la escena de combate esté activa/visible.
        // Si no puedes usar UnloadSceneAsync, usa SceneManager.LoadScene(EscenaCombate); para recargarla.
        
        // Al regresar o recargar la escena de combate, los scripts DynamicBotModel.Start() y FuncTurnos.Start()
        // se ejecutarán y leerán el nuevo valor de GameState.porkemonDelJugador.

        yield return null; // Pequeña espera para asegurar la descarga/carga

        // NOTA IMPORTANTE: Si la escena de combate ya estaba cargada y solo la desactivaste,
        // ¡DEBES ACTIVARLA Y LUEGO LLAMAR A FUNCIONAR!
        
        // Dado que recargar la escena es la opción más simple que tenías antes:
        SceneTransitionManager.Instance.LoadScene(EscenaCombate); 
    }
    
    public void CancelarCambio()
    {
        // Esto solo vuelve a la escena de combate sin hacer el cambio
        SceneTransitionManager.Instance.LoadScene(EscenaCombate);
    }
}