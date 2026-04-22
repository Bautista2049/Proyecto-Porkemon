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
        if (collision.gameObject.CompareTag("Player"))
        {
            if (botPorkemonData == null)
            {
                return;
            }

            if (GameState.porkemonDelBot != null &&
                GameState.porkemonDelBot.BaseData == this.botPorkemonData &&
                GameState.porkemonDelBot.VidaActual <= 0)
            {
                if (popupText != null)
                {
                    popupText.gameObject.SetActive(true);
                    popupText.text = "Este Porkemon está debilitado. No puedes iniciar un combate.";
                    StartCoroutine(DesactivarTexto(3f));
                }
                return;
            }

            Vector3 posicionGuardada = CombatTransitionHelper.CalcularPosicionRetorno(collision);
            GameState.GuardarPosicionJugador(posicionGuardada, SceneManager.GetActiveScene().name);
            GameState.porkemonDelBot = new Porkemon(botPorkemonData, nivelSpawn);

            CombatTransitionHelper.IniciarTransicionCombate(nombreEscena);
        }
    }

    private IEnumerator DesactivarTexto(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (popupText != null)
        {
            popupText.gameObject.SetActive(false);
        }
    }
}
