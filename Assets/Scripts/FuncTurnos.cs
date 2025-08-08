using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FuncTurnos : MonoBehaviour
{
    public HealthPoints jugador1; // jugador humano
    public HealthPoints jugador2; // bot
    public GameObject turnoJ1Prefab;
    public GameObject turnoJ2Prefab;
    public int danioPorAtaque = 20;
    private bool isPlayer1Turn = true; // El turno inicial

    private bool botAtacando = false; // para evitar ataques múltiples simultáneos

    private void Start()
    {
        if (jugador1 == null || jugador2 == null)
        {
            HealthPoints[] jugadores = FindObjectsOfType<HealthPoints>();
            if (jugadores.Length >= 2)
            {
                jugador1 = jugadores[0];
                jugador2 = jugadores[1];
            }
        }

        if (turnoJ1Prefab == null)
            turnoJ1Prefab = GameObject.Find("Turno(J1)");
        if (turnoJ2Prefab == null)
            turnoJ2Prefab = GameObject.Find("Turno(J2)");

        ActualizarUI();

        // Si empieza el turno del bot, arrancar ataque automático
        if (!isPlayer1Turn)
            StartCoroutine(AtacarBotConDelay());
    }

    public void Atacar()
    {
        if (!PuedeAtacar()) return;

        // Solo el jugador humano puede atacar manualmente
        if (!isPlayer1Turn) return;

        jugador2?.RecibirDanio(danioPorAtaque);

        CambiarTurno();
        VerificarFinCombate();

        // Si ahora es turno del bot, arrancar ataque automático
        if (!isPlayer1Turn)
            StartCoroutine(AtacarBotConDelay());
    }

    private IEnumerator AtacarBotConDelay()
    {
        if (botAtacando) yield break;
        botAtacando = true;

        // Esperar 1.5 segundos antes de atacar (simula "pensar")
        yield return new WaitForSeconds(1.5f);

        if (!PuedeAtacar()) // revisa si el combate sigue
        {
            botAtacando = false;
            yield break;
        }

        if (!isPlayer1Turn) // confirma que es turno del bot
        {
            jugador1?.RecibirDanio(danioPorAtaque);
            CambiarTurno();
            VerificarFinCombate();
        }

        botAtacando = false;
    }

    private void CambiarTurno()
    {
        isPlayer1Turn = !isPlayer1Turn;
        ActualizarUI();
        Debug.Log(isPlayer1Turn ? "Turno del Jugador 1" : "Turno del Bot");
    }

    private void ActualizarUI()
    {
        if (turnoJ1Prefab != null)
        {
            turnoJ1Prefab.SetActive(isPlayer1Turn);
            var texto = turnoJ1Prefab.GetComponentInChildren<Text>();
            if (texto != null) texto.text = "Turno del Jugador 1";
        }

        if (turnoJ2Prefab != null)
        {
            turnoJ2Prefab.SetActive(!isPlayer1Turn);
            var texto = turnoJ2Prefab.GetComponentInChildren<Text>();
            if (texto != null) texto.text = "Turno del Bot";
        }
    }

    private void VerificarFinCombate()
    {
        if (jugador1.stats.health <= 0)
        {
            Debug.Log("¡El Bot gana!");
            SceneManager.LoadScene("Escena Principal");
        }
        else if (jugador2.stats.health <= 0)
        {
            Debug.Log("¡El Jugador gana!");
            SceneManager.LoadScene("Escena Principal");
        }
    }

    public bool PuedeAtacar()
    {
        return jugador1 != null && jugador1.stats.health > 0 &&
               jugador2 != null && jugador2.stats.health > 0;
    }

    public bool GetCurrentTurn()
    {
        return isPlayer1Turn;
    }
}
