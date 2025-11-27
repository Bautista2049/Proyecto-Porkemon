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

        // Guardar posición del jugador como hace Transicion_Combate
        Vector3 posicionGuardada = collision.transform.position;
        if (collision.contactCount > 0)
        {
            Vector3 normal = collision.GetContact(0).normal;
            normal.y = 0f;
            if (normal.sqrMagnitude > 0.0001f)
            {
                normal.Normalize();
                posicionGuardada += normal * 0.75f;
            }
        }

        GameState.GuardarPosicionJugador(posicionGuardada, SceneManager.GetActiveScene().name);

        // Configurar combate de boss
        GameState.esCombateBoss = true;
        GameState.porkemonDelBot = new Porkemon(giratinaData, nivelGiratina);

        if (GestorDeBatalla.instance != null)
        {
            GestorDeBatalla.instance.combateIniciado = false;
        }

        // Mantener la cámara principal entre escenas, igual que Transicion_Combate
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Object.DontDestroyOnLoad(mainCamera.gameObject);
        }

        SceneManager.LoadScene(nombreEscenaBoss);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
