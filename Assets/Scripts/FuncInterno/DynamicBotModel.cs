using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DynamicBotModel : MonoBehaviour
{
    [Header("Model Setup")]
    public GameObject modelParentContainer; // The GameObject whose transform will hold the model (e.g., a child object for the model)
    [SerializeField] private bool isPlayerModel = false;

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
                botPork = GestorDeBatalla.instance.porkemonBot;
            }
            else if (GameState.porkemonDelBot != null)
            {
                botPork = GameState.porkemonDelBot;
            }

            if (botPork == null)
            {
                Debug.LogWarning("No bot Pokémon data available for dynamic model loading.");
                return;
            }

            string modelName = botPork.BaseData.nombre;
            UpdateModel(modelName);
        }
    }

    public void UpdateModel(string modelName)
    {
        GameObject modelPrefab = Resources.Load<GameObject>("Porkemons/" + modelName);

        if (modelPrefab != null)
        {
            Transform modelParent = (modelParentContainer != null ? modelParentContainer.transform : transform);

            // Clear any existing model in the parent
            foreach (Transform child in modelParent)
            {
                Destroy(child.gameObject);
            }

            // Instantiate the correct model
            Transform modelTransform = Instantiate(modelPrefab, modelParent).transform;
            modelTransform.localPosition = Vector3.zero;
            modelTransform.localRotation = Quaternion.identity;
            modelTransform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogWarning("Model prefab not found in Resources/Porkemons/ for " + modelName + ". Ensure the prefab is named exactly like the Pokémon and placed in Resources/Porkemons/.");
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
}
