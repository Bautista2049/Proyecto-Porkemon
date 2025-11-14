using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuPausa : MonoBehaviour
{
    public static MenuPausa instance;

    public GameObject menuPausaPanel;
    public TextMeshProUGUI textoNotificacion;
    public GameObject panelNotificacion;
    public GameObject panelBienvenida;
    public TMP_InputField inputNombre;
    public TextMeshProUGUI textoBienvenida;
    public GameObject panelInstrucciones;
    private string nombreJugador;

    private bool juegoPausado = false;
    private bool primeraVez = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("[MenuPausa] Iniciando MenuPausa...");
        
        if (menuPausaPanel != null)
            menuPausaPanel.SetActive(false);
        
        if (panelNotificacion != null)
            panelNotificacion.SetActive(false);
            
        int bienvenidaCompletada = PlayerPrefs.GetInt("BienvenidaCompletada", 0);
        primeraVez = (bienvenidaCompletada == 0);
        
        Debug.Log($"[MenuPausa] Es primera vez: {primeraVez} (PlayerPrefs BienvenidaCompletada: {bienvenidaCompletada})");
            
        if (primeraVez)
        {
            Debug.Log("[MenuPausa] Mostrando pantalla de bienvenida (primera vez)");
            
            if (panelBienvenida != null) 
            {
                panelBienvenida.SetActive(true);
                Debug.Log("[MenuPausa] Panel de bienvenida activado");
            }
            
            if (panelInstrucciones != null) 
            {
                panelInstrucciones.SetActive(false);
                Debug.Log("[MenuPausa] Panel de instrucciones desactivado inicialmente");
            }
            
            Time.timeScale = 0f;
        }
        else
        {
            Debug.Log("[MenuPausa] No es la primera vez, ocultando paneles");
            if (panelBienvenida != null) 
            {
                panelBienvenida.SetActive(false);
                Debug.Log("[MenuPausa] Panel de bienvenida desactivado");
            }
            
            if (panelInstrucciones != null) 
            {
                panelInstrucciones.SetActive(false);
                Debug.Log("[MenuPausa] Panel de instrucciones desactivado");
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (juegoPausado)
            {
                ReanudarJuego();
            }
            else
            {
                PausarJuego();
            }
        }
    }

    public void PausarJuego()
    {
        juegoPausado = true;
        if (menuPausaPanel != null)
            menuPausaPanel.SetActive(true);
        
        Time.timeScale = 0f;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReanudarJuego()
    {
        juegoPausado = false;
        if (menuPausaPanel != null)
            menuPausaPanel.SetActive(false);
        
        Time.timeScale = 1f;
    }

    public void GuardarJuego()
    {
        try
        {
            if (GestorDeBatalla.instance != null)
            {
                PlayerPrefs.SetInt("CantidadPokemones", GestorDeBatalla.instance.equipoJugador.Count);
                
                for (int i = 0; i < GestorDeBatalla.instance.equipoJugador.Count; i++)
                {
                    Porkemon pokemon = GestorDeBatalla.instance.equipoJugador[i];
                    string porkemonJson = JsonUtility.ToJson(pokemon);
                    PlayerPrefs.SetString($"Pokemon_{i}", porkemonJson);
                }

                PlayerPrefs.SetInt("CantidadObjetos", GestorDeBatalla.instance.inventarioBattleItems.Count);
                
                for (int i = 0; i < GestorDeBatalla.instance.inventarioBattleItems.Count; i++)
                {
                    BattleItem item = GestorDeBatalla.instance.inventarioBattleItems[i];
                    if (item != null)
                    {
                        string itemJson = JsonUtility.ToJson(item);
                        PlayerPrefs.SetString($"Item_{i}", itemJson);
                    }
                }
            }

            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
            {
                Vector3 pos = jugador.transform.position;
                PlayerPrefs.SetFloat("PosX", pos.x);
                PlayerPrefs.SetFloat("PosY", pos.y);
                PlayerPrefs.SetFloat("PosZ", pos.z);
            }

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            PlayerPrefs.SetString("UltimoGuardado", timestamp);
            
            PlayerPrefs.Save();
            
            MostrarNotificacion($"Juego guardado correctamente\n{timestamp}");
        }
        catch (Exception e)
        {
            MostrarNotificacion($"Error al guardar: {e.Message}");
        }
    }

    public void CargarJuego()
    {
        try
        {
            if (!PlayerPrefs.HasKey("UltimoGuardado"))
            {
                MostrarNotificacion("No hay partida guardada");
                return;
            }

            if (GestorDeBatalla.instance != null)
            {
                int cantidadPokemones = PlayerPrefs.GetInt("CantidadPokemones", 0);
                GestorDeBatalla.instance.equipoJugador.Clear();
                
                for (int i = 0; i < cantidadPokemones; i++)
                {
                    if (PlayerPrefs.HasKey($"Pokemon_{i}"))
                    {
                        string porkemonJson = PlayerPrefs.GetString($"Pokemon_{i}");
                        Porkemon pokemon = JsonUtility.FromJson<Porkemon>(porkemonJson);
                        if (pokemon != null)
                        {
                            GestorDeBatalla.instance.equipoJugador.Add(pokemon);
                        }
                    }
                }

                int cantidadObjetos = PlayerPrefs.GetInt("CantidadObjetos", 0);
                GestorDeBatalla.instance.inventarioBattleItems.Clear();
                
                for (int i = 0; i < cantidadObjetos; i++)
                {
                    if (PlayerPrefs.HasKey($"Item_{i}"))
                    {
                        string itemJson = PlayerPrefs.GetString($"Item_{i}");
                        BattleItem item = JsonUtility.FromJson<BattleItem>(itemJson);
                        if (item != null)
                        {
                            GestorDeBatalla.instance.inventarioBattleItems.Add(item);
                        }
                    }
                }
            }

            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null && PlayerPrefs.HasKey("PosX"))
            {
                float x = PlayerPrefs.GetFloat("PosX");
                float y = PlayerPrefs.GetFloat("PosY");
                float z = PlayerPrefs.GetFloat("PosZ");
                jugador.transform.position = new Vector3(x, y, z);
            }

            string timestamp = PlayerPrefs.GetString("UltimoGuardado");
            MostrarNotificacion($"Juego cargado correctamente\nÚltimo guardado: {timestamp}");
        }
        catch (Exception e)
        {
            MostrarNotificacion($"Error al cargar: {e.Message}");
        }
    }

    public void AbrirMenuOrdenarPokemones()
    {
        Time.timeScale = 1f;
        GameState.player1Turn = true;
        GameState.modoOrdenamiento = true;
        SceneTransitionManager.Instance.LoadScene("Escena CambioPorkemon");
    }

    public void AbrirMenuOrdenarObjetos()
    {
        Time.timeScale = 1f;
        GameState.player1Turn = false;
        GameState.modoOrdenamiento = true;
        SceneTransitionManager.Instance.LoadScene("Escena CambioPorkemon");
    }

    public void SalirAlMenu()
    {
        Time.timeScale = 1f;
        SceneTransitionManager.Instance.LoadScene("Interfaz de Menu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void MostrarNotificacion(string mensaje)
    {
        if (textoNotificacion != null)
        {
            textoNotificacion.text = mensaje;
        }
        
        if (panelNotificacion != null)
        {
            panelNotificacion.SetActive(true);
            StartCoroutine(OcultarNotificacion());
        }
    }

    private IEnumerator OcultarNotificacion()
    {
        yield return new WaitForSecondsRealtime(3f);
        if (panelNotificacion != null)
        {
            panelNotificacion.SetActive(false);
        }
    }
    
    private void MostrarPantallaBienvenida()
    {
        Debug.Log("[MenuPausa] Mostrando pantalla de bienvenida");
        Time.timeScale = 0f;
        juegoPausado = true;
        
        if (panelBienvenida != null)
        {
            Debug.Log("[MenuPausa] Activando panel de bienvenida");
            panelBienvenida.SetActive(true);
            if (inputNombre != null)
            {
                inputNombre.Select();
                Debug.Log("[MenuPausa] Campo de entrada de nombre seleccionado");
            }
        }
        else
        {
            Debug.LogError("[MenuPausa] No se encontró el panel de bienvenida");
        }
    }
    
    public void OnAceptarNombre()
    {
        if (inputNombre != null && !string.IsNullOrEmpty(inputNombre.text.Trim()))
        {
            nombreJugador = inputNombre.text.Trim();
            PlayerPrefs.SetString("NombreJugador", nombreJugador);
            
            if (panelBienvenida != null) panelBienvenida.SetActive(false);
            if (panelInstrucciones != null) panelInstrucciones.SetActive(true);
        }
    }
    
    public void EmpezarJuego()
    {
        Debug.Log("[MenuPausa] EmpezarJuego() llamado");
        
        if (primeraVez)
        {
            Debug.Log("[MenuPausa] Marcando bienvenida como completada");
            PlayerPrefs.SetInt("BienvenidaCompletada", 1);
            PlayerPrefs.Save();
            primeraVez = false;
        }
        
        if (panelBienvenida != null) 
        {
            Debug.Log("[MenuPausa] Desactivando panel de bienvenida");
            panelBienvenida.SetActive(false);
            panelBienvenida.hideFlags = HideFlags.NotEditable;
        }
        
        if (panelInstrucciones != null) 
        {
            Debug.Log("[MenuPausa] Desactivando panel de instrucciones");
            panelInstrucciones.SetActive(false);
            panelInstrucciones.hideFlags = HideFlags.NotEditable;
        }
        
        Debug.Log("[MenuPausa] Reanudando juego...");
        ReanudarJuego();
    }
    
    public void MostrarMensajeVictoria()
    {
        string nombre = PlayerPrefs.GetString("NombreJugador", "Entrenador");
        MostrarNotificacion($"¡Felicidades {nombre}! Has atrapado todos los Pokémon.");
        
        Time.timeScale = 0f;
        juegoPausado = true;
    }
    
    public void OpcionesMenuVictoria()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            PlayerPrefs.DeleteKey("PartidaGuardada");
            PlayerPrefs.DeleteKey("NombreJugador");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
