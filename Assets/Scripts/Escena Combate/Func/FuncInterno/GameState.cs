using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
    public static bool posicionJugadorGuardadaDisponible;
    public static Vector3 posicionJugadorGuardada;
    public static string escenaPosicionGuardada;

    public static float multiplicadorDinero = 1f;
    public static float multiplicadorExp = 1f;
    public static float multiplicadorCaptura = 1f;
    public static float multiplicadorPreciosTienda = 1f;

    public static void GuardarPosicionJugador(Vector3 posicion, string escena)
    {
        posicionJugadorGuardada = posicion;
        posicionJugadorGuardadaDisponible = true;
        escenaPosicionGuardada = escena;
    }
}

[System.Serializable]
class SaveData
{
    public bool player1Turn;
    public PorkemonSaveData porkemonDelJugador;
    public PorkemonSaveData porkemonDelBot;
}