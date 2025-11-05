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

    [Header("Controladores de Modelos")]
    public DynamicBotModel playerModelManager;
    public DynamicBotModel botModelManager;

    [Header("Efectos de Captura")]
    public GameObject pokebolaPrefab; // Tu prefab 3D de la Pokebola
    public Transform posicionLanzamientoJugador; // Un 'Empty' en la posición del jugador
    public Transform posicionBotPorkemon; // El 'Transform' del 'jugador2'
    public ParticleSystem particulaCapturaPrefab;
    public ParticleSystem particulaFalloPrefab;
    public AudioClip audioSacudida;
    public AudioClip audioCaptura;
    public AudioClip audioFallo;

    [Header("UI de Turno")]
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

        // 1. Configurar los datos de los Porkemon
        jugador1.Setup(GestorDeBatalla.instance.porkemonJugador);
        jugador2.Setup(GestorDeBatalla.instance.porkemonBot);

        // 2. Cargar los modelos 3D usando nuestras referencias
        // Esto soluciona el bug del modelo que desaparece al recargar la escena
        if (playerModelManager != null && jugador1.porkemon != null)
        {
            playerModelManager.UpdateModel(jugador1.porkemon.BaseData.nombre);
        }
        else
        {
            Debug.LogError("FuncTurnos: 'playerModelManager' no está asignado o no hay Porkemon de jugador.");
        }

        if (botModelManager != null && jugador2.porkemon != null)
        {
            botModelManager.UpdateModel(jugador2.porkemon.BaseData.nombre);
        }
        else
        {
            Debug.LogError("FuncTurnos: 'botModelManager' no está asignado o no hay Porkemon de bot.");
        }

        // 3. Configurar el turno
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
        // --- AÑADIDO: Comprobación de item (Porkebola) ---
        else if (isPlayer1Turn && GameState.itemSeleccionado != null && enCombate && !ConsolaEnJuego.instance.isTyping)
        {
            StartCoroutine(RutinaLanzarPorkebola());
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

    // --- MÉTODOS DE CAPTURA AÑADIDOS ---

    private float GetBallMultiplier(BattleItemType type)
    {
        switch (type)
        {
            case BattleItemType.Porkebola: return 1.0f;
            case BattleItemType.Superbola: return 1.5f;
            case BattleItemType.Ultrabola: return 2.0f;
            case BattleItemType.Masterbola: return 255f; // Captura asegurada
            default: return 1.0f;
        }
    }

    private IEnumerator RutinaLanzarPorkebola()
    {
        enCombate = false;
        
        // 1. Obtener la bola y el objetivo
        BattleItem bolaUsada = GameState.itemSeleccionado;
        GameState.itemSeleccionado = null;
        Porkemon porkemonSalvaje = jugador2.porkemon;

        // 2. Consumir el objeto
        bolaUsada.cantidad--;
        if (bolaUsada.cantidad <= 0)
        {
            GestorDeBatalla.instance.inventarioBattleItems.Remove(bolaUsada);
        }

        // 3. Mensaje inicial y animación de lanzamiento
        Debug.Log($"¡Lanzaste una {bolaUsada.nombre}!");
        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);

        GameObject pokebolaInstancia = Instantiate(pokebolaPrefab, posicionLanzamientoJugador.position, posicionLanzamientoJugador.rotation);
        AudioSource audioSource = pokebolaInstancia.GetComponent<AudioSource>();
        
        // Animación simple de movimiento (puedes reemplazarla con DoTween o una animación de Animator)
        float duracionLanzamiento = 0.75f;
        float tiempoPasado = 0f;
        Vector3 posInicial = posicionLanzamientoJugador.position;
        
        while (tiempoPasado < duracionLanzamiento)
        {
            tiempoPasado += Time.deltaTime;
            pokebolaInstancia.transform.position = Vector3.Lerp(posInicial, posicionBotPorkemon.position, tiempoPasado / duracionLanzamiento);
            yield return null;
        }
        
        // 4. Ocultar el modelo del bot
        botModelManager.PlayExitAnimation(); // Llama a la animación de salida
        yield return new WaitForSeconds(0.5f); // Espera a que el modelo se oculte

        // 5. Calcular el resultado
        float multiplier = GetBallMultiplier(bolaUsada.type);
        ResultadoCaptura resultado = CalculadorCaptura.IntentarCaptura(porkemonSalvaje, multiplier);

        Animator bolaAnimator = pokebolaInstancia.GetComponent<Animator>();

        // 6. Animación de Sacudidas
        if (!resultado.esCritica)
        {
            for (int i = 0; i < resultado.sacudidas; i++)
            {
                Debug.Log("..."); 
                if (bolaAnimator != null) bolaAnimator.SetTrigger("Sacudida"); // Asume que tienes un Trigger "Sacudida"
                if (audioSource != null && audioSacudida != null) audioSource.PlayOneShot(audioSacudida);
                yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
                yield return new WaitForSeconds(1.5f); // Duración de la sacudida
            }
        }
        else
        {
            Debug.Log("¡Captura crítica!");
            yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
        }

        // 7. Procesar el resultado
        if (resultado.capturado)
        {
            // ¡ÉXITO!
            Debug.Log($"¡Gotcha! ¡{porkemonSalvaje.BaseData.nombre} ha sido capturado!");
            if (particulaCapturaPrefab != null) Instantiate(particulaCapturaPrefab, pokebolaInstancia.transform.position, Quaternion.identity);
            if (audioSource != null && audioCaptura != null) audioSource.PlayOneShot(audioCaptura);
            
            yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
            yield return new WaitForSeconds(2f);
            
            Destroy(pokebolaInstancia);
            
            // Usamos el método de GestorDeBatalla
            GestorDeBatalla.instance.PokemonCapturado(porkemonSalvaje);
        }
        else
        {
            // ¡FALLO!
            Debug.Log($"¡Oh no! ¡{porkemonSalvaje.BaseData.nombre} se ha escapado!");
            if (particulaFalloPrefab != null) Instantiate(particulaFalloPrefab, pokebolaInstancia.transform.position, Quaternion.identity);
            if (audioSource != null && audioFallo != null) audioSource.PlayOneShot(audioFallo);
            
            Destroy(pokebolaInstancia);
            
            // El modelo del bot regresa
            botModelManager.PlayEnterAnimation(); // Llama a la animación de entrada
            
            yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
            yield return new WaitForSeconds(1f);

            // Ceder el turno al Bot
            CambiarTurno();
        }
        
        enCombate = true;
    }
}