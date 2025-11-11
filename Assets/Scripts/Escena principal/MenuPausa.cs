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

    private bool juegoPausado = false;

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
        if (menuPausaPanel != null)
            menuPausaPanel.SetActive(false);
        
        if (panelNotificacion != null)
            panelNotificacion.SetActive(false);
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

                PlayerPrefs.SetInt("CantidadObjetosCompleto", GestorDeBatalla.instance.inventarioCompleto.Count);
                for (int i = 0; i < GestorDeBatalla.instance.inventarioCompleto.Count; i++)
                {
                    BattleItem item = GestorDeBatalla.instance.inventarioCompleto[i];
                    if (item != null)
                    {
                        string itemJson = JsonUtility.ToJson(item);
                        PlayerPrefs.SetString($"ItemCompleto_{i}", itemJson);
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

                int cantidadObjetosCompleto = PlayerPrefs.GetInt("CantidadObjetosCompleto", 0);
                GestorDeBatalla.instance.inventarioCompleto.Clear();
                for (int i = 0; i < cantidadObjetosCompleto; i++)
                {
                    if (PlayerPrefs.HasKey($"ItemCompleto_{i}"))
                    {
                        string itemJson = PlayerPrefs.GetString($"ItemCompleto_{i}");
                        BattleItem item = JsonUtility.FromJson<BattleItem>(itemJson);
                        if (item != null)
                        {
                            GestorDeBatalla.instance.inventarioCompleto.Add(item);
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

    public void ReiniciarJuego()
    {
        if (GestorDeBatalla.instance != null)
        {
            GestorDeBatalla.instance.equipoJugador.Clear();
            GestorDeBatalla.instance.ResetearCombate();
        }
        
        GameState.player1Turn = true;
        GameState.ataqueSeleccionado = null;
        GameState.itemSeleccionado = null;
        GameState.porkemonDelJugador = null;
        GameState.porkemonDelBot = null;
        GameState.nombreGanador = "";
        GameState.experienciaGanada = 0;
        GameState.equipoGanador.Clear();
        GameState.victoriaFueCaptura = false;
        GameState.npcYaHablo = false;
        
        Time.timeScale = 1f;
        
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("Escena Principal");
        }
        else
        {
            SceneManager.LoadScene("Escena Principal");
        }
    }
}
