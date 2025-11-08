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
            if (equipoGanador[i].VidaActual > 0) // Solo mostrar experiencia para Porkemons vivos
            {
                indicePorkemonActual = i;
                yield return StartCoroutine(AnimarExperienciaPorkemon(equipoGanador[i]));
                yield return new WaitForSeconds(delayEntrePorkemons);
            }
        }

        // Después de mostrar toda la experiencia, regresar al menú principal
        yield return new WaitForSeconds(3f);
        SceneTransitionManager.Instance.LoadScene("Escena Principal");
    }

    private IEnumerator AnimarExperienciaPorkemon(Porkemon porkemon)
    {
        // Actualizar nombre del Porkemon
        if (textoNombrePorkemon != null)
        {
            textoNombrePorkemon.text = porkemon.BaseData.nombre;
        }

        // Calcular experiencia por participante
        int participantes = equipoGanador.Count(p => p.VidaActual > 0);
        int expPorParticipante = experienciaTotal / participantes;

        // Guardar nivel inicial
        int nivelInicial = porkemon.Nivel;
        int experienciaInicial = porkemon.Experiencia;

        // Calcular experiencia necesaria para el siguiente nivel
        int expParaSiguienteNivel = porkemon.ExperienciaParaSiguienteNivel;

        // Configurar barra de experiencia
        if (barraExperiencia != null)
        {
            barraExperiencia.maxValue = expParaSiguienteNivel;
            barraExperiencia.value = experienciaInicial % expParaSiguienteNivel; // Experiencia actual en el nivel
        }

        // Actualizar texto inicial
        if (textoNivel != null)
        {
            textoNivel.text = $"Nv. {nivelInicial}";
        }

        if (textoExperiencia != null)
        {
            textoExperiencia.text = $"+{expPorParticipante} EXP";
        }

        // Animar la barra de experiencia
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

        // Aplicar experiencia real al Porkemon
        porkemon.GanarExperiencia(expPorParticipante);

        // Verificar si subió de nivel
        if (porkemon.Nivel > nivelInicial)
        {
            // Animación de subida de nivel
            if (textoNivel != null)
            {
                textoNivel.text = $"Nv. {porkemon.Nivel}";
            }

            // Resetear barra para el nuevo nivel
            if (barraExperiencia != null)
            {
                barraExperiencia.maxValue = porkemon.ExperienciaParaSiguienteNivel;
                barraExperiencia.value = 0;
            }

            Debug.Log($"{porkemon.BaseData.nombre} subió al nivel {porkemon.Nivel}!");
        }
        else
        {
            // Actualizar barra con la experiencia final
            if (barraExperiencia != null)
            {
                barraExperiencia.value = porkemon.Experiencia % expParaSiguienteNivel;
            }
        }
    }
}
