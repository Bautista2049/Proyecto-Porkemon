using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DynamicBotModel : MonoBehaviour
{
    [Header("Model Setup")]
    public GameObject modelParentContainer; 
    [SerializeField] public bool isPlayerModel = false;

    void Start()
    {
        if (isPlayerModel)
        {
            UpdateModelForCurrentPokemon();
        }
        else
        {
            Porkemon botPork = null;
            if (GestorDeBatalla.instance != null && GestorDeBatalla.instance.porkemonBot != null)
            {
                // La fuente de verdad en la escena de combate es GestorDeBatalla.porkemonBot
                botPork = GestorDeBatalla.instance.porkemonBot;
            }
string modelName = botPork.BaseData.nombre;
            if (botPork == null)
            {
                
                Debug.Log("Loading bot model: " + modelName);
                UpdateModel(modelName);
            }

            
            UpdateModel(modelName);
        }
    }

    public void UpdateModel(string modelName)
    {
        // Ruta de carga: Resources/Porkemons/{NombreDelPokemon}
        GameObject modelPrefab = Resources.Load<GameObject>("Porkemons/" + modelName);

        if (modelPrefab != null)
        {
            Transform modelParent = modelParentContainer != null ? modelParentContainer.transform : transform;

            foreach (Transform child in modelParent)
            {
                Destroy(child.gameObject);
            }

            Transform modelTransform = Instantiate(modelPrefab, modelParent).transform;
            modelTransform.localPosition = Vector3.zero;
            modelTransform.localRotation = Quaternion.identity;
            modelTransform.localScale = Vector3.one;
        }
        else
        {
            // Mensaje de error mejorado
            Debug.LogError($"ERROR: Prefab no encontrado");
        }
    }

    private void UpdateModelForCurrentPokemon()
    {
        Porkemon currentPork = null;
        if (GestorDeBatalla.instance != null && GestorDeBatalla.instance.porkemonJugador != null)
        {
            currentPork = GestorDeBatalla.instance.porkemonJugador;
        }
        else if (GameState.porkemonDelJugador != null)
        {
            currentPork = GameState.porkemonDelJugador;
        }

        if (currentPork == null)
        {
            Debug.LogWarning("No player Pokémon data available for dynamic model loading.");
            return;
        }

        string modelName = currentPork.BaseData.nombre;
        UpdateModel(modelName);
    }
    
    private Animator GetCurrentAnimator()
    {
        if (modelParentContainer != null && modelParentContainer.transform.childCount > 0)
        {
            return modelParentContainer.transform.GetChild(0).GetComponent<Animator>();
        }
        return null;
    }
    
    public void PlayExitAnimation()
    {
        Animator anim = GetCurrentAnimator();
        if (anim != null)
        {
            anim.CrossFade("Exit", 0.1f);
        }
    }
    
    public void PlayEnterAnimation()
    {
        Animator anim = GetCurrentAnimator();
        if (anim != null)
        {
            anim.Play("Enter");
        }
    }

    public void ClearCurrentModel()
    {
        Transform modelParent = modelParentContainer != null ? modelParentContainer.transform : transform;
        foreach (Transform child in modelParent)
        {
            Destroy(child.gameObject);
        }
    }
}