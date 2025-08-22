using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegresarCombate : MonoBehaviour
{
   public void CargarCombate()
    {
        SceneManager.LoadScene("Escena de Combate");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
