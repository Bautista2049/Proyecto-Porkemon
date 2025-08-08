using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("Turn Prefabs")]
    public GameObject turnoJ1Prefab;  // Referencia al prefab Turno(J1)
    public GameObject turnoJ2Prefab;  // Referencia al prefab Turno(J2)

    [Header("Turn Settings")]
    public bool isPlayer1Turn = true; // Comienza con el turno del Jugador 1

    private void Start()
    {
        // Buscar prefabs si no están asignados
        if (turnoJ1Prefab == null)
            turnoJ1Prefab = GameObject.Find("Turno(J1)");
        if (turnoJ2Prefab == null)
            turnoJ2Prefab = GameObject.Find("Turno(J2)");

        // Asegurarse de que los prefabs estén correctamente asignados
        if (turnoJ1Prefab == null || turnoJ2Prefab == null)
        {
            Debug.LogError("TurnManager: Los prefabs de turno no están asignados en el inspector!");
            return;
        }

        // Establecer el estado inicial de los turnos
        UpdateTurnDisplay();
    }

    // Método para cambiar de turno
    public void SwitchTurn()
    {
        isPlayer1Turn = !isPlayer1Turn;
        UpdateTurnDisplay();
    }

    // Método para actualizar la visualización de los turnos
    private void UpdateTurnDisplay()
    {
        if (turnoJ1Prefab != null)
            turnoJ1Prefab.SetActive(isPlayer1Turn);
        if (turnoJ2Prefab != null)
            turnoJ2Prefab.SetActive(!isPlayer1Turn);
            
        Debug.Log(isPlayer1Turn ? "Turno del Jugador 1" : "Turno del Jugador 2");
    }

    // Método para establecer manualmente el turno del Jugador 1
    public void SetPlayer1Turn()
    {
        isPlayer1Turn = true;
        UpdateTurnDisplay();
    }

    // Método para establecer manualmente el turno del Jugador 2
    public void SetPlayer2Turn()
    {
        isPlayer1Turn = false;
        UpdateTurnDisplay();
    }

    // Método para obtener el turno actual
    public bool GetCurrentTurn()
    {
        return isPlayer1Turn;
    }
}
