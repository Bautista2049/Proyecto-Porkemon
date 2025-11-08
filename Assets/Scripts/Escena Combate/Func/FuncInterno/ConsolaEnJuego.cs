using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ConsolaEnJuego : MonoBehaviour
{
    public static ConsolaEnJuego instance;

    public Text uiText;
    public TextMeshProUGUI tmpText;
    public float typingSpeed = 0.03f;

    public bool isTyping { get; private set; } = false;

    private List<string> filteredLogs = new List<string>();
    private const int maxLogLines = 3;
    private Coroutine typingCoroutine;

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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
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
            AddLogMessage(logString);
        }
    }

    private void AddLogMessage(string message)
    {
        if (filteredLogs.Count >= maxLogLines)
        {
            filteredLogs.RemoveAt(0);
        }
        filteredLogs.Add(message);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        typingCoroutine = StartCoroutine(MostrarMensaje());
    }

    public IEnumerator MostrarMensaje()
    {
        isTyping = true;
        string fullMessage = filteredLogs[filteredLogs.Count - 1];
        string currentText = "";

        string previousText = string.Join("\n", filteredLogs.Take(filteredLogs.Count - 1));
        if (!string.IsNullOrEmpty(previousText))
        {
            previousText += "\n";
        }

        foreach (char letter in fullMessage.ToCharArray())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentText = fullMessage;
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
        typingCoroutine = null;
    }

    private void ActualizarTextoConsola()
    {
        if (typingCoroutine == null && filteredLogs.Count > 0)
        {
            string consoleText = string.Join("\n", filteredLogs);

            if (tmpText != null) tmpText.text = consoleText;
            else if (uiText != null) uiText.text = consoleText;
        }
    }
}
