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

            if (GameState.porkemonDelBot != null &&
                GameState.porkemonDelBot.BaseData == this.botPorkemonData &&
                GameState.porkemonDelBot.VidaActual <= 0)
            {
                if (popupText != null)
                {
                    popupText.gameObject.SetActive(true);
                    popupText.text = "Este Porkemon está debilitado. No puedes iniciar un combate.";
                    StartCoroutine(DesactivarTexto(3f));
                }
                return;
            }

            GameState.porkemonDelBot = new Porkemon(botPorkemonData, nivelSpawn);
            if (GestorDeBatalla.instance != null)
            {
                GestorDeBatalla.instance.combateIniciado = false;
            }

            // Make the main camera persist across scene loads
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                DontDestroyOnLoad(mainCamera.gameObject);
            }

            SceneManager.LoadScene(nombreEscena);
            Cursor.lockState = CursorLockMode.None;
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
