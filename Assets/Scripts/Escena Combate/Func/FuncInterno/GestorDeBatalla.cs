using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GestorDeBatalla : MonoBehaviour
{
    void Start()
    {
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
         
    }
    public static GestorDeBatalla instance;

    [Header("Musica General")]
    public bool usarPlaylist = false;
    public AudioClip musicaUnica;
    public List<AudioClip> playlist;

    [Header("Musica Combate")]
    public AudioClip musicaCombate;

    [Range(0f, 1f)]
    public float volumenMusicaNormal = 0.7f;
    [Range(0f, 1f)]
    public float volumenMusicaCombate = 0.8f;

    private AudioSource musicSource;
    private Coroutine playlistCoroutine;
    private int playlistIndex = 0;

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
    

    private List<string> pokemonesParaAtrapar = new List<string>
    {
        "KittyZap", "Charmander", "PunchBug", "Draguscular"
    };
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            musicSource = GetComponent<AudioSource>();
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
            }
            musicSource.playOnAwake = false;
            musicSource.loop = false;

            SceneManager.sceneLoaded += OnSceneLoaded;

            ResetearCombate();

            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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

        if (inventarioCompleto.Count == 0)
        {
            inventarioCompleto = new List<BattleItem>
            {
                new BattleItem(BattleItemType.Pocion, "Poción", "Restaura 20 PS", 1),
                new BattleItem(BattleItemType.Porkebola, "Pokéball", "Atrapa Pokémon salvajes más fácilmente", 5),

                new BattleItem(BattleItemType.Superpocion, "Superpoción", "Restaura 50 PS", 0),
                new BattleItem(BattleItemType.Hiperpocion, "Hiperpoción", "Restaura 120 PS", 0),
                new BattleItem(BattleItemType.Pocionmaxima, "Poción Máxima", "Restaura todos los PS", 0),
                new BattleItem(BattleItemType.Revivir, "Revivir", "Revive un Pokémon con 30% de PS", 0),
                new BattleItem(BattleItemType.RevivirMax, "Revivir Máx", "Revive un Pokémon con todos sus PS", 0),

                new BattleItem(BattleItemType.AtaqueX, "Ataque X", "Aumenta el ataque en 1 nivel", 0),
                new BattleItem(BattleItemType.DefensaX, "Defensa X", "Aumenta la defensa en 1 nivel", 0),
                new BattleItem(BattleItemType.AtaqueEspecialX, "Ataque Especial X", "Aumenta el ataque especial en 1 nivel", 0),
                new BattleItem(BattleItemType.DefensaEspecialX, "Defensa Especial X", "Aumenta la defensa en 1 nivel", 0),
                new BattleItem(BattleItemType.VelocidadX, "Velocidad X", "Aumenta la velocidad en 1 nivel", 0),
                new BattleItem(BattleItemType.PrecisionX, "Precisión X", "Aumenta la precisión en 1 nivel", 0),
                new BattleItem(BattleItemType.CriticoX, "Crítico X", "Aumenta la probabilidad de golpe crítico", 0),
                new BattleItem(BattleItemType.ProteccionX, "Protección X", "Aumenta la evasión durante 5 turnos", 0),

                new BattleItem(BattleItemType.Superbola, "Superball", "Más efectiva que una Pokéball normal", 0),
                new BattleItem(BattleItemType.Ultrabola, "Ultraball", "Muy efectiva para Pokémon difíciles de atrapar", 0),
                new BattleItem(BattleItemType.Masterbola, "Masterball", "Atrapa cualquier Pokémon sin fallar", 0)
            };
        }

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Escena de Combate")
        {
            PlayBattleMusic();
        }
        else
        {
            PlayWorldMusic();
        }
    }

    private void PlayWorldMusic()
    {
        if (musicSource == null)
            return;

        if (playlistCoroutine != null)
        {
            StopCoroutine(playlistCoroutine);
            playlistCoroutine = null;
        }

        if (usarPlaylist && playlist != null && playlist.Count > 0)
        {
            playlistIndex = 0;
            playlistCoroutine = StartCoroutine(PlaylistLoop());
        }
        else if (musicaUnica != null)
        {
            musicSource.loop = true;
            musicSource.clip = musicaUnica;
            musicSource.volume = volumenMusicaNormal;
            musicSource.Play();
        }
    }

    private IEnumerator PlaylistLoop()
    {
        while (true)
        {
            if (playlist == null || playlist.Count == 0)
                yield break;

            if (playlistIndex >= playlist.Count)
                playlistIndex = 0;

            AudioClip clip = playlist[playlistIndex];
            playlistIndex++;

            if (clip == null)
                continue;

            musicSource.loop = false;
            musicSource.clip = clip;
            musicSource.volume = volumenMusicaNormal;
            musicSource.Play();

            yield return new WaitForSeconds(clip.length);
        }
    }

    private void PlayBattleMusic()
    {
        if (musicSource == null || musicaCombate == null)
            return;

        if (playlistCoroutine != null)
        {
            StopCoroutine(playlistCoroutine);
            playlistCoroutine = null;
        }

        musicSource.loop = true;
        musicSource.clip = musicaCombate;
        musicSource.volume = volumenMusicaCombate;
        musicSource.Play();
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

        if (SeCompletoLaDex())
        {
            string nombreJugador = PlayerPrefs.GetString("NombreJugador", "Entrenador");
            Debug.Log($"¡Felicidades {nombreJugador}! Has atrapado todos los Pokémon.\n¡Eres un Maestro Pokémon!");
            
            Time.timeScale = 0f;
            
            Debug.Log("¿Quieres volver a jugar? (S/N)");
            
            StartCoroutine(EsperarInputVictoria());
        }
        else
        {
            GameState.nombreGanador = pokemon.BaseData.nombre;
            GameState.experienciaGanada = equipoJugador.CalcularExperienciaGanada(new List<Porkemon> { pokemon });
            GameState.equipoGanador = new List<Porkemon>(equipoJugador);
            GameState.victoriaFueCaptura = true;
            GameState.dineroGanado = Mathf.Max(1, GameState.experienciaGanada / 2);
            GameState.dineroJugador += GameState.dineroGanado;

            StartCoroutine(FinalizarCombate(true));
        }
    }
    
    private bool SeCompletoLaDex()
    {
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
                PlayerPrefs.DeleteKey("PartidaGuardada");
                PlayerPrefs.DeleteKey("NombreJugador");
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                esperandoRespuesta = false;
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
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
            if (SeCompletoLaDex())
            {
                SceneManager.LoadScene("InterfazDeMenu");
            }
            else
            {
                SceneTransitionManager.Instance.LoadScene("Escena de Victoria");
            }
        }
    }

    public void SincronizarInventarioCompleto(BattleItem itemReferencia)
    {
        if (itemReferencia == null) return;

        var itemCompleto = inventarioCompleto.Find(i => i != null &&
            i.type == itemReferencia.type && i.nombre == itemReferencia.nombre);

        if (itemCompleto != null)
        {
            itemCompleto.cantidad = itemReferencia.cantidad;

            if (itemCompleto.cantidad <= 0)
            {
                inventarioCompleto.Remove(itemCompleto);
            }
        }
    }
}
