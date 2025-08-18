using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControladorPorkemon : MonoBehaviour
{
    [Header("Data")]
    public Porkemon porkemon;

    [Header("UI")]
    public Slider barraSalud;
    public Text textoSalud;
    public Text textoNombre;

    public void Setup(Porkemon pInstance)
    {
        porkemon = pInstance;

        if (textoNombre != null)
            textoNombre.text = porkemon.BaseData.nombre;

        porkemon.OnHPChanged += ActualizarUI;
        ActualizarUI();
    }

    private void OnDestroy()
    {
        if (porkemon != null)
        {
            porkemon.OnHPChanged -= ActualizarUI;
        }
    }

    public void ActualizarUI()
    {
        if (porkemon == null) return;

        if (barraSalud != null)
        {
            barraSalud.maxValue = porkemon.VidaMaxima;
            barraSalud.value = porkemon.VidaActual;
        }

        if (textoSalud != null)
        {
            textoSalud.text = $"{porkemon.VidaActual} / {porkemon.VidaMaxima}";
        }
    }

    public bool RecibirDanio(AtaqueData ataque, Porkemon atacante)
    {
        float chanceDeAcertar = ataque.precision;
        if (this.porkemon.Velocidad > atacante.Velocidad)
        {
            chanceDeAcertar -= (this.porkemon.Velocidad - atacante.Velocidad) / 5f;
        }

        if (Random.Range(1, 101) > chanceDeAcertar)
        {
            Debug.Log($"¡El ataque {ataque.nombreAtaque} ha fallado!");
            return false;
        }

        if (ataque.categoria == CategoriaAtaque.Estado)
        {
            return false;
        }

        float statAtaqueAtacante;
        float defensaOponente;

        if (ataque.categoria == CategoriaAtaque.Fisico)
        {
            statAtaqueAtacante = atacante.Ataque;
            defensaOponente = this.porkemon.Defensa;
        }
        else
        {
            statAtaqueAtacante = atacante.Espiritu;
            defensaOponente = this.porkemon.Espiritu;
        }

        float danioBruto = ataque.poder * (statAtaqueAtacante / 10f);
        float reduccionPorDefensa = defensaOponente / (defensaOponente + 100f);
        float danioNeto = danioBruto * (1 - reduccionPorDefensa);

        float multiplicadorCritico = 1f;
        

        float danioFinal = danioNeto * multiplicadorCritico;
        danioFinal *= Random.Range(0.85f, 1.0f);

        int danioTotal = Mathf.Max(1, Mathf.FloorToInt(danioFinal));

        porkemon.VidaActual -= danioTotal;
        Debug.Log($"{atacante.BaseData.nombre} hace {danioTotal} de daño con el ataque {ataque.nombreAtaque} a {porkemon.BaseData.nombre}.");

        if (Random.Range(0, 100f) < ataque.chanceCritico)
        {
            Debug.Log("¡Un golpe crítico!");
            multiplicadorCritico = 3f;
        }

        if (porkemon.VidaActual <= 0)
        {
            porkemon.VidaActual = 0;
            return true;
        }
        return false;
    }
}