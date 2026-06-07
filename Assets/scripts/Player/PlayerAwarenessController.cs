using Core;
using HealthSystem.Runtime;
using UnityEngine;

public class PlayerAwarenessController : MonoBehaviour
{

    // get público para que otros scripts accedan
    // private set para que solo este script pueda modificarlo
    public bool isAware { get; private set; } // propiedad para saber si el enemigo vio al player

    public Vector2 DirectionToPlayer { get; private set; } // propiedad para ir al player

    [SerializeField] private float playerAwarenessDistance = 4f; // distancia a la que el enemy puede ver al player

    private Transform player; // referencia al transform del player
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float knockback = 10f;
    private float _lastAttackTime = -1f;

    private void Awake()
    {
        var playerMovement = GameObject.FindWithTag("Player");

        if (playerMovement != null)
        {
            player = playerMovement.transform; // obtiene el transform del player
        }
    }

    void Update()
    {
        // si el player no existe o murió, reset y salir
        if (player == null)
        {
            isAware = false;
            DirectionToPlayer = Vector2.zero;
            return;
        }

        Vector2 enemyToPlayerVector = player.position - transform.position; // vector del enemigo al player
        DirectionToPlayer = enemyToPlayerVector.normalized; // dirección normalizada al player

        // se comparan distancias al cuadrado
        float sqrDistance = enemyToPlayerVector.sqrMagnitude;
        float sqrAwarenessDistance = playerAwarenessDistance * playerAwarenessDistance;

        // si la distancia es menor o igual al rango, lo ve
        isAware = sqrDistance <= sqrAwarenessDistance;
        if (sqrDistance <= attackDistance
            && _lastAttackTime + attackCooldown <= Time.time
            && player.TryGetComponent(out IHealthComponent health))
        {
            health.TakeDamage(1);
            if (player.TryGetComponent(out Rigidbody2D rigidbody2D))
                rigidbody2D.AddForce(DirectionToPlayer * knockback, ForceMode2D.Impulse);
            _lastAttackTime = Time.time;
        }
    }
}
