using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CargarEscPrincipal : MonoBehaviour
{
    public void CargarPrincipal()
    {
        SceneManager.LoadScene("Escena Principal");
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