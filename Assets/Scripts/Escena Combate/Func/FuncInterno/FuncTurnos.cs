﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FuncTurnos : MonoBehaviour
{
    public ControladorPorkemon jugador1;
    public ControladorPorkemon jugador2;
    public DynamicBotModel playerModelManager;
    public DynamicBotModel botModelManager;
    public GameObject pokebolaPrefab;
    public Transform posicionLanzamientoJugador;
    public Transform posicionBotPorkemon;
    public ParticleSystem particulaCapturaPrefab;
    public ParticleSystem particulaFalloPrefab;
    public AudioClip audioSacudida;
    public AudioClip audioCaptura;
    public AudioClip audioFallo;
    public GameObject turnoJ1Prefab;
    public GameObject turnoBotPrefab;
    public GameObject panelDeAccionesDelJugador;

    private bool isPlayer1Turn;
    private bool enCombate = true;
    private bool corutinaEnEjecucion = false;

    private void Start()
    {
        if (GestorDeBatalla.instance != null)
            GestorDeBatalla.instance.IniciarBatalla();

        jugador1.Setup(GestorDeBatalla.instance.porkemonJugador);
        jugador2.Setup(GestorDeBatalla.instance.porkemonBot);

        if (playerModelManager && jugador1.porkemon != null)
            playerModelManager.UpdateModel(jugador1.porkemon.BaseData.nombre);

        if (botModelManager && jugador2.porkemon != null)
            botModelManager.UpdateModel(jugador2.porkemon.BaseData.nombre);

        if (!GestorDeBatalla.instance.combateIniciado)
        {
            isPlayer1Turn = jugador1.porkemon.Velocidad >= jugador2.porkemon.Velocidad;
            GestorDeBatalla.instance.combateIniciado = true;
        }
        else
            isPlayer1Turn = GameState.player1Turn;

        ActualizarUI();

        if (!isPlayer1Turn)
            StartCoroutine(RutinaAtaqueBot());
    }

    private void Update()
    {
        if (!corutinaEnEjecucion && isPlayer1Turn && GameState.ataqueSeleccionado != null && enCombate && !ConsolaEnJuego.instance.isTyping)
            StartCoroutine(RutinaAtaqueJugador());
        else if (!corutinaEnEjecucion && isPlayer1Turn && GameState.itemSeleccionado != null && enCombate && !ConsolaEnJuego.instance.isTyping)
            StartCoroutine(RutinaLanzarPorkebola());
    }

    private IEnumerator RutinaAtaqueJugador()
    {
        corutinaEnEjecucion = true;
        enCombate = false;
        AtaqueData ataque = GameState.ataqueSeleccionado;
        GameState.ataqueSeleccionado = null;

        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
        jugador1.porkemon.UsarAtaqueElemental(jugador2.porkemon, ataque);
        bool debilitado = jugador2.porkemon.VidaActual <= 0;

        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
        jugador1.porkemon.AplicarDanioPorEstado();
        jugador2.porkemon.AplicarDanioPorEstado();

        if (debilitado) VerificarFinCombate();
        else CambiarTurno();

        enCombate = true;
        corutinaEnEjecucion = false;
    }

    private IEnumerator RutinaAtaqueBot()
    {
        corutinaEnEjecucion = true;
        enCombate = false;

        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
        yield return new WaitForSeconds(0.5f);

        if (jugador2.porkemon.Ataques.Count > 0)
        {
            int indiceAtaque = Random.Range(0, jugador2.porkemon.Ataques.Count);
            AtaqueData ataqueDelBot = jugador2.porkemon.Ataques[indiceAtaque];

            jugador2.porkemon.UsarAtaqueElemental(jugador1.porkemon, ataqueDelBot);
            bool debilitado = jugador1.porkemon.VidaActual <= 0;

            yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
            jugador1.porkemon.AplicarDanioPorEstado();
            jugador2.porkemon.AplicarDanioPorEstado();

            if (debilitado) VerificarFinCombate();
            else CambiarTurno();
        }

        enCombate = true;
        corutinaEnEjecucion = false;
    }

    public void CambiarTurno()
    {
        isPlayer1Turn = !isPlayer1Turn;
        GameState.player1Turn = isPlayer1Turn;
        ActualizarUI();

        if (!isPlayer1Turn && PuedeCombatir())
            StartCoroutine(RutinaAtaqueBot());
    }

    private void ActualizarUI()
    {
        turnoJ1Prefab?.SetActive(isPlayer1Turn);
        turnoBotPrefab?.SetActive(!isPlayer1Turn);
        panelDeAccionesDelJugador?.SetActive(isPlayer1Turn);

        if (jugador1.porkemon != null)
            jugador1.porkemon.puedeAtacar = isPlayer1Turn;
    }

    private void VerificarFinCombate()
    {
        if (jugador1.porkemon.VidaActual <= 0)
            SceneTransitionManager.Instance.LoadScene("Escena de muerte");
        else if (jugador2.porkemon.VidaActual <= 0)
        {
            GameState.nombreGanador = jugador1.porkemon.BaseData.nombre;
            GameState.experienciaGanada = GestorDeBatalla.instance.equipoJugador[0].CalcularExperienciaGanada(jugador2.porkemon, true);
            GameState.equipoGanador = new List<Porkemon>(GestorDeBatalla.instance.equipoJugador);
            SceneTransitionManager.Instance.LoadScene("Escena de Victoria");
        }
    }

    public bool PuedeCombatir()
    {
        return jugador1.porkemon.VidaActual > 0 && jugador2.porkemon.VidaActual > 0;
    }

    private float GetBallMultiplier(BattleItemType type)
    {
        switch (type)
        {
            case BattleItemType.Porkebola: return 1f;
            case BattleItemType.Superbola: return 1.5f;
            case BattleItemType.Ultrabola: return 2f;
            case BattleItemType.Masterbola: return 255f;
            default: return 1f;
        }
    }

    private IEnumerator RutinaLanzarPorkebola()
    {
        corutinaEnEjecucion = true;
        enCombate = false;

        BattleItem bolaUsada = GameState.itemSeleccionado;
        GameState.itemSeleccionado = null;
        Porkemon porkemonSalvaje = jugador2.porkemon;

        bolaUsada.cantidad--;
        if (bolaUsada.cantidad <= 0)
            GestorDeBatalla.instance.inventarioBattleItems.Remove(bolaUsada);

        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);

        GameObject pokebolaInstancia = Instantiate(pokebolaPrefab, posicionLanzamientoJugador.position, posicionLanzamientoJugador.rotation);
        AudioSource audioSource = pokebolaInstancia.GetComponent<AudioSource>();

        float duracionLanzamiento = 0.75f;
        float tiempoPasado = 0f;
        Vector3 posInicial = posicionLanzamientoJugador.position;

        while (tiempoPasado < duracionLanzamiento)
        {
            tiempoPasado += Time.deltaTime;
            pokebolaInstancia.transform.position = Vector3.Lerp(posInicial, posicionBotPorkemon.position, tiempoPasado / duracionLanzamiento);
            yield return null;
        }

        botModelManager.PlayExitAnimation();
        yield return new WaitForSeconds(0.5f);

        float multiplier = GetBallMultiplier(bolaUsada.type);
        ResultadoCaptura resultado = CalculadorCaptura.IntentarCaptura(porkemonSalvaje, multiplier);
        Animator bolaAnimator = pokebolaInstancia.GetComponent<Animator>();

        if (!resultado.esCritica)
        {
            for (int i = 0; i < resultado.sacudidas; i++)
            {
                if (bolaAnimator != null) bolaAnimator.SetTrigger("Sacudida");
                if (audioSource && audioSacudida) audioSource.PlayOneShot(audioSacudida);
                yield return new WaitForSeconds(1.5f);
            }
        }

        if (resultado.capturado)
        {
            if (particulaCapturaPrefab)
            {
                var p = Instantiate(particulaCapturaPrefab, pokebolaInstancia.transform.position, Quaternion.identity);
                Destroy(p.gameObject, 3f);
            }
            if (audioSource && audioCaptura) audioSource.PlayOneShot(audioCaptura);
            yield return new WaitForSeconds(2f);
            Destroy(pokebolaInstancia);
            GestorDeBatalla.instance.PokemonCapturado(porkemonSalvaje);
        }
        else
        {
            if (particulaFalloPrefab)
            {
                var p = Instantiate(particulaFalloPrefab, pokebolaInstancia.transform.position, Quaternion.identity);
                Destroy(p.gameObject, 3f);
            }
            if (audioSource && audioFallo) audioSource.PlayOneShot(audioFallo);
            Destroy(pokebolaInstancia);
            botModelManager.PlayEnterAnimation();
            yield return new WaitForSeconds(1f);
            CambiarTurno();
        }

        enCombate = true;
        corutinaEnEjecucion = false;
    }
}
