using UnityEngine;

public class SceneTransitionInitializer : MonoBehaviour
{
    [Header("Fade Prefab")]
    public GameObject fadePrefab;

    void Start()
    {
        if (SceneTransitionManager.Instance == null)
        {
            GameObject go = new GameObject("SceneTransitionManager");
            SceneTransitionManager stm = go.AddComponent<SceneTransitionManager>();
            stm.fadePrefab = fadePrefab;
            stm.InitializeFade();
        }
        else
        {
            SceneTransitionManager.Instance.fadePrefab = fadePrefab;
            SceneTransitionManager.Instance.InitializeFade();
        }
    }
}
