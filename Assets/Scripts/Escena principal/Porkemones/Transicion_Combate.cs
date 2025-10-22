using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Transicion_Combate : MonoBehaviour
{
    [SerializeField] private string nombreEscena = "Escena de Combate" ;

    [Header("Porkemon Data")]
    public PorkemonData botPorkemonData;
    public int nivelSpawn = 5;

    private void OnCollisionEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (botPorkemonData == null)
            {
                return;
            }

            if (GameState.porkemonDelBot != null && GameState.porkemonDelBot.VidaActual <= 0)
            {
                Debug.Log("El Porkemon del bot está debilitado. No puedes iniciar un combate.");
                return;
            }

            
            GameState.porkemonDelBot = new Porkemon(botPorkemonData, nivelSpawn);

            SceneManager.LoadScene(nombreEscena);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    
}
