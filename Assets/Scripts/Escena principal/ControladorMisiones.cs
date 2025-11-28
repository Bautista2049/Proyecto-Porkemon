using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogosMision
{
    [TextArea(3, 5)] public string[] dialogoInicial;
    [TextArea(3, 5)] public string[] dialogoMisionActiva;
    [TextArea(3, 5)] public string[] dialogoMisionCompletada;
    [TextArea(3, 5)] public string[] dialogoRecompensaEntregada;
}

public class ControladorMisiones : MonoBehaviour
{
    #region Singleton
    private static ControladorMisiones _instancia;
    public static ControladorMisiones Instancia 
    { 
        get
        {
            if (_instancia == null)
            {
                _instancia = FindObjectOfType<ControladorMisiones>();
                if (_instancia == null)
                {
                    GameObject go = new GameObject("ControladorMisiones");
                    _instancia = go.AddComponent<ControladorMisiones>();
                    DontDestroyOnLoad(go);
                    _instancia.InicializarDialogosPorDefecto();
                }
            }
            return _instancia;
        } 
    }
    #endregion

    #region Estado de la Misión
    public bool misionAceptada = false;
    public bool misionCompletada = false;
    public bool recompensaEntregada = false;
    public int pokemonesDerrotados = 0;
    public const int POKEMONES_NECESARIOS = 5;
    public bool panelMisionesVisible = false;
    #endregion

    #region Diálogos
    public DialogosMision dialogos = new DialogosMision();
    #endregion

