using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalirJuego : MonoBehaviour
{
    public void CerrarJuego()
    {
        Application.Quit();

        // Para que también funcione al probarlo en el Editor de Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

