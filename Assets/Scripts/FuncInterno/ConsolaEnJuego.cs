using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ConsolaEnJuego : MonoBehaviour
{
    public static ConsolaEnJuego instance;

    [Header("UI")]
    public Text uiText;
    public TextMeshProUGUI tmpText;

    [Header("Typewriter Effect")]
    public float typingSpeed = 0.03f; // Seconds per character, faster for better UX

    public bool isTyping { get; private set; } = false;

    private List<string> filteredLogs = new List<string>();
    private const int maxLogLines = 3;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
            lowerLogString.Contains("inmune"))
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

        // Stop any ongoing typing
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Start typing the new message
        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        isTyping = true;
        string fullMessage = filteredLogs[filteredLogs.Count - 1];
        string currentText = "";

        // Get previous messages
        string previousText = string.Join("\n", filteredLogs.Take(filteredLogs.Count - 1));
        if (!string.IsNullOrEmpty(previousText))
        {
            previousText += "\n";
        }

        foreach (char letter in fullMessage.ToCharArray())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Skip to end if space pressed
                currentText = fullMessage;
                string displayText = previousText + currentText;

                if (tmpText != null)
                {
                    tmpText.text = displayText;
                }
                else if (uiText != null)
                {
                    uiText.text = displayText;
                }
                break;
            }

            currentText += letter;
            string displayText = previousText + currentText;

            if (tmpText != null)
            {
                tmpText.text = displayText;
            }
            else if (uiText != null)
            {
                uiText.text = displayText;
            }
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    private void ActualizarTextoConsola()
    {
        // This is now handled by the typing coroutine, but keep for fallback
        if (typingCoroutine == null && filteredLogs.Count > 0)
        {
            string consoleText = string.Join("\n", filteredLogs);

            if (tmpText != null)
            {
                tmpText.text = consoleText;
            }
            else if (uiText != null)
            {
                uiText.text = consoleText;
            }
        }
    }
}
