using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class ControladorExperienciaVictoria : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider barraExperiencia;
    public TextMeshProUGUI textoExperiencia;
    public TextMeshProUGUI textoNivel;
    public TextMeshProUGUI textoNombrePorkemon;

    [Header("Animation Settings")]
    public float duracionAnimacion = 2f;
    public float delayEntrePorkemons = 1f;

    private List<Porkemon> equipoGanador;
    private int experienciaTotal;
    private int indicePorkemonActual = 0;

    void Start()
    {
        equipoGanador = GameState.equipoGanador;
        experienciaTotal = GameState.experienciaGanada;

        if (equipoGanador.Count > 0)
        {
            StartCoroutine(MostrarExperienciaPorkemons());
        }
    }

    public IEnumerator MostrarExperienciaPorkemons()
    {
        for (int i = 0; i < equipoGanador.Count; i++)
        {
            if (equipoGanador[i].VidaActual > 0) 
            {
                indicePorkemonActual = i;
                yield return StartCoroutine(AnimarExperienciaPorkemon(equipoGanador[i]));
                yield return new WaitForSeconds(delayEntrePorkemons);
            }
        }

     
        yield return new WaitForSeconds(3f);
        
        CargarEscPrincipal cargador = FindObjectOfType<CargarEscPrincipal>();
        if (cargador != null && GameState.esCombateBoss)
        {
            cargador.RecargarEscenaBoss();
        }
        else
        {
            SceneTransitionManager.Instance.LoadScene("Escena Principal");
        }
    }

    private IEnumerator AnimarExperienciaPorkemon(Porkemon porkemon)
    {
        if (textoNombrePorkemon != null)
        {
            textoNombrePorkemon.text = porkemon.BaseData.nombre;
        }
        int participantes = equipoGanador.Count(p => p.VidaActual > 0);
        int expPorParticipante = experienciaTotal / participantes;

        int nivelInicial = porkemon.Nivel;
        int experienciaInicial = porkemon.Experiencia;

        int expParaSiguienteNivel = porkemon.ExperienciaParaSiguienteNivel;
        if (barraExperiencia != null)
        {
            barraExperiencia.maxValue = expParaSiguienteNivel;
            barraExperiencia.value = experienciaInicial % expParaSiguienteNivel; 
        }

        if (textoNivel != null)
        {
            textoNivel.text = $"Nv. {nivelInicial}";
        }

        if (textoExperiencia != null)
        {
            textoExperiencia.text = $"+{expPorParticipante} EXP";
        }

        float tiempoInicio = Time.time;
        float valorInicial = barraExperiencia.value;
        float valorFinal = Mathf.Min(expParaSiguienteNivel, valorInicial + expPorParticipante);

        while (Time.time - tiempoInicio < duracionAnimacion)
        {
            float progreso = (Time.time - tiempoInicio) / duracionAnimacion;
            float valorActual = Mathf.Lerp(valorInicial, valorFinal, progreso);

            if (barraExperiencia != null)
            {
                barraExperiencia.value = valorActual;
            }

            yield return null;
        }

        porkemon.GanarExperiencia(expPorParticipante);

        if (porkemon.Nivel > nivelInicial)
        {
            if (textoNivel != null)
            {
                textoNivel.text = $"Nv. {porkemon.Nivel}";
            }

            if (barraExperiencia != null)
            {
                barraExperiencia.maxValue = porkemon.ExperienciaParaSiguienteNivel;
                barraExperiencia.value = 0;
            }

            Debug.Log($"{porkemon.BaseData.nombre} subió al nivel {porkemon.Nivel}!");
        }
        else
        {
            if (barraExperiencia != null)
            {
                barraExperiencia.value = porkemon.Experiencia % expParaSiguienteNivel;
            }
        }
    }
}
