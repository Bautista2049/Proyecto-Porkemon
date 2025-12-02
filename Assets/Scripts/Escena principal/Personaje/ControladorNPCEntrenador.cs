using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))] // Asegura que el NPC tenga un NavMeshAgent
public class ControladorNPCEntrenador : MonoBehaviour
{
    [Header("Configuración del Entrenador")]
    [SerializeField] private bool esEntrenador = true;
    [SerializeField] private bool esJefe = false;
    [SerializeField] private float rangoDeteccion = 8f;
    [SerializeField] private string nombreEscenaCombate = "Escena de Combate";

    [Header("Configuración de Patrulla")]
    [SerializeField] private Transform[] puntosPatrulla; // Puntos entre los que el NPC patrullará
    private int indicePuntoActual = 0;

    [Header("Equipo del NPC")]
    [SerializeField] private List<PorkemonData> equipoBase = new List<PorkemonData>();
    [SerializeField] private int tamanoEquipoAleatorio = 3;

    [Header("Referencias")]
    [SerializeField] private SphereCollider triggerDeteccion;
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;
    private Transform jugador;
    private bool jugadorDetectado = false;
    private bool enPatrulla = false;
    private List<Porkemon> equipoSeleccionado = new List<Porkemon>();

    private void Start()
    {
        if (!esEntrenador)
        {
            enabled = false;
            return;
        }

        agent = GetComponent<NavMeshAgent>();

        // Configurar el trigger de detección
        if (triggerDeteccion == null)
        {
            triggerDeteccion = gameObject.AddComponent<SphereCollider>();
            triggerDeteccion.isTrigger = true;
        }
        triggerDeteccion.radius = rangoDeteccion;

        // Inicia la patrulla si hay puntos definidos
        if (puntosPatrulla.Length > 0)
        {
            IniciarPatrulla();
        }
    }

    private void Update()
    {
        // Si está patrullando y ha llegado a su destino, va al siguiente punto
        if (enPatrulla && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            IrAlSiguientePunto();
        }

        // Actualiza la animación de caminar basándose en la velocidad del NavMeshAgent
        if (animator != null)
        {
            animator.SetBool("Walking", agent.velocity.magnitude > 0.1f);
        }
    }

    private void IniciarPatrulla()
    {
        if (puntosPatrulla.Length == 0) return;
        enPatrulla = true;
        agent.isStopped = false;
        IrAlSiguientePunto();
    }

    private void DetenerPatrulla()
    {
        enPatrulla = false;
        agent.isStopped = true;
    }

    private void IrAlSiguientePunto()
    {
        if (puntosPatrulla.Length == 0) return;
        agent.destination = puntosPatrulla[indicePuntoActual].position;
        indicePuntoActual = (indicePuntoActual + 1) % puntosPatrulla.Length;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!esEntrenador || jugadorDetectado || !other.CompareTag("Player")) return;

        jugador = other.transform;
        jugadorDetectado = true;
        DetenerPatrulla();

        Debug.Log("Jugador detectado por NPC Entrenador");

        if (esJefe)
        {
            // Si es Jefe, se detiene, mira al jugador y espera la colisión
            Vector3 direction = (jugador.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
            Debug.Log("NPC Jefe esperando colisión con el jugador");
        }
        else
        {            
            // Si es un entrenador normal, se acerca al jugador usando el NavMeshAgent
            StartCoroutine(AproximarseAlJugador());
            Debug.Log("NPC Entrenador comenzando a acercarse al jugador");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // El combate se inicia por colisión solo si es un Jefe que ya ha detectado al jugador
        if (esJefe && jugadorDetectado && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colisión detectada con el jugador - Iniciando combate con Jefe");
            IniciarCombate();
        }
    }

    private IEnumerator AproximarseAlJugador()
    {
        agent.isStopped = false;

        while (jugadorDetectado && jugador != null)
        {
            agent.destination = jugador.position;

            // Si el NPC llega lo suficientemente cerca del jugador, inicia el combate (y no es jefe)
            if (!esJefe && agent.remainingDistance < 2.0f && agent.remainingDistance != 0)
            {
                IniciarCombate();
                yield break;
            }
            
            yield return null;
        }

        // Si el jugador sale del rango, el NPC detiene su movimiento
        if(!jugadorDetectado)
        {
             agent.isStopped = true;
             IniciarPatrulla();
        }
    }

    private void IniciarCombate()
    {
        if (!jugador) return;
        
        DetenerPatrulla(); // Nos aseguramos de que esté detenido
        Debug.Log("Iniciando combate con NPC Entrenador");

        SeleccionarEquipoAleatorio();

        if (GestorDeBatalla.instance != null)
        {
            GestorDeBatalla.instance.ConfigurarEquipoNPC(equipoSeleccionado);
            GameState.esCombateBoss = true;
        }

        GameState.GuardarPosicionJugador(jugador.position, SceneManager.GetActiveScene().name);

        SceneManager.LoadScene(nombreEscenaCombate);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SeleccionarEquipoAleatorio()
    {
        equipoSeleccionado.Clear();
        if (equipoBase.Count == 0) return;

        List<PorkemonData> disponibles = new List<PorkemonData>(equipoBase);
        int cantidadSeleccionar = esJefe ? Mathf.Min(tamanoEquipoAleatorio, disponibles.Count) : 1;
        if(esJefe) cantidadSeleccionar = Mathf.Max(2, cantidadSeleccionar); 

        for (int i = 0; i < cantidadSeleccionar; i++)
        {
            if (disponibles.Count == 0) break;
            int indiceAleatorio = Random.Range(0, disponibles.Count);
            PorkemonData seleccionado = disponibles[indiceAleatorio];
            equipoSeleccionado.Add(new Porkemon(seleccionado, seleccionado.nivel));
            disponibles.RemoveAt(indiceAleatorio);
        }

        string tipoNPC = esJefe ? "Jefe" : "Entrenador";
        Debug.Log($"Equipo seleccionado para NPC {tipoNPC}: {equipoSeleccionado.Count} Pokémon");
    }

    public void SetEquipoBase(List<PorkemonData> nuevoEquipo) => equipoBase = new List<PorkemonData>(nuevoEquipo);
    public bool EsEntrenadorActivo() => esEntrenador;
}