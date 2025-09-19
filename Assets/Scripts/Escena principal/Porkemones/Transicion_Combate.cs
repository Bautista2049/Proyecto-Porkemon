using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transicion_Combate : MonoBehaviour
{
    [SerializeField] private string nombreEscena = "Escena de Combate" ; // pone aca el nombre de la escena a la que queres ir

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player")) // tu personaje tiene que tener el tag "Player"
        {
            SceneManager.LoadScene(nombreEscena);
            Debug.Log("aaaaaaaa");
        }
    }
}
