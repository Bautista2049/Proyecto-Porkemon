using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ConsolaEnJuego : MonoBehaviour
{
    [Header("UI")]
    public Text uiText;
    public TextMeshProUGUI tmpText;

    private List<string> filteredLogs = new List<string>();
    private const int maxLogLines = 15;

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
        ActualizarTextoConsola();
    }

    private void ActualizarTextoConsola()
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
