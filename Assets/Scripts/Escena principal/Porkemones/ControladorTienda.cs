using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ControladorTienda : MonoBehaviour
{
    public TextMeshProUGUI textoDineroActual;
    public TextMeshProUGUI textoPrecio;
    public TextMeshProUGUI textoMensaje;
    public TextMeshProUGUI textoDescripcionSeleccionada;
    public List<Button> botonesItems;
    public List<TiendaItemData> itemsTienda = new List<TiendaItemData>(); // items del día actual
    public List<TiendaItemData> catalogoBase = new List<TiendaItemData>(); // catálogo total de la tienda
    public List<TiendaItemData> catalogoTienda = new List<TiendaItemData>(); // catálogo fijo de la tienda

    private HashSet<int> indicesSeleccionados = new HashSet<int>();
    private int totalCompra = 0;

    private static int ultimoDiaGenerado = -1;
    private static List<TiendaItemData> cacheItemsDia = new List<TiendaItemData>();

    private void Start()
    {
        if (!GameState.modoTienda)
        {
            gameObject.SetActive(false);
            return;
        }

        InicializarItemsDelDia();
        ActualizarUICompleta();
    }

    private void OnEnable()
    {
        if (!GameState.modoTienda)
            return;

        InicializarItemsDelDia();
        ActualizarUICompleta();
    }

    private void InicializarItemsDelDia()
    {
        // Inicializar catálogo fijo de la tienda si no está inicializado
        if (catalogoTienda == null || catalogoTienda.Count == 0)
        {
            catalogoTienda = new List<TiendaItemData>
            {
                new TiendaItemData { type = BattleItemType.Pocion, nombre = "Poción", descripcion = "Restaura 20 PS", stock = 99, precio = 200 },
                new TiendaItemData { type = BattleItemType.Superpocion, nombre = "Superpoción", descripcion = "Restaura 50 PS", stock = 99, precio = 400 },
                new TiendaItemData { type = BattleItemType.Hiperpocion, nombre = "Hiperpoción", descripcion = "Restaura 120 PS", stock = 99, precio = 600 },
                new TiendaItemData { type = BattleItemType.Pocionmaxima, nombre = "Poción Máxima", descripcion = "Restaura todos los PS", stock = 99, precio = 800 },
                new TiendaItemData { type = BattleItemType.Revivir, nombre = "Revivir", descripcion = "Revive un Pokémon con 30% de PS", stock = 99, precio = 1000 },
                new TiendaItemData { type = BattleItemType.RevivirMax, nombre = "Revivir Máx", descripcion = "Revive un Pokémon con todos sus PS", stock = 99, precio = 1500 },
                new TiendaItemData { type = BattleItemType.Porkebola, nombre = "Pokéball", descripcion = "Atrapa Pokémon salvajes más fácilmente", stock = 99, precio = 150 },
                new TiendaItemData { type = BattleItemType.Superbola, nombre = "Superball", descripcion = "Más efectiva que una Pokéball normal", stock = 99, precio = 300 },
                new TiendaItemData { type = BattleItemType.Ultrabola, nombre = "Ultraball", descripcion = "Muy efectiva para Pokémon difíciles de atrapar", stock = 99, precio = 600 },
                new TiendaItemData { type = BattleItemType.Masterbola, nombre = "Masterball", descripcion = "Atrapa cualquier Pokémon sin fallar", stock = 99, precio = 2000 },
                new TiendaItemData { type = BattleItemType.AtaqueX, nombre = "Ataque X", descripcion = "Aumenta el ataque en 1 nivel", stock = 99, precio = 500 },
                new TiendaItemData { type = BattleItemType.DefensaX, nombre = "Defensa X", descripcion = "Aumenta la defensa en 1 nivel", stock = 99, precio = 500 },
                new TiendaItemData { type = BattleItemType.AtaqueEspecialX, nombre = "Ataque Especial X", descripcion = "Aumenta el ataque especial en 1 nivel", stock = 99, precio = 500 },
                new TiendaItemData { type = BattleItemType.DefensaEspecialX, nombre = "Defensa Especial X", descripcion = "Aumenta la defensa especial en 1 nivel", stock = 99, precio = 500 },
                new TiendaItemData { type = BattleItemType.VelocidadX, nombre = "Velocidad X", descripcion = "Aumenta la velocidad en 1 nivel", stock = 99, precio = 500 },
                new TiendaItemData { type = BattleItemType.PrecisionX, nombre = "Precisión X", descripcion = "Aumenta la precisión en 1 nivel", stock = 99, precio = 500 },
                new TiendaItemData { type = BattleItemType.CriticoX, nombre = "Crítico X", descripcion = "Aumenta la probabilidad de golpe crítico", stock = 99, precio = 500 },
                new TiendaItemData { type = BattleItemType.ProteccionX, nombre = "Protección X", descripcion = "Aumenta la evasión durante 5 turnos", stock = 99, precio = 500 }
            };
        }

        int currentDay = 0;
        if (TimeOfDayManager.Instance != null)
        {
            currentDay = TimeOfDayManager.Instance.currentDay;
        }

        // Si ya generamos los items para este día, restauramos desde el caché
        if (ultimoDiaGenerado == currentDay && cacheItemsDia.Count > 0)
        {
            itemsTienda = new List<TiendaItemData>();
            foreach (var it in cacheItemsDia)
            {
                itemsTienda.Add(CopiarItem(it));
            }
            return;
        }

        itemsTienda.Clear();
        cacheItemsDia.Clear();
        indicesSeleccionados.Clear();

        int numItemsDia = Mathf.Min(6, catalogoTienda.Count);

        List<int> indicesDisponibles = new List<int>();
        for (int i = 0; i < catalogoTienda.Count; i++)
        {
            indicesDisponibles.Add(i);
        }

        for (int j = 0; j < numItemsDia; j++)
        {
            if (indicesDisponibles.Count == 0)
                break;

            int idxLista = Random.Range(0, indicesDisponibles.Count);
            int idxCatalogo = indicesDisponibles[idxLista];
            indicesDisponibles.RemoveAt(idxLista);

            TiendaItemData baseItem = catalogoTienda[idxCatalogo];
            if (baseItem == null)
                continue;

            TiendaItemData itemDia = CopiarItem(baseItem);
            itemsTienda.Add(itemDia);
            cacheItemsDia.Add(CopiarItem(itemDia));
        }

        ultimoDiaGenerado = currentDay;
    }

    public void ToggleSeleccionItem(int index)
    {
        if (index < 0 || index >= itemsTienda.Count)
            return;

        TiendaItemData item = itemsTienda[index];
        if (item == null || item.stock <= 0)
            return;

        if (indicesSeleccionados.Contains(index))
            indicesSeleccionados.Remove(index);
        else
            indicesSeleccionados.Add(index);

        RecalcularTotal();
        ConfigurarBotones();
        ActualizarDescripcionSeleccionada(index);
    }

    private void ActualizarDescripcionSeleccionada(int index)
    {
        if (textoDescripcionSeleccionada == null)
            return;

        if (index >= 0 && index < itemsTienda.Count)
        {
            TiendaItemData item = itemsTienda[index];
            if (item != null)
            {
                textoDescripcionSeleccionada.text = item.nombre + "\n" + item.descripcion;
                return;
            }
        }

        textoDescripcionSeleccionada.text = string.Empty;
    }

    public void ConfirmarCompra()
    {
        if (indicesSeleccionados.Count == 0)
            return;

        if (GameState.dineroJugador < totalCompra)
        {
            if (textoMensaje != null)
                textoMensaje.text = "No te alcanza el dinero";
            return;
        }

        GameState.dineroJugador -= totalCompra;

        foreach (int index in indicesSeleccionados)
        {
            if (index < 0 || index >= itemsTienda.Count)
                continue;

            TiendaItemData item = itemsTienda[index];
            if (item == null || item.stock <= 0)
                continue;

            item.stock = Mathf.Max(0, item.stock - 1);

            if (cacheItemsDia != null && index >= 0 && index < cacheItemsDia.Count && cacheItemsDia[index] != null)
            {
                cacheItemsDia[index].stock = item.stock;
            }

            if (GestorDeBatalla.instance != null)
            {
                var inventario = GestorDeBatalla.instance.inventarioCompleto;
                BattleItem existente = inventario.Find(i => i != null && i.type == item.type && i.nombre == item.nombre);
                if (existente != null)
                {
                    existente.cantidad += 1;
                }
                else
                {
                    inventario.Add(new BattleItem(item.type, item.nombre, item.descripcion, 1));
                }
            }
        }

        indicesSeleccionados.Clear();

        if (textoMensaje != null)
            textoMensaje.text = "Compra realizada";

        ActualizarUICompleta();
    }

    public void ConfirmarCompraYSalir()
    {
        int dineroAntes = GameState.dineroJugador;
        int seleccionAntes = indicesSeleccionados.Count;

        ConfirmarCompra();

        bool huboCompra = (seleccionAntes > 0 && GameState.dineroJugador < dineroAntes);
        if (huboCompra)
        {
            GameState.modoTienda = false;
            SceneTransitionManager.GetInstance().LoadScene("Escena Principal");
        }
    }

    private void ActualizarUICompleta()
    {
        ConfigurarBotones();
        RecalcularTotal();
    }

    private void ConfigurarBotones()
    {
        if (botonesItems == null)
            return;

        for (int i = 0; i < botonesItems.Count; i++)
        {
            Button boton = botonesItems[i];
            if (boton == null)
                continue;

            if (i < itemsTienda.Count && itemsTienda[i] != null)
            {
                boton.gameObject.SetActive(true);

                TiendaItemData item = itemsTienda[i];
                bool agotado = item.stock <= 0;
                boton.interactable = !agotado;

                TextMeshProUGUI texto = boton.GetComponentInChildren<TextMeshProUGUI>();
                if (texto != null)
                {
                    bool seleccionado = indicesSeleccionados.Contains(i);
                    string prefijo = (!agotado && seleccionado) ? "[X] " : "[ ] ";

                    string lineaNombre = $"{prefijo}{item.nombre}";
                    string lineaDescripcion = item.descripcion;

                    int precioActual = ObtenerPrecioActual(item);

                    string lineaCantidadPrecio = agotado
                        ? "Stock: 0 - AGOTADO"
                        : $"Stock: {item.stock} - ${precioActual}";

                    texto.text = $"{lineaNombre}\n{lineaCantidadPrecio}";
                }

                boton.onClick.RemoveAllListeners();
                int indexCopia = i;
                boton.onClick.AddListener(() => ToggleSeleccionItem(indexCopia));
            }
            else
            {
                boton.gameObject.SetActive(false);
            }
        }
    }

    private void RecalcularTotal()
    {
        totalCompra = 0;

        foreach (int index in indicesSeleccionados)
        {
            if (index < 0 || index >= itemsTienda.Count)
                continue;

            TiendaItemData item = itemsTienda[index];
            if (item != null)
                totalCompra += Mathf.Max(0, ObtenerPrecioActual(item));
        }

        if (textoDineroActual != null)
            textoDineroActual.text = $"Dinero: {GameState.dineroJugador}";

        if (textoPrecio != null)
            textoPrecio.text = $"Total: {totalCompra}";
    }

    private int ObtenerPrecioActual(TiendaItemData item)
    {
        if (item == null)
            return 0;

        float mult = Mathf.Max(0.1f, GameState.multiplicadorPreciosTienda);
        int precio = Mathf.RoundToInt(item.precio * mult);
        return Mathf.Max(0, precio);
    }

    private static TiendaItemData CopiarItem(TiendaItemData source)
    {
        if (source == null)
            return null;

        TiendaItemData copy = new TiendaItemData();
        copy.type = source.type;
        copy.nombre = source.nombre;
        copy.descripcion = source.descripcion;
        copy.stock = source.stock;
        copy.precio = source.precio;
        return copy;
    }

    private static int GetPrecioPorDefecto(BattleItemType type)
    {
        switch (type)
        {
            case BattleItemType.Pocion: return 200;
            case BattleItemType.Superpocion: return 400;
            case BattleItemType.Hiperpocion: return 600;
            case BattleItemType.Pocionmaxima: return 800;
            case BattleItemType.Revivir: return 1000;
            case BattleItemType.RevivirMax: return 1500;
            case BattleItemType.Porkebola: return 150;
            case BattleItemType.Superbola: return 300;
            case BattleItemType.Ultrabola: return 600;
            case BattleItemType.Masterbola: return 2000;
            case BattleItemType.AtaqueX:
            case BattleItemType.DefensaX:
            case BattleItemType.AtaqueEspecialX:
            case BattleItemType.DefensaEspecialX:
            case BattleItemType.VelocidadX:
            case BattleItemType.PrecisionX:
            case BattleItemType.CriticoX:
            case BattleItemType.ProteccionX:
                return 500;
            default:
                return 200;
        }
    }
}

[System.Serializable]
public class TiendaItemData
{
    public BattleItemType type;
    public string nombre;
    public string descripcion;
    public int stock;
    public int precio;
}

