using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NPCPatrulla : MonoBehaviour
{
    [Header("Configuración de Patrulla")]
    public List<Transform> checkpoints = new List<Transform>();
    public float velocidadPatrulla = 3f;
    public float distanciaMinima = 0.5f;
    
    [Header("Configuración de Persecución")]
    public float rangoPersecucion = 10f;
    public float velocidadPersecucion = 6f;
    public Transform jugador;
    
    [Header("Mensajes")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI popupText;
    public string mensajePrimeraVez = "¡Bienvenido a Pueblo Porkemon! Atrapa a todos los Porkemons para ganar.";
    public string mensajeVictoria = "¡Felicidades! Has capturado a todos los Porkemons. ¡Eres el campeón!";
    
    private int checkpointActual = 0;
    private bool jugadorAtrapado = false;
    private bool mostrandoMensaje = false;
    private bool npcDesactivado = false;
    
    void Start()
    {
        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                jugador = playerObj.transform;
            }
        }
        
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }
    
    void Update()
    {
        if (GameState.TodosLosPokemonCapturados())
        {
            npcDesactivado = false;
        }
        
        if (npcDesactivado || jugadorAtrapado || mostrandoMensaje)
        {
            return;
        }
        
        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                jugador = playerObj.transform;
            }
            return;
        }
        
        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);
        
        if (distanciaAlJugador <= rangoPersecucion)
        {
            Perseguir();
        }
        else
        {
            Patrullar();
        }
    }
    
    void Patrullar()
    {
        if (checkpoints.Count == 0)
            return;
        
        if (checkpointActual >= checkpoints.Count)
            checkpointActual = 0;
        
        Transform targetCheckpoint = checkpoints[checkpointActual];
        
        if (targetCheckpoint == null)
        {
            checkpointActual++;
            return;
        }
        
        Vector3 targetPos = targetCheckpoint.position;
        Vector3 currentPos = transform.position;
        currentPos.y = targetPos.y;
        
        Vector3 direccion = (targetPos - currentPos).normalized;
        
        if (direccion.magnitude > 0.01f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 5f);
        }
        
        transform.position = Vector3.MoveTowards(transform.position, targetPos, velocidadPatrulla * Time.deltaTime);
        
        float distanciaAlCheckpoint = Vector3.Distance(transform.position, targetPos);
        
        if (distanciaAlCheckpoint < distanciaMinima)
        {
            checkpointActual++;
            if (checkpointActual >= checkpoints.Count)
            {
                checkpointActual = 0;
            }
        }
    }
    
    void Perseguir()
    {
        Vector3 direccion = (jugador.position - transform.position).normalized;
        direccion.y = 0;
        
        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 7f);
        }
        
        transform.position += direccion * velocidadPersecucion * Time.deltaTime;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !jugadorAtrapado)
        {
            jugadorAtrapado = true;
            StartCoroutine(ManejadorColisionJugador());
        }
    }
    
    private IEnumerator ManejadorColisionJugador()
    {
        bool todosCapturados = GameState.TodosLosPokemonCapturados();
        
        if (todosCapturados)
        {
            MostrarMensaje(mensajeVictoria);
            yield return new WaitForSeconds(3f);
            IrAEscenaVictoria();
        }
        else
        {
            if (!GameState.npcYaHablo)
            {
                MostrarMensaje(mensajePrimeraVez);
                GameState.npcYaHablo = true;
                npcDesactivado = true;
            }
            else
            {
                MostrarMensaje("¡Sigue buscando! Aún faltan Porkemons por capturar.");
            }
            
            yield return new WaitForSeconds(3f);
            OcultarMensaje();
        }
        
        jugadorAtrapado = false;
    }
    
    private void MostrarMensaje(string mensaje)
    {
        mostrandoMensaje = true;
        
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }
        
        if (popupText != null)
        {
            popupText.text = mensaje;
        }
    }
    
    private void OcultarMensaje()
    {
        mostrandoMensaje = false;
        
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }
    
    private void IrAEscenaVictoria()
    {
        GameState.nombreGanador = "Jugador";
        GameState.victoriaFueCaptura = false;
        
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("Interfaz de Menu");
        }
        else
        {
            SceneManager.LoadScene("Interfaz de Menu");
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoPersecucion);
        
        if (checkpoints.Count > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < checkpoints.Count; i++)
            {
                if (checkpoints[i] != null)
                {
                    Gizmos.DrawSphere(checkpoints[i].position, 0.3f);
                    
                    if (i < checkpoints.Count - 1 && checkpoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(checkpoints[i].position, checkpoints[i + 1].position);
                    }
                }
            }
            
            if (checkpoints[checkpoints.Count - 1] != null && checkpoints[0] != null)
            {
                Gizmos.DrawLine(checkpoints[checkpoints.Count - 1].position, checkpoints[0].position);
            }
        }
    }
}
