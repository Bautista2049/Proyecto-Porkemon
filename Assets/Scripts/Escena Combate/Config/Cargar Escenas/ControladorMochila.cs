using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ControladorMochila : MonoBehaviour
{
    [Header("UI de Mochila")]
    public List<Button> botonesItems;
    public TextMeshProUGUI tituloTexto;

    private List<BattleItem> inventario;

    void Start()
    {
        if (GestorDeBatalla.instance == null)
        {
            return;
        }
        inventario = GestorDeBatalla.instance.inventarioBattleItems;
        ActualizarBotonesItems();
        tituloTexto.text = $"Objetos de Batalla ({inventario.Count})";
    }

    void ActualizarBotonesItems()
    {
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

    public void UsarItem(BattleItem item)
    {
        if (item.cantidad <= 0)
        {
            return;
        }

        Porkemon porkemonActivo = GestorDeBatalla.instance.porkemonJugador;
        if (porkemonActivo == null)
        {
            return;
        }

        // Aplicar efecto del item
        AplicarEfectoItem(item, porkemonActivo);

        // Reducir cantidad
        item.cantidad--;

        // Si se agota, remover del inventario
        if (item.cantidad <= 0)
        {
            GestorDeBatalla.instance.inventarioBattleItems.Remove(item);
        }

        // Actualizar UI
        ActualizarBotonesItems();
        tituloTexto.text = $"Objetos de Batalla ({GestorDeBatalla.instance.inventarioBattleItems.Count})";

        // Volver al combate
        SceneTransitionManager.Instance.LoadScene("Escena de combate");
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

    public void CancelarMochila()
    {
        SceneTransitionManager.Instance.LoadScene("Escena de combate");
    }
}
