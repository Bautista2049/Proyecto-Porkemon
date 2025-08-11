﻿using System.Collections;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Interfaz de Menu");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void CerrarJuego()
    {
        Application.Quit();

        // Para que también funcione al probarlo en el Editor de Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void GuardarEstado()
    {
        try
        {
            // Guardar estado actual
            PlayerPrefs.SetInt("Player1Turn", GameState.player1Turn ? 1 : 0);
            
            // Guardar datos de pokemones si existen
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
            
            // Guardar timestamp
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            PlayerPrefs.SetString("UltimoGuardado", timestamp);
            
            PlayerPrefs.Save();
            
            // Mostrar notificación con fecha actual
            MostrarNotificacion($"Estado guardado correctamente\n{timestamp}");
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
                // Cargar turno
                GameState.player1Turn = PlayerPrefs.GetInt("Player1Turn", 1) == 1;
                
                // Cargar pokemones
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
