using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ataques : MonoBehaviour
{
    public void VolverCombate()
    {
        SceneManager.LoadScene("Luchar Escena");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Ataque1()
    {
        HealthPoints.instancia.RecibirDanio(10);
        VolverCombate();
    }

    public void Ataque2()
    {
        HealthPoints.instancia.RecibirDanio(20);
        VolverCombate();
    }

    public void Ataque3()
    {
        HealthPoints.instancia.RecibirDanio(30);
        VolverCombate();
    }

    public void Ataque4()
    {
        HealthPoints.instancia.RecibirDanio(40);
        VolverCombate();
    }
}
