using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorDeBatalla : MonoBehaviour
{
    public static GestorDeBatalla instance;

    public ConsolaEnJuego consolaEnJuego;

    [Header("Porkemon del Jugador")]
    public List<PorkemonData> dataEquipoJugador;
    public List<Porkemon> equipoJugador = new List<Porkemon>();

    [Header("Porkemon del Bot")]
    public PorkemonData dataInicialBot;
    public Porkemon porkemonBot;

    [Header("Puntos de Spawn")]
    public Transform puntoSpawnJugador;
    public Vector3 escalaModeloJugador = new Vector3(1.0f, 1.0f, 1.0f);
    public Transform puntoSpawnBot; 
    public Vector3 escalaModeloBot = new Vector3(1.0f, 1.0f, 1.0f);
    public List<BattleItem> inventarioBattleItems = new List<BattleItem>();
    public bool combateIniciado = false;
    public Transform posicionJugador;
    
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

        if (GameState.porkemonDelBot != null)
        {
            porkemonBot = GameState.porkemonDelBot;
        }
        else
        {
            porkemonBot = new Porkemon(dataInicialBot, dataInicialBot.nivel);
            GameState.porkemonDelBot = porkemonBot;
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
        inventarioBattleItems.Add(new BattleItem(BattleItemType.Porkebola, "Porkebola", "Un objeto para capturar Porkemon salvajes.", 10));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.Superbola, "Superbola", "Una Porkebola con mejor ratio de captura.", 5));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.Pocion, "Poción", "Restaura 20 PS de un Porkemon.", 5));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.Superpocion, "Superpoción", "Restaura 50 PS de un Porkemon.", 3));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.Hiperpocion, "Hiperpoción", "Restaura 200 PS de un Porkemon.", 2));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.Maxipocion, "Maxipoción", "Restaura todos los PS de un Porkemon.", 1));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.Revivir, "Revivir", "Revive a un Porkemon debilitado con la mitad de sus PS.", 2));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.RevivirMax, "Revivir Máx", "Revive a un Porkemon debilitado con todos sus PS.", 1));
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

    public void PokemonCapturado(Porkemon pokemon)
    {
        if (equipoJugador.Count < 6)
        {
            equipoJugador.Add(pokemon);
            Debug.Log($"{pokemon.BaseData.nombre} ha sido añadido a tu equipo!");
        }
        else
        {
            Debug.Log($"{pokemon.BaseData.nombre} ha sido enviado al PC!");
        }
        StartCoroutine(FinalizarCombate(true));
    }

    public bool UsarItemCuracionEnPorkemon(BattleItemType tipoItem, Porkemon objetivo)
    {
        if (objetivo == null)
        {
            Debug.LogError("No se puede usar el item: el Porkemon objetivo es nulo.");
            return false;
        }

        BattleItem itemEnInventario = inventarioBattleItems.Find(item => item.type == tipoItem);
        
        if (itemEnInventario == null || itemEnInventario.quantity <= 0)
        {
            Debug.Log($"No tienes {tipoItem} en tu inventario.");
            if (consolaEnJuego != null)
            {
                consolaEnJuego.AgregarTexto($"No tienes {tipoItem} en tu inventario.");
            }
            return false;
        }

        bool itemUsado = objetivo.UsarItemCuracion(tipoItem);
        
        if (itemUsado)
        {
            itemEnInventario.quantity--;
            
            if (itemEnInventario.quantity <= 0)
            {
                inventarioBattleItems.Remove(itemEnInventario);
            }
            
            if (consolaEnJuego != null)
            {
                string mensaje = tipoItem switch
                {
                    BattleItemType.Pocion => $"{objetivo.BaseData.nombre} usó Poción y recuperó PS.",
                    BattleItemType.Superpocion => $"{objetivo.BaseData.nombre} usó Superpoción y recuperó PS.",
                    BattleItemType.Hiperpocion => $"{objetivo.BaseData.nombre} usó Hiperpoción y recuperó PS.",
                    BattleItemType.Maxipocion => $"{objetivo.BaseData.nombre} usó Maxipoción y recuperó todos sus PS.",
                    BattleItemType.Revivir => $"{objetivo.BaseData.nombre} fue revivido con Revivir.",
                    BattleItemType.RevivirMax => $"{objetivo.BaseData.nombre} fue revivido con Revivir Máx.",
                    _ => $"{objetivo.BaseData.nombre} usó {tipoItem}."
                };
                consolaEnJuego.AgregarTexto(mensaje);
            }
            
            Debug.Log($"Item {tipoItem} usado exitosamente en {objetivo.BaseData.nombre}.");
            return true;
        }
        else
        {
            if (consolaEnJuego != null)
            {
                consolaEnJuego.AgregarTexto($"No se pudo usar {tipoItem} en {objetivo.BaseData.nombre}.");
            }
            return false;
        }
    }
    
    private IEnumerator FinalizarCombate(bool victoria)
    {
        yield return new WaitUntil(() => ConsolaEnJuego.instance != null && !ConsolaEnJuego.instance.isTyping);
        yield return new WaitForSeconds(1.5f);
        
        if (victoria)
        {
            SceneTransitionManager.Instance.LoadScene("Escena Principal");
        }
    }
}