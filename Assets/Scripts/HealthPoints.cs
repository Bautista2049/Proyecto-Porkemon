using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthPoints : MonoBehaviour
{
    public static HealthPoints instancia;
    public int saludMaxima = 100;
    public static int saludActual;
    public Slider barraSalud;
    public Text textoSalud;

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        if (barraSalud == null)
        {
            barraSalud = GameObject.Find("BarraSaludJugador1").GetComponent<Slider>();
        }
        if (saludActual <= 0 || saludActual > saludMaxima)
        {
            saludActual = saludMaxima;
        }
        barraSalud.maxValue = saludMaxima;
        barraSalud.value = saludActual;

        if (textoSalud == null)
        {
            textoSalud = GameObject.Find("TextoSaludJugador1").GetComponent<Text>();
        }
        ActualizarUI();
    }

    public void RecibirDanio(int danio)
    {
        saludActual -= danio;
        saludActual = Mathf.Clamp(saludActual, 0, saludMaxima);
        barraSalud.value = saludActual;
        ActualizarUI();

        if (saludActual == 0)
        {
            SceneManager.LoadScene("Escena de muerte");
        }
    }

    void ActualizarUI()
    {
        if (textoSalud != null)
        {
            textoSalud.text = "Salud: " + saludActual.ToString();
        }
        // Cambia el color del Fill del slider a rojo
        if (barraSalud != null && barraSalud.fillRect != null)
        {
            barraSalud.fillRect.GetComponent<Image>().color = Color.red;
        }
    }
}
