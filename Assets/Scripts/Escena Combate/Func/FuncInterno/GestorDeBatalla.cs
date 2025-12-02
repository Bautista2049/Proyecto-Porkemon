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
    public bool usarPlaylist = false;
    public AudioClip musicaUnica;
    public List<AudioClip> playlist;
    public AudioClip musicaCombate;

    [Range(0f, 1f)]
    public float volumenMusicaNormal = 0.7f;
    [Range(0f, 1f)]
    public float volumenMusicaCombate = 0.8f;

    private AudioSource musicSource;
    private AudioSource battleMusicSource;
    private Coroutine playlistCoroutine;
    private int playlistIndex = 0;
    private bool worldPaused = false;
    private bool inBattle = false;

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
    
    public Camera cameraCombate;
    public ParticleSystem psImpactoFuego;
    public ParticleSystem psImpactoAgua;
    public ParticleSystem psImpactoPlanta;
    public ParticleSystem psImpactoElectrico;
    public ParticleSystem psImpactoHielo;
    public ParticleSystem psImpactoVeneno;
    public ParticleSystem psImpactoRoca;
    public ParticleSystem psImpactoTierra;
    public ParticleSystem psImpactoGenerico;
    public ParticleSystem psBuffJugador;

    private ParticleSystem instanciaBuffJugador;
    private Coroutine shakeCoroutine;

    private List<string> pokemonesParaAtrapar = new List<string>
    {
        "KittyZap", "Charmander", "PunchBug", "Draguscular", "Bellsprout"
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

            battleMusicSource = gameObject.AddComponent<AudioSource>();
            battleMusicSource.playOnAwake = false;
            battleMusicSource.loop = true;

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
                new BattleItem(BattleItemType.Pocion, "Poción", "Restaura 20 PS", 0),
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

                // Las Pokéballs mantienen su stock fijo y no se randomizan.
                new BattleItem(BattleItemType.Porkebola, "Pokéball", "Atrapa Pokémon salvajes más fácilmente", 5),
                new BattleItem(BattleItemType.Superbola, "Superball", "Más efectiva que una Pokéball normal", 0),
                new BattleItem(BattleItemType.Ultrabola, "Ultraball", "Muy efectiva para Pokémon difíciles de atrapar", 0),
                new BattleItem(BattleItemType.Masterbola, "Masterball", "Atrapa cualquier Pokémon sin fallar", 0)
            };

            // Randomiza el stock de los objetos de la tienda
            foreach (var item in inventarioCompleto)
            {
                bool esPokeball = item.type == BattleItemType.Porkebola || 
                                  item.type == BattleItemType.Superbola ||
                                  item.type == BattleItemType.Ultrabola ||
                                  item.type == BattleItemType.Masterbola;

                // Si el objeto tiene descripción y NO es una pokeball, randomiza su stock.
                if (!string.IsNullOrEmpty(item.descripcion) && !esPokeball)
                {
                    item.cantidad = Random.Range(1, 6); // Stock aleatorio entre 1 y 5
                }
            }
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
            inBattle = true;

            if (musicSource != null && musicSource.isPlaying)
            {
                worldPaused = true;
                musicSource.Pause();
            }

            PlayBattleMusic();
        }
        else
        {
            // Escenas de mundo (no combate)
            if (inBattle)
            {
                // Venimos de un combate: detener música de combate y reanudar la del mundo
                inBattle = false;

                if (battleMusicSource != null && battleMusicSource.isPlaying)
                {
                    battleMusicSource.Stop();
                }

                if (musicSource != null && musicSource.clip != null)
                {
                    worldPaused = false;
                    musicSource.volume = volumenMusicaNormal;
                    musicSource.UnPause();
                }
                else
                {
                    PlayWorldMusic();
                }

                if (usarPlaylist && playlist != null && playlist.Count > 0 && playlistCoroutine == null)
                {
                    playlistCoroutine = StartCoroutine(PlaylistLoop());
                }
            }
            else
            {
                // Primera escena de mundo o recarga sin haber pasado por combate.
                if (musicSource == null || (!musicSource.isPlaying && musicSource.clip == null))
                {
                    PlayWorldMusic();
                }
                // Si ya hay música del mundo sonando, no hacemos nada para no reiniciarla.
            }
        }
    }

    private void PlayWorldMusic()
    {
        if (musicSource == null)
            return;

        worldPaused = false;

        if (usarPlaylist && playlist != null && playlist.Count > 0)
        {
            // Iniciar playlist solo si aún no está corriendo
            if (playlistCoroutine == null)
            {
                playlistCoroutine = StartCoroutine(PlaylistLoop());
            }
        }
        else if (musicaUnica != null)
        {
            if (musicSource.clip == musicaUnica && musicSource.isPlaying)
                return; // Ya se está reproduciendo

            if (playlistCoroutine != null)
            {
                StopCoroutine(playlistCoroutine);
                playlistCoroutine = null;
            }

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

            // Si la música del mundo está pausada (por combate), espera a que se reanude
            if (worldPaused)
            {
                yield return null;
                continue;
            }

            // Si no hay nada sonando, iniciar o continuar la playlist
            if (!musicSource.isPlaying)
            {
                if (musicSource.clip == null || !playlist.Contains(musicSource.clip))
                {
                    if (playlistIndex >= playlist.Count)
                        playlistIndex = 0;

                    AudioClip clip = playlist[playlistIndex];
                    playlistIndex++;

                    if (clip == null)
                    {
                        yield return null;
                        continue;
                    }

                    musicSource.loop = false;
                    musicSource.clip = clip;
                    musicSource.time = 0f;
                    musicSource.volume = volumenMusicaNormal;
                    musicSource.Play();
                }
            }

            yield return null;
        }
    }

    private void PlayBattleMusic()
    {
        if (battleMusicSource == null || musicaCombate == null)
            return;

        battleMusicSource.loop = true;
        battleMusicSource.clip = musicaCombate;
        battleMusicSource.volume = volumenMusicaCombate;
        battleMusicSource.Play();
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

    // Método para configurar el equipo del NPC dinámicamente
    public void ConfigurarEquipoNPC(List<Porkemon> equipoNPC)
    {
        equipoBot.Clear();
        equipoBot.AddRange(equipoNPC);

        if (equipoBot.Count > 0)
        {
            porkemonBot = equipoBot[0];
            GameState.porkemonDelBot = porkemonBot;
        }
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

    public void ReproducirEfectosAtaque(AtaqueData ataque, Porkemon atacante, Porkemon defensor)
    {
        if (ataque == null)
            return;

        Transform posImpacto = ObtenerPosImpacto(defensor);
        ParticleSystem prefab = ObtenerParticulaImpacto(ataque.tipo);

        if (prefab != null)
            InstanciarParticulaImpacto(prefab, posImpacto);

        if (ataque.tipo == TipoElemental.Roca || ataque.tipo == TipoElemental.Tierra)
            IniciarSacudidaCamara(0.15f, 0.4f);
    }

    private ParticleSystem ObtenerParticulaImpacto(TipoElemental tipo)
    {
        switch (tipo)
        {
            case TipoElemental.Fuego: return psImpactoFuego;
            case TipoElemental.Agua: return psImpactoAgua;
            case TipoElemental.Planta: return psImpactoPlanta;
            case TipoElemental.Electrico: return psImpactoElectrico;
            case TipoElemental.Hielo: return psImpactoHielo;
            case TipoElemental.Veneno: return psImpactoVeneno;
            case TipoElemental.Roca: return psImpactoRoca;
            case TipoElemental.Tierra: return psImpactoTierra;
            default: return psImpactoGenerico != null ? psImpactoGenerico : psImpactoFuego;
        }
    }

    private Transform ObtenerPosImpacto(Porkemon defensor)
    {
        if (defensor != null)
        {
            if (defensor == porkemonBot || defensor == GameState.porkemonDelBot)
                return puntoSpawnBot ?? posicionJugador;

            if (defensor == porkemonJugador || defensor == GameState.porkemonDelJugador)
                return posicionJugador ?? puntoSpawnBot;
        }

        return puntoSpawnBot ?? posicionJugador;
    }

    private void InstanciarParticulaImpacto(ParticleSystem prefab, Transform posImpacto)
    {
        if (prefab == null || posImpacto == null)
            return;

        ParticleSystem instancia = Instantiate(prefab, posImpacto.position, prefab.transform.rotation);

        var main = instancia.main;
        float duracion = main.duration + main.startLifetime.constantMax;
        if (!instancia.isPlaying)
            instancia.Play();
        Destroy(instancia.gameObject, duracion);
    }

    public void ActivarBuffVisualJugador(float duracion = -1f)
    {
        if (psBuffJugador == null || posicionJugador == null)
            return;

        if (instanciaBuffJugador == null)
        {
            instanciaBuffJugador = Instantiate(psBuffJugador, posicionJugador.position, Quaternion.identity, posicionJugador);
        }

        instanciaBuffJugador.gameObject.SetActive(true);
        instanciaBuffJugador.Play(true);

        if (duracion > 0f)
        {
            StartCoroutine(DesactivarBuffVisualJugadorTrasTiempo(duracion));
        }
    }

    private IEnumerator DesactivarBuffVisualJugadorTrasTiempo(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);

        if (instanciaBuffJugador != null)
        {
            instanciaBuffJugador.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            instanciaBuffJugador.gameObject.SetActive(false);
        }
    }

    public void DesactivarBuffVisualJugador()
    {
        if (instanciaBuffJugador != null)
        {
            instanciaBuffJugador.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            instanciaBuffJugador.gameObject.SetActive(false);
        }
    }

    public void IniciarSacudidaCamara(float intensidad, float duracion)
    {
        if (cameraCombate == null)
            return;

        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(SacudirCamaraRutina(intensidad, duracion));
    }

    private IEnumerator SacudirCamaraRutina(float intensidad, float duracion)
    {
        if (cameraCombate == null)
            yield break;

        Transform camTransform = cameraCombate.transform;
        Vector3 posicionOriginal = camTransform.localPosition;
        Quaternion rotacionOriginal = camTransform.localRotation;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            float desplazamientoX = Random.Range(-1f, 1f) * intensidad;
            float desplazamientoY = Random.Range(-1f, 1f) * intensidad;
            camTransform.localPosition = posicionOriginal + new Vector3(desplazamientoX, desplazamientoY, 0f);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        camTransform.localPosition = posicionOriginal;
        camTransform.localRotation = rotacionOriginal;
        shakeCoroutine = null;
    }
}