using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ControladorUICombate : MonoBehaviour
{
    private Animator transition;

    [Header("Referencias")]
    public List<Button> botonesDeAtaque;
    public GameObject panelDeAccionesDelJugador;
    public GameObject panelMovs;
    public Button botonMochila;

    void Start()
    {
        transition = GetComponentInChildren<Animator>();
        AsignarAtaquesABotones();

        if (panelMovs != null)
        {
            panelMovs.SetActive(false);
        }

        // Asignar función al botón de mochila
        if (botonMochila != null)
        {
            botonMochila.onClick.AddListener(AbrirMochila);
        }
    }

    void AsignarAtaquesABotones()
    {
        if (GestorDeBatalla.instance == null || GestorDeBatalla.instance.porkemonJugador == null)
        {
            Debug.LogError("No se encontró el Gestor de Batalla o el Porkemon del jugador.");
            return;
        }

        List<AtaqueData> ataques = GestorDeBatalla.instance.porkemonJugador.Ataques;

        for (int i = 0; i < botonesDeAtaque.Count; i++)
        {
            if (i < ataques.Count)
            {
                AtaqueData ataqueActual = ataques[i];
                botonesDeAtaque[i].gameObject.SetActive(true);

                TextMeshProUGUI tmpText = botonesDeAtaque[i].GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    tmpText.text = ataqueActual.nombreAtaque;
                }
                else
                {
                    Text textoBoton = botonesDeAtaque[i].GetComponentInChildren<Text>();
                    if (textoBoton != null)
                    {
                        textoBoton.text = ataqueActual.nombreAtaque;
                    }
                    else
                    {
                        Debug.LogWarning($"El botón {i} no tiene un componente Text o TextMeshProUGUI.");
                    }
                }

                botonesDeAtaque[i].onClick.RemoveAllListeners();
                botonesDeAtaque[i].onClick.AddListener(() => SeleccionarAtaque(ataqueActual));
            }
            else
            {
                botonesDeAtaque[i].gameObject.SetActive(false);
            }
        }
    }

    void SeleccionarAtaque(AtaqueData ataque)
    {
        if (GestorDeBatalla.instance.porkemonJugador.puedeAtacar)
        {
            GameState.ataqueSeleccionado = ataque;
            GestorDeBatalla.instance.porkemonJugador.puedeAtacar = false;

            if (panelMovs != null)
            {
                panelMovs.SetActive(false);
            }
            if (panelDeAccionesDelJugador != null)
            {
                panelDeAccionesDelJugador.SetActive(true);
            }

            VolverAlCombate();
        }
    }

    public void VolverAlCombate()
    {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void IrAtaques()
    {
        if (panelDeAccionesDelJugador != null)
        {
            panelDeAccionesDelJugador.SetActive(false);
        }
        if (panelMovs != null)
        {
            panelMovs.SetActive(true);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AbrirMochila()
    {
        // Establecer flag para indicar que vamos a modo mochila
        GameState.player1Turn = false; // Usar como flag temporal

        // Cargar escena de mochila (usando la escena de cambio como base por ahora)
        // En el futuro, crear una escena dedicada "Escena Mochila"
        SceneTransitionManager.Instance.LoadScene("Escena CambioPorkemon");
    }

    public IEnumerator SceneLoad()
    {
        transition.SetTrigger("StartTransition");
        yield return new WaitForSeconds(1f);
    }
}
