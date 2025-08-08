using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthPoints : MonoBehaviour
{
    public Stats stats;

    public Slider barraSalud;
    public Text textoSalud;

    private void Start()
    {
        if (stats == null)
        {
            // Valor por defecto, modificar si quieres
            stats = new Stats(1, 100, 10, 5, 2, 3);
        }

        ActualizarUI();
    }

    public void RecibirDanio(float danio)
    {
        stats.health -= danio;
        stats.health = Mathf.Clamp(stats.health, 0, stats.maxHealth);
        ActualizarUI();

        if (stats.health <= 0)
        {
            Morir();
        }
    }

    private void ActualizarUI()
    {
        if (barraSalud != null)
        {
            barraSalud.maxValue = stats.maxHealth;
            barraSalud.value = stats.health;
        }

        if (textoSalud != null)
        {
            textoSalud.text = $"{stats.health} / {stats.maxHealth}";
        }
    }

    private void Morir()
    {
        Debug.Log(gameObject.name + " ha muerto.");
        SceneManager.LoadScene("Escena de muerte"); // Cambia el nombre si querés otra escena
    }
}
