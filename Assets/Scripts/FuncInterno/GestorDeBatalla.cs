using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GestorDeBatalla : MonoBehaviour
{
    public static GestorDeBatalla instance;

    public List<Porkemon> equipoJugador = new List<Porkemon>();
    public Porkemon porkemonBot;
    public Porkemon porkemonDelBot;

    [Header("Datos Iniciales del Jugador")]
    public List<PorkemonData> dataEquipoJugador; 
    
    [Header("Datos del Bot")]
    public PorkemonData dataInicialBot;

    public bool combateIniciado = false;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            ResetearCombate();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetearCombate()
    {
        equipoJugador.Clear();

        foreach (var data in dataEquipoJugador)
        {
            Porkemon nuevo = new Porkemon(data, data.nivel);
            equipoJugador.Add(nuevo);
            if (equipoJugador.Count >= 6) break;
        }

        if (equipoJugador.Count > 0)
        {
            GameState.porkemonDelJugador = equipoJugador[0];
        }

        if (GameState.porkemonDelBot == null)
        {
            porkemonBot = new Porkemon(dataInicialBot, dataInicialBot.nivel);
            GameState.porkemonDelBot = porkemonBot;
        }
        else
        {
            porkemonBot = GameState.porkemonDelBot;
        }

        combateIniciado = false;
    }

    public Porkemon GetPorkemonActivoJugador()
    {
        return GameState.porkemonDelJugador;
    }

    public void CambiarPorkemonActivo(Porkemon nuevo)
    {
        if (nuevo == null || nuevo.VidaActual <= 0) return;

        GameState.porkemonDelJugador = nuevo;
        Debug.Log($"Ahora el Pokémon activo del jugador es {nuevo.BaseData.nombre}");
    }

    public Porkemon porkemonJugador
    {
        get { return GameState.porkemonDelJugador; }
        set { GameState.porkemonDelJugador = value; }
    }

}
