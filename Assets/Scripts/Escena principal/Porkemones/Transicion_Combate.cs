using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Transicion_Combate : MonoBehaviour
{
    [SerializeField] private string nombreEscena = "Escena de Combate" ;
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI popupText;

    public PorkemonData botPorkemonData;
    public int nivelSpawn;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[Transicion_Combate] Colisión detectada con: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");
        
        if (!collision.gameObject.CompareTag("Player")) return;
        
        Debug.Log("[Transicion_Combate] Jugador detectado - Iniciando secuencia de combate");
        
        if (botPorkemonData == null)
        {
            Debug.LogError("[Transicion_Combate] Error: ¡No hay datos de Pokémon asignados!");
            return;
        }
        
        Debug.Log($"[Transicion_Combate] Datos del Pokémon encontrados: {botPorkemonData.nombre} (Nivel: {nivelSpawn})");

        if (GameState.porkemonDelBot != null && 
            GameState.porkemonDelBot.BaseData == botPorkemonData &&
            GameState.porkemonDelBot.VidaActual <= 0)
        {
            Debug.Log("[Transicion_Combate] Este Pokémon ya fue derrotado");
            if (popupText != null)
            {
                popupText.gameObject.SetActive(true);
                popupText.text = "¡Este Porkemon ya fue derrotado! Busca otro para luchar.";
                StartCoroutine(DesactivarTexto(3f));
            }
            return;
        }

        if (GestorDeBatalla.instance != null)
        {
            Debug.Log("[Transicion_Combate] Iniciando combate salvaje...");
            
            GestorDeBatalla.instance.IniciarBatalla(
                esSalvaje: true,
                pokemonSalvaje: botPorkemonData,
                nivelSalvaje: nivelSpawn
            );
            
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Debug.Log("[Transicion_Combate] Haciendo que la cámara principal persista");
                DontDestroyOnLoad(mainCamera.gameObject);
            }
            else
            {
                Debug.LogWarning("[Transicion_Combate] No se encontró la cámara principal en la escena");
            }

            Debug.Log($"[Transicion_Combate] Cargando escena de combate: {nombreEscena}");
            try 
            {
                SceneManager.LoadScene(nombreEscena);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("[Transicion_Combate] Carga de escena iniciada con éxito");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Transicion_Combate] Error al cargar la escena {nombreEscena}: {e.Message}");
            }
        }
    }

    private IEnumerator DesactivarTexto(float delay)
    {
        Debug.Log($"[Transicion_Combate] Hiding popup text after {delay} seconds");
        yield return new WaitForSeconds(delay);
        if (popupText != null)
        {
            popupText.gameObject.SetActive(false);
            Debug.Log("[Transicion_Combate] Popup text hidden");
        }
    }
}
