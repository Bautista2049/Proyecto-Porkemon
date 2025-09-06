using UnityEngine;

public class SceneTransitionInitializer : MonoBehaviour
{
    [Header("Fade Prefab")]
    public GameObject fadePrefab;

    void Start()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.fadePrefab = fadePrefab;
            SceneTransitionManager.Instance.InitializeFade();
        }
    }
}
