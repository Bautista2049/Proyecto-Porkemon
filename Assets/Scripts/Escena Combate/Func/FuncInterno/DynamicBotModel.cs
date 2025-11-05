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
    public void UpdateModel(string modelName)
    {
        // Ruta de carga: Resources/Porkemons/{NombreDelPokemon}
        GameObject modelPrefab = Resources.Load<GameObject>("Porkemons/" + modelName);

        if (modelPrefab != null)
        {
            Transform modelParent = modelParentContainer != null ? modelParentContainer.transform : transform;

            // Limpiar modelo anterior
            foreach (Transform child in modelParent)
            {
                Destroy(child.gameObject);
            }

            // Instanciar nuevo modelo
            Transform modelTransform = Instantiate(modelPrefab, modelParent).transform;
            modelTransform.localPosition = Vector3.zero;
            modelTransform.localRotation = Quaternion.identity;
            modelTransform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogError($"ERROR: Prefab no encontrado en 'Resources/Porkemons/{modelName}'");
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
        // Esta función es llamada por la Porkebola
        // Debería ocultar el modelo, tal vez con una animación o simplemente desactivándolo
        Transform modelParent = modelParentContainer != null ? modelParentContainer.transform : transform;
        foreach (Transform child in modelParent)
        {
            child.gameObject.SetActive(false); // Oculta el modelo
        }
    }
    
    public void PlayEnterAnimation()
    {
        // El modelo reaparece si la captura falla
        Transform modelParent = modelParentContainer != null ? modelParentContainer.transform : transform;
        foreach (Transform child in modelParent)
        {
            child.gameObject.SetActive(true); // Muestra el modelo
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