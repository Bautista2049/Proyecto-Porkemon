using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ControladorCambio : MonoBehaviour
{
    [Header("UI")]
    public List<Button> botonesPokemon;
    public TextMeshProUGUI tituloTexto;

    private List<Porkemon> equipoJugador;

    void Start()
    {
        Debug.Log("ControladorCambio Start - Iniciando");
        if (GestorDeBatalla.instance == null)
        {
            Debug.LogError("GestorDeBatalla.instance es null");
            return;
        }
        equipoJugador = GestorDeBatalla.instance.equipoJugador;
        Debug.Log($"EquipoJugador count: {equipoJugador.Count}");
        ActualizarBotones();
        tituloTexto.text = $"Tienes {equipoJugador.Count} Pokémon";
        Debug.Log("ControladorCambio Start - Completado");
    }
     

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
                    // Disable button or visually mark as active
                    botonesPokemon[i].interactable = false;
                    botonesPokemon[i].onClick.RemoveAllListeners();
                    // Optionally change color or add icon here
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
        Debug.Log($"OnButtonClicked called with index {index}");
        if (index < 0 || index >= equipoJugador.Count)
        {
            Debug.LogWarning($"Index {index} is out of range. Equipo count: {equipoJugador.Count}");
            return;
        }

        Porkemon selected = equipoJugador[index];
        Debug.Log($"Button clicked for {selected.BaseData.nombre}");
        SeleccionarPorkemon(selected);
    }

    public void SeleccionarPorkemon(Porkemon nuevo)
    {
        Debug.Log($"SeleccionarPorkemon called with {nuevo?.BaseData.nombre ?? "null"}");
        if (nuevo == null || nuevo.VidaActual <= 0)
        {
            Debug.LogWarning("Porkemon is null or has no health");
            return;
        }

        GestorDeBatalla.instance.porkemonJugador = nuevo;
        GameState.porkemonDelJugador = nuevo;

        Debug.Log($"New active Porkemon: {GestorDeBatalla.instance.porkemonJugador.BaseData.nombre}");
        Debug.Log($"Cambiaste a {nuevo.BaseData.nombre}. Turno gastado.");

        GameState.player1Turn = false;

        SceneTransitionManager.Instance.LoadScene("Escena de combate");
    }
}
