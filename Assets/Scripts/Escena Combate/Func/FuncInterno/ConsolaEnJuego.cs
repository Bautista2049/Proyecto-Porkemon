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

    public Text uiText;
    public TextMeshProUGUI tmpText;
    public float typingSpeed = 0.03f;
    public string consolaCanvasTag = "ConsolaCanvas";
    public bool mostrarSoloEnCombate = false;

    public bool isTyping { get; private set; } = false;

    private List<string> filteredLogs = new List<string>();
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
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
            
            consolaCanvas = GetComponentInChildren<Canvas>(true);
            if (consolaCanvas == null)
            {
                consolaCanvas = GetComponent<Canvas>();
            }
            
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            ResetConsole(); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        BuscarReferenciasUI();
        ResetConsole(); 
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ConfigurarConsolaEnNuevaEscena(scene.name));
        }
        else
        {
            ConfigurarConsolaInmediato(scene.name);
        }
    }
    
    private void ConfigurarConsolaInmediato(string sceneName)
    {
        BuscarReferenciasUI();
        
        if (mostrarSoloEnCombate)
        {
            bool esCombate = sceneName == "Escena de Combate";
            MostrarConsola(esCombate);
        }
        else
        {
            MostrarConsola(true);
        }
        
        if (sceneName == "Escena de Combate")
        {
            ResetConsole();
        }
    }
    
    private IEnumerator ConfigurarConsolaEnNuevaEscena(string sceneName)
    {
        yield return null;
        ConfigurarConsolaInmediato(sceneName);
    }
    
    private void BuscarReferenciasUI()
    {
        if ((tmpText != null || uiText != null) && 
            ((tmpText != null && tmpText.gameObject.scene.IsValid()) || 
             (uiText != null && uiText.gameObject.scene.IsValid())))
        {
            return;
        }
        
        if (tmpText == null)
        {
            tmpText = GetComponentInChildren<TextMeshProUGUI>(true);
        }
        
        if (uiText == null && tmpText == null)
        {
            uiText = GetComponentInChildren<Text>(true);
        }
        
        if ((tmpText == null && uiText == null) && consolaCanvas != null)
        {
            tmpText = consolaCanvas.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmpText == null)
            {
                uiText = consolaCanvas.GetComponentInChildren<Text>(true);
            }
        }
        
        if (tmpText == null && uiText == null)
        {
            GameObject consolaCanvasObj = GameObject.FindGameObjectWithTag(consolaCanvasTag);
            if (consolaCanvasObj != null)
            {
                tmpText = consolaCanvasObj.GetComponentInChildren<TextMeshProUGUI>(true);
                if (tmpText == null)
                {
                    uiText = consolaCanvasObj.GetComponentInChildren<Text>(true);
                }
            }
        }
    }
    
    public void MostrarConsola(bool mostrar)
    {
        if (consolaCanvas != null)
        {
            consolaCanvas.gameObject.SetActive(mostrar);
        }
        else
        {
            gameObject.SetActive(mostrar);
        }
    }

    public void ResetConsole()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        isTyping = false;
        isProcessingQueue = false;
        
        filteredLogs.Clear();
        messageQueue.Clear();
        
        ActualizarTextoConsola(true);
        
        Debug.Log("ConsolaEnJuego Reiniciada.");
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string lowerLogString = logString.ToLower();
        if (lowerLogString.Contains("ataque") ||
            lowerLogString.Contains("daño") ||
            lowerLogString.Contains("quema") ||
            lowerLogString.Contains("paraliza") ||
            lowerLogString.Contains("confundi") ||
            lowerLogString.Contains("defensa") ||
            lowerLogString.Contains("velocidad") ||
            lowerLogString.Contains("confundido") ||
            lowerLogString.Contains("precisión") ||
            lowerLogString.Contains("envenenado") ||
            lowerLogString.Contains("efi") ||
            lowerLogString.Contains("retrocedido") ||
            lowerLogString.Contains("efect") ||
            lowerLogString.Contains("critico") ||
            lowerLogString.Contains("inmune") ||
            lowerLogString.Contains("usó") ||
            lowerLogString.Contains("aumentado") ||
            lowerLogString.Contains("recuperó") ||
            lowerLogString.Contains("recupero") ||
            lowerLogString.Contains("ps!") ||
            lowerLogString.Contains("lanzaste") ||
            lowerLogString.Contains("...") ||
            lowerLogString.Contains("gotcha") ||
            lowerLogString.Contains("escapado"))
        {
            EnqueueLogMessage(logString);
        }
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
            
            if (filteredLogs.Count >= maxLogLines)
            {
                filteredLogs.RemoveAt(0);
            }
            filteredLogs.Add(message);
            
            yield return StartCoroutine(MostrarMensaje(message));
            
            if (messageQueue.Count > 0)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        isProcessingQueue = false;
        typingCoroutine = null;
    }

    public IEnumerator MostrarMensaje(string messageToWrite)
    {
        isTyping = true;
        string currentText = "";

        string previousText = string.Join("\n", filteredLogs.Take(filteredLogs.Count - 1));
        if (!string.IsNullOrEmpty(previousText))
        {
            previousText += "\n";
        }

        foreach (char letter in messageToWrite.ToCharArray())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentText = messageToWrite;
                string displayText = previousText + currentText; 

                if (tmpText != null) tmpText.text = displayText;
                else if (uiText != null) uiText.text = displayText;
                
                break;
            }

            currentText += letter;
            string currentDisplayText = previousText + currentText; 

            if (tmpText != null) tmpText.text = currentDisplayText;
            else if (uiText != null) uiText.text = currentDisplayText;
            
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void ActualizarTextoConsola(bool forceClear = false)
    {
        if (forceClear)
        {
            if (tmpText != null) tmpText.text = "";
            else if (uiText != null) uiText.text = "";
        }
        else if (typingCoroutine == null && filteredLogs.Count > 0)
        {
            string consoleText = string.Join("\n", filteredLogs);

            if (tmpText != null) tmpText.text = consoleText;
            else if (uiText != null) uiText.text = consoleText;
        }
    }
}