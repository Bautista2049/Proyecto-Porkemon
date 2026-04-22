using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script unificado de navegación entre escenas.
/// Fusiona CargarEscena.cs y CargarEscPrincipal.cs que tenían métodos duplicados.
/// </summary>
public class NavegadorEscenas : MonoBehaviour
{
    public void CargarPrincipal()
    {
        SceneTransitionManager.GetInstance().LoadScene("Escena Principal");
    }

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

    public void RecargarEscenaBoss()
    {
        if (GameState.esCombateBoss)
        {
            GameState.porkemonDelBot = new Porkemon(GameState.porkemonDelBot.BaseData, GameState.porkemonDelBot.Nivel);
            
            if (GestorDeBatalla.instance != null)
            {
                GestorDeBatalla.instance.combateIniciado = false;
            }
            
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Object.DontDestroyOnLoad(mainCamera.gameObject);
            }
            
            SceneTransitionManager.Instance.LoadScene("EscenaCombateBoss");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
