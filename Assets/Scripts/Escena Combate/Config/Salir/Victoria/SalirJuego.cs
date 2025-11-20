using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class SalirJuego : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelNotificacion;
    public TextMeshProUGUI textoNotificacion;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneTransitionManager.Instance.LoadScene("Interfaz de Menu");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void CerrarJuego()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void GuardarEstado()
    {
        try
        {
            
            PlayerPrefs.SetInt("Player1Turn", GameState.player1Turn ? 1 : 0);
            PlayerPrefs.SetInt("DineroJugador", GameState.dineroJugador);
            
            if (GameState.porkemonDelJugador != null)
            {
                string porkemonJugadorJson = JsonUtility.ToJson(GameState.porkemonDelJugador);
                PlayerPrefs.SetString("PorkemonJugador", porkemonJugadorJson);
            }
            
            if (GameState.porkemonDelBot != null)
            {
                string porkemonBotJson = JsonUtility.ToJson(GameState.porkemonDelBot);
                PlayerPrefs.SetString("PorkemonBot", porkemonBotJson);
            }
            
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            PlayerPrefs.SetString("UltimoGuardado", timestamp);
            
            PlayerPrefs.Save();
            
            MostrarNotificacion($"Estado guardado correctamente\n{timestamp} en {Application.persistentDataPath}");
        }
        catch (Exception e)
        {
            MostrarNotificacion($"Error al guardar: {e.Message}");
        }
    }

    public void CargarEstado()
    {
        try
        {
            if (PlayerPrefs.HasKey("UltimoGuardado"))
            {
                GameState.player1Turn = PlayerPrefs.GetInt("Player1Turn", 1) == 1;
                GameState.dineroJugador = PlayerPrefs.GetInt("DineroJugador", 0);
                
                if (PlayerPrefs.HasKey("PorkemonJugador"))
                {
                    string porkemonJugadorJson = PlayerPrefs.GetString("PorkemonJugador");
                    GameState.porkemonDelJugador = JsonUtility.FromJson<Porkemon>(porkemonJugadorJson);
                }
                
                if (PlayerPrefs.HasKey("PorkemonBot"))
                {
                    string porkemonBotJson = PlayerPrefs.GetString("PorkemonBot");
                    GameState.porkemonDelBot = JsonUtility.FromJson<Porkemon>(porkemonBotJson);
                }
                
                string timestamp = PlayerPrefs.GetString("UltimoGuardado");
                MostrarNotificacion($"Estado cargado correctamente\nÚltimo guardado: {timestamp}");
            }
            else
            {
                MostrarNotificacion("No hay estado guardado");
            }
        }
        catch (Exception e)
        {
            MostrarNotificacion($"Error al cargar: {e.Message}");
        }
    }

    private void MostrarNotificacion(string mensaje)
    {
        if (textoNotificacion != null)
        {
            textoNotificacion.text = mensaje;
        }
        
        if (panelNotificacion != null)
        {
            panelNotificacion.SetActive(true);
            StartCoroutine(OcultarNotificacion());
        }
    }

    private IEnumerator OcultarNotificacion()
    {
        yield return new WaitForSeconds(3f);
        if (panelNotificacion != null)
        {
            panelNotificacion.SetActive(false);
        }
    }
}
