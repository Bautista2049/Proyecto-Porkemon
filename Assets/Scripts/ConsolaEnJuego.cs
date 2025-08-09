using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsolaEnJuego : MonoBehaviour
{
    [Header("UI")]
    public Text uiText;
    public TextMeshProUGUI tmpText;

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
        ActualizarTextoConsola(logString);
    }

    private void ActualizarTextoConsola(string message)
    {
        if (tmpText != null)
        {
            tmpText.text = message;
        }
        else if (uiText != null)
        {
            uiText.text = message;
        }
    }
}