using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorNPCEntrenador : MonoBehaviour
{
    [Header("Configuración del Entrenador")]
    [SerializeField] private bool esEntrenador = true; // Checkbox "Es NPC Entrenador"
    [SerializeField] private bool esJefe = false; // Checkbox "Es Jefe" (tiene más de 1 Pokémon)
    [SerializeField] private float rangoDeteccion = 5f; // Rango de visión
    [SerializeField] private float velocidadMovimiento = 2f;
    [SerializeField] private string nombreEscenaCombate = "Escena de Combate";

    [Header("Equipo del NPC")]
    [SerializeField] private List<PorkemonData> equipoBase = new List<PorkemonData>(); // Lista de Pokémon posibles (hasta 6)
    [SerializeField] private int tamanoEquipoAleatorio = 3; // Número de Pokémon a seleccionar aleatoriamente (solo si es Jefe)

    [Header("Referencias")]
    [SerializeField] private SphereCollider triggerDeteccion;
    [SerializeField] private Collider colliderCombate; // Collider para detectar colisión física con el jugador
    [SerializeField] private Animator animator;
    [SerializeField] private Transform puntoMirada;

    private bool jugadorDetectado = false;
    private Transform jugador;
    private bool enMovimiento = false;
    private List<Porkemon> equipoSeleccionado = new List<Porkemon>();

    private void Start()
    {
        if (!esEntrenador)
        {
            enabled = false;
            return;
        }

        // Configurar el trigger de detección
        if (triggerDeteccion == null)
        {
            triggerDeteccion = gameObject.AddComponent<SphereCollider>();
            triggerDeteccion.isTrigger = true;
            triggerDeteccion.radius = rangoDeteccion;
        }
        else
        {
            triggerDeteccion.radius = rangoDeteccion;
            triggerDeteccion.isTrigger = true;
        }

        // Desactivar el trigger inicialmente si es necesario
        triggerDeteccion.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!esEntrenador || jugadorDetectado) return;

        if (other.CompareTag("Player"))
        {
            jugador = other.transform;
            jugadorDetectado = true;
            Debug.Log("Jugador detectado por NPC Entrenador");

            if (esJefe)
            {
                // Si es Jefe, congelar y esperar colisión
                if (animator != null)
                {
                    animator.SetBool("Walking", false);
                    // Aquí puedes agregar más lógica para detener animaciones específicas
                }
                enMovimiento = false;
                Debug.Log("NPC Jefe congelado esperando colisión con el jugador");
            }
            else
            {
                // Si es Entrenador normal, acercarse al jugador
                StartCoroutine(AproximarseAlJugador());
                Debug.Log("NPC Entrenador comenzando a acercarse al jugador");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Si el jugador sale del rango, resetear estado
            if (jugadorDetectado)
            {
                jugadorDetectado = false;
                jugador = null;
                enMovimiento = false;

                // Detener animación de caminar
                if (animator != null)
                {
                    animator.SetBool("Walking", false);
                }

                Debug.Log("Jugador salió del rango de detección");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!esEntrenador || !jugadorDetectado) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colisión detectada con el jugador - Iniciando combate");
            IniciarCombate();
        }
    }

    private IEnumerator AproximarseAlJugador()
    {
        enMovimiento = true;

        while (jugadorDetectado && jugador != null)
        {
            // Calcular distancia al jugador
            float distancia = Vector3.Distance(transform.position, jugador.position);

            // Si estamos lo suficientemente cerca, iniciar combate
            if (distancia <= 2f) // Distancia adecuada para combate
            {
                IniciarCombate();
                yield break;
            }

            // Moverse hacia el jugador
            Vector3 direccion = (jugador.position - transform.position).normalized;
            direccion.y = 0; // Mantener en plano horizontal

            transform.position += direccion * velocidadMovimiento * Time.deltaTime;

            // Rotar hacia el jugador
            if (direccion != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 5f);
            }

            // Actualizar animación
            if (animator != null)
            {
                animator.SetBool("Walking", true);
            }

            yield return null;
        }

        enMovimiento = false;

        // Resetear animación si ya no nos movemos
        if (animator != null)
        {
            animator.SetBool("Walking", false);
        }
    }

    private void IniciarCombate()
    {
        Debug.Log("Iniciando combate con NPC Entrenador");

        // Seleccionar equipo aleatorio
        SeleccionarEquipoAleatorio();

        // Configurar el combate
        if (GestorDeBatalla.instance != null)
        {
            GestorDeBatalla.instance.ConfigurarEquipoNPC(equipoSeleccionado);
            GameState.esCombateBoss = true; // Marcar como combate contra entrenador
        }

        // Guardar posición del jugador
        GameState.GuardarPosicionJugador(jugador.position, SceneManager.GetActiveScene().name);

        // Cargar escena de combate
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            DontDestroyOnLoad(mainCamera.gameObject);
        }

        SceneManager.LoadScene(nombreEscenaCombate);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SeleccionarEquipoAleatorio()
    {
        equipoSeleccionado.Clear();

        if (equipoBase.Count == 0) return;

        // Crear una lista temporal con los Pokémon disponibles
        List<PorkemonData> disponibles = new List<PorkemonData>(equipoBase);

        // Determinar cuántos Pokémon seleccionar basado en si es Jefe o no
        int cantidadSeleccionar;
        if (esJefe)
        {
            // Si es Jefe, seleccionar aleatoriamente entre 2 y el máximo disponible
            cantidadSeleccionar = Mathf.Min(tamanoEquipoAleatorio, disponibles.Count);
            cantidadSeleccionar = Mathf.Max(2, cantidadSeleccionar); // Mínimo 2 para jefes
        }
        else
        {
            // Si es Entrenador normal (no Jefe), solo 1 Pokémon
            cantidadSeleccionar = 1;
        }

        for (int i = 0; i < cantidadSeleccionar; i++)
        {
            if (disponibles.Count == 0) break;

            // Seleccionar aleatoriamente
            int indiceAleatorio = Random.Range(0, disponibles.Count);
            PorkemonData seleccionado = disponibles[indiceAleatorio];

            // Crear instancia del Pokémon
            Porkemon nuevoPokemon = new Porkemon(seleccionado, seleccionado.nivel);
            equipoSeleccionado.Add(nuevoPokemon);

            // Remover de la lista de disponibles para no repetir
            disponibles.RemoveAt(indiceAleatorio);
        }

        string tipoNPC = esJefe ? "Jefe" : "Entrenador";
        Debug.Log($"Equipo seleccionado para NPC {tipoNPC}: {equipoSeleccionado.Count} Pokémon");
        foreach (var pokemon in equipoSeleccionado)
        {
            Debug.Log($"- {pokemon.BaseData.nombre} (Nivel {pokemon.Nivel})");
        }
    }

    // Método público para configurar el equipo desde el editor
    public void SetEquipoBase(List<PorkemonData> nuevoEquipo)
    {
        equipoBase = new List<PorkemonData>(nuevoEquipo);
    }

    // Método para verificar si el NPC está activo como entrenador
    public bool EsEntrenadorActivo()
    {
        return esEntrenador;
    }
}
