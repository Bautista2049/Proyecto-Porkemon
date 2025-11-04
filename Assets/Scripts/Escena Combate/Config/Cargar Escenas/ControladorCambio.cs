﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ControladorCambio : MonoBehaviour
{
    public List<Button> botonesPokemon;
    public TextMeshProUGUI tituloTexto;

    private List<Porkemon> equipoJugador;
    private bool esMochila = false;

    void Start()
    {
        if (GestorDeBatalla.instance == null)
        {
            return;
        }
        esMochila = SceneManager.GetActiveScene().name == "Escena CambioPorkemon" &&
                   GameState.player1Turn == false;

        if (esMochila)
        {
            InicializarMochila();
        }
        else
        {
            equipoJugador = GestorDeBatalla.instance.equipoJugador;
            ActualizarBotones();
            tituloTexto.text = $"Tienes {equipoJugador.Count} Pokémon";
        }
    }

    private void InicializarMochila()
    {
        tituloTexto.text = "Objetos de Batalla";
        List<BattleItem> inventario = GestorDeBatalla.instance.inventarioBattleItems;

        for (int i = 0; i < botonesPokemon.Count; i++)
        {
            if (i < inventario.Count)
            {
                BattleItem item = inventario[i];
                botonesPokemon[i].gameObject.SetActive(true);

                TextMeshProUGUI texto = botonesPokemon[i].GetComponentInChildren<TextMeshProUGUI>();
                if (texto != null)
                    texto.text = $"{item.nombre} x{item.cantidad}";

                botonesPokemon[i].onClick.RemoveAllListeners();
                int index = i;
                botonesPokemon[i].onClick.AddListener(() => UsarItem(index));
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
        List<BattleItem> inventario = GestorDeBatalla.instance.inventarioBattleItems;
        if (index < 0 || index >= inventario.Count)
        {
            return;
        }

        BattleItem selectedItem = inventario[index];
        if (selectedItem.cantidad <= 0)
        {
            return;
        }if (EsPorkebola(selectedItem.type))
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

                if (p == activo)
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

        Porkemon selected = equipoJugador[index];
        SeleccionarPorkemon(selected);
    }

    public void SeleccionarPorkemon(Porkemon nuevo)
    {
        if (nuevo == null || nuevo.VidaActual <= 0)
        {
            return;
        }

        GestorDeBatalla.instance.porkemonJugador = nuevo;
        GameState.porkemonDelJugador = nuevo; 

        GameState.player1Turn = false; 

        SceneTransitionManager.Instance.LoadScene("Escena de combate");
    }
    
    public void CancelarCambio()
    {
        SceneTransitionManager.Instance.LoadScene("Escena de combate");
    }
}