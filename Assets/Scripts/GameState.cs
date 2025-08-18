using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class GameState
{
    public static bool player1Turn = true;
    public static AtaqueData ataqueSeleccionado;
    public static Porkemon porkemonDelJugador;
    public static Porkemon porkemonDelBot;
    public static string nombreGanador;

    public static void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/porkemon.dat");

        SaveData data = new SaveData();
        data.player1Turn = player1Turn;
        data.porkemonDelJugador = porkemonDelJugador.GetDataForSave();
        data.porkemonDelBot = porkemonDelBot.GetDataForSave();

        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Juego guardado!");
    }

    public static void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/porkemon.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/porkemon.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();

            player1Turn = data.player1Turn;
            porkemonDelJugador.LoadDataFromSave(data.porkemonDelJugador);
            porkemonDelBot.LoadDataFromSave(data.porkemonDelBot);

            Debug.Log("Juego cargado!");
        }
        else
        {
            Debug.LogError("No se encontró el archivo de guardado!");
        }
    }
}

[System.Serializable]
class SaveData
{
    public bool player1Turn;
    public PorkemonSaveData porkemonDelJugador;
    public PorkemonSaveData porkemonDelBot;
}
