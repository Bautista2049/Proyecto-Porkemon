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

    private string lastSceneName = null;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadScene("Interfaz de Menu");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
        if (sceneName == "Interfaz de Menu")
        {
            lastSceneName = SceneManager.GetActiveScene().name;
        }
        StartCoroutine(LoadSceneCoroutine(sceneName, LoadSceneMode.Single));
    }

    public void LoadAdditive(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, LoadSceneMode.Additive));
    }

    public void ReturnToLastScene()
    {
        if (!string.IsNullOrEmpty(lastSceneName))
        {
            LoadScene(lastSceneName);
            lastSceneName = null;
        }
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, LoadSceneMode mode)
    {
        // Skip fade transition for main scene
        if (sceneName != "Escena Principal")
        {
            // Start fade out
            if (transitionAnimator != null)
            {
                transitionAnimator.SetTrigger("StartTranstition");
                yield return new WaitForSeconds(fadeDuration);
            }
        }

        // Load the scene
        SceneManager.LoadScene(sceneName, mode);

        // Wait for scene to load
        yield return null;

        // The FadeIn animation will play automatically when the scene loads (except for main scene)
    }
}
