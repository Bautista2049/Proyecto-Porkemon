using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // O using UnityEngine.UI; si usas UI Text normal

public class SistemaCuracion : MonoBehaviour
{
    // Define el comportamiento de esta instancia del script
    public enum Modo { TriggerEntrada, NPCInteraccion, TriggerSalida }
    [Header("Configuración de Modo")]
    public Modo modo;

    [Header("Configuración de Escenas")]
    public string nombreEscenaCuracion = "EscenaDeCuracion";
    public string nombreEscenaAnterior = "EscenaPrincipal";

    [Header("Configuración del NPC (Solo para modo NPC)")]
    public GameObject panelDialogo; // Panel principal con el texto de curación
    public TextMeshProUGUI textoDialogo;
    public GameObject panelInteraccionPrompt; // Objeto con el aviso para interactuar (ej. "Pulsa E")
    public KeyCode teclaInteraccion = KeyCode.E;
    private bool interaccionActiva = false;
    private bool curacionEnProgreso = false;

    void Start()
    {
        // Nos aseguramos de que los paneles estén ocultos al iniciar
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (panelInteraccionPrompt != null) panelInteraccionPrompt.SetActive(false);
    }

    // --- Lógica de Triggers ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que tu jugador tenga el tag "Player"
        {
            if (modo == Modo.TriggerEntrada)
            {
                SceneManager.LoadScene(nombreEscenaCuracion);
            }
            else if (modo == Modo.TriggerSalida)
            {
                SceneManager.LoadScene(nombreEscenaAnterior);
            }
            else if (modo == Modo.NPCInteraccion)
            {
                // Cuando el jugador entra en el radio, se activa la posibilidad de interactuar y se muestra el aviso.
                interaccionActiva = true;
                if (panelInteraccionPrompt != null) panelInteraccionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && modo == Modo.NPCInteraccion)
        {
            // Cuando el jugador sale del radio, se desactiva todo.
            interaccionActiva = false;
            if (panelDialogo != null) panelDialogo.SetActive(false);
            if (panelInteraccionPrompt != null) panelInteraccionPrompt.SetActive(false);
        }
    }

    // --- Lógica para la Interacción con el NPC ---
    void Update()
    {
        // Si el jugador está en el área, no hay una curación en proceso y pulsa la tecla E...
        if (modo == Modo.NPCInteraccion && interaccionActiva && !curacionEnProgreso && Input.GetKeyDown(teclaInteraccion))
        {
            // Ocultamos el prompt de interacción para dar paso al panel de curación.
            if (panelInteraccionPrompt != null) panelInteraccionPrompt.SetActive(false);
            StartCoroutine(SecuenciaDeCuracion());
        }
    }

    private IEnumerator SecuenciaDeCuracion()
    {
        curacionEnProgreso = true;

        // 1. Activa el panel de diálogo y muestra el mensaje de "curando"
        if (panelDialogo != null && textoDialogo != null)
        {
            panelDialogo.SetActive(true);
            textoDialogo.text = "Curando los pokemones...";
        }

        // 2. Espera 5 segundos mientras se "realiza" la curación.
        yield return new WaitForSeconds(5f);

        // 3. Ejecuta la lógica de curación real.
        CurarEquipo();

        // 4. Oculta el panel de diálogo.
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }
        
        curacionEnProgreso = false;

        // 5. Si el jugador sigue dentro del área, volvemos a mostrar el aviso para interactuar de nuevo.
        if(interaccionActiva && panelInteraccionPrompt != null)
        {
            panelInteraccionPrompt.SetActive(true);
        }
    }

    private void CurarEquipo()
    {
        if (GestorDeBatalla.instance == null || GestorDeBatalla.instance.equipoJugador == null)
        {
            Debug.LogError("Error: No se pudo encontrar la instancia de GestorDeBatalla o el equipo del jugador.");
            return;
        }

        foreach (var porkemon in GestorDeBatalla.instance.equipoJugador)
        {
            if (porkemon != null)
            {
                // Restaura la salud al máximo.
                porkemon.VidaActual = porkemon.VidaMaxima;
                
                // Elimina estados alterados.
                porkemon.Estado = EstadoAlterado.Ninguno;

                // ¡IMPORTANTE! Para restaurar los PP, necesitas modificar tu sistema.
                // La clase `AtaqueData` es un ScriptableObject y no puede guardar los PP actuales de un solo Porkemon.
                // Debes crear una nueva clase (ej: `AtaqueEnUso`) que contenga el `AtaqueData` y una variable `ppActuales`.
                // Luego, en tu clase `Porkemon`, la lista `Ataques` debería ser de esa nueva clase.
                // Ejemplo de cómo se haría:
                // foreach (var ataque in porkemon.Ataques) 
                // { 
                //     ataque.ppActuales = ataque.AtaqueData.pp;
                // }
            }
        }

        Debug.Log("¡El equipo del jugador ha sido curado por completo!");
    }
}