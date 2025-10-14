using UnityEngine;

public class DynamicBotModel : MonoBehaviour
{
    [Header("Model Setup")]
    public Transform modelParent; // The transform where the model should be instantiated (e.g., a child object for the model)

    void Start()
    {
        if (GameState.porkemonDelBot == null)
        {
            Debug.LogWarning("No bot Pokémon data available for dynamic model loading.");
            return;
        }

        string modelName = GameState.porkemonDelBot.BaseData.nombre;
        // Assume models are prefabs in Resources/Porkemons/ named after the Pokémon (e.g., "Aiekamon")
        GameObject modelPrefab = Resources.Load<GameObject>("Porkemons/" + modelName);

        if (modelPrefab != null)
        {
            // Clear any existing model in the parent
            if (modelParent != null)
            {
                foreach (Transform child in modelParent)
                {
                    Destroy(child.gameObject);
                }
            }

            // Instantiate the correct model
            Transform modelTransform = Instantiate(modelPrefab, modelParent != null ? modelParent : transform).transform;
            modelTransform.localPosition = Vector3.zero;
            modelTransform.localRotation = Quaternion.identity;
            modelTransform.localScale = Vector3.one;

            Debug.Log("Loaded dynamic model for bot Pokémon: " + modelName);
        }
        else
        {
            Debug.LogWarning("Model prefab not found in Resources/Porkemons/ for " + modelName + ". Ensure the prefab is named exactly like the Pokémon and placed in Resources/Porkemons/.");
        }
    }
}
