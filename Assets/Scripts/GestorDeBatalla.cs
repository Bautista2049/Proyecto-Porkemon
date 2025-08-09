using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GestorDeBatalla : MonoBehaviour
{
    public static GestorDeBatalla instance;

    public Porkemon porkemonJugador;
    public Porkemon porkemonBot;

    public PorkemonData dataInicialJugador;
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
        porkemonJugador = new Porkemon(dataInicialJugador, dataInicialJugador.nivel);
        porkemonBot = new Porkemon(dataInicialBot, dataInicialBot.nivel);
        combateIniciado = false;
    }
}