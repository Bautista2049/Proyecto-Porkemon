using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogosMision
{
    [TextArea(3, 5)] public string[] dialogoInicial;
    [TextArea(3, 5)] public string[] dialogoMisionActiva;
    [TextArea(3, 5)] public string[] dialogoMisionCompletada;
    [TextArea(3, 5)] public string[] dialogoRecompensaEntregada;
}

public class ControladorMisiones : MonoBehaviour
{
    private static ControladorMisiones _instancia;
    public static ControladorMisiones Instancia 
    { 
        get
        {
            if (_instancia == null)
            {
                _instancia = FindObjectOfType<ControladorMisiones>();
                if (_instancia == null)
                {
                    GameObject go = new GameObject("ControladorMisiones");
                    _instancia = go.AddComponent<ControladorMisiones>();
                    DontDestroyOnLoad(go);
                    _instancia.InicializarDialogosPorDefecto();
                }
            }
            return _instancia;
        } 
    }

    // Estado de la misión
    public bool misionAceptada = false;
    public bool misionCompletada = false;
    public bool recompensaEntregada = false;
    public int pokemonesDerrotados = 0;
    public const int POKEMONES_NECESARIOS = 5;
    public bool panelMisionesVisible = false;

    // Diálogos
    public DialogosMision dialogos = new DialogosMision();

    private void Awake()
    {
        if (_instancia == null)
        {
            _instancia = this;
            DontDestroyOnLoad(gameObject);
            InicializarDialogosPorDefecto();
        }
        else if (_instancia != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void InicializarDialogosPorDefecto()
    {
        if (dialogos.dialogoInicial == null || dialogos.dialogoInicial.Length == 0)
        {
            dialogos.dialogoInicial = new string[]
            {
                "Hola viajero",
                "No pareces de estos lares",
                "Si ayudas con un problema que tengo te compensaré",
                "Necesito que derrotes o captures cinco pokemones salvajes",
                "Hazlo y te daré algo que te será muy útil en tu aventura"
            };
        }

        if (dialogos.dialogoMisionActiva == null || dialogos.dialogoMisionActiva.Length == 0)
        {
            dialogos.dialogoMisionActiva = new string[]
            {
                "Sigue adelante, viajero",
                "Recuerda que necesito que derrotes o captures cinco pokemones salvajes",
                "Vuelve cuando hayas completado tu tarea"
            };
        }

        if (dialogos.dialogoMisionCompletada == null || dialogos.dialogoMisionCompletada.Length == 0)
        {
            dialogos.dialogoMisionCompletada = new string[]
            {
                "¡Excelente trabajo, viajero!",
                "Has completado la misión que te encomendé",
                "Como prometí, aquí tienes tu recompensa",
                "Te daré una Ultrabola y un Charmander que te serán de gran ayuda"
            };
        }

        if (dialogos.dialogoRecompensaEntregada == null || dialogos.dialogoRecompensaEntregada.Length == 0)
        {
            dialogos.dialogoRecompensaEntregada = new string[]
            {
                "Gracias por tu ayuda, viajero",
                "Espero que esta Ultrabola y tu nuevo Charmander te sirvan bien en tu aventura",
                "¡Buena suerte en tu viaje!"
            };
        }
    }

    public void AceptarMision()
    {
        misionAceptada = true;
        misionCompletada = false;
        recompensaEntregada = false;
        pokemonesDerrotados = 0;
    }

    public void CompletarMision()
    {
        misionCompletada = true;
    }

    public void EntregarRecompensa()
    {
        recompensaEntregada = true;
    }

    public void IncrementarPokemonesDerrotados()
    {
        if (misionAceptada && !misionCompletada)
        {
            pokemonesDerrotados++;
            if (pokemonesDerrotados >= POKEMONES_NECESARIOS)
            {
                CompletarMision();
            }
        }
    }
}