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
    public static bool victoriaFueCaptura = false;
    public static bool modoOrdenamiento = false;
}

[System.Serializable]
class SaveData
{
    public bool player1Turn;
    public PorkemonSaveData porkemonDelJugador;
    public PorkemonSaveData porkemonDelBot;
}