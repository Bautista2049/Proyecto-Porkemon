using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BedInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public Transform camaPosicionDormir;   // Lugar exacto donde se acuesta el jugador
    public Transform jugador;              // Transform del jugador

    [Header("UI")]
    public GameObject panelDormir;         // Panel que muestra el texto "Dormir (E)"
    public TMP_Text textoDormirTMP;
    public Text textoDormirUI;
    public string mensajeDormir = "Dormir (E)";
    public string mensajeNoEsNoche = "No es de noche todavia";
    public float duracionMensajeNoEsNoche = 2f;

    [Header("Configuración")]
    public KeyCode teclaDormir = KeyCode.E;
    public float duracionSimulacionSueño = 2f; // segundos que dura la animación de dormir
    public float distanciaMaximaInteraccion = 3f;

    private bool estaDurmiendo = false;
    private Vector3 posicionOriginalJugador;
    private bool mostrandoMensajeTemporal;
    private Coroutine mensajeCoroutine;

    private void Start()
    {
        if (panelDormir != null)
        {
            panelDormir.SetActive(false);
            if (textoDormirTMP == null)
                textoDormirTMP = panelDormir.GetComponentInChildren<TMP_Text>(true);
            if (textoDormirUI == null)
                textoDormirUI = panelDormir.GetComponentInChildren<Text>(true);
        }

        SetMensaje(mensajeDormir);
    }

    private void Update()
    {
        if (estaDurmiendo)
            return;

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

        bool tieneManager = TimeOfDayManager.Instance != null;
        bool puedeDormir = tieneManager && TimeOfDayManager.Instance.PuedeDormir();

        if (panelDormir != null)
        {
            panelDormir.SetActive(mirandoALaCama);
            if (mirandoALaCama && !mostrandoMensajeTemporal)
                SetMensaje(mensajeDormir);
        }

        if (!mirandoALaCama)
        {
            ResetMensajeTemporal();
            return;
        }

        if (!tieneManager)
            return;

        if (Input.GetKeyDown(teclaDormir))
        {
            if (puedeDormir)
            {
                StartCoroutine(CorutinaDormir());
            }
            else
            {
                MostrarMensajeNoEsNoche();
            }
        }
    }

    private IEnumerator CorutinaDormir()
    {
        estaDurmiendo = true;

        if (jugador == null || camaPosicionDormir == null)
        {
            yield break;
        }

        posicionOriginalJugador = jugador.position;
        jugador.position = camaPosicionDormir.position;

        yield return new WaitForSeconds(duracionSimulacionSueño);

        if (TimeOfDayManager.Instance != null)
        {
            TimeOfDayManager.Instance.SkipToMorning();
        }

        jugador.position = posicionOriginalJugador;

        if (panelDormir != null)
            panelDormir.SetActive(false);

        ResetMensajeTemporal();

        estaDurmiendo = false;
    }

    private IEnumerator MostrarMensajeTemporal()
    {
        mostrandoMensajeTemporal = true;
        SetMensaje(mensajeNoEsNoche);
        yield return new WaitForSeconds(duracionMensajeNoEsNoche);
        mostrandoMensajeTemporal = false;
        mensajeCoroutine = null;
        SetMensaje(mensajeDormir);
    }

    private void MostrarMensajeNoEsNoche()
    {
        if (mensajeCoroutine != null)
            StopCoroutine(mensajeCoroutine);

        mensajeCoroutine = StartCoroutine(MostrarMensajeTemporal());
    }

    private void ResetMensajeTemporal()
    {
        if (mensajeCoroutine != null)
        {
            StopCoroutine(mensajeCoroutine);
            mensajeCoroutine = null;
        }

        mostrandoMensajeTemporal = false;
        SetMensaje(mensajeDormir);
    }

    private void SetMensaje(string contenido)
    {
        if (textoDormirTMP != null)
            textoDormirTMP.text = contenido;
        if (textoDormirUI != null)
            textoDormirUI.text = contenido;
    }
}