    #region UI Diálogo
    [Header("UI Diálogo")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public Button botonContinuar;
    public Button botonAceptarMision;
    public Button botonRechazarMision;
    #endregion

    #region UI Progreso
    [Header("UI Progreso")]
    public GameObject panelProgreso;
    public Slider sliderProgreso;
    public TextMeshProUGUI textoProgreso;
    public Image rellenoSlider;
    public TextMeshProUGUI tituloMision;
    #endregion

    #region Configuración
    [Header("Configuración")]
    public float velocidadEscritura = 0.05f;
    public KeyCode teclaInteraccion = KeyCode.E;
    public Color colorInicio = Color.red;
    public Color colorMitad = Color.yellow;
    public Color colorFinal = Color.green;
    public bool animarSlider = true;
    public float velocidadAnimacion = 2f;
    #endregion

    #region Variables Privadas
    private bool jugadorCerca = false;
    private bool enDialogo = false;
    private int indiceDialogoActual = 0;
    private string[] dialogoActual;
    private float sliderActual = 0f;
    private float sliderObjetivo = 0f;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (_instancia == null)
        {
            _instancia = this;
            DontDestroyOnLoad(gameObject);
            InicializarDialogosPorDefecto();
        }
        else if (_instancia != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InicializarReferencias();
        ConfigurarBotones();
        ActualizarEstadoUI();
        OcultarUIAlInicio();
    }

    private void OnEnable()
    {
        if (_instancia == null)
        {
            _instancia = this;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InicializarReferencias();
        ConfigurarBotones();
        ActualizarEstadoUI();
    }

    private void Update()
    {
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
        if (misionAceptada && !misionCompletada)
        {
            ActualizarSliderProgreso();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            Debug.Log("Jugador cerca del NPC");
            MostrarDialogoApropiado();
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
    #endregion

    #region Inicialización
    private void InicializarDialogosPorDefecto()
    {
        if (dialogos.dialogoInicial == null || dialogos.dialogoInicial.Length == 0)
        {
            dialogos.dialogoInicial = new string[]
            {
                "Hola viajero",
                "No pareces de estos lares",
                "Si ayudas con un problema que tengo te compensaré",
                "Necesito que derrotes o captures cinco pokemones salvajes",
                "Hazlo y te daré algo que te será muy útil en tu aventura"
            };
        }

        if (dialogos.dialogoMisionActiva == null || dialogos.dialogoMisionActiva.Length == 0)
        {
            dialogos.dialogoMisionActiva = new string[]
            {
                "Sigue adelante, viajero",
                "Recuerda que necesito que derrotes o captures cinco pokemones salvajes",
                "Vuelve cuando hayas completado tu tarea"
            };
        }

        if (dialogos.dialogoMisionCompletada == null || dialogos.dialogoMisionCompletada.Length == 0)
        {
            dialogos.dialogoMisionCompletada = new string[]
            {
                "¡Excelente trabajo, viajero!",
                "Has completado la misión que te encomendé",
                "Como prometí, aquí tienes tu recompensa",
                "Te daré una Ultrabola y un Charmander que te serán de gran ayuda"
            };
        }

        if (dialogos.dialogoRecompensaEntregada == null || dialogos.dialogoRecompensaEntregada.Length == 0)
        {
            dialogos.dialogoRecompensaEntregada = new string[]
            {
                "Gracias por tu ayuda, viajero",
                "Espero que esta Ultrabola y tu nuevo Charmander te sirvan bien en tu aventura",
                "¡Buena suerte en tu viaje!"
            };
        }
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
        if (textoProgreso == null && panelProgreso != null)
            textoProgreso = panelProgreso.GetComponentInChildren<TextMeshProUGUI>();
        if (rellenoSlider == null && sliderProgreso != null)
            rellenoSlider = sliderProgreso.fillRect.GetComponent<Image>();
        if (tituloMision == null && panelProgreso != null)
            tituloMision = panelProgreso.GetComponentInChildren<TextMeshProUGUI>();
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
            botonAceptarMision.onClick.AddListener(AceptarMisionUI);
        }
        
        if (botonRechazarMision != null)
        {
            botonRechazarMision.onClick.RemoveAllListeners();
            botonRechazarMision.onClick.AddListener(RechazarMision);
        }
    }

    private void OcultarUIAlInicio()
    {
        if (panelDialogo != null)
            panelDialogo.SetActive(false);

        if (panelProgreso != null)
            panelProgreso.SetActive(false);

        // Inicializar slider si existe
        if (sliderProgreso != null)
        {
            sliderProgreso.minValue = 0f;
            sliderProgreso.maxValue = 1f;
            sliderProgreso.value = 0f;
        }
    }
    #endregion

    #region Gestión de Misiones
    public void AceptarMision()
    {
        misionAceptada = true;
        misionCompletada = false;
        recompensaEntregada = false;
        pokemonesDerrotados = 0;
        Debug.Log("Misión aceptada: Derrotar o captura 5 Pokémon salvajes");
    }

    public void CompletarMision()
    {
        misionCompletada = true;
        Debug.Log("¡Misión completada! Habla con el NPC para recibir tu recompensa");
    }

    public void EntregarRecompensa()
    {
        if (misionCompletada && !recompensaEntregada)
        {
            recompensaEntregada = true;
            DarRecompensas();
            Debug.Log("Recompensa entregada");
        }
    }

    public void IncrementarPokemonesDerrotados()
    {
        if (misionAceptada && !misionCompletada)
        {
            pokemonesDerrotados++;
            Debug.Log($"Progreso misión: {pokemonesDerrotados}/{POKEMONES_NECESARIOS} Pokémon");

            if (pokemonesDerrotados >= POKEMONES_NECESARIOS)
            {
                CompletarMision();
            }
        }
    }

    private void DarRecompensas()
    {
        // Dar Ultrabola
        var gestor = GestorDeBatalla.instance;
        if (gestor != null)
        {
            var ultrabola = new BattleItem(BattleItemType.Ultrabola, "Ultrabola", "Muy efectiva para Pokémon difíciles de atrapar", 1);
            gestor.inventarioCompleto.Add(ultrabola);
            Debug.Log("Has recibido 1 Ultrabola");

            // Dar Charmander
            var charmanderData = Resources.Load<PorkemonData>("Porkemones/Charmander");
            if (charmanderData != null)
            {
                var charmander = new Porkemon(charmanderData, 5); // Nivel 5
                gestor.equipoJugador.Add(charmander);
                Debug.Log("Has recibido un Charmander nivel 5");
            }
            else
            {
                Debug.LogError("No se encontró el PorkemonData de Charmander en Resources/Porkemones/");
            }
        }
        else
        {
            Debug.LogWarning("GestorDeBatalla.instance no encontrado para dar recompensas");
        }
    }

    public float GetProgresoNormalizado()
    {
        return (float)pokemonesDerrotados / POKEMONES_NECESARIOS;
    }

    public void ReiniciarMision()
    {
        misionAceptada = false;
        misionCompletada = false;
        recompensaEntregada = false;
        pokemonesDerrotados = 0;
        sliderActual = 0f;
        sliderObjetivo = 0f;
        if (sliderProgreso != null)
            sliderProgreso.value = 0f;
        Debug.Log("Misión reiniciada");
    }
    #endregion

    #region Sistema de Diálogos
    private void MostrarDialogoApropiado()
    {
        if (recompensaEntregada)
        {
            MostrarDialogo(dialogos.dialogoRecompensaEntregada);
        }
        else if (misionCompletada)
        {
            MostrarDialogo(dialogos.dialogoMisionCompletada);
        }
        else if (misionAceptada)
        {
            MostrarDialogo(dialogos.dialogoMisionActiva);
        }
        else
        {
            MostrarDialogo(dialogos.dialogoInicial);
        }
    }

    private void MostrarDialogo(string[] dialogo)
    {
        if (panelDialogo == null)
        {
            Debug.LogError("¡Error! Panel de diálogo no encontrado");
            return;
        }

        enDialogo = true;
        panelDialogo.SetActive(true);
        panelMisionesVisible = true;
        
        // Determinar si es el último diálogo de la secuencia inicial
        bool esMision = (dialogo == dialogos.dialogoInicial && 
                         indiceDialogoActual == dialogos.dialogoInicial.Length - 1);
        
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
        if (dialogoActual == null) return;
        
        indiceDialogoActual++;

        if (indiceDialogoActual >= dialogoActual.Length)
        {
            CerrarDialogo();
        }
        else
        {
            // Verificar si es el último diálogo de la secuencia
            bool esUltimoDialogo = (dialogoActual == dialogos.dialogoInicial && 
                                   indiceDialogoActual == dialogos.dialogoInicial.Length - 1);
            
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

    private void AceptarMisionUI()
    {
        AceptarMision();
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
        panelMisionesVisible = false;
        textoDialogo.text = "";
        indiceDialogoActual = 0;
        MostrarSliderProgreso(false);
    }

    private void IniciarDialogo()
    {
        if (recompensaEntregada)
        {
            Debug.Log("Mostrando diálogo de recompensa entregada");
            MostrarDialogo(dialogos.dialogoRecompensaEntregada);
        }
        else if (misionCompletada)
        {
            MostrarDialogo(dialogos.dialogoMisionCompletada);
        }
        else if (misionAceptada)
        {
            MostrarDialogo(dialogos.dialogoMisionActiva);
        }
        else
        {
            MostrarDialogo(dialogos.dialogoInicial);
        }
    }
    #endregion

    #region UI Progreso
    private void ActualizarEstadoUI()
    {
        // Actualizar visibilidad del panel de progreso
        if (misionAceptada && !misionCompletada)
        {
            MostrarSliderProgreso(true);
            ActualizarSliderProgreso();
        }
        else
        {
            MostrarSliderProgreso(false);
        }
    }

    private void MostrarSliderProgreso(bool mostrar)
    {
        if (panelProgreso == null) 
        {
            Debug.LogWarning("Panel de progreso no encontrado");
            return;
        }
        
        bool debeMostrar = mostrar && misionAceptada && !misionCompletada;
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
            float progreso = GetProgresoNormalizado();
            sliderObjetivo = progreso;

            if (!animarSlider)
            {
                sliderProgreso.value = progreso;
            }

            // Actualizar texto
            textoProgreso.text = $"{pokemonesDerrotados}/{POKEMONES_NECESARIOS} Pokémon";

            // Actualizar título
            if (tituloMision != null)
            {
                tituloMision.text = "Misión: Derrotar 5 Pokémon salvajes";
            }

            // Actualizar color del slider
            ActualizarColorSlider(progreso);

            // Ocultar slider si la misión está completada
            if (misionCompletada)
            {
                MostrarSliderProgreso(false);
            }
        }
    }

    private void AnimarSlider()
    {
        if (sliderProgreso != null)
        {
            sliderActual = Mathf.Lerp(sliderActual, sliderObjetivo, Time.deltaTime * velocidadAnimacion);
            sliderProgreso.value = sliderActual;
        }
    }

    private void ActualizarColorSlider(float progreso)
    {
        if (rellenoSlider != null)
        {
            Color colorActual;

            if (progreso < 0.5f)
            {
                // De rojo a amarillo
                float t = progreso * 2f;
                colorActual = Color.Lerp(colorInicio, colorMitad, t);
            }
            else
            {
                // De amarillo a verde
                float t = (progreso - 0.5f) * 2f;
                colorActual = Color.Lerp(colorMitad, colorFinal, t);
            }

            rellenoSlider.color = colorActual;
        }
    }

    public void MostrarProgreso(bool mostrar)
    {
        if (panelProgreso != null)
        {
            panelProgreso.SetActive(mostrar && misionAceptada);
        }
    }

    public void ReiniciarProgreso()
    {
        sliderActual = 0f;
        sliderObjetivo = 0f;
        if (sliderProgreso != null)
            sliderProgreso.value = 0f;
    }
    #endregion
}