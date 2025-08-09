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
    public GameObject panelDeAccionesDelJugador; // El panel que contiene "Luchar", "Huir", etc.
    
    private bool isPlayer1Turn;
    private bool botAtacando = false;

    private void Start()
    {
        // Asignamos los porkemon persistentes desde el Gestor de Batalla
        jugador1.Setup(GestorDeBatalla.instance.porkemonJugador);
        jugador2.Setup(GestorDeBatalla.instance.porkemonBot);

        // Determinar el turno inicial basado en la velocidad
        if (jugador1.porkemon.Velocidad >= jugador2.porkemon.Velocidad)
        {
            isPlayer1Turn = true;
        }
        else
        {
            isPlayer1Turn = false;
        }
        GameState.player1Turn = isPlayer1Turn;
        
        ActualizarUI();

        if (!isPlayer1Turn && PuedeCombatir())
        {
            StartCoroutine(RutinaAtaqueBot());
        }
    }

    private IEnumerator RutinaAtaqueBot()
    {
        if (botAtacando || !PuedeCombatir()) yield break;
        botAtacando = true;

        yield return new WaitForSeconds(1.5f);

        // El bot elige un ataque al azar de su lista
        if (jugador2.porkemon.Ataques.Count > 0)
        {
            int indiceAtaque = Random.Range(0, jugador2.porkemon.Ataques.Count);
            AtaqueData ataqueDelBot = jugador2.porkemon.Ataques[indiceAtaque];
            
            Debug.Log($"{jugador2.porkemon.BaseData.nombre} usa {ataqueDelBot.nombreAtaque}");
            
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

        botAtacando = false;
    }

    private void CambiarTurno()
    {
        isPlayer1Turn = !isPlayer1Turn;
        GameState.player1Turn = isPlayer1Turn;
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        turnoJ1Prefab?.SetActive(isPlayer1Turn);
        turnoBotPrefab?.SetActive(!isPlayer1Turn);

        // Activar o desactivar los botones del jugador según el turno
        panelDeAccionesDelJugador?.SetActive(isPlayer1Turn);
    }

    private void VerificarFinCombate()
    {
        if (jugador1.porkemon.VidaActual <= 0)
        {
            Debug.Log("¡El Contrincante gana!");
            SceneManager.LoadScene("Escena de muerte");
        }
        else if (jugador2.porkemon.VidaActual <= 0)
        {
            Debug.Log("¡El Jugador gana!");
            SceneManager.LoadScene("Escena Principal");
        }
    }

    public bool PuedeCombatir()
    {
        return jugador1.porkemon.VidaActual > 0 && jugador2.porkemon.VidaActual > 0;
    }
}