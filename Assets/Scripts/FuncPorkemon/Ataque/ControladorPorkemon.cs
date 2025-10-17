using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ControladorPorkemon : MonoBehaviour
{
    [Header("Data")]
    public Porkemon porkemon;

    [Header("UI")]
    public Slider barraSalud;
    public Text textoSalud;
    public Text textoNombre;
    public TextMeshProUGUI textoMensaje; // Para mostrar mensajes de daño con efecto typewriter

    [Header("Model")]
    public GameObject modelParentContainer; // The GameObject whose transform will hold the model (e.g., a child object for the model)

    public void Setup(Porkemon pInstance)
    {
        porkemon = pInstance;

        if (textoNombre != null)
            textoNombre.text = porkemon.BaseData.nombre;

        porkemon.OnHPChanged += ActualizarUI;
        ActualizarUI();

        LoadModel();
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
            StartCoroutine(MostrarMensajeTypewriter($"¡El ataque {ataque.nombreAtaque} ha fallado!"));
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

        // Mostrar mensaje con efecto typewriter
        string mensajeDanio = $"{atacante.BaseData.nombre} hace {danioTotal} de daño con el ataque {ataque.nombreAtaque} a {porkemon.BaseData.nombre}.";
        StartCoroutine(MostrarMensajeTypewriter(mensajeDanio));

        if (Random.Range(0, 100f) < ataque.chanceCritico)
        {
            multiplicadorCritico = 3f;
            StartCoroutine(MostrarMensajeTypewriter("¡Un golpe crítico!"));
        }

        if (porkemon.VidaActual <= 0)
        {
            porkemon.VidaActual = 0;
            return true;
        }
        return false;
    }

    private void LoadModel()
    {
        if (porkemon == null || porkemon.BaseData.modeloPrefab == null) return;

        Transform modelParent = (modelParentContainer != null ? modelParentContainer.transform : transform);

        if (modelParent.gameObject.scene != null)
        {
            foreach (Transform child in modelParent)
            {
                DestroyImmediate(child.gameObject, true);
            }
        }

        GameObject modelInstance = Instantiate(porkemon.BaseData.modeloPrefab);

        if (modelParent.gameObject.scene != null)
        {
            modelInstance.transform.SetParent(modelParent);
            modelInstance.transform.localPosition = Vector3.zero;
            modelInstance.transform.localRotation = Quaternion.identity;
            modelInstance.transform.localScale = Vector3.one;
        }
        else
        {
            modelInstance.transform.position = modelParent.position;
            modelInstance.transform.rotation = modelParent.rotation;
            modelInstance.transform.localScale = modelParent.localScale;
        }
    }

    private IEnumerator MostrarMensajeTypewriter(string mensaje)
    {
        Debug.Log($"{mensaje}");
        if (textoMensaje != null)
        {

            textoMensaje.text = "";
            foreach (char letra in mensaje)
            {
                textoMensaje.text += letra;
                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(2f);
            textoMensaje.text = "";
        }
    }
}
