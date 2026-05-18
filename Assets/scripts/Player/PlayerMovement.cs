using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // usa el rigidbody del player
        animator = GetComponent<Animator>(); // usa el animator del player
    }

    void Update()
    {
        // raw devuelve 0, 1 o -1
        float inputHorizontal = Input.GetAxisRaw("Horizontal"); // lee el input horizontal del player
        float inputVertical = Input.GetAxisRaw("Vertical"); // lee el input vertical del player

        moveInput = new Vector2(inputHorizontal, inputVertical); // crea un vector de movimiento con el input del player

        UpdateAnimation(moveInput); // actualiza la animación del player

        moveInput.Normalize(); // normaliza el vector para que no vaya más rápido en diagonal
    }

    private void FixedUpdate()
    {
        SetVelocity();
    }

    private void SetVelocity()
    {
        rb.linearVelocity = moveInput * moveSpeed; // se asigna a la velocidad del rb el vector * la velocidad
    }

    void UpdateAnimation (Vector2 dir)
    {
        bool isWalking = dir.sqrMagnitude > 0; // si el vector tiene magnitud: está caminando
        animator.SetBool("isWalking", isWalking); // asigna el bool de la animación

        if (isWalking)
        {
            // si se mueve, asigno las direcciones a los params de walk
            animator.SetFloat("InputX", dir.x);
            animator.SetFloat("InputY", dir.y);

            // guardo la última dirección para el idle
            animator.SetFloat("LastInputX", dir.x);
            animator.SetFloat("LastInputY", dir.y);
        }
    }

    private void OnDisable()
    {
        moveInput = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // detiene el movimiento al desactivar el player
        }
        if (animator != null)
        {
            animator.SetBool("isWalking", false); // detiene la animación al desactivar el player
        }
    }
}
