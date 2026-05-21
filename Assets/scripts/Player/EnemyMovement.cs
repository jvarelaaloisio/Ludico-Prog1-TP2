using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 2f; // velocidad del enemigo

    private Rigidbody2D rb; // referencia al rigidbody del enemigo
    private PlayerAwarenessController awarenessController; // referencia al PlayerAwarenessController
    private Vector2 targetDirection; // dirección hacia el player
    private Animator animator;
    private SpriteRenderer spriteRenderer; // lo necesito para girar el sprite

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        awarenessController = GetComponent<PlayerAwarenessController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        UpdateTargetDirection();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {        
        SetVelocity(); // fixedupdate se usa exclusivamente en físicas
    }

    private void UpdateTargetDirection()
    {
        // si el enemigo ve al player, se guarda la dirección
        // si no, se resetea a 0 para detener el movimiento
        if (awarenessController.isAware)
        {
            targetDirection = awarenessController.DirectionToPlayer;
        }
        else
        {
            targetDirection = Vector2.zero;
        }
    }

    private void SetVelocity()
    {
        rb.linearVelocity = targetDirection * moveSpeed;
    }

    private void UpdateAnimation()
    {
        // misma lógica que playermovement
        bool isWalking = targetDirection.sqrMagnitude > 0;
        animator.SetBool("isWalking", isWalking);

        if (isWalking)
        {
            animator.SetFloat("InputX", targetDirection.x);
            animator.SetFloat("InputY", targetDirection.y);

            animator.SetFloat("LastInputX", targetDirection.x);
            animator.SetFloat("LastInputY", targetDirection.y);

            if (targetDirection.x > 0f)
            {
                spriteRenderer.flipX = false; // desactiva el espejo cuando va a la derecha
            }
            else if (targetDirection.x < 0f)
            {
                spriteRenderer.flipX = true;  // activa el espejo cuando va a la izquierda
            }
        }
    }
}
