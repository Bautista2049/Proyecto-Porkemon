using System.Collections.Generic;
using UnityEngine;

public static class SistemaMisiones
{
    public static bool misionCombateActivada = false;
    public static bool misionCombateCompletada = false;
    public static int combatesRealizados = 0;
    public const int COMBATES_REQUERIDOS = 5;

    public static void ActivarMisionCombate()
    {
        if (!misionCombateCompletada)
        {
            misionCombateActivada = true;
            combatesRealizados = 0;
            Debug.Log("Misión de combate activada: Derrota o captura 5 Pokémon salvajes");
        }
    }

    public static void IncrementarCombates()
    {
        if (misionCombateActivada && !misionCombateCompletada)
        {
            combatesRealizados++;
            Debug.Log($"Progreso misión: {combatesRealizados}/{COMBATES_REQUERIDOS} combates");

            if (combatesRealizados >= COMBATES_REQUERIDOS)
            {
                CompletarMision();
            }
        }
    }

    private static void CompletarMision()
    {
        misionCombateCompletada = true;
        misionCombateActivada = false;
        Debug.Log("¡Misión completada! Habla con el NPC para recibir tu recompensa");
    }

    public static void DarRecompensas()
    {
        if (misionCombateCompletada)
        {
            // Dar Ultrabola
            var gestor = GestorDeBatalla.instance;
            if (gestor != null)
            {
                var ultrabola = new BattleItem(BattleItemType.Ultrabola, "Ultrabola", "Muy efectiva para Pokémon difíciles de atrapar", 1);
                gestor.inventarioCompleto.Add(ultrabola);
                Debug.Log("Has recibido 1 Ultrabola");

                // Dar Charmander
                var charmanderData = Resources.Load<PorkemonData>("Porkemones/Charmander");
                if (charmanderData != null)
                {
                    var charmander = new Porkemon(charmanderData, 5); // Nivel 5
                    gestor.equipoJugador.Add(charmander);
                    Debug.Log("Has recibido un Charmander nivel 5");
                }
                else
                {
                    Debug.LogError("No se encontró el PorkemonData de Charmander en Resources/Porkemones/");
                }
            }
        }
    }

    public static float GetProgresoNormalizado()
    {
        return (float)combatesRealizados / COMBATES_REQUERIDOS;
    }

    public static void ReiniciarMision()
    {
        misionCombateActivada = false;
        misionCombateCompletada = false;
        combatesRealizados = 0;
    }
}
