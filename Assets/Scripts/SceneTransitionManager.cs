using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Fade Settings")]
    public GameObject fadePrefab;
    public float fadeDuration = 1f;

    private Animator transitionAnimator;
    private GameObject fadeInstance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFade();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeFade()
    {
        if (fadePrefab != null && fadeInstance == null)
        {
            fadeInstance = Instantiate(fadePrefab);
            DontDestroyOnLoad(fadeInstance);
            transitionAnimator = fadeInstance.GetComponentInChildren<Animator>();
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Start fade out
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("StartTranstition");
            yield return new WaitForSeconds(fadeDuration);
        }

        // Load the scene
        SceneManager.LoadScene(sceneName);

        // Wait for scene to load
        yield return null;

        // The FadeIn animation will play automatically when the scene loads
    }
}
