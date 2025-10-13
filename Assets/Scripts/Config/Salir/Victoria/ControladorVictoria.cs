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
            if (!string.IsNullOrEmpty(GameState.nombreGanador))
            {
                textoVictoria.text = $"¡Tu {GameState.nombreGanador} ha Ganado!";
            }
            else
            {
                textoVictoria.text = "¡Has Ganado!";
            }
        }

        // No longer auto-return to main menu - experience animation will handle this
        // StartCoroutine(RegresarAlMenuPrincipal());
    }

    // Removed the auto-return coroutine since experience controller handles scene transition
}
