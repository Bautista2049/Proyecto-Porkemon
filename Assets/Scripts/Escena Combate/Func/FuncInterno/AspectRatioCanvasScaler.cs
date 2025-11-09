using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class AspectRatioCanvasScaler : MonoBehaviour
{
    public Vector2 referenceResolution = new Vector2(1024, 768);

    private CanvasScaler scaler;

    void Start()
    {
        scaler = GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = referenceResolution;

        // Calcular el aspecto de referencia y el actual
        float referenceAspect = referenceResolution.x / referenceResolution.y;
        float screenAspect = Screen.width / (float)Screen.height;

        // Configurar el modo de coincidencia
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        // Si la pantalla es más ancha que la referencia (ej. 16:9 vs 4:3), coincidir altura; de lo contrario, ancho
        scaler.matchWidthOrHeight = (screenAspect > referenceAspect) ? 1 : 0;
    }
}
