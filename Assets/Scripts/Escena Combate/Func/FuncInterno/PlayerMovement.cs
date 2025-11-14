using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variables públicas para configurar la velocidad en el Inspector
    public float runSpeed = 5.0f;
    public float rotationSpeed = 10.0f;

    // Componentes que el script necesita
    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        // Obtener los componentes Rigidbody y Animator del personaje
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // MUY IMPORTANTE: Congelar la rotación para que el personaje no se voltee 
        // con las colisiones del Rigidbody.
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    void FixedUpdate()
    {
        // Usamos FixedUpdate porque estamos interactuando con el Rigidbody (física)
        HandleMovementAndAnimation();
    }

    void HandleMovementAndAnimation()
    {
        // 1. Obtener la entrada del jugador
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Vector de dirección en el plano XZ (horizontal)
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // MOVER al personaje usando Rigidbody.velocity
            
            // Calcular la velocidad deseada
            Vector3 targetVelocity = direction * runSpeed;

            // Mantener la velocidad vertical actual (para gravedad, saltos, etc.)
            targetVelocity.y = rb.velocity.y; 

            // Aplicar la velocidad
            rb.velocity = targetVelocity;

            // ROTAR al personaje para que mire en la dirección del movimiento
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                Time.deltaTime * rotationSpeed
            );

            // SINCRONIZAR el Animator (ponemos IsRunning en true)
            animator.SetBool("IsRunning", true);
        }
        else
        {
            // El jugador no se está moviendo, parar el Rigidbody en X y Z
            rb.velocity = new Vector3(0, rb.velocity.y, 0); 
            
            // SINCRONIZAR el Animator (ponemos IsRunning en false)
            animator.SetBool("IsRunning", false);
        }
    }
}