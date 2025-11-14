using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    public static SceneTransitionManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("SceneTransitionManager");
            Instance = go.AddComponent<SceneTransitionManager>();
        }
        return Instance;
    }

    [Header("Fade Settings")]
    public GameObject fadePrefab;
    public float fadeDuration = 1f;

    [Header("Camera Orbit")]
    public bool autoRotate = true;
    public float rotationSpeed = 30f;
    public Transform orbitCenter;

    private Animator transitionAnimator;
    private GameObject fadeInstance;
    private string lastSceneName = null;

    private Camera mainCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
            InitializeFade();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (autoRotate && mainCamera != null && orbitCenter != null)
        {
            mainCamera.transform.RotateAround(orbitCenter.position, Vector3.up, rotationSpeed * Time.deltaTime);
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

        if (sceneName == "Interfaz de Menu" && sceneName != SceneManager.GetActiveScene().name)
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Escena de Combate")
        {
            mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.transform.position = new Vector3(-322.7f, 12.76f, 57.6f);
                mainCamera.transform.rotation = Quaternion.Euler(23.223f, 0f, 0f);
                if (orbitCenter == null)
                {
                    GameObject orbitCenterObj = new GameObject("OrbitCenter");
                    orbitCenter = orbitCenterObj.transform;
                    orbitCenter.position = new Vector3(-322.7f, 3.91f, 84.52f);
                }
                autoRotate = true;
                rotationSpeed = 8f;
            }
        }
        else if (scene.name == "Escena Principal")
        {
            if (mainCamera != null)
            {
                autoRotate = false;
            }
        }
        else if (scene.name == "Escena de Victoria")
        {
            if (mainCamera != null)
            {
                autoRotate = false;
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
