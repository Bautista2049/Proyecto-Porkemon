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
    public List<Porkemon> equipoBot = new List<Porkemon>();
    public Porkemon porkemonBot;
    public Porkemon porkemonDelBot;

    public List<PorkemonData> dataEquipoJugador;
    public PorkemonData dataInicialBot;
    public Transform puntoSpawnBot; 
    public Vector3 escalaModeloBot = new Vector3(1.0f, 1.0f, 1.0f);
    public List<BattleItem> inventarioCompleto = new List<BattleItem>();
    public List<BattleItem> inventarioBattleItems = new List<BattleItem>();
    public bool combateIniciado = false;
    public Transform posicionJugador;
    
    // Lista de nombres de Pokémon que se deben atrapar para ganar
    private List<string> pokemonesParaAtrapar = new List<string>
    {
        "Bulbasaur", "Charmander", "Squirtle", "Pikachu" // Ajusta esta lista según tus Pokémon
    };
    
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
        // Inicializar equipo de Pokémon
        equipoJugador.Clear();
        foreach (var data in dataEquipoJugador)
        {
            Porkemon nuevo = new Porkemon(data, data.nivel);
            equipoJugador.Add(nuevo);
            if (equipoJugador.Count >= 6) break;
        }

        // Inicializar inventario completo (solo si está vacío)
        if (inventarioCompleto.Count == 0)
        {
            inventarioCompleto = new List<BattleItem>
            {
                // Objetos de curación
                new BattleItem(BattleItemType.Pocion, "Poción", "Restaura 20 PS", 5),
                new BattleItem(BattleItemType.Superpocion, "Superpoción", "Restaura 50 PS", 3),
                new BattleItem(BattleItemType.Hiperpocion, "Hiperpoción", "Restaura 120 PS", 2),
                new BattleItem(BattleItemType.Pocionmaxima, "Poción Máxima", "Restaura todos los PS", 1),
                new BattleItem(BattleItemType.Revivir, "Revivir", "Revive un Pokémon con 30% de PS", 2),
                new BattleItem(BattleItemType.RevivirMax, "Revivir Máx", "Revive un Pokémon con todos sus PS", 1),
                
                // Objetos de mejora de estadísticas
                new BattleItem(BattleItemType.AtaqueX, "Ataque X", "Aumenta el ataque en 1 nivel", 2),
                new BattleItem(BattleItemType.DefensaX, "Defensa X", "Aumenta la defensa en 1 nivel", 2),
                new BattleItem(BattleItemType.AtaqueEspecialX, "Ataque Especial X", "Aumenta el ataque especial en 1 nivel", 2),
                new BattleItem(BattleItemType.DefensaEspecialX, "Defensa Especial X", "Aumenta la defensa especial en 1 nivel", 2),
                new BattleItem(BattleItemType.VelocidadX, "Velocidad X", "Aumenta la velocidad en 1 nivel", 2),
                new BattleItem(BattleItemType.PrecisionX, "Precisión X", "Aumenta la precisión en 1 nivel", 2),
                new BattleItem(BattleItemType.CriticoX, "Crítico X", "Aumenta la probabilidad de golpe crítico", 1),
                new BattleItem(BattleItemType.ProteccionX, "Protección X", "Aumenta la evasión durante 5 turnos", 1),
                
                // Pokébolas
                new BattleItem(BattleItemType.Porkebola, "Pokéball", "Atrapa Pokémon salvajes más fácilmente", 5),
                new BattleItem(BattleItemType.Superbola, "Superball", "Más efectiva que una Pokéball normal", 3),
                new BattleItem(BattleItemType.Ultrabola, "Ultraball", "Muy efectiva para Pokémon difíciles de atrapar", 2),
                new BattleItem(BattleItemType.Masterbola, "Masterball", "Atrapa cualquier Pokémon sin fallar", 1)
            };
        }

        // Inicializar inventario de combate con los primeros 6 objetos
        inventarioBattleItems.Clear();
        int maxItems = Mathf.Min(6, inventarioCompleto.Count);
        for (int i = 0; i < maxItems; i++)
        {
            inventarioBattleItems.Add(inventarioCompleto[i]);
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

        if (GameState.porkemonDelJugador != null)
        {
            porkemonJugador = GameState.porkemonDelJugador;
        }
        else if (equipoJugador.Count > 0)
        {
            porkemonJugador = equipoJugador[0];
            GameState.porkemonDelJugador = porkemonJugador;
        }
        
        combateIniciado = true;
    }

    private void InicializarInventarioBattleItems()
    {
        inventarioBattleItems.Add(new BattleItem(BattleItemType.Pocion, "Poción", "Restaura 20 PS de un Porkemon", 5));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.Superpocion, "Superpoción", "Restaura 50 PS de un Porkemon", 3));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.Hiperpocion, "Hiperpoción", "Restaura 200 PS de un Porkemon", 2));
        inventarioBattleItems.Add(new BattleItem(BattleItemType.Pocionmaxima, "Poción Máxima", "Restaura todos los PS de un Porkemon", 1));
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

        // Verificar si se completó la Pokédex
        if (SeCompletoLaDex())
        {
            // Mostrar mensaje de victoria
            string nombreJugador = PlayerPrefs.GetString("NombreJugador", "Entrenador");
            Debug.Log($"¡Felicidades {nombreJugador}! Has atrapado todos los Pokémon.\n¡Eres un Maestro Pokémon!");
            
            // Pausar el juego
            Time.timeScale = 0f;
            
            // Mostrar opciones (S/N)
            Debug.Log("¿Quieres volver a jugar? (S/N)");
            
            // Usar el Update para detectar la entrada del usuario
            StartCoroutine(EsperarInputVictoria());
        }
        else
        {
            // Continuar con el combate normal
            GameState.nombreGanador = pokemon.BaseData.nombre;
            GameState.experienciaGanada = equipoJugador.CalcularExperienciaGanada(new List<Porkemon> { pokemon });
            GameState.equipoGanador = new List<Porkemon>(equipoJugador);
            GameState.victoriaFueCaptura = true;
            
            StartCoroutine(FinalizarCombate(true));
        }
    }
    
    private bool SeCompletoLaDex()
    {
        // Verificar si el jugador tiene todos los Pokémon necesarios
        foreach (var pokemonNombre in pokemonesParaAtrapar)
        {
            bool encontrado = false;
            foreach (var pokemon in equipoJugador)
            {
                if (pokemon.BaseData.nombre == pokemonNombre)
                {
                    encontrado = true;
                    break;
                }
            }
            if (!encontrado) return false;
        }
        return true;
    }
    
    private IEnumerator EsperarInputVictoria()
    {
        bool esperandoRespuesta = true;
        
        while (esperandoRespuesta)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                // Reiniciar el juego
                PlayerPrefs.DeleteKey("PartidaGuardada");
                PlayerPrefs.DeleteKey("NombreJugador");
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                esperandoRespuesta = false;
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                // Salir del juego
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
                esperandoRespuesta = false;
            }
            yield return null;
        }
    }
    
    private IEnumerator FinalizarCombate(bool victoria)
    {
        yield return new WaitUntil(() => !ConsolaEnJuego.instance.isTyping);
        yield return new WaitForSeconds(1.5f);

        if (victoria)
        {
            // Verificar si ya se capturaron todos los Pokémon
            if (SeCompletoLaDex())
            {
                // Cargar escena de victoria final
                SceneManager.LoadScene("InterfazDeMenu");
            }
            else
            {
                SceneTransitionManager.Instance.LoadScene("Escena de Victoria");
            }
        }
    }
}
