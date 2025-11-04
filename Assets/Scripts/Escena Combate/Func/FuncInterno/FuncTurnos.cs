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
            StartCoroutine(RutinaAtaqueBot());
        }
    }

    private void Update()
    {
        if (isPlayer1Turn && GameState.ataqueSeleccionado != null && enCombate && !ConsolaEnJuego.instance.isTyping)
        {
            StartCoroutine(RutinaAtaqueJugador());
        }
    }

    private IEnumerator RutinaAtaqueJugador()
    {
        enCombate = false; 
        AtaqueData ataque = GameState.ataqueSeleccionado;
        GameState.ataqueSeleccionado = null;

        Debug.Log($"{jugador1.porkemon.BaseData.nombre} usa {ataque.nombreAtaque}.");
        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);

        jugador1.porkemon.UsarAtaqueElemental(jugador2.porkemon, ataque);
        bool debilitado = jugador2.porkemon.VidaActual <= 0;
        
        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
        
        jugador1.porkemon.AplicarDanioPorEstado();
        jugador2.porkemon.AplicarDanioPorEstado();
        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);

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
        
        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
        yield return new WaitForSeconds(0.5f);

        if (jugador2.porkemon.Ataques.Count > 0)
        {
            int indiceAtaque = Random.Range(0, jugador2.porkemon.Ataques.Count);
            AtaqueData ataqueDelBot = jugador2.porkemon.Ataques[indiceAtaque];
            
            Debug.Log($"Turno del Bot. {jugador2.porkemon.BaseData.nombre} usa {ataqueDelBot.nombreAtaque}.");
            yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);

            jugador2.porkemon.UsarAtaqueElemental(jugador1.porkemon, ataqueDelBot);
            bool debilitado = jugador1.porkemon.VidaActual <= 0;

            yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
            
            jugador1.porkemon.AplicarDanioPorEstado();
            jugador2.porkemon.AplicarDanioPorEstado();
            yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);

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

    public void CambiarTurno()
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
            Debug.Log("El Pokémon del jugador ha sido derrotado. Intentando cargar Escena de muerte.");

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

                SceneTransitionManager.Instance.LoadScene("Escena de Victoria");
            }
        }
    }

    public bool PuedeCombatir()
    {
        return jugador1.porkemon.VidaActual > 0 && jugador2.porkemon.VidaActual > 0;
    }
}