using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// OBSOLETO: Reemplazado por NavegadorEscenas.cs que unifica este script con CargarEscPrincipal.
/// Para migrar: reemplaza este componente por NavegadorEscenas en el Inspector.
/// </summary>
[System.Obsolete("Usar NavegadorEscenas.cs en su lugar")]
public class CargarEscena : MonoBehaviour
{
    public void CargarMenu()
    {
        SceneTransitionManager.Instance.LoadScene("Interfaz de Menu");
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