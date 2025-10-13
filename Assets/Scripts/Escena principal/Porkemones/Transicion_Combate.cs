using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Transicion_Combate : MonoBehaviour
{
    [SerializeField] private string nombreEscena = "Escena de Combate" ;
    [SerializeField] private GameObject popupPanel; 
    [SerializeField] private TextMeshProUGUI popupText; 

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            if (GameState.porkemonDelBot != null && GameState.porkemonDelBot.VidaActual <= 0)
            {
                Debug.Log("¡Qué raro! Este Pokémon ya está debilitado.");
                MostrarPopup("¡Qué raro! Este Pokémon ya está debilitado.");
                return;
            }

            SceneManager.LoadScene(nombreEscena);
            Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        }
    }

    private void MostrarPopup(string mensaje)
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
            if (popupText != null)
            {
                popupText.text = mensaje;
            }
            
            StartCoroutine(OcultarPopup(3f));
        }
    }

    private IEnumerator OcultarPopup(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }
}
