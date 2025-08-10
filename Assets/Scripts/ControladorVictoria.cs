using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        StartCoroutine(RegresarAlMenuPrincipal());
    }

    private IEnumerator RegresarAlMenuPrincipal()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Escena Principal");
    }
}