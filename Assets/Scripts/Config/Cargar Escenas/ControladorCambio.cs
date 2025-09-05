using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ControladorCambio : MonoBehaviour
{
    [Header("UI")]
    public List<Button> botonesPokemon;
    public TextMeshProUGUI tituloTexto;

    private List<Porkemon> equipoJugador;

    void Start()
    {
        equipoJugador = GestorDeBatalla.instance.equipoJugador;
        ActualizarBotones();
        tituloTexto.text = $"Tienes {equipoJugador.Count} Pokémon";
    }

    void ActualizarBotones()
    {
        for (int i = 0; i < botonesPokemon.Count; i++)
        {
            if (i < equipoJugador.Count)
            {
                Porkemon p = equipoJugador[i];
                botonesPokemon[i].gameObject.SetActive(true);

                TextMeshProUGUI texto = botonesPokemon[i].GetComponentInChildren<TextMeshProUGUI>();
                if (texto != null)
                    texto.text = $"{p.BaseData.nombre} ({p.VidaActual}/{p.VidaMaxima})";

                botonesPokemon[i].onClick.RemoveAllListeners();
                botonesPokemon[i].onClick.AddListener(() => SeleccionarPorkemon(p));

                botonesPokemon[i].interactable = p.VidaActual > 0;
            }
            else
            {
                botonesPokemon[i].gameObject.SetActive(false);
            }
        }
    }

    public void SeleccionarPorkemon(Porkemon nuevo)
    {
        if (nuevo == null || nuevo.VidaActual <= 0) return;

        GestorDeBatalla.instance.porkemonJugador = nuevo;
        GameState.porkemonDelJugador = nuevo;

        Debug.Log($"Cambiaste a {nuevo.BaseData.nombre}. Turno gastado.");

        GameState.player1Turn = false; 

        SceneManager.LoadScene("Escena de combate");
    }
}
