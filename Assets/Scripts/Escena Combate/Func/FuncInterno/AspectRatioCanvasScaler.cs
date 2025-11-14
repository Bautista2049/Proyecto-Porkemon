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

        float referenceAspect = referenceResolution.x / referenceResolution.y;
        float screenAspect = Screen.width / (float)Screen.height;

        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = (screenAspect > referenceAspect) ? 1 : 0;
    }
}
