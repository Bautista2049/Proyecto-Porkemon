using UnityEngine;
using UnityEngine.SceneManagement;

public class Ataques : MonoBehaviour
{
     [Tooltip("Asignar en el Inspector el jugador que recibirá el daño")]
    public HealthPoints objetivo;

    private TurnManager turnManager;

    private void Start()
    {
        // Buscar el TurnManager en la escena
        turnManager = FindObjectOfType<TurnManager>();
    }

    public void VolverCombate()
    {
        SceneManager.LoadScene("Luchar Escena");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Método para obtener el objetivo correcto según el turno
    private HealthPoints GetTargetPlayer()
    {
        if (turnManager == null)
        {
            Debug.LogError("TurnManager no encontrado!");
            return objetivo; // Usar el objetivo asignado manualmente como fallback
        }

        // Buscar todos los jugadores en la escena
        HealthPoints[] allPlayers = FindObjectsOfType<HealthPoints>();
        
        // Si es turno del Jugador 1, atacar al Jugador 2
        // Si es turno del Jugador 2, atacar al Jugador 1
        foreach (HealthPoints player in allPlayers)
        {
            // Asumimos que el nombre del GameObject indica a qué jugador pertenece
            if (turnManager.GetCurrentTurn())
            {
                // Turno del Jugador 1 - buscar Jugador 2
                if (player.gameObject.name.Contains("Jugador2") || player.gameObject.name.Contains("Player2"))
                {
                    return player;
                }
            }
            else
            {
                // Turno del Jugador 2 - buscar Jugador 1
                if (player.gameObject.name.Contains("Jugador1") || player.gameObject.name.Contains("Player1"))
                {
                    return player;
                }
            }
        }

        // Si no encontramos por nombre, usar el objetivo asignado
        return objetivo;
    }

    public void Ataque1()
    {
        HealthPoints target = GetTargetPlayer();
        if (target != null)
            target.RecibirDanio(10);
        
        VolverCombate();
    }

    public void Ataque2()
    {
        HealthPoints target = GetTargetPlayer();
        if (target != null)
            target.RecibirDanio(20);
        
        VolverCombate();
    }

    public void Ataque3()
    {
        HealthPoints target = GetTargetPlayer();
        if (target != null)
            target.RecibirDanio(30);
        
        VolverCombate();
    }

    public void Ataque4()
    {
        HealthPoints target = GetTargetPlayer();
        if (target != null)
            target.RecibirDanio(40);
        
        VolverCombate();
    }
}
