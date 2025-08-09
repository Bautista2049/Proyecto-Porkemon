using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorUICombate : MonoBehaviour
{
    [Header("Referencias (Arrastrar desde la Escena)")]
    public List<Button> botonesDeAtaque; // Arrastra tus 4 botones aquí

    void Start()
    {
        AsignarAtaquesABotones();
    }

    void AsignarAtaquesABotones()
    {
        if (GestorDeBatalla.instance == null || GestorDeBatalla.instance.porkemonJugador == null)
        {
            Debug.LogError("No se encontró el Gestor de Batalla o el Porkemon del jugador.");
            return;
        }

        List<AtaqueData> ataques = GestorDeBatalla.instance.porkemonJugador.Ataques;

        // Recorrer los 4 botones
        for (int i = 0; i < botonesDeAtaque.Count; i++)
        {
            // Si hay un ataque para este botón
            if (i < ataques.Count)
            {
                AtaqueData ataqueActual = ataques[i];
                botonesDeAtaque[i].gameObject.SetActive(true);

                // Asignar nombre al texto del botón
                Text textoBoton = botonesDeAtaque[i].GetComponentInChildren<Text>();
                if (textoBoton != null)
                {
                    textoBoton.text = ataqueActual.nombreAtaque;
                }

                // Limpiar listeners viejos y añadir el nuevo
                botonesDeAtaque[i].onClick.RemoveAllListeners();
                botonesDeAtaque[i].onClick.AddListener(() => SeleccionarAtaque(ataqueActual));
            }
            else // Si no hay ataque para este botón, desactivarlo
            {
                botonesDeAtaque[i].gameObject.SetActive(false);
            }
        }
    }

    void SeleccionarAtaque(AtaqueData ataque)
    {
        Debug.Log($"El jugador usa {ataque.nombreAtaque}");

        Porkemon jugador = GestorDeBatalla.instance.porkemonJugador;
        Porkemon bot = GestorDeBatalla.instance.porkemonBot;

        if (jugador == null || bot == null)
        {
            Debug.LogError("No se encontraron los datos del jugador o del bot en GameState.");
            return;
        }

        int danio = ataque.poder;
        bot.VidaActual -= danio;

        // Cambiamos el turno para que sea el del bot
        GameState.player1Turn = false;

        // Ya no necesitamos guardar el ataque, porque ya lo usamos.
        GameState.ataqueSeleccionado = null;

        VolverAlCombate();
    }

    public void VolverAlCombate()
    {
        
        SceneManager.LoadScene("Escena de combate");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void IrAtaques()
    {
        
        
        SceneManager.LoadScene("Luchar Escena");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    
}