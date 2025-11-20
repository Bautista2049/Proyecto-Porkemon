using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ControladorMochila : MonoBehaviour
{
    public List<Button> botonesItems;
    public TextMeshProUGUI tituloTexto;
    public AudioClip sonidoSacudida;
    public AudioClip sonidoCaptura;
    public ParticleSystem efectoCaptura;

    private List<BattleItem> inventario;

    void Start()
    {
        if (GestorDeBatalla.instance == null)
        {
            Debug.LogWarning("GestorDeBatalla.instance es null en ControladorMochila.Start");
            return;
        }
        inventario = GestorDeBatalla.instance.inventarioBattleItems;
        ActualizarBotonesItems();
        tituloTexto.text = $"Objetos de Batalla ({inventario.Count})";
    }

    void ActualizarBotonesItems()
    {
        if (inventario != null)
        {
            inventario.RemoveAll(i => i == null || i.cantidad <= 0);
        }

        for (int i = 0; i < botonesItems.Count; i++)
        {
            if (i < inventario.Count)
            {
                BattleItem item = inventario[i];
                botonesItems[i].gameObject.SetActive(true);

                TextMeshProUGUI texto = botonesItems[i].GetComponentInChildren<TextMeshProUGUI>();
                if (texto != null)
                    texto.text = $"{item.nombre} x{item.cantidad}";

                botonesItems[i].onClick.RemoveAllListeners();
                int index = i;
                botonesItems[i].onClick.AddListener(() => OnItemClicked(index));
            }
            else
            {
                botonesItems[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnItemClicked(int index)
    {
        if (index < 0 || index >= inventario.Count)
        {
            return;
        }

        BattleItem selectedItem = inventario[index];
        UsarItem(selectedItem);
    }

    private BattleItem itemSeleccionado;
    private bool esperandoSeleccionPokemon = false;

    public void OnPokemonSeleccionado(Porkemon pokemon)
    {
        if (!esperandoSeleccionPokemon || itemSeleccionado == null) return;

        AplicarEfectoItem(itemSeleccionado, pokemon);
        
        itemSeleccionado.cantidad--;

        GestorDeBatalla.instance.SincronizarInventarioCompleto(itemSeleccionado);

        if (itemSeleccionado.cantidad <= 0)
        {
            GestorDeBatalla.instance.inventarioBattleItems.Remove(itemSeleccionado);
        }

        ActualizarBotonesItems();
        tituloTexto.text = $"Objetos de Batalla ({GestorDeBatalla.instance.inventarioBattleItems.Count})";
        
        StartCoroutine(VolverCombateConDelay());
        
        itemSeleccionado = null;
        esperandoSeleccionPokemon = false;
    }

    public void UsarItem(BattleItem item)
    {
        if (item.cantidad <= 0)
        {
            Debug.Log("Intentaron usar un item sin cantidad.");
            return;
        }

        if (GestorDeBatalla.instance == null)
        {
            Debug.LogError("GestorDeBatalla.instance es null en UsarItem");
            return;
        }

        if (item.type == BattleItemType.Revivir || item.type == BattleItemType.RevivirMax)
        {
            GameState.itemSeleccionado = item;
            GameState.modoRevivir = true;
            GameState.player1Turn = true;
            SceneTransitionManager.Instance.LoadScene("Escena CambioPorkemon");
            return;
        }

        Porkemon porkemonActivo = GestorDeBatalla.instance.porkemonJugador;
        if (porkemonActivo == null)
        {
            Debug.LogError("porkemonActivo es null en UsarItem");
            return;
        }

        AplicarEfectoItem(item, porkemonActivo);
        item.cantidad--;

        GestorDeBatalla.instance.SincronizarInventarioCompleto(item);

        if (item.cantidad <= 0)
        {
            GestorDeBatalla.instance.inventarioBattleItems.Remove(item);
        }

        ActualizarBotonesItems();
        tituloTexto.text = $"Objetos de Batalla ({GestorDeBatalla.instance.inventarioBattleItems.Count})";

        Debug.Log($"Usado {item.nombre} sobre {porkemonActivo.BaseData?.nombre ?? "sin-nombre"}. Cantidad restante: {item.cantidad}");

        StartCoroutine(VolverCombateConDelay());
    }

    private IEnumerator VolverCombateConDelay()
    {
        yield return new WaitForSeconds(1.5f);  
        SceneTransitionManager.Instance.LoadScene("Escena de combate");
    }

    private void AplicarEfectoItem(BattleItem item, Porkemon porkemon)
    {
        switch (item.type)
        {
            case BattleItemType.Pocion:
                int curacion20 = Mathf.Min(20, porkemon.VidaMaxima - porkemon.VidaActual);
                porkemon.VidaActual += curacion20;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Recuperó {curacion20} PS!");
                break;
            case BattleItemType.Superpocion:
                int curacion50 = Mathf.Min(50, porkemon.VidaMaxima - porkemon.VidaActual);
                porkemon.VidaActual += curacion50;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Recuperó {curacion50} PS!");
                break;
            case BattleItemType.Hiperpocion:
                int curacion200 = Mathf.Min(200, porkemon.VidaMaxima - porkemon.VidaActual);
                porkemon.VidaActual += curacion200;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Recuperó {curacion200} PS!");
                break;
            case BattleItemType.Pocionmaxima:
                int curacionMax = porkemon.VidaMaxima - porkemon.VidaActual;
                porkemon.VidaActual = porkemon.VidaMaxima;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Recuperó {curacionMax} PS!");
                break;
            case BattleItemType.AtaqueX:
                porkemon.AumentarAtaque(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Ataque aumentado!");
                break;
            case BattleItemType.DefensaX:
                porkemon.AumentarDefensa(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Defensa aumentada!");
                break;
            case BattleItemType.AtaqueEspecialX:
                porkemon.AumentarEspiritu(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Ataque Especial aumentado!");
                break;
            case BattleItemType.DefensaEspecialX:
                porkemon.AumentarEspiritu(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Defensa Especial aumentada!");
                break;
            case BattleItemType.VelocidadX:
                porkemon.AumentarVelocidad(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Velocidad aumentada!");
                break;
            case BattleItemType.PrecisionX:
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Precisión aumentada!");
                break;
            case BattleItemType.CriticoX:
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Índice crítico aumentado!");
                break;
            case BattleItemType.ProteccionX:
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Protección activada!");
                break;
            case BattleItemType.Porkebola:
                StartCoroutine(IniciarCaptura());
                break;
            case BattleItemType.Superbola:
                StartCoroutine(IniciarCaptura());
                break;
            case BattleItemType.Ultrabola:
                StartCoroutine(IniciarCaptura());
                break;
            case BattleItemType.Masterbola:
                StartCoroutine(IniciarCaptura());
                break;
            case BattleItemType.RotoPremio:
                GameState.multiplicadorDinero = 3f;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡Las recompensas de dinero aumentarán esta batalla!");
                break;
            case BattleItemType.RotoExp:
                GameState.multiplicadorExp = 1.5f;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡La experiencia ganada aumentará esta batalla!");
                break;
            case BattleItemType.RotoBoost:
                porkemon.AumentarAtaque(2);
                porkemon.AumentarDefensa(2);
                porkemon.AumentarEspiritu(2);
                porkemon.AumentarVelocidad(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡Todas sus estadísticas han aumentado!");
                break;
            case BattleItemType.RotoCatch:
                GameState.multiplicadorCaptura = 2f;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡La probabilidad de captura ha aumentado esta batalla!");
                break;
            case BattleItemType.RotoOferta:
                GameState.multiplicadorPreciosTienda = 0.5f;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡Los precios de la tienda se han reducido temporalmente!");
                break;
        }
    }

    private bool CalcularCaptura(Porkemon pokemon, BattleItemType tipoPorkebola)
    {
        float vidaActual = pokemon.BaseData.vidaMaxima;
        float vidaMaxima = pokemon.BaseData.vidaMaxima;
        float probabilidadBase = (pokemon.BaseData.baseRate * (1 - (vidaActual / vidaMaxima))) / 1.5f;
        switch (tipoPorkebola)
        {
            case BattleItemType.Porkebola:
                break;
        }
        
        return Random.Range(0f, 100f) < probabilidadBase;
    }

    private IEnumerator IniciarCaptura()
    {
        Porkemon pokemonSalvaje = GestorDeBatalla.instance.porkemonBot;
        if (pokemonSalvaje == null)
        {
            Debug.LogError("No hay pokemon salvaje para capturar");
            yield break;
        }
        GameObject pokebola = Instantiate(Resources.Load<GameObject>("Objects/Pokebola"));
        Vector3 posicionInicial = GestorDeBatalla.instance.posicionJugador.position;
        pokebola.transform.position = posicionInicial;
        float duracion = 1f;
        float tiempoPasado = 0;
        Vector3 posicionFinal = GestorDeBatalla.instance.puntoSpawnBot.position;

        while (tiempoPasado < duracion)
        {
            tiempoPasado += Time.deltaTime;
            float t = tiempoPasado / duracion;
            
            Vector3 altura = Vector3.up * 2f * Mathf.Sin(t * Mathf.PI);
            pokebola.transform.position = Vector3.Lerp(posicionInicial, posicionFinal, t) + altura;
            pokebola.transform.Rotate(Vector3.right * 360 * Time.deltaTime);

            yield return null;
        }

        bool capturaExitosa = CalcularCaptura(pokemonSalvaje, BattleItemType.Porkebola);
        
        if (capturaExitosa)
        {
            GestorDeBatalla.instance.porkemonBot = null;
            yield return StartCoroutine(AnimacionCapturaExitosa(pokebola));
            GestorDeBatalla.instance.PokemonCapturado(pokemonSalvaje);
        }
        else
        {
            yield return StartCoroutine(AnimacionCapturaFallida(pokebola));
        }

        StartCoroutine(VolverCombateConDelay());
    }

    private IEnumerator AnimacionCapturaExitosa(GameObject porkebola)
    {
        AudioSource audioSource = porkebola.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = porkebola.AddComponent<AudioSource>();
            
        for (int i = 0; i < 3; i++)
        {
            if (sonidoSacudida != null)
                audioSource.PlayOneShot(sonidoSacudida);
            
            porkebola.transform.localScale = Vector3.one * 1.2f;
            yield return new WaitForSeconds(0.2f);
            porkebola.transform.localScale = Vector3.one;
            yield return new WaitForSeconds(0.5f);
        }
        
        if (sonidoCaptura != null)
            audioSource.PlayOneShot(sonidoCaptura);
        
        if (efectoCaptura != null)
        {
            ParticleSystem particulas = Instantiate(efectoCaptura, porkebola.transform.position, Quaternion.identity);
            Destroy(particulas.gameObject, 2f);
        }
        
        yield return new WaitForSeconds(1f);
        Destroy(porkebola);
    }

    private IEnumerator AnimacionCapturaFallida(GameObject pokebola)
    {
        pokebola.transform.localScale = Vector3.one * 1.2f;
        yield return new WaitForSeconds(0.5f);
        Destroy(pokebola);
        Debug.Log("El Pokémon se ha escapado!");
    }

    public void CancelarMochila()
    {
        SceneTransitionManager.Instance.LoadScene("Escena de combate");
    }
}
