using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GestorDeBatalla : MonoBehaviour
{
    public static GestorDeBatalla instance;

    public List<Porkemon> equipoJugador = new List<Porkemon>();
    public Porkemon porkemonBot;
    public Porkemon porkemonDelBot;

    public List<PorkemonData> dataEquipoJugador;

    public PorkemonData dataInicialBot;
    
    public Transform puntoSpawnBot; 

    public List<BattleItem> inventarioBattleItems = new List<BattleItem>();

    public bool combateIniciado = false;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            ResetearCombate();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetearCombate()
    {
        equipoJugador.Clear();

        foreach (var data in dataEquipoJugador)
        {
            Porkemon nuevo = new Porkemon(data, data.nivel);
            equipoJugador.Add(nuevo);
            if (equipoJugador.Count >= 6) break;
        }

        if (equipoJugador.Count > 0)
        {
            GameState.porkemonDelJugador = equipoJugador[0];
        }

        if (GameState.porkemonDelBot == null)
        {
            porkemonBot = new Porkemon(dataInicialBot, dataInicialBot.nivel);
            GameState.porkemonDelBot = porkemonBot;
        }
        else
        {
            porkemonBot = GameState.porkemonDelBot;
        }

        if (inventarioBattleItems.Count == 0)
        {
            InicializarInventarioBattleItems();
        }

        combateIniciado = false;
    }

    public void IniciarBatalla()
    {
        if (combateIniciado) return;

        Porkemon botActivo = GameState.porkemonDelBot;
        
        if (botActivo != null)
        {
            GameObject modeloPrefabBot = botActivo.BaseData.modeloPrefab; 
            
            if (modeloPrefabBot != null && puntoSpawnBot != null)
            {
                Instantiate(modeloPrefabBot, puntoSpawnBot.position, puntoSpawnBot.rotation);
            }

            Porkemon jugadorActivo = GameState.porkemonDelJugador;
        }
        
        combateIniciado = true;
    }

    private void InicializarInventarioBattleItems()
    {
        inventarioBattleItems.Add(new BattleItem(BattleItemType.AtaqueX, "Ataque X", "Aumenta el Ataque en 2 niveles", 1));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.DefensaX, "Defensa X", "Aumenta la Defensa en 2 niveles", 1));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.AtaqueEspecialX, "Ataque Especial X", "Aumenta el Ataque Especial en 2 niveles", 1));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.DefensaEspecialX, "Defensa Especial X", "Aumenta la Defensa Especial en 2 niveles", 1));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.VelocidadX, "Velocidad X", "Aumenta la Velocidad en 2 niveles", 1));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.PrecisionX, "Precisión X", "Aumenta la Precisión en 2 niveles", 1));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.CriticoX, "Crítico X", "Aumenta el índice de golpe crítico en 2 niveles", 1));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.ProteccionX, "Protección X", "Evita que las estadísticas bajen durante 5 turnos", 1));
    }

    public Porkemon GetPorkemonActivoJugador()
    {
        return GameState.porkemonDelJugador;
    }

    public void CambiarPorkemonActivo(Porkemon nuevo)
    {
        if (nuevo == null || nuevo.VidaActual <= 0) return;

        GameState.porkemonDelJugador = nuevo;
    }

    public Porkemon porkemonJugador
    {
        get { return GameState.porkemonDelJugador; }
        set { GameState.porkemonDelJugador = value; }
    }
}