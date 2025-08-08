using UnityEngine;
using UnityEngine.UI;

public class CombateCiclico : MonoBehaviour
{
    [Header("Jugadores")]
    public HealthPoints jugador1;
    public HealthPoints jugador2;
    
    [Header("UI Turnos")]
    public GameObject turnoJ1Prefab;
    public GameObject turnoJ2Prefab;
    
    [Header("Configuración")]
    public int danioPorAtaque = 20;
    
    private TurnManager turnManager;
    
    private void Start()
    {
        // Buscar automáticamente los jugadores si no están asignados
        if (jugador1 == null || jugador2 == null)
        {
            HealthPoints[] jugadores = FindObjectsOfType<HealthPoints>();
            if (jugadores.Length >= 2)
            {
                jugador1 = jugadores[0];
                jugador2 = jugadores[1];
            }
        }
        
        // Buscar el TurnManager
        turnManager = FindObjectOfType<TurnManager>();
        if (turnManager == null)
        {
            Debug.LogError("CombateCiclico: TurnManager no encontrado en la escena!");
            return;
        }
        
        // Buscar prefabs de turno si no están asignados
        if (turnoJ1Prefab == null)
            turnoJ1Prefab = GameObject.Find("Turno(J1)");
        if (turnoJ2Prefab == null)
            turnoJ2Prefab = GameObject.Find("Turno(J2)");
            
        // Sincronizar UI con TurnManager
        SincronizarConTurnManager();
    }
    
    public void Atacar()
    {
        if (!PuedeAtacar())
            return;
        
        // Determinar quién ataca según el turno actual
        if (turnManager.GetCurrentTurn())
        {
            // Jugador 1 ataca al Jugador 2
            if (jugador2 != null)
                jugador2.RecibirDanio(danioPorAtaque);
        }
        else
        {
            // Jugador 2 ataca al Jugador 1
            if (jugador1 != null)
                jugador1.RecibirDanio(danioPorAtaque);
        }
        
        // Cambiar turno usando TurnManager
        turnManager.SwitchTurn();
        
        // Sincronizar UI con el nuevo turno
        SincronizarConTurnManager();
        
        // Verificar si alguien perdió
        VerificarFinCombate();
    }
    
    private void SincronizarConTurnManager()
    {
        if (turnManager == null) return;
        
        bool esTurnoJ1 = turnManager.GetCurrentTurn();
        
        if (turnoJ1Prefab != null)
            turnoJ1Prefab.SetActive(esTurnoJ1);
        if (turnoJ2Prefab != null)
            turnoJ2Prefab.SetActive(!esTurnoJ1);
    }
    
    private void VerificarFinCombate()
    {
        if (jugador1 != null && jugador1.saludActual <= 0)
        {
            Debug.Log("¡Jugador 2 gana!");
            // Aquí puedes cargar la escena de victoria o derrota
        }
        else if (jugador2 != null && jugador2.saludActual <= 0)
        {
            Debug.Log("¡Jugador 1 gana!");
            // Aquí puedes cargar la escena de victoria o derrota
        }
    }
    
    public bool PuedeAtacar()
    {
        return (jugador1 != null && jugador1.saludActual > 0) && 
               (jugador2 != null && jugador2.saludActual > 0) &&
               (turnManager != null);
    }
    
    public string GetTurnoActual()
    {
        if (turnManager == null) return "Sin TurnManager";
        return turnManager.GetCurrentTurn() ? "Jugador 1" : "Jugador 2";
    }
}
