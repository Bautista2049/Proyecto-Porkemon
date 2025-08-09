using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GestorDeBatalla : MonoBehaviour
{
    public static GestorDeBatalla instance;

    // Los datos de los porkemon que persistirán entre escenas
    public Porkemon porkemonJugador;
    public Porkemon porkemonBot;

    // Las plantillas para crear los porkemon al inicio del juego
    public PorkemonData dataInicialJugador;
    public PorkemonData dataInicialBot;

    private void Awake()
    {
        // --- Patrón Singleton ---
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            // Creamos los porkemon la primera vez
            porkemonJugador = new Porkemon(dataInicialJugador, dataInicialJugador.nivel);
            porkemonBot = new Porkemon(dataInicialBot, dataInicialBot.nivel);
        }
        else
        {
            // Si ya existe una instancia, nos destruimos para que solo haya una.
            Destroy(gameObject);
        }
    }
}