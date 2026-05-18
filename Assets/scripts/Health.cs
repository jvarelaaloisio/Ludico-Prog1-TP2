using UnityEngine;
using UnityEngine.Events; // con esto manejamos eventos

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Eventos de vida")]
    public UnityEvent OnDamageTaken;
    public UnityEvent OnDeath;
    
    public bool isDead => currentHealth <= 0f; // el enemigo está muerto?

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // si ya está muerto, no hace nada

        currentHealth -= damage; // resta el daño a la salud actual

        OnDamageTaken?.Invoke(); // invoca el evento de daño recibido

        if (currentHealth <= 0f)
        {
            currentHealth = 0f; // evita que la salud sea negativa
            Die(); // llama al método
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto.");

        OnDeath?.Invoke(); // invoca el evento de muerte
        // desde el inspector puedo hacer que al morir se destruya el objeto
        // también se le puede arrastrar un script de gamemanager
    }
}
