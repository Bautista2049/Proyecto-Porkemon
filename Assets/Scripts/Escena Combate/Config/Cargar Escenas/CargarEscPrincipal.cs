using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CargarEscPrincipal : MonoBehaviour
{
    public void CargarPrincipal()
    {
        SceneTransitionManager.GetInstance().LoadScene("Escena Principal");
    }
    
    public void CargaCambio()
    {
        SceneTransitionManager.Instance.LoadScene("Escena CambioPorkemon");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void VolverAUltEscena()
    {
        SceneTransitionManager.Instance.ReturnToLastScene();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GuardarJuego()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("EscenaGuardada", escenaActual);
        PlayerPrefs.Save();
    }

    public void CargarUltimaVersion()
    {
        // Leer la última escena guardada. Si no hay, usamos Escena Principal.
        string escenaGuardada = PlayerPrefs.GetString("EscenaGuardada", "");

        if (string.IsNullOrEmpty(escenaGuardada))
        {
            escenaGuardada = "Escena Principal";
        }

        SceneTransitionManager.GetInstance().LoadScene(escenaGuardada);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }
}
