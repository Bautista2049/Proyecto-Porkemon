using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class FuncTurnos : MonoBehaviour
{
    [Header("Controladores de Porkemon")]
    public ControladorPorkemon jugador1;
    public ControladorPorkemon jugador2;

    [Header("UI de Turnos")]
    public GameObject turnoJ1Prefab;
    public GameObject turnoBotPrefab;
    public GameObject panelDeAccionesDelJugador;
    
    private bool isPlayer1Turn;
    private bool enCombate = true;

    private void Start()
    {
        jugador1.Setup(GestorDeBatalla.instance.porkemonJugador);
        jugador2.Setup(GestorDeBatalla.instance.porkemonBot);

        if (!GestorDeBatalla.instance.combateIniciado)
        {
            isPlayer1Turn = jugador1.porkemon.Velocidad >= jugador2.porkemon.Velocidad;
            GestorDeBatalla.instance.combateIniciado = true;
        }
        else
        {
            isPlayer1Turn = GameState.player1Turn;
        }

        ActualizarUI();

        if (!isPlayer1Turn)
        {
            StartCoroutine(RutinaAtaqueBot());
        }
    }

    private void Update()
    {
        if (isPlayer1Turn && GameState.ataqueSeleccionado != null && enCombate)
        {
            StartCoroutine(RutinaAtaqueJugador());
        }
    }

    private IEnumerator RutinaAtaqueJugador()
    {
        enCombate = false;
        AtaqueData ataque = GameState.ataqueSeleccionado;
        GameState.ataqueSeleccionado = null;

        bool debilitado = jugador2.RecibirDanio(ataque, jugador1.porkemon);

        yield return new WaitForSeconds(1.5f);

        if (debilitado)
        {
            VerificarFinCombate();
        }
        else
        {
            CambiarTurno();
        }
        enCombate = true;
    }

    private IEnumerator RutinaAtaqueBot()
    {
        enCombate = false;
        yield return new WaitForSeconds(1.5f);

        if (jugador2.porkemon.Ataques.Count > 0)
        {
            int indiceAtaque = Random.Range(0, jugador2.porkemon.Ataques.Count);
            AtaqueData ataqueDelBot = jugador2.porkemon.Ataques[indiceAtaque];
            
            bool debilitado = jugador1.RecibirDanio(ataqueDelBot, jugador2.porkemon);

            if (debilitado)
            {
                VerificarFinCombate();
            }
            else
            {
                CambiarTurno();
            }
        }
        enCombate = true;
    }

    private void CambiarTurno()
    {
        isPlayer1Turn = !isPlayer1Turn;
        GameState.player1Turn = isPlayer1Turn;
        ActualizarUI();

        if (!isPlayer1Turn && PuedeCombatir())
        {
            StartCoroutine(RutinaAtaqueBot());
        }
    }

    private void ActualizarUI()
    {
        turnoJ1Prefab?.SetActive(isPlayer1Turn);
        turnoBotPrefab?.SetActive(!isPlayer1Turn);
        panelDeAccionesDelJugador?.SetActive(isPlayer1Turn);

        if (jugador1.porkemon != null)
        {
            jugador1.porkemon.puedeAtacar = isPlayer1Turn;
        }
    }

    private void VerificarFinCombate()
    {
        if (jugador1.porkemon.VidaActual <= 0)
        {
            SceneManager.LoadScene("Escena de muerte");
        }
        else if (jugador2.porkemon.VidaActual <= 0)
        {
            GameState.nombreGanador = jugador1.porkemon.BaseData.nombre;
            SceneManager.LoadScene("Escena de Victoria");
        }
    }

    public bool PuedeCombatir()
    {
        return jugador1.porkemon.VidaActual > 0 && jugador2.porkemon.VidaActual > 0;
    }
}