using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class ConsolaEnJuego : MonoBehaviour
{
    public static ConsolaEnJuego instance;

    [Header("Referencias UI (Opcional, se buscan automáticamente)")]
    public Text uiText;
    public TextMeshProUGUI tmpText;
    
    [Header("Configuración de Comportamiento")]
    public float typingSpeed = 0.03f;
    public string nombreEscenaCombate = "Escena de Combate";

    public bool isTyping { get; private set; } = false;

    private List<string> messageHistory = new List<string>();
    private const int maxLogLines = 3;
    private Coroutine typingCoroutine;
    private Queue<string> messageQueue = new Queue<string>();
    private bool isProcessingQueue = false;
    private Canvas consolaCanvas;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (transform.parent != null) transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Asegurarse de que la consola está correctamente configurada en la escena inicial.
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Application.logMessageReceived -= HandleLog;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Espera un frame para que la nueva escena se cargue completamente antes de buscar la UI.
        StartCoroutine(SetupConsoleForScene(scene));
    }

    private IEnumerator SetupConsoleForScene(Scene scene)
    {
        yield return null; // Espera un frame.

        BuscarYConfigurarUI();

        bool esEscenaDeCombate = scene.name == nombreEscenaCombate;

        if (consolaCanvas != null)
        {
            consolaCanvas.gameObject.SetActive(esEscenaDeCombate);
        }

        // Limpia logs anteriores al cambiar de escena.
        Application.logMessageReceived -= HandleLog;

        if (esEscenaDeCombate)
        {
            // Si estamos en la escena de combate, nos suscribimos para recibir logs.
            Application.logMessageReceived += HandleLog;
            ResetConsole();
        }
    }
    
    /// <summary>
    /// Captura todos los mensajes de log de Unity mientras esté suscrito.
    /// </summary>
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Filtro para evitar que la consola se loguee a sí misma.
        if (logString.StartsWith("ConsolaEnJuego:")) return;
        
        // Añade cualquier otro log a la cola para ser mostrado.
        EnqueueLogMessage(logString);
    }

    private void EnqueueLogMessage(string message)
    {
        messageQueue.Enqueue(message);
        if (!isProcessingQueue)
        {
            typingCoroutine = StartCoroutine(ProcessMessageQueue());
        }
    }

    private IEnumerator ProcessMessageQueue()
    {
        isProcessingQueue = true;
        while (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();
            if (messageHistory.Count >= maxLogLines) messageHistory.RemoveAt(0);
            messageHistory.Add(message);
            
            yield return StartCoroutine(AnimateMessage(message));
            
            if (messageQueue.Count > 0) yield return new WaitForSeconds(0.5f);
        }
        isProcessingQueue = false;
        typingCoroutine = null;
    }

    private IEnumerator AnimateMessage(string messageToWrite)
    {
        isTyping = true;
        string currentFrameText = "";
        string previousLines = string.Join("\n", messageHistory.Take(messageHistory.Count - 1));
        if (!string.IsNullOrEmpty(previousLines)) previousLines += "\n";

        foreach (char letter in messageToWrite.ToCharArray())
        {
            if (Input.GetKeyDown(KeyCode.Space)) // Permite saltar la animación.
            {
                currentFrameText = messageToWrite;
                SetText(previousLines + currentFrameText);
                break;
            }
            currentFrameText += letter;
            SetText(previousLines + currentFrameText);
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    private void SetText(string text)
    {
        if (tmpText != null) tmpText.text = text;
        else if (uiText != null) uiText.text = text;
    }

    public void ResetConsole()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isTyping = false;
        isProcessingQueue = false;
        messageHistory.Clear();
        messageQueue.Clear();
        SetText("");
    }

    private void BuscarYConfigurarUI()
    {
        if (consolaCanvas == null) consolaCanvas = GetComponentInChildren<Canvas>(true);
        if (tmpText == null) tmpText = GetComponentInChildren<TextMeshProUGUI>(true);
        if (uiText == null && tmpText == null) uiText = GetComponentInChildren<Text>(true);

        if (consolaCanvas == null)
        {
            Debug.LogWarning("ConsolaEnJuego: No se encontró un Canvas hijo. La visibilidad no puede ser gestionada.");
        }
        if (tmpText == null && uiText == null)
        {
            Debug.LogWarning("ConsolaEnJuego: No se encontró un componente Text o TextMeshProUGUI.");
        }
    }
}