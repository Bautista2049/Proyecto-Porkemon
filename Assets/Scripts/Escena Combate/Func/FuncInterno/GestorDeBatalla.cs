using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GestorDeBatalla : MonoBehaviour
{
    public static GestorDeBatalla instance;

    public List<Porkemon> equipoJugador = new List<Porkemon>();
    public Porkemon porkemonBot;
    public Porkemon porkemonDelBot;

    public List<PorkemonData> dataEquipoJugador;
    public PorkemonData dataInicialBot;
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
            InicializarEstadoDelJuego();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InicializarEstadoDelJuego()
    {
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
    
        if (inventarioBattleItems.Count == 0)
        {
            InicializarInventarioBattleItems();
        }
    }

    public void ResetearCombate()
    {
        // Este método ahora puede usarse para reiniciar solo lo necesario para un nuevo combate,
        // sin borrar el inventario o el equipo capturado.

        combateIniciado = false;
    }

    public void IniciarBatalla()
    {
        if (combateIniciado) return;

        // Asegurarse de que el jugador tiene un Porkemon activo
        if (porkemonJugador == null && equipoJugador.Count > 0)
        {
            porkemonJugador = equipoJugador.Find(p => p.VidaActual > 0) ?? equipoJugador[0];
        }

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
        inventarioBattleItems.Add(new BattleItem(BattleItemType.Pocion, "Poción", "Restaura un 10% de la vida máxima.", 3));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.SuperPocion, "Superpoción", "Restaura un 50% de la vida máxima.", 1));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.HiperPocion, "Hiperpoción", "Restaura el 100% de la vida.", 1));
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
    }

    public static void AplicarEfecto(BattleItem item, Porkemon porkemon)
    {
        switch (item.type)
        {
            case BattleItemType.Pocion:
                porkemon.Curar(0.10f);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre} y recuperó vida.");
                break;
            case BattleItemType.SuperPocion:
                porkemon.Curar(0.50f);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre} y recuperó bastante vida.");
                break;
            case BattleItemType.HiperPocion:
                porkemon.Curar(1.0f);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre} y recuperó toda su vida.");
                break;
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
                porkemon.AumentarEspiritu(2); // Asumo que aumenta Espíritu, ajustar si es otra stat
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Defensa Especial aumentada!");
                break;
            case BattleItemType.VelocidadX:
                porkemon.AumentarVelocidad(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Velocidad aumentada!");
                break;
            // Los casos de Porkebola se manejan por separado en los controladores
            default:
                Debug.Log($"El efecto para {item.nombre} no está implementado.");
                break;
        }
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
    
    private IEnumerator FinalizarCombate(bool victoria)
    {
        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
        yield return new WaitForSeconds(1.5f);
        
        if (victoria)
        {
            // No cargamos la escena de victoria, volvemos al mundo principal
            // O si quieres la pantalla de exp, carga "Escena de Victoria"
            SceneTransitionManager.Instance.LoadScene("Escena Principal");
        }
    }
}