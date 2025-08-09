using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControladorPorkemon : MonoBehaviour
{
    [Header("Data")]
    public Porkemon porkemon; // La instancia de este porkemon en combate

    [Header("UI")]
    public Slider barraSalud;
    public Text textoSalud;
    public Text textoNombre;

    public void Setup(Porkemon pInstance)
    {
        porkemon = pInstance;

        if (textoNombre != null)
            textoNombre.text = porkemon.BaseData.nombre;

        // Nos suscribimos al evento. Ahora ActualizarUI se llamará sola.
        porkemon.OnHPChanged += ActualizarUI;

        // Actualizamos la UI una vez al principio.
        ActualizarUI();
    }

    private void OnDestroy()
    {
        // Nos desuscribimos para evitar errores.
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
        // La velocidad del defensor reduce la probabilidad de acierto
        if (this.porkemon.Velocidad > atacante.Velocidad)
        {
            chanceDeAcertar -= (this.porkemon.Velocidad - atacante.Velocidad) / 5f; // Factor de balance
        }

        if (Random.Range(1, 101) > chanceDeAcertar)
        {
            Debug.Log($"¡El ataque {ataque.nombreAtaque} ha fallado!");
            return false; // El ataque falla, no hay daño
        }

        
        if (ataque.categoria == CategoriaAtaque.Estado)
        {
            porkemon.VidaActual -= 0; // No hace daño, pero actualiza UI si es necesario
            return false;
        }

        float statAtaqueAtacante;
        float defensaOponente;

        if (ataque.categoria == CategoriaAtaque.Fisico)
        {
            statAtaqueAtacante = atacante.Ataque;
            defensaOponente = this.porkemon.Defensa;
        }
        else // CategoriaAtaque.Especial
        {
            statAtaqueAtacante = atacante.Espiritu;
            defensaOponente = this.porkemon.Espiritu;
        }

        float danioBruto = ataque.poder * (statAtaqueAtacante / 10f);
        float reduccionPorDefensa = defensaOponente / (defensaOponente + 100f);
        float danioNeto = danioBruto * (1 - reduccionPorDefensa);

        
        float multiplicadorCritico = 1f;
        if (Random.Range(0, 100f) < ataque.chanceCritico)
        {
            Debug.Log("¡Un golpe crítico!");
            multiplicadorCritico = 3f; // Daño x3 como pediste
        }

        
        float danioFinal = danioNeto * multiplicadorCritico;
        danioFinal *= Random.Range(0.85f, 1.0f); // Pequeña variación final

        int danioTotal = Mathf.Max(1, Mathf.FloorToInt(danioFinal));

       
        porkemon.VidaActual -= danioTotal;
        Debug.Log($"{atacante.BaseData.nombre} hace {danioTotal} de daño a {porkemon.BaseData.nombre}.");

        if (porkemon.VidaActual <= 0)
        {
            porkemon.VidaActual = 0;
            return true; // Devuelve true si el Porkemon fue debilitado
        }
        return false;
    }
}