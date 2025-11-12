using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    
    private Rigidbody rb;
    private Animator animator;
    private Vector3 movement;

    void Start()
    {
        // Obtiene el Animator del hijo
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Entrada del teclado
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Calcula la dirección de movimiento
        movement = new Vector3(horizontal, 0f, vertical).normalized;
        
        // Actualiza el parámetro del Animator
        if (animator != null)
        {
            // Solo usamos la magnitud del movimiento (0 = quieto, 1 = moviendose)
            animator.SetFloat("velocity", movement.magnitude);
        }
    }

    void FixedUpdate()
    {
        // Mueve al personaje
        if (movement.magnitude >= 0.1f)
        {
            // Rota hacia la dirección del movimiento
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(Quaternion.Euler(0f, angle, 0f));
            
            // Mueve al personaje
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }
}