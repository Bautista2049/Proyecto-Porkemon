﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class FuncTurnos : MonoBehaviour
{
    public ControladorPorkemon jugador1;
    public ControladorPorkemon jugador2;

    public GameObject turnoJ1Prefab;
    public GameObject turnoBotPrefab;
    public GameObject panelDeAccionesDelJugador;
    
    private bool isPlayer1Turn;
    private bool enCombate = true;

    private void Start()
    {
        if (GestorDeBatalla.instance != null)
        {
            GestorDeBatalla.instance.IniciarBatalla();
        }

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
            StartCoroutine(RutinaTurnoBotCompleto());
        }
    }

    private void Update()
    {
        if (!enCombate) return;
        
        if (isPlayer1Turn && GameState.ataqueSeleccionado != null)
        {
            ContinuarTurnoJugador();
        }

        if (jugador1.porkemon.VidaActual <= 0 || jugador2.porkemon.VidaActual <= 0)
        {
            enCombate = false;
            VerificarFinCombate();
        }
    }

    private void ContinuarTurnoJugador()
    {
        if (jugador1.porkemon.UsarAtaqueElemental(jugador2.porkemon, GameState.ataqueSeleccionado))
        {
            StartCoroutine(RutinaFinTurno());
        }
        else
        {
            StartCoroutine(RutinaFinTurno()); 
        }

        GameState.ataqueSeleccionado = null;
    }

    private IEnumerator RutinaTurnoBotCompleto()
    {
        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
        yield return new WaitForSeconds(0.5f); 

        AtaqueData ataqueBot = SeleccionarAtaqueBot(jugador2.porkemon, jugador1.porkemon);
        
        string mensaje = $"Turno del Bot. {jugador2.porkemon.BaseData.nombre} usa {ataqueBot.nombreAtaque}.";
        Debug.Log(mensaje); 

        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
        yield return new WaitForSeconds(0.5f); 

        if (jugador2.porkemon.UsarAtaqueElemental(jugador1.porkemon, ataqueBot))
        {
        }
        else
        {
        }

        StartCoroutine(RutinaFinTurno());
    }

    private IEnumerator RutinaFinTurno()
    {
        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping); 

        jugador1.porkemon.AplicarDanioPorEstado();
        jugador2.porkemon.AplicarDanioPorEstado();

        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping); 

        jugador1.ActualizarUI();
        jugador2.ActualizarUI();

        if (jugador1.porkemon.VidaActual <= 0 || jugador2.porkemon.VidaActual <= 0)
        {
            enCombate = false;
            VerificarFinCombate();
            yield break;
        }

        isPlayer1Turn = !isPlayer1Turn;
        GameState.player1Turn = isPlayer1Turn;
        ActualizarUI();

        if (!isPlayer1Turn)
        {
            StartCoroutine(RutinaTurnoBotCompleto());
        }
    }
    
    private AtaqueData SeleccionarAtaqueBot(Porkemon atacante, Porkemon defensor)
    {
        AtaqueData mejorAtaque = atacante.Ataques.FirstOrDefault(a => CalculadorDanioElemental.CalcularEfectividad(a.tipo, defensor.BaseData.tipo1, defensor.BaseData.tipo2) > 1f && a.pp > 0);
        
        if (mejorAtaque == null)
        {
            mejorAtaque = atacante.Ataques.FirstOrDefault(a => a.pp > 0);
        }

        return mejorAtaque;
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
            if (!ConsolaEnJuego.instance.isTyping)
            {
                SceneTransitionManager.Instance.LoadScene("Escena de muerte");
            }
        }
        else if (jugador2.porkemon.VidaActual <= 0)
        {
            if (!ConsolaEnJuego.instance.isTyping)
            {
                GameState.nombreGanador = jugador1.porkemon.BaseData.nombre;

                GameState.experienciaGanada = GestorDeBatalla.instance.equipoJugador[0].CalcularExperienciaGanada(jugador2.porkemon, true);
                GameState.equipoGanador = new List<Porkemon>(GestorDeBatalla.instance.equipoJugador);

                SceneTransitionManager.Instance.LoadScene("Escena de victoria");
            }
        }
    }
}