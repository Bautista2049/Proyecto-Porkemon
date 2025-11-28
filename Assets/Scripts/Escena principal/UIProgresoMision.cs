using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIProgresoMision : MonoBehaviour
{
    [Header("Slider")]
    public Slider sliderProgreso;
    public Image fondoSlider;
    public Image rellenoSlider;

    [Header("Texto")]
    public TextMeshProUGUI textoProgreso;
    public TextMeshProUGUI tituloMision;

    [Header("Colores")]
    public Color colorInicio = Color.red;
    public Color colorMitad = Color.yellow;
    public Color colorFinal = Color.green;

    [Header("Animación")]
    public bool animarSlider = true;
    public float velocidadAnimacion = 2f;

    private float sliderActual = 0f;
    private float sliderObjetivo = 0f;

    void Start()
    {
        // Configurar slider si no está asignado
        if (sliderProgreso == null)
            sliderProgreso = GetComponentInChildren<Slider>();

        if (textoProgreso == null)
            textoProgreso = GetComponentInChildren<TextMeshProUGUI>();

        // Inicializar valores
        if (sliderProgreso != null)
        {
            sliderProgreso.minValue = 0f;
            sliderProgreso.maxValue = 1f;
            sliderProgreso.value = 0f;
        }

        ActualizarUI();
    }

    void Update()
    {
        if (SistemaMisiones.misionCombateActivada && !SistemaMisiones.misionCombateCompletada)
        {
            ActualizarUI();

            if (animarSlider)
            {
                AnimarSlider();
            }
        }
        else
        {
            // Ocultar UI si no hay misión activa
            if (gameObject.activeSelf && 
                !SistemaMisiones.misionCombateActivada && 
                !SistemaMisiones.misionCombateCompletada)
            {
                gameObject.SetActive(false);
            }
            else if (!gameObject.activeSelf && SistemaMisiones.misionCombateActivada)
            {
                gameObject.SetActive(true);
            }
        }
    }

    private void ActualizarUI()
    {
        float progreso = SistemaMisiones.GetProgresoNormalizado();
        sliderObjetivo = progreso;

        if (!animarSlider && sliderProgreso != null)
        {
            sliderProgreso.value = progreso;
        }

        // Actualizar texto
        if (textoProgreso != null)
        {
            textoProgreso.text = $"{SistemaMisiones.combatesRealizados}/{SistemaMisiones.COMBATES_REQUERIDOS} combates";
        }

        // Actualizar título
        if (tituloMision != null)
        {
            tituloMision.text = "Misión: Derrotar 5 Pokémon salvajes";
        }

        // Actualizar color del slider
        ActualizarColorSlider(progreso);
    }

    private void AnimarSlider()
    {
        if (sliderProgreso != null)
        {
            sliderActual = Mathf.Lerp(sliderActual, sliderObjetivo, Time.deltaTime * velocidadAnimacion);
            sliderProgreso.value = sliderActual;
        }
    }

    private void ActualizarColorSlider(float progreso)
    {
        if (rellenoSlider != null)
        {
            Color colorActual;

            if (progreso < 0.5f)
            {
                // De rojo a amarillo
                float t = progreso * 2f;
                colorActual = Color.Lerp(colorInicio, colorMitad, t);
            }
            else
            {
                // De amarillo a verde
                float t = (progreso - 0.5f) * 2f;
                colorActual = Color.Lerp(colorMitad, colorFinal, t);
            }

            rellenoSlider.color = colorActual;
        }
    }

    public void MostrarProgreso(bool mostrar)
    {
        gameObject.SetActive(mostrar && SistemaMisiones.misionCombateActivada);
    }

    public void ReiniciarProgreso()
    {
        sliderActual = 0f;
        sliderObjetivo = 0f;
        if (sliderProgreso != null)
            sliderProgreso.value = 0f;
    }
}
