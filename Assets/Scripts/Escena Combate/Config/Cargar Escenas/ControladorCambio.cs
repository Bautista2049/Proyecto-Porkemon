using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ControladorCambio : MonoBehaviour
{
    public List<Button> botonesPokemon;
    public TextMeshProUGUI tituloTexto;
    public GameObject panelConfirmacion;
    public Button botonConfirmar;
    public Button botonCancelar;

    private List<Porkemon> equipoJugador;
    private int primerSeleccionado = -1;
    private static List<BattleItem> objetosSeleccionados = new List<BattleItem>();
    private const int MAX_OBJETOS_COMBATE = 6;
    private bool mochilaInicializada = false;

    void Start()
    {
        if (GestorDeBatalla.instance == null)
        {
            Debug.LogError("GestorDeBatalla no encontrado");
            return;
        }

        if (botonConfirmar != null)
            botonConfirmar.onClick.AddListener(ConfirmarCambios);
            
        if (botonCancelar != null)
            botonCancelar.onClick.AddListener(CancelarCambio);
            
        if (panelConfirmacion != null)
            panelConfirmacion.SetActive(false);

        bool esMochila = (GameState.player1Turn == false && !GameState.modoRevivir);

        if (esMochila)
        {
            InicializarMochila();
        }
        else
        {
            equipoJugador = GestorDeBatalla.instance.equipoJugador;
            ActualizarBotones();
            if (GameState.modoRevivir)
            {
                tituloTexto.text = "Selecciona un Pokémon para revivir";
            }
            else if (GameState.modoOrdenamiento)
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
        List<BattleItem> inventarioAMostrar = GameState.modoOrdenamiento ? 
            GestorDeBatalla.instance.inventarioCompleto : 
            GestorDeBatalla.instance.inventarioBattleItems;
        if (inventarioAMostrar != null)
        {
            inventarioAMostrar.RemoveAll(i => i == null || i.cantidad <= 0);
        }
        
        if (GameState.modoOrdenamiento)
        {
            if (!mochilaInicializada)
            {
                objetosSeleccionados.Clear();
                foreach (var item in GestorDeBatalla.instance.inventarioBattleItems)
                {
                    var itemEncontrado = GestorDeBatalla.instance.inventarioCompleto.Find(i => 
                        i.nombre == item.nombre && i.type == item.type);
                    if (itemEncontrado != null)
                    {
                        objetosSeleccionados.Add(itemEncontrado);
                    }
                }
                mochilaInicializada = true;
            }
            tituloTexto.text = $"Selecciona hasta {MAX_OBJETOS_COMBATE} objetos para el combate ({objetosSeleccionados.Count}/{MAX_OBJETOS_COMBATE})";
        }
        else
        {
            tituloTexto.text = "Objetos de Batalla";
        }

        for (int i = 0; i < botonesPokemon.Count; i++)
        {
            if (i < inventarioAMostrar.Count)
            {
                BattleItem item = inventarioAMostrar[i];
                botonesPokemon[i].gameObject.SetActive(true);

                TextMeshProUGUI texto = botonesPokemon[i].GetComponentInChildren<TextMeshProUGUI>();
                if (texto != null)
                {
                    if (GameState.modoOrdenamiento)
                    {
                        bool seleccionado = objetosSeleccionados.Exists(obj => obj.nombre == item.nombre && obj.type == item.type);
                        texto.text = $"{(seleccionado ? "[X] " : "[ ] ")}{item.nombre} x{item.cantidad}";
                        
                        ColorBlock colors = botonesPokemon[i].colors;
                        colors.normalColor = seleccionado ? new Color(0.3f, 0.6f, 0.3f) : Color.white;
                        botonesPokemon[i].colors = colors;
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
            if (selectedItem.type == BattleItemType.Revivir || selectedItem.type == BattleItemType.RevivirMax)
            {
                GameState.itemSeleccionado = selectedItem;
                GameState.modoRevivir = true;
                GameState.player1Turn = true;
                SceneTransitionManager.Instance.LoadScene("Escena CambioPorkemon");
                return;
            }

            Porkemon porkemonActivo = GestorDeBatalla.instance.porkemonJugador;
            if (porkemonActivo == null)
            {
                return;
            }

            AplicarEfectoItem(selectedItem, porkemonActivo);
            selectedItem.cantidad--;
            
            GestorDeBatalla.instance.SincronizarInventarioCompleto(selectedItem);

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

        if (GameState.modoRevivir)
        {
            if (nuevo.VidaActual > 0)
            {
                return;
            }

            BattleItem item = GameState.itemSeleccionado;
            if (item == null)
            {
                return;
            }

            if (item.type == BattleItemType.Revivir)
            {
                nuevo.VidaActual = Mathf.FloorToInt(nuevo.VidaMaxima * 0.3f);
                nuevo.Estado = EstadoAlterado.Ninguno;
                Debug.Log($"{nuevo.BaseData.nombre} ha revivido con el 30% de sus PS!");
            }
            else if (item.type == BattleItemType.RevivirMax)
            {
                nuevo.VidaActual = nuevo.VidaMaxima;
                nuevo.Estado = EstadoAlterado.Ninguno;
                Debug.Log($"{nuevo.BaseData.nombre} ha revivido con todos sus PS!");
            }
            else
            {
                return;
            }

            item.cantidad--;
            
            GestorDeBatalla.instance.SincronizarInventarioCompleto(item);

            if (item.cantidad <= 0)
            {
                GestorDeBatalla.instance.inventarioBattleItems.Remove(item);
            }

            GameState.itemSeleccionado = null;
            GameState.modoRevivir = false;

            GameState.player1Turn = false;
            SceneTransitionManager.Instance.LoadScene("Escena de combate");
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
        List<BattleItem> inventario = GestorDeBatalla.instance.inventarioCompleto;
        if (index < 0 || index >= inventario.Count) return;

        BattleItem item = inventario[index];
        
       
        var itemSeleccionado = objetosSeleccionados.Find(i => i.nombre == item.nombre && i.type == item.type);

        if (itemSeleccionado != null)
        {
            objetosSeleccionados.Remove(itemSeleccionado);
        }
        else
        {
            if (objetosSeleccionados.Count < MAX_OBJETOS_COMBATE)
            {
                var copiaItem = new BattleItem(item.type, item.nombre, item.descripcion, item.cantidad);
                objetosSeleccionados.Add(copiaItem);
            }
            else
            {
                Debug.Log($"Solo puedes seleccionar hasta {MAX_OBJETOS_COMBATE} objetos para el combate");
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

    public void ConfirmarCambios()
    {
        if (GameState.modoOrdenamiento)
        {
            if (GameState.player1Turn == false) 
            {
                if (objetosSeleccionados.Count > MAX_OBJETOS_COMBATE)
                {
                    Debug.LogError("¡No puedes seleccionar más de 6 objetos para el combate!");
                    return;
                }

                GestorDeBatalla.instance.inventarioBattleItems.Clear();
                
                foreach (var item in objetosSeleccionados)
                {
                    var itemCompleto = GestorDeBatalla.instance.inventarioCompleto.Find(i => 
                        i.nombre == item.nombre && i.type == item.type);
                        
                    if (itemCompleto != null)
                    {
                        var itemCopia = new BattleItem(item.type, item.nombre, item.descripcion, item.cantidad);
                        GestorDeBatalla.instance.inventarioBattleItems.Add(itemCopia);
                    }
                }
                
                Debug.Log($"Inventario de combate actualizado con {GestorDeBatalla.instance.inventarioBattleItems.Count} objetos");
            }
            
            if (panelConfirmacion != null)
                panelConfirmacion.SetActive(false);
                
            primerSeleccionado = -1;
            GameState.modoOrdenamiento = false;
            SceneTransitionManager.Instance.LoadScene("Escena Principal");
        }
    }

    public void CancelarCambio()
    {
        if (GameState.modoOrdenamiento)
        {
            if (panelConfirmacion != null)
            {
                panelConfirmacion.SetActive(true);
            }
            else
            {
                RealizarCancelacion();
            }
        }
        else
        {
            GameState.player1Turn = true;
            SceneTransitionManager.Instance.LoadScene("Escena de combate");
        }
    }
    
    private void RealizarCancelacion()
    {
        if (GameState.player1Turn == false)
        {
            objetosSeleccionados = new List<BattleItem>(GestorDeBatalla.instance.inventarioBattleItems);
        }
        primerSeleccionado = -1;
        GameState.modoOrdenamiento = false;
        SceneTransitionManager.Instance.LoadScene("Escena Principal");
    }
}