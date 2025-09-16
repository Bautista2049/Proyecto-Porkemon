using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Fade Settings")]
    public GameObject fadePrefab;
    public float fadeDuration = 1f;

    [Header("Camera Orbit")]
    public bool autoRotate = false;
    public float rotationSpeed = 30f;
    public Transform orbitCenter;

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
        else if (Instance == this)
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

        if (autoRotate && orbitCenter != null)
        {
            float angle = rotationSpeed * Time.deltaTime;
            transform.RotateAround(orbitCenter.position, Vector3.up, angle);
        }
    }

    public void InitializeFade()
    {
        if (fadePrefab != null)
        {
            if (fadeInstance != null)
            {
                Destroy(fadeInstance);
            }
            fadeInstance = Instantiate(fadePrefab);
            DontDestroyOnLoad(fadeInstance);
            transitionAnimator = fadeInstance.GetComponentInChildren<Animator>();
        }
    }

    public void DestroyFadeInstance()
    {
        if (fadeInstance != null)
        {
            Destroy(fadeInstance);
            fadeInstance = null;
            transitionAnimator = null;
        }
    }

    public void LoadScene(string sceneName)
    {
        DestroyFadeInstance();

        if (sceneName == "Interfaz de Menu")
        {
            lastSceneName = SceneManager.GetActiveScene().name;
        }

        StartCoroutine(LoadSceneCoroutine(sceneName, LoadSceneMode.Single));
    }

    public void LoadAdditive(string sceneName)
    {
        DestroyFadeInstance();
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
        if (sceneName != "Escena Principal")
        {
            if (transitionAnimator != null)
            {
                transitionAnimator.SetTrigger("StartTransition");
                yield return new WaitForSeconds(fadeDuration);
            }
        }
        else if (sceneName == "Escena Principal"
              || sceneName == "Interfaz de Menu"
              || sceneName == "Escena de Victoria"
              || sceneName == "Escena de Derrota")
        {
            yield return new WaitForSeconds(0.5f);
        }

        SceneManager.LoadScene(sceneName, mode);
        yield return null;
    }
}
