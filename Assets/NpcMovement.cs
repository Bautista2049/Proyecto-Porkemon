using UnityEngine;

public class NpcMovement : MonoBehaviour
{
    public float runSpeed = 5.0f;
    public float rotationSpeed = 10.0f;

    private Rigidbody rb;
    private Animator animator;

    private void Awake()
    {
        // Al cargar una escena, si hay una posición guardada, mueve al jugador allí.
        // Esto es clave para volver al punto exacto después de un combate.
        if (GameState.posicionJugadorGuardadaDisponible)
        {
            transform.position = GameState.posicionJugadorGuardada;
            GameState.posicionJugadorGuardadaDisponible = false; // Se usa la posición solo una vez.
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    void FixedUpdate()
    {
        HandleMovementAndAnimation();
    }

    void HandleMovementAndAnimation()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            Vector3 targetVelocity = direction * runSpeed;

            targetVelocity.y = rb.velocity.y;
            rb.velocity = targetVelocity;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );

            animator.SetBool("IsRunning", true);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);

            animator.SetBool("IsRunning", false);
        }
    }
}
