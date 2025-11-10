using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ControladorCambio : MonoBehaviour
{
    public List<Button> botonesPokemon;
    public TextMeshProUGUI tituloTexto;

    private List<Porkemon> equipoJugador;
    private int primerSeleccionado = -1;
    private List<BattleItem> objetosSeleccionados = new List<BattleItem>();
    private const int MAX_OBJETOS_COMBATE = 6;

    void Start()
    {
        if (GestorDeBatalla.instance == null)
        {
            return;
        }

        bool esMochila = (GameState.player1Turn == false);

        if (esMochila)
        {
            InicializarMochila();
        }
        else
        {
            equipoJugador = GestorDeBatalla.instance.equipoJugador;
            ActualizarBotones();
            if (GameState.modoOrdenamiento)
            {
                tituloTexto.text = $"Ordenar Pokémon ({equipoJugador.Count})";
            }
            else
            {
                tituloTexto.text = $"Tienes {equipoJugador.Count} Pokémon";
            }
        }
    }

    private void InicializarMochila()
    {
        List<BattleItem> inventario = GestorDeBatalla.instance.inventarioBattleItems;
        
        if (GameState.modoOrdenamiento)
        {
            if (objetosSeleccionados.Count == 0)
            {
                objetosSeleccionados = new List<BattleItem>(inventario.GetRange(0, Mathf.Min(MAX_OBJETOS_COMBATE, inventario.Count)));
            }
            tituloTexto.text = $"Seleccionar Objetos ({objetosSeleccionados.Count}/{MAX_OBJETOS_COMBATE})";
        }
        else
        {
            tituloTexto.text = "Objetos de Batalla";
        }

        for (int i = 0; i < botonesPokemon.Count; i++)
        {
            if (i < inventario.Count)
            {
                BattleItem item = inventario[i];
                botonesPokemon[i].gameObject.SetActive(true);

                TextMeshProUGUI texto = botonesPokemon[i].GetComponentInChildren<TextMeshProUGUI>();
                if (texto != null)
                {
                    if (GameState.modoOrdenamiento)
                    {
                        bool seleccionado = objetosSeleccionados.Contains(item);
                        texto.text = $"{(seleccionado ? "[X] " : "[ ] ")}{item.nombre} x{item.cantidad}";
                    }
                    else
                    {
                        texto.text = $"{item.nombre} x{item.cantidad}";
                    }
                }

                botonesPokemon[i].onClick.RemoveAllListeners();
                int index = i;
                
                if (GameState.modoOrdenamiento)
                {
                    bool seleccionado = objetosSeleccionados.Contains(item);
                    ColorBlock colors = botonesPokemon[i].colors;
                    colors.normalColor = seleccionado ? new Color(0.3f, 0.6f, 0.3f) : Color.white;
                    botonesPokemon[i].colors = colors;
                    botonesPokemon[i].onClick.AddListener(() => ToggleSeleccionObjeto(index));
                }
                else
                {
                    botonesPokemon[i].onClick.AddListener(() => UsarItem(index));
                }
            }
            else
            {
                botonesPokemon[i].gameObject.SetActive(false);
            }
        }
    }

    private bool EsPorkebola(BattleItemType type)
    {
        return type == BattleItemType.Porkebola ||
               type == BattleItemType.Superbola ||
               type == BattleItemType.Ultrabola ||
               type == BattleItemType.Masterbola;
    }

    private void UsarItem(int index)
    {
        if (GameState.modoOrdenamiento)
        {
            return;
        }

        List<BattleItem> inventario = GestorDeBatalla.instance.inventarioBattleItems;
        if (index < 0 || index >= inventario.Count)
        {
            return;
        }

        BattleItem selectedItem = inventario[index];
        if (selectedItem.cantidad <= 0)
        {
            return;
        }

        if (EsPorkebola(selectedItem.type))
        {
            GameState.itemSeleccionado = selectedItem;
            GameState.player1Turn = true;
            SceneTransitionManager.Instance.LoadScene("Escena de combate");
        }
        else
        {
            Porkemon porkemonActivo = GestorDeBatalla.instance.porkemonJugador;
            if (porkemonActivo == null)
            {
                return;
            }

            AplicarEfectoItem(selectedItem, porkemonActivo);
            selectedItem.cantidad--;

            if (selectedItem.cantidad <= 0)
            {
                GestorDeBatalla.instance.inventarioBattleItems.Remove(selectedItem);
            }
            GameState.player1Turn = false;
            SceneTransitionManager.Instance.LoadScene("Escena de combate");
        }
    }

    private void AplicarEfectoItem(BattleItem item, Porkemon porkemon)
    {
        switch (item.type)
        {
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
        }
    }

    void ActualizarBotones()
    {
        Porkemon activo = GestorDeBatalla.instance.porkemonJugador;

        for (int i = 0; i < botonesPokemon.Count; i++)
        {
            if (i < equipoJugador.Count)
            {
                Porkemon p = equipoJugador[i];
                botonesPokemon[i].gameObject.SetActive(true);

                TextMeshProUGUI texto = botonesPokemon[i].GetComponentInChildren<TextMeshProUGUI>();
                if (texto != null)
                    texto.text = $"{p.BaseData.nombre} ({p.VidaActual}/{p.VidaMaxima})";

                if (GameState.modoOrdenamiento)
                {
                    botonesPokemon[i].interactable = true;
                    botonesPokemon[i].onClick.RemoveAllListeners();
                    ColorBlock colors = botonesPokemon[i].colors;
                    colors.normalColor = Color.white;
                    botonesPokemon[i].colors = colors;
                    int index = i; 
                    botonesPokemon[i].onClick.AddListener(() => OnButtonClicked(index));
                }
                else if (p == activo)
                {
                    botonesPokemon[i].interactable = false;
                    botonesPokemon[i].onClick.RemoveAllListeners();
                }
                else
                {
                    botonesPokemon[i].interactable = p.VidaActual > 0;
                    botonesPokemon[i].onClick.RemoveAllListeners();
                    int index = i; 
                    botonesPokemon[i].onClick.AddListener(() => OnButtonClicked(index));
                }
            }
            else
            {
                botonesPokemon[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnButtonClicked(int index)
    {
        if (index < 0 || index >= equipoJugador.Count)
        {
            return;
        }

        if (GameState.modoOrdenamiento)
        {
            if (primerSeleccionado == -1)
            {
                primerSeleccionado = index;
                ColorearBoton(index, new Color(0.3f, 0.6f, 0.3f));
            }
            else
            {
                IntercambiarPokemones(primerSeleccionado, index);
                ColorearBoton(primerSeleccionado, Color.white);
                primerSeleccionado = -1;
                ActualizarBotones();
            }
        }
        else
        {
            Porkemon selected = equipoJugador[index];
            SeleccionarPorkemon(selected);
        }
    }

    public void SeleccionarPorkemon(Porkemon nuevo)
    {
        if (GameState.modoOrdenamiento || nuevo == null)
        {
            return;
        }

        if (nuevo.VidaActual <= 0)
        {
            return;
        }

        GestorDeBatalla.instance.porkemonJugador = nuevo;
        GameState.porkemonDelJugador = nuevo; 

        GameState.player1Turn = false;

        SceneTransitionManager.Instance.LoadScene("Escena de combate");
    }
    
    private void ToggleSeleccionObjeto(int index)
    {
        List<BattleItem> inventario = GestorDeBatalla.instance.inventarioBattleItems;
        if (index < 0 || index >= inventario.Count) return;

        BattleItem item = inventario[index];

        if (objetosSeleccionados.Contains(item))
        {
            objetosSeleccionados.Remove(item);
        }
        else
        {
            if (objetosSeleccionados.Count < MAX_OBJETOS_COMBATE)
            {
                objetosSeleccionados.Add(item);
            }
        }

        InicializarMochila();
    }

    public void IntercambiarPokemones(int indice1, int indice2)
    {
        if (indice1 == indice2) return;
        if (indice1 < 0 || indice1 >= equipoJugador.Count) return;
        if (indice2 < 0 || indice2 >= equipoJugador.Count) return;

        Porkemon temp = equipoJugador[indice1];
        equipoJugador[indice1] = equipoJugador[indice2];
        equipoJugador[indice2] = temp;
    }

    private void ColorearBoton(int index, Color color)
    {
        if (index >= 0 && index < botonesPokemon.Count)
        {
            ColorBlock colors = botonesPokemon[index].colors;
            colors.normalColor = color;
            botonesPokemon[index].colors = colors;
        }
    }

    public void CancelarCambio()
    {
        if (GameState.modoOrdenamiento)
        {
            if (GameState.player1Turn == false)
            {
                GestorDeBatalla.instance.inventarioBattleItems.Clear();
                GestorDeBatalla.instance.inventarioBattleItems.AddRange(objetosSeleccionados);
            }
            primerSeleccionado = -1;
            GameState.modoOrdenamiento = false;
            SceneTransitionManager.Instance.LoadScene("Escena Principal");
        }
        else
        {
            GameState.player1Turn = true;
            SceneTransitionManager.Instance.LoadScene("Escena de combate");
        }
    }
}