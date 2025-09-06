using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CargarEscPrincipal : MonoBehaviour
{
    public void CargarPrincipal()
    {
        SceneTransitionManager.Instance.LoadScene("Escena Principal");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void CargaCambio()
    {
        SceneTransitionManager.Instance.LoadScene("Escena CambioPorkemon");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}