using System.Collections;
using UnityEngine;

public class BedInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public Transform camaPosicionDormir;   // Lugar exacto donde se acuesta el jugador
    public Transform jugador;              // Transform del jugador

    [Header("UI")]
    public GameObject panelDormir;         // Panel que muestra el texto "Dormir (E)"

    [Header("Configuración")]
    public KeyCode teclaDormir = KeyCode.E;
    public float duracionSimulacionSueño = 2f; // segundos que dura la animación de dormir
    public float distanciaMaximaInteraccion = 3f;

    private bool estaDurmiendo = false;
    private Vector3 posicionOriginalJugador;

    private void Start()
    {
        if (panelDormir != null)
            panelDormir.SetActive(false);
    }

    private void Update()
    {
        if (estaDurmiendo)
            return;

        if (jugador == null)
            return;

        // Raycast desde el jugador hacia adelante para detectar la cama
        bool mirandoALaCama = false;
        RaycastHit hit;
        Vector3 origen = jugador.position + Vector3.up * 1f;
        Vector3 direccion = jugador.forward;

        if (Physics.Raycast(origen, direccion, out hit, distanciaMaximaInteraccion))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                mirandoALaCama = true;
            }
        }

        // Mostrar u ocultar el panel según si el raycast está sobre la cama
        if (panelDormir != null)
            panelDormir.SetActive(mirandoALaCama);

        if (!mirandoALaCama || TimeOfDayManager.Instance == null)
            return;

        bool puedeDormir = TimeOfDayManager.Instance.PuedeDormir();

        if (puedeDormir && Input.GetKeyDown(teclaDormir))
        {
            StartCoroutine(CorutinaDormir());
        }
    }

    private IEnumerator CorutinaDormir()
    {
        estaDurmiendo = true;

        if (jugador == null || camaPosicionDormir == null)
        {
            yield break;
        }

        // Guardar posición original donde interactuó con la cama
        posicionOriginalJugador = jugador.position;

        // Mover al jugador a la posición de la cama
        jugador.position = camaPosicionDormir.position;

        // Aquí podrías desactivar el movimiento del jugador / reproducir animación

        // Esperar simulando que duerme
        yield return new WaitForSeconds(duracionSimulacionSueño);

        // Saltar a la mañana usando el gestor de tiempo global
        if (TimeOfDayManager.Instance != null)
        {
            TimeOfDayManager.Instance.SkipToMorning();
        }

        // Devolver al jugador a la posición original
        jugador.position = posicionOriginalJugador;

        if (panelDormir != null)
            panelDormir.SetActive(false);

        estaDurmiendo = false;
    }

    // Ya no usamos triggers; toda la interacción es por raycast desde el jugador.
}
