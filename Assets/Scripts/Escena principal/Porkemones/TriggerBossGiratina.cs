using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerBossGiratina : MonoBehaviour
{
    [SerializeField] private string nombreEscenaBoss = "EscenaCombateBoss";
    [SerializeField] private PorkemonData giratinaData;
    [SerializeField] private int nivelGiratina = 70;
    [Header("Configuración Boss")]
    [SerializeField] private bool esJefe = true;
    [SerializeField] private float distanciaDeteccion = 6f;
    [SerializeField] private float velocidadMovimiento = 3f;

    private Transform objetivoJugador;

    private void Update()
    {
        if (!esJefe)
            return;

        if (objetivoJugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                objetivoJugador = playerObj.transform;
            }
        }

        if (objetivoJugador == null)
            return;

        float distancia = Vector3.Distance(transform.position, objetivoJugador.position);
        if (distancia <= distanciaDeteccion)
        {
            Vector3 destino = new Vector3(objetivoJugador.position.x, transform.position.y, objetivoJugador.position.z);
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadMovimiento * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (giratinaData == null)
        {
            Debug.LogWarning("TriggerBossGiratina: No hay PorkemonData asignado para Giratina.");
            return;
        }

        // Guardar posición del jugador
        Vector3 posicionGuardada = CombatTransitionHelper.CalcularPosicionRetorno(collision);
        GameState.GuardarPosicionJugador(posicionGuardada, SceneManager.GetActiveScene().name);

        // Configurar combate de boss
        GameState.esCombateBoss = true;
        GameState.porkemonDelBot = new Porkemon(giratinaData, nivelGiratina);

        CombatTransitionHelper.IniciarTransicionCombate(nombreEscenaBoss);
    }
}
