using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CargarEscena : MonoBehaviour
{
    public void CargarMenu()
    {
        SceneManager.LoadScene("Interfaz de Menu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CargaCambio()
    {
        SceneManager.LoadScene("Escena CambioPorkemon");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}