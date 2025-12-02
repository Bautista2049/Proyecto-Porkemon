using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static PorkemonExtension;

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
        GameState.multiplicadorDinero = 1f;
        GameState.multiplicadorExp = 1.5f;
        GameState.multiplicadorCaptura = 1f;

        if (GestorDeBatalla.instance != null)
            GestorDeBatalla.instance.IniciarBatalla();

        if (ConsolaEnJuego.instance != null)
        {
            ConsolaEnJuego.instance.ResetConsole();
        }

        jugador1.Setup(GestorDeBatalla.instance.porkemonJugador);
        jugador2.Setup(GestorDeBatalla.instance.porkemonBot);

        if (playerModelManager != null)
        {
            playerModelManager.ClearCurrentModel();
            if (jugador1.porkemon != null)
                playerModelManager.UpdateModel(jugador1.porkemon.BaseData.nombre);
        }

        if (botModelManager != null)
        {
            botModelManager.ClearCurrentModel();
            if (jugador2.porkemon != null)
                botModelManager.UpdateModel(jugador2.porkemon.BaseData.nombre);
        }

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
            StartCoroutine(RutinaUsarItem());
    }

    private IEnumerator RutinaAtaqueJugador()
    {
        corutinaEnEjecucion = true;
        enCombate = false;
        AtaqueData ataque = GameState.ataqueSeleccionado;
        GameState.ataqueSeleccionado = null;

        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
        if (playerModelManager != null && jugador1.porkemon != null)
            playerModelManager.PlayAttackAnimationByName(jugador1.porkemon, ataque);
        jugador1.porkemon.UsarAtaqueElemental(jugador2.porkemon, ataque);

        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
        jugador1.porkemon.AplicarDanioPorEstado();
        jugador2.porkemon.AplicarDanioPorEstado();

        if (jugador1.porkemon.VidaActual <= 0 || jugador2.porkemon.VidaActual <= 0)
            yield return StartCoroutine(VerificarFinCombate());
        else
            CambiarTurno();

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

            if (botModelManager != null && jugador2.porkemon != null)
                botModelManager.PlayAttackAnimationByName(jugador2.porkemon, ataqueDelBot);
            jugador2.porkemon.UsarAtaqueElemental(jugador1.porkemon, ataqueDelBot);

            yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
            jugador1.porkemon.AplicarDanioPorEstado();
            jugador2.porkemon.AplicarDanioPorEstado();

            if (jugador1.porkemon.VidaActual <= 0 || jugador2.porkemon.VidaActual <= 0)
                yield return StartCoroutine(VerificarFinCombate());
            else
                CambiarTurno();
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

    private IEnumerator VerificarFinCombate()
    {
        if (jugador1.porkemon.VidaActual <= 0)
        {
            var equipoJugador = GestorDeBatalla.instance.equipoJugador;
            var siguientePokemon = equipoJugador.Find(p => p != jugador1.porkemon && p.VidaActual > 0);

            if (siguientePokemon != null)
            {
                Debug.Log($"¡{jugador1.porkemon.BaseData.nombre} se ha debilitado!");
                yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);

                jugador1.porkemon = siguientePokemon;
                GameState.porkemonDelJugador = siguientePokemon;

                jugador1.Setup(siguientePokemon);
                if (playerModelManager != null)
                {
                    playerModelManager.UpdateModel(siguientePokemon.BaseData.nombre);
                }

                Debug.Log($"¡Adelante {siguientePokemon.BaseData.nombre}!");
                yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);

                CambiarTurno();
            }
            else
            {
                GameState.esCombateBoss = false;
                SceneTransitionManager.Instance.LoadScene("Escena de muerte");
            }
        }
        else if (jugador2.porkemon.VidaActual <= 0)
        {
            // Verificar si hay más Pokémon en el equipo del NPC (entrenador)
            if (GameState.esCombateBoss && GestorDeBatalla.instance.equipoBot.Count > 1)
            {
                var equipoBot = GestorDeBatalla.instance.equipoBot;
                var siguientePokemon = equipoBot.Find(p => p != jugador2.porkemon && p.VidaActual > 0);

                if (siguientePokemon != null)
                {
                    Debug.Log($"¡El {jugador2.porkemon.BaseData.nombre} enemigo se ha debilitado!");
                    yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);

                    jugador2.porkemon = siguientePokemon;
                    GameState.porkemonDelBot = siguientePokemon;

                    jugador2.Setup(siguientePokemon);
                    if (botModelManager != null)
                    {
                        botModelManager.UpdateModel(siguientePokemon.BaseData.nombre);
                    }

                    Debug.Log($"¡El entrenador envía a {siguientePokemon.BaseData.nombre}!");
                    yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);

                    CambiarTurno();
                    yield break;
                }
            }

            // Victoria del jugador
            GameState.nombreGanador = jugador1.porkemon.BaseData.nombre;

            int expBase = GestorDeBatalla.instance.equipoJugador.CalcularExperienciaGanada(new List<Porkemon> { jugador2.porkemon });
            GameState.experienciaGanada = Mathf.RoundToInt(expBase * Mathf.Max(0.1f, GameState.multiplicadorExp));
            GameState.equipoGanador = new List<Porkemon>(GestorDeBatalla.instance.equipoJugador);
            GameState.victoriaFueCaptura = false;

            int dineroBase = Mathf.Max(1, GameState.experienciaGanada / 2);
            GameState.dineroGanado = Mathf.RoundToInt(dineroBase * Mathf.Max(0.1f, GameState.multiplicadorDinero));
            GameState.dineroJugador += GameState.dineroGanado;

            GameState.multiplicadorDinero = 1f;
            GameState.multiplicadorExp = 1.5f;
            GameState.multiplicadorCaptura = 1f;

            GameState.esCombateBoss = false;
            SceneTransitionManager.Instance.LoadScene("Escena de Victoria");
        }
    }

    public bool PuedeCombatir()
    {
        return jugador1.porkemon.VidaActual > 0 && jugador2.porkemon.VidaActual > 0;
    }

    private float GetBallMultiplier(BattleItemType type)
    {
        float baseMult;
        switch (type)
        {
            case BattleItemType.Porkebola: baseMult = 1f; break;
            case BattleItemType.Superbola: baseMult = 1.5f; break;
            case BattleItemType.Ultrabola: baseMult = 2f; break;
            case BattleItemType.Masterbola: baseMult = 255f; break;
            default: baseMult = 1f; break;
        }

        return baseMult * Mathf.Max(0.1f, GameState.multiplicadorCaptura);
    }

    private IEnumerator RutinaUsarItem()
    {
        corutinaEnEjecucion = true;
        enCombate = false;

        BattleItem itemUsado = GameState.itemSeleccionado;
        GameState.itemSeleccionado = null;

        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);

        if (itemUsado.type == BattleItemType.Porkebola || itemUsado.type == BattleItemType.Superbola || itemUsado.type == BattleItemType.Ultrabola || itemUsado.type == BattleItemType.Masterbola)
        {
            if (GameState.esCombateBoss)
            {
                Debug.Log("No puedes capturar el Porkemon de un entrenador.");
                yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);

                enCombate = true;
                corutinaEnEjecucion = false;
                yield break;
            }

            yield return StartCoroutine(RutinaLanzarPorkebola(itemUsado));
        }
        else
        {
            AplicarEfectoItem(itemUsado, jugador1.porkemon);
            itemUsado.cantidad--;
            GestorDeBatalla.instance.SincronizarInventarioCompleto(itemUsado);

            if (itemUsado.cantidad <= 0)
                GestorDeBatalla.instance.inventarioBattleItems.Remove(itemUsado);
            
            yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
            yield return new WaitForSeconds(1f);
            CambiarTurno();
        }

        enCombate = true;
        corutinaEnEjecucion = false;
    }

    private void AplicarEfectoItem(BattleItem item, Porkemon porkemon)
    {
        switch (item.type)
        {
            case BattleItemType.Pocion:
                int curacion20 = Mathf.Min(20, porkemon.VidaMaxima - porkemon.VidaActual);
                porkemon.VidaActual += curacion20;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Recuperó {curacion20} PS!");
                break;
            case BattleItemType.Superpocion:
                int curacion50 = Mathf.Min(50, porkemon.VidaMaxima - porkemon.VidaActual);
                porkemon.VidaActual += curacion50;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Recuperó {curacion50} PS!");
                break;
            case BattleItemType.Hiperpocion:
                int curacion200 = Mathf.Min(200, porkemon.VidaMaxima - porkemon.VidaActual);
                porkemon.VidaActual += curacion200;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Recuperó {curacion200} PS!");
                break;
            case BattleItemType.Pocionmaxima:
                int curacionMax = porkemon.VidaMaxima - porkemon.VidaActual;
                porkemon.VidaActual = porkemon.VidaMaxima;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Recuperó {curacionMax} PS!");
                break;
            case BattleItemType.AtaqueX:
                porkemon.AumentarAtaque(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Ataque aumentado!");
                if (GestorDeBatalla.instance != null)
                    GestorDeBatalla.instance.ActivarBuffVisualJugador(-1f);
                break;
            case BattleItemType.DefensaX:
                porkemon.AumentarDefensa(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Defensa aumentada!");
                if (GestorDeBatalla.instance != null)
                    GestorDeBatalla.instance.ActivarBuffVisualJugador(-1f);
                break;
            case BattleItemType.AtaqueEspecialX:
                porkemon.AumentarEspiritu(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Ataque Especial aumentado!");
                if (GestorDeBatalla.instance != null)
                    GestorDeBatalla.instance.ActivarBuffVisualJugador(-1f);
                break;
            case BattleItemType.DefensaEspecialX:
                porkemon.AumentarEspiritu(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Defensa Especial aumentada!");
                if (GestorDeBatalla.instance != null)
                    GestorDeBatalla.instance.ActivarBuffVisualJugador(-1f);
                break;
            case BattleItemType.VelocidadX:
                porkemon.AumentarVelocidad(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Velocidad aumentada!");
                if (GestorDeBatalla.instance != null)
                    GestorDeBatalla.instance.ActivarBuffVisualJugador(-1f);
                break;
            case BattleItemType.PrecisionX:
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Precisión aumentada!");
                break;
            case BattleItemType.CriticoX:
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Índice crítico aumentado!");
                break;
            case BattleItemType.RotoPremio:
                GameState.multiplicadorDinero = 3f;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡Las recompensas de dinero aumentarán esta batalla!");
                break;
            case BattleItemType.RotoExp:
                GameState.multiplicadorExp = 1.5f;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡La experiencia ganada aumentará esta batalla!");
                break;
            case BattleItemType.RotoBoost:
                porkemon.AumentarAtaque(2);
                porkemon.AumentarDefensa(2);
                porkemon.AumentarEspiritu(2);
                porkemon.AumentarVelocidad(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡Todas sus estadísticas han aumentado!");
                if (GestorDeBatalla.instance != null)
                    GestorDeBatalla.instance.ActivarBuffVisualJugador(-1f);
                break;
            case BattleItemType.RotoCatch:
                GameState.multiplicadorCaptura = 2f;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡La probabilidad de captura ha aumentado esta batalla!");
                break;
            case BattleItemType.RotoOferta:
                GameState.multiplicadorPreciosTienda = 0.5f;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡Los precios de la tienda se han reducido temporalmente!");
                break;
        }
    }

    private IEnumerator RutinaLanzarPorkebola(BattleItem bolaUsada)
    {
        Porkemon porkemonSalvaje = jugador2.porkemon;

        bolaUsada.cantidad--;
        GestorDeBatalla.instance.SincronizarInventarioCompleto(bolaUsada);

        if (bolaUsada.cantidad <= 0)
            GestorDeBatalla.instance.inventarioBattleItems.Remove(bolaUsada);

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
    }
}