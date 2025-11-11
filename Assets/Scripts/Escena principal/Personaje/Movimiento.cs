using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadCaminar = 5f;
    public float velocidadCorrer = 10f;
    public float suavizadoRotacion = 10f;
    
    [Header("Gravedad y Salto")]
    public float gravedad = -15f;
    public float velocidadCaida = -2f;
    public float radioDeteccionSuelo = 0.4f;
    public LayerMask capaSuelo;
    public Transform verificadorSuelo;
    
    // Referencias
    private CharacterController controller;
    private Camera camPrincipal;
    private float velocidadActual;
    private float velocidadVertical;
    private bool estaEnSuelo;
    private Vector3 velocidadMovimiento = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        camPrincipal = Camera.main;
        
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }
        
        // Configuración inicial del CharacterController
        controller.skinWidth = 0.08f;
        controller.minMoveDistance = 0.001f;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        
        // 1. Verificar si está en el suelo
        estaEnSuelo = Physics.CheckSphere(verificadorSuelo.position, radioDeteccionSuelo, capaSuelo);
        
        // 2. Manejar la gravedad
        if (estaEnSuelo && velocidadVertical < 0)
        {
            velocidadVertical = -2f; // Pequeña fuerza hacia abajo para mantener al personaje pegado al suelo
        }
        
        // Aplicar gravedad
        if (!estaEnSuelo)
        {
            velocidadVertical += gravedad * Time.deltaTime;
        }

        // 3. Obtener entrada del teclado
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Solo mover si hay entrada
        if (horizontal != 0 || vertical != 0)
        {
            // Calcular dirección de movimiento relativa a la cámara
            Vector3 direccionDeseada = (camPrincipal.transform.forward * vertical + camPrincipal.transform.right * horizontal).normalized;
            direccionDeseada.y = 0; // Mantener el movimiento en el plano horizontal
            
            // Determinar velocidad actual (caminar o correr)
            velocidadActual = Input.GetKey(KeyCode.LeftShift) ? velocidadCorrer : velocidadCaminar;
            
            // Aplicar movimiento
            velocidadMovimiento = direccionDeseada * velocidadActual;
            
            // Rotar el personaje en la dirección del movimiento
            if (direccionDeseada != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionDeseada);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, suavizadoRotacion * Time.deltaTime);
            }
        }
        else
        {
            velocidadMovimiento = Vector3.zero;
        }
        
        // Aplicar movimiento vertical (gravedad)
        velocidadMovimiento.y = velocidadVertical;
        
        // Mover el personaje
        controller.Move(velocidadMovimiento * Time.deltaTime);
    }
    
    // Dibujar gizmos para el verificador de suelo (solo en el editor)
    private void OnDrawGizmosSelected()
    {
        if (verificadorSuelo == null) return;
        
        Gizmos.color = estaEnSuelo ? Color.green : Color.red;
        Gizmos.DrawWireSphere(verificadorSuelo.position, radioDeteccionSuelo);
    }
}