using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ataques : MonoBehaviour
{
    public void CargarCombate()
    {
        SceneManager.LoadScene("Luchar Escena");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
