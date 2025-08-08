using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthPoints : MonoBehaviour
{
    public int saludMaxima = 100;
    public int saludActual; // ahora NO es static
    public Slider barraSalud;
    public Text textoSalud;

    void Start()
    {
        // Si no asignaste en el Inspector, busca en hijos
        if (barraSalud == null) barraSalud = GetComponentInChildren<Slider>();
        if (textoSalud == null) textoSalud = GetComponentInChildren<Text>();

        saludActual = saludMaxima;

        if (barraSalud != null)
        {
            barraSalud.maxValue = saludMaxima;
            barraSalud.value = saludActual;
        }

        ActualizarUI();
    }

    public void RecibirDanio(int danio)
    {
        saludActual -= danio;
        saludActual = Mathf.Clamp(saludActual, 0, saludMaxima);

        if (barraSalud != null)
            barraSalud.value = saludActual;

        ActualizarUI();

        if (saludActual <= 0)
        {
            SceneManager.LoadScene("Escena de muerte");
        }
    }

    void ActualizarUI()
    {
        if (textoSalud != null)
            textoSalud.text = "Salud: " + saludActual.ToString();

        if (barraSalud != null && barraSalud.fillRect != null)
            barraSalud.fillRect.GetComponent<Image>().color = Color.red;
    }
}
