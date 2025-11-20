using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class InteraccionTiendaRaycast : MonoBehaviour
{
    public Transform jugador;
    public GameObject panelInteraccion;
    public TMP_Text textoInteraccionTMP;
    public Text textoInteraccionUI;
    public string mensajeInteractuar = "Tienda (E)";
    public string mensajeNoAlcanza = "No te alcanza el dinero";
    public KeyCode teclaInteractuar = KeyCode.E;
    public float distanciaMaximaInteraccion = 3f;
    public int costoEntrada = 0;
    public string nombreEscenaTienda = "Escena Tienda";

    private bool mostrandoMensajeTemporal;
    private float tiempoMensajeRestante;

    private void Start()
    {
        if (panelInteraccion != null)
        {
            panelInteraccion.SetActive(false);
            if (textoInteraccionTMP == null)
                textoInteraccionTMP = panelInteraccion.GetComponentInChildren<TMP_Text>(true);
            if (textoInteraccionUI == null)
                textoInteraccionUI = panelInteraccion.GetComponentInChildren<Text>(true);
        }

        SetMensaje(mensajeInteractuar);
    }

    private void Update()
    {
        if (jugador == null)
            return;

        bool mirandoATienda = false;
        RaycastHit hit;
        Vector3 origen = jugador.position + Vector3.up * 1f;
        Vector3 direccion = jugador.forward;

        if (Physics.Raycast(origen, direccion, out hit, distanciaMaximaInteraccion))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                mirandoATienda = true;
            }
        }

        if (panelInteraccion != null)
        {
            panelInteraccion.SetActive(mirandoATienda);
            if (mirandoATienda && !mostrandoMensajeTemporal)
                SetMensaje(mensajeInteractuar);
        }

        if (!mirandoATienda)
        {
            ResetMensajeTemporal();
            return;
        }

        if (Input.GetKeyDown(teclaInteractuar))
        {
            if (GameState.dineroJugador >= costoEntrada)
            {
                if (costoEntrada > 0)
                    GameState.dineroJugador -= costoEntrada;

                GameState.modoTienda = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SceneManager.LoadScene(nombreEscenaTienda);
            }
            else
            {
                MostrarMensajeNoAlcanza();
            }
        }

        if (mostrandoMensajeTemporal)
        {
            tiempoMensajeRestante -= Time.deltaTime;
            if (tiempoMensajeRestante <= 0f)
            {
                mostrandoMensajeTemporal = false;
                SetMensaje(mensajeInteractuar);
            }
        }
    }

    private void MostrarMensajeNoAlcanza()
    {
        mostrandoMensajeTemporal = true;
        tiempoMensajeRestante = 2f;
        SetMensaje(mensajeNoAlcanza);
    }

    private void ResetMensajeTemporal()
    {
        mostrandoMensajeTemporal = false;
        tiempoMensajeRestante = 0f;
        SetMensaje(mensajeInteractuar);
    }

    private void SetMensaje(string contenido)
    {
        if (textoInteraccionTMP != null)
            textoInteraccionTMP.text = contenido;
        if (textoInteraccionUI != null)
            textoInteraccionUI.text = contenido;
    }
}
