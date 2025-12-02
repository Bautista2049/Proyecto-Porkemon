using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    public static bool player1Turn = true;
    public static AtaqueData ataqueSeleccionado;
    public static BattleItem itemSeleccionado;
    public static Porkemon porkemonDelJugador;
    public static Porkemon porkemonDelBot;
    public static string nombreGanador;
    public static int experienciaGanada = 0;
    public static List<Porkemon> equipoGanador = new List<Porkemon>();
    public static int dineroJugador = 500;
    public static int dineroGanado = 0;
    public static bool victoriaFueCaptura = false;
    public static bool modoOrdenamiento = false;
    public static bool modoTienda = false;
    public static bool modoRevivir = false;
    public static bool esCombateBoss = false;

    // --- Sistema de Posición y Entrenadores Vencidos ---
    public static bool posicionJugadorGuardadaDisponible;
    public static Vector3 posicionJugadorGuardada;
    public static string escenaPosicionGuardada;
    public static string ultimoEntrenadorCombatidoID; // ID del último NPC con el que se luchó.
    public static List<string> entrenadoresVencidosIDs = new List<string>(); // Lista de IDs de NPCs ya vencidos.

    // --- Multiplicadores de Juego ---
    public static float multiplicadorDinero = 1f;
    public static float multiplicadorExp = 1.5f; // Experiencia aumentada un 50%
    public static float multiplicadorCaptura = 1f;
    public static float multiplicadorPreciosTienda = 0.75f; // Precios de la tienda reducidos un 25%

    public static void GuardarPosicionJugador(Vector3 posicion, string escena)
    {
        posicionJugadorGuardada = posicion;
        posicionJugadorGuardadaDisponible = true;
        escenaPosicionGuardada = escena;
    }

    /// <summary>
    /// Añade el ID del último entrenador combatido a la lista de vencidos.
    /// </summary>
    public static void MarcarUltimoEntrenadorComoVencido()
    {
        if (!string.IsNullOrEmpty(ultimoEntrenadorCombatidoID) && !entrenadoresVencidosIDs.Contains(ultimoEntrenadorCombatidoID))
        {
            entrenadoresVencidosIDs.Add(ultimoEntrenadorCombatidoID);
        }
    }
}