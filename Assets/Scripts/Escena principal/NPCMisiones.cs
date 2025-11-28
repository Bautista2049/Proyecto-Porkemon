using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class NPCMisiones : MonoBehaviour
{
    [Header("Diálogos")]
    [Tooltip("Los diálogos ahora se manejan desde el ControladorMisiones")]
    [SerializeField] private bool usarDialogosPorDefecto = true;

    [Header("UI")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public Button botonContinuar;
    public Button botonAceptarMision;
    public Button botonRechazarMision;

    [Header("UI Slider Progreso")]
    public GameObject panelProgreso;
    public Slider sliderProgreso;
    public TextMeshProUGUI textoProgreso;

    [Header("Configuración")]
    public float velocidadEscritura = 0.05f;
    public KeyCode teclaInteraccion = KeyCode.E;

    private bool jugadorCerca = false;
    private bool enDialogo = false;
    private int indiceDialogoActual = 0;
    private string[] dialogoActual;

    private ControladorMisiones controladorMisiones;

    private void Start()
    {
        // Asegurarse de que el controlador de misiones esté inicializado
        if (controladorMisiones == null)
        {
            controladorMisiones = ControladorMisiones.Instancia;
        }

        // Suscribirse al evento de carga de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Inicializar referencias
        InicializarReferencias();
        
        // Configurar botones
        ConfigurarBotones();
        
        // Actualizar UI basado en el estado de la misión
        ActualizarEstadoUI();

        // Inicializar diálogos por defecto si no están configurados
        if (dialogoInicial.Length == 0)
        {
            dialogoInicial = new string[]
            {
                "Hola viajero",
                "No pareces de estos lares",
                "Si ayudas con un problema que tengo te compensaré",
                "Necesito que derrotes o captures cinco pokemones salvajes",
                "Hazlo y te daré algo que te será muy útil en tu aventura"
            };
        }

        if (dialogoMisionActiva.Length == 0)
        {
            dialogoMisionActiva = new string[]
            {
                "Sigue adelante, viajero",
                "Recuerda que necesito que derrotes o captures cinco pokemones salvajes",
                "Vuelve cuando hayas completado tu tarea"
            };
        }

        if (dialogoMisionCompletada.Length == 0)
        {
            dialogoMisionCompletada = new string[]
            {
                "¡Excelente trabajo, viajero!",
                "Has completado la misión que te encomendé",
                "Como prometí, aquí tienes tu recompensa",
                "Te daré una Ultrabola y un Charmander que te serán de gran ayuda"
            };
        }

        if (dialogoRecompensaEntregada.Length == 0)
        {
            dialogoRecompensaEntregada = new string[]
            {
                "Gracias por tu ayuda, viajero",
                "Espero que esta Ultrabola y tu nuevo Charmander te sirvan bien en tu aventura",
                "¡Buena suerte en tu viaje!"
            };
        }

        // Ocultar UI al inicio
        if (panelDialogo != null)
            panelDialogo.SetActive(false);

        if (panelProgreso != null)
            panelProgreso.SetActive(false);

        // Configurar botones
        if (botonContinuar != null)
            botonContinuar.onClick.AddListener(() => SiguienteDialogo());

        if (botonAceptarMision != null)
            botonAceptarMision.onClick.AddListener(AceptarMision);

        if (botonRechazarMision != null)
            botonRechazarMision.onClick.AddListener(RechazarMision);

        // Actualizar slider si está activo
        ActualizarSliderProgreso();
    }

    private void OnEnable()
    {
        // Asegurarse de que el controlador de misiones esté inicializado
        if (controladorMisiones == null)
        {
            controladorMisiones = ControladorMisiones.Instancia;
        }
    }

    private void OnDisable()
    {
        // Limpiar referencias
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reasignar referencias cuando la escena se carga
        InicializarReferencias();
        ConfigurarBotones();
        ActualizarEstadoUI();
    }

    private void InicializarReferencias()
    {
        // Buscar referencias si no están asignadas
        if (panelDialogo == null) panelDialogo = GameObject.Find("PanelDialogo");
        if (textoDialogo == null && panelDialogo != null) 
            textoDialogo = panelDialogo.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (panelProgreso == null) panelProgreso = GameObject.Find("PanelProgreso");
        if (sliderProgreso == null && panelProgreso != null)
            sliderProgreso = panelProgreso.GetComponentInChildren<Slider>();
    }

    private void ConfigurarBotones()
    {
        if (botonContinuar != null)
        {
            botonContinuar.onClick.RemoveAllListeners();
            botonContinuar.onClick.AddListener(SiguienteDialogo);
        }
        
        if (botonAceptarMision != null)
        {
            botonAceptarMision.onClick.RemoveAllListeners();
            botonAceptarMision.onClick.AddListener(AceptarMision);
        }
        
        if (botonRechazarMision != null)
        {
            botonRechazarMision.onClick.RemoveAllListeners();
            botonRechazarMision.onClick.AddListener(RechazarMision);
        }
    }

    private void ActualizarEstadoUI()
    {
        if (controladorMisiones == null) return;
        
        // Actualizar visibilidad del panel de progreso
        if (controladorMisiones.misionAceptada && !controladorMisiones.misionCompletada)
        {
            MostrarSliderProgreso(true);
            ActualizarSliderProgreso();
        }
        else
        {
            MostrarSliderProgreso(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && controladorMisiones != null)
        {
            jugadorCerca = true;
            Debug.Log("Jugador cerca del NPC");
            
            // Mostrar el diálogo apropiado basado en el estado de la misión
            if (controladorMisiones.recompensaEntregada)
            {
                MostrarDialogo(dialogoRecompensaEntregada);
            }
            else if (controladorMisiones.misionCompletada)
            {
                MostrarDialogo(dialogoMisionCompletada);
            }
            else if (controladorMisiones.misionAceptada)
            {
                MostrarDialogo(dialogoMisionActiva);
            }
            else
            {
                MostrarDialogo(dialogoInicial);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (enDialogo)
            {
                CerrarDialogo();
            }
        }
    }

    private void MostrarDialogo(string[] dialogo)
    {
        if (panelDialogo == null || controladorMisiones == null)
        {
            Debug.LogError("¡Error! Referencias faltantes en MostrarDialogo");
            return;
        }

        enDialogo = true;
        panelDialogo.SetActive(true);
        controladorMisiones.panelMisionesVisible = true;
        
        // Determinar si es el último diálogo de la secuencia inicial
        bool esMision = (dialogo == controladorMisiones.dialogos.dialogoInicial && 
                         indiceDialogoActual == controladorMisiones.dialogos.dialogoInicial.Length - 1);
        
        if (botonContinuar != null) botonContinuar.gameObject.SetActive(!esMision);
        if (botonAceptarMision != null) botonAceptarMision.gameObject.SetActive(esMision);
        if (botonRechazarMision != null) botonRechazarMision.gameObject.SetActive(esMision);
        
        dialogoActual = dialogo;
        StartCoroutine(MostrarTexto(dialogoActual[indiceDialogoActual]));
    }

    private IEnumerator MostrarTexto(string texto)
    {
        textoDialogo.text = "";
        foreach (char letra in texto)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }
    }

    private void SiguienteDialogo()
    {
        if (dialogoActual == null || controladorMisiones == null) return;
        
        indiceDialogoActual++;

        if (indiceDialogoActual >= dialogoActual.Length)
        {
            CerrarDialogo();
        }
        else
        {
            // Verificar si es el último diálogo de la secuencia
            bool esUltimoDialogo = (dialogoActual == controladorMisiones.dialogos.dialogoInicial && 
                                   indiceDialogoActual == controladorMisiones.dialogos.dialogoInicial.Length - 1);
            
            if (esUltimoDialogo)
            {
                // Mostrar botones de aceptar/rechazar
                if (botonContinuar != null) botonContinuar.gameObject.SetActive(false);
                if (botonAceptarMision != null) botonAceptarMision.gameObject.SetActive(true);
                if (botonRechazarMision != null) botonRechazarMision.gameObject.SetActive(true);
            }
            
            StartCoroutine(MostrarTexto(dialogoActual[indiceDialogoActual]));
        }
    }

    private void AceptarMision()
    {
        controladorMisiones.AceptarMision();
        CerrarDialogo();
        MostrarSliderProgreso(true);
    }

    private void RechazarMision()
    {
        CerrarDialogo();
    }

    private void CerrarDialogo()
    {
        enDialogo = false;
        panelDialogo.SetActive(false);
        controladorMisiones.panelMisionesVisible = false;
        textoDialogo.text = "";
        MostrarSliderProgreso(false);
    }

    private void MostrarSliderProgreso(bool mostrar)
    {
        if (panelProgreso == null || controladorMisiones == null) 
        {
            Debug.LogWarning("Referencias faltantes en MostrarSliderProgreso");
            return;
        }
        
        bool debeMostrar = mostrar && controladorMisiones.misionAceptada && !controladorMisiones.misionCompletada;
        panelProgreso.SetActive(debeMostrar);
        
        if (debeMostrar)
        {
            ActualizarSliderProgreso();
        }
    }

    private void ActualizarSliderProgreso()
    {
        if (sliderProgreso != null && textoProgreso != null)
        {
            float progreso = (float)controladorMisiones.pokemonesDerrotados / ControladorMisiones.POKEMONES_NECESARIOS;
            sliderProgreso.value = progreso;
            textoProgreso.text = $"{controladorMisiones.pokemonesDerrotados}/{ControladorMisiones.POKEMONES_NECESARIOS} Pokémon";

            // Ocultar slider si la misión está completada
            if (controladorMisiones.misionCompletada)
            {
                MostrarSliderProgreso(false);
            }
        }
    }

    private void IniciarDialogo()
    {
        if (controladorMisiones == null)
        {
            Debug.LogError("ControladorMisiones no encontrado");
            return;
        }

        Debug.Log("Iniciando diálogo...");
        if (controladorMisiones.recompensaEntregada)
        {
            Debug.Log("Mostrando diálogo de recompensa entregada");
            MostrarDialogo(controladorMisiones.dialogos.dialogoRecompensaEntregada);
        }
        else if (controladorMisiones.misionCompletada)
        {
            MostrarDialogo(controladorMisiones.dialogos.dialogoMisionCompletada);
        }
        else if (controladorMisiones.misionAceptada)
        {
            MostrarDialogo(controladorMisiones.dialogos.dialogoMisionActiva);
        }
        else
        {
            MostrarDialogo(controladorMisiones.dialogos.dialogoInicial);
        }
    }

    void Update()
    {
        if (controladorMisiones == null) return;
        
        if (jugadorCerca && !enDialogo && Input.GetKeyDown(teclaInteraccion))
        {
            Debug.Log("Tecla E presionada cerca del NPC");
            IniciarDialogo();
        }
        else if (enDialogo && Input.GetKeyDown(KeyCode.Space))
        {
            SiguienteDialogo();
        }

        // Actualizar slider continuamente si está activo
        if (controladorMisiones.misionAceptada && !controladorMisiones.misionCompletada)
        {
            ActualizarSliderProgreso();
        }
    }
}
