using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorVictoria : MonoBehaviour
{
    [Header("UI")]
    public Text textoVictoria;

    void Start()
    {
        if (textoVictoria != null)
        {
            if (GameState.victoriaFueCaptura)
            {
                textoVictoria.text = $"¡Has capturado a {GameState.nombreGanador}!";
            }
            else if (!string.IsNullOrEmpty(GameState.nombreGanador))
            {
                textoVictoria.text = $"¡Tu {GameState.nombreGanador} ha Ganado!";
            }
            else
            {
                textoVictoria.text = "¡Has Ganado!";
            }
        }

    }
}
