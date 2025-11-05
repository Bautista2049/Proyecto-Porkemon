﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ControladorCambio : MonoBehaviour
{
    [Header("UI")]
    public List<Button> botonesPokemon;
    public TextMeshProUGUI tituloTexto;

    private List<Porkemon> equipoJugador;
    private bool esMochila = false; // Flag para determinar si es escena de mochila

    void Start()
    {
        if (GestorDeBatalla.instance == null)
        {
            return;
        }

        // Esta lógica detecta si entramos en modo "Mochila" o "Cambio"
        esMochila = SceneManager.GetActiveScene().name == "Escena CambioPorkemon" &&
                   GameState.player1Turn == false; // Usabas esto como flag

        // CORRECCIÓN: Si abrimos la mochila, el turno DEBERÍA ser del jugador.
        // Vamos a usar el 'itemSeleccionado' como un mejor indicador.
        // O mejor, el flag 'player1Turn == false' que usabas en ControladorUICombate es para CAMBIAR,
        // pero para la mochila lo ponías en false...
        
        // Vamos a simplificar. Asumimos que si entras a esta escena, es para cambiar o para la mochila.
        // Tu script 'CargarEscena' y 'ControladorUICombate' cargan 'Escena CambioPorkemon'
        
        // La lógica original para detectar la mochila es un poco confusa.
        // Vamos a asumir que si el 'tituloTexto' es "Objetos de Batalla", es la mochila.
        // O mejor, vamos a re-usar tu flag 'GameState.player1Turn == false' que ponías en 'AbrirMochila'
        
        esMochila = (GameState.player1Turn == false); // Asumimos que 'AbrirMochila' puso esto en false

        if (esMochila)
        {
            // Modo Mochila
            InicializarMochila();
        }
        else
        {
            // Modo Cambio de Pokémon
            equipoJugador = GestorDeBatalla.instance.equipoJugador;
            ActualizarBotones();
            tituloTexto.text = $"Tienes {equipoJugador.Count} Pokémon";
        }
    }

    private void InicializarMochila()
    {
        // Cambiar título
        tituloTexto.text = "Objetos de Batalla";

        // Usar los botones para mostrar items
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

    // --- NUEVO HELPER ---
    private bool EsPorkebola(BattleItemType type)
    {
        return type == BattleItemType.Porkebola ||
               type == BattleItemType.Superbola ||
               type == BattleItemType.Ultrabola ||
               type == BattleItemType.Masterbola;
    }

    // --- MÉTODO 'UsarItem' MODIFICADO ---
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
        }

        // Si es una Porkebola
        if (EsPorkebola(selectedItem.type))
        {
            // TODO: Añadir comprobación de "No puedes usar esto en una batalla de entrenador"
            
            // Guardamos la bola seleccionada y volvemos al combate
            GameState.itemSeleccionado = selectedItem;
            GameState.player1Turn = true; // El turno sigue siendo nuestro
            SceneTransitionManager.Instance.LoadScene("Escena de combate");
        }
        // Si es un objeto de stat (lógica que ya tenías)
        else
        {
            Porkemon porkemonActivo = GestorDeBatalla.instance.porkemonJugador;
            if (porkemonActivo == null)
            {
                return;
            }

            // Aplicar efecto del item
            AplicarEfectoItem(selectedItem, porkemonActivo);

            // Reducir cantidad
            selectedItem.cantidad--;

            // Si se agota, remover del inventario
            if (selectedItem.cantidad <= 0)
            {
                GestorDeBatalla.instance.inventarioBattleItems.Remove(selectedItem);
            }
            
            // IMPORTANTE: Al usar un objeto de stat, PIERDES EL TURNO.
            GameState.player1Turn = false; // Cedemos el turno al bot

            // Volver al combate
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
     
    // --- LÓGICA DE CAMBIO DE POKEMON (SIN CAMBIOS) ---
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

        GameState.player1Turn = false; // Cedemos el turno

        SceneTransitionManager.Instance.LoadScene("Escena de combate");
    }
    
    public void CancelarCambio()
    {
        // Si cancelamos, volvemos al combate
        // Importante: ¡Debemos restaurar el estado del turno!
        // Si entramos aquí, 'esMochila' es false, así que el turno era del jugador.
        GameState.player1Turn = true;
        SceneTransitionManager.Instance.LoadScene("Escena de combate");
    }
}