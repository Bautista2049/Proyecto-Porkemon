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
        // Si el catálogo base está vacío, intentamos llenarlo automáticamente
        // usando el inventario completo del GestorDeBatalla (lista global de objetos).
        if ((catalogoBase == null || catalogoBase.Count == 0) &&
            GestorDeBatalla.instance != null &&
            GestorDeBatalla.instance.inventarioCompleto != null &&
            GestorDeBatalla.instance.inventarioCompleto.Count > 0 &&
            ultimoDiaGenerado == -1 && cacheItemsDia.Count == 0)
        {
            catalogoBase = new List<TiendaItemData>();
            foreach (var bi in GestorDeBatalla.instance.inventarioCompleto)
            {
                if (bi == null)
                    continue;

                TiendaItemData data = new TiendaItemData();
                data.type = bi.type;
                data.nombre = bi.nombre;
                data.descripcion = bi.descripcion;
                data.stock = Mathf.Max(1, bi.cantidad);
                data.precio = GetPrecioPorDefecto(bi.type);
                catalogoBase.Add(data);
            }
        }

        // Si aun así el catálogo base está vacío pero el usuario configuró itemsTienda manualmente
        // la primera vez, los usamos como catálogo base.
        if ((catalogoBase == null || catalogoBase.Count == 0) && itemsTienda != null && itemsTienda.Count > 0 && ultimoDiaGenerado == -1 && cacheItemsDia.Count == 0)
        {
            catalogoBase = new List<TiendaItemData>();
            foreach (var it in itemsTienda)
            {
                if (it != null)
                    catalogoBase.Add(CopiarItem(it));
            }
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

        if (catalogoBase == null || catalogoBase.Count == 0)
            return;

        int numItemsDia = Mathf.Min(6, catalogoBase.Count);

        List<int> indicesDisponibles = new List<int>();
        for (int i = 0; i < catalogoBase.Count; i++)
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

            TiendaItemData baseItem = catalogoBase[idxCatalogo];
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
                    string lineaCantidadPrecio = agotado
                        ? "Stock: 0 - AGOTADO"
                        : $"Stock: {item.stock} - ${item.precio}";

                    texto.text = $"{lineaNombre}\n{lineaDescripcion}\n{lineaCantidadPrecio}";
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
                totalCompra += Mathf.Max(0, item.precio);
        }

        if (textoDineroActual != null)
            textoDineroActual.text = $"Dinero: {GameState.dineroJugador}";

        if (textoPrecio != null)
            textoPrecio.text = $"Total: {totalCompra}";
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

