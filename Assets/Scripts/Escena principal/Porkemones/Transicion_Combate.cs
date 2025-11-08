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

    public PorkemonData botPorkemonData;
    public int nivelSpawn;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (botPorkemonData == null)
            {
                return;
            }

            if (GameState.porkemonDelBot != null && GameState.porkemonDelBot.VidaActual <= 0)
                {
                    popupText.gameObject.SetActive(true);
                    popupText.text = "Este Porkemon está debilitado. No puedes iniciar un combate.";
                    popupText.gameObject.SetActive(true);
                    popupText.text = "El Porkemon del bot está debilitado. No puedes iniciar un combate.";
                return;
            }
            
            GameState.porkemonDelBot = new Porkemon(botPorkemonData, nivelSpawn);

            // Set the bot's Pokémon based on this spawned one
            GameState.porkemonDelBot = new Porkemon(botPorkemonData, botPorkemonData.nivel);
            Cursor.visible = true;
        }
    }

    private IEnumerator DesactivarTexto(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (popupText != null)
        {
            popupText.gameObject.SetActive(false);
        }
    }
}