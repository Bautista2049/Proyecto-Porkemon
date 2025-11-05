using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBotModel : MonoBehaviour
{
    public GameObject modelParentContainer;
    [SerializeField] public bool isPlayerModel = false;

    public void UpdateModel(string modelName)
    {
        GameObject modelPrefab = Resources.Load<GameObject>("Porkemons/" + modelName);
        if (modelPrefab == null) return;

        Transform modelParent = modelParentContainer != null ? modelParentContainer.transform : transform;
        foreach (Transform child in modelParent)
            Destroy(child.gameObject);

        Transform modelTransform = Instantiate(modelPrefab, modelParent).transform;
        modelTransform.localPosition = Vector3.zero;
        modelTransform.localRotation = Quaternion.identity;
        modelTransform.localScale = Vector3.one;

        Resources.UnloadUnusedAssets();
    }

    public void PlayExitAnimation()
    {
        Transform modelParent = modelParentContainer != null ? modelParentContainer.transform : transform;
        foreach (Transform child in modelParent)
            child.gameObject.SetActive(false);
    }

    public void PlayEnterAnimation()
    {
        Transform modelParent = modelParentContainer != null ? modelParentContainer.transform : transform;
        foreach (Transform child in modelParent)
            child.gameObject.SetActive(true);
    }

    public void ClearCurrentModel()
    {
        Transform modelParent = modelParentContainer != null ? modelParentContainer.transform : transform;
        foreach (Transform child in modelParent)
            Destroy(child.gameObject);

        Resources.UnloadUnusedAssets();
    }
}
