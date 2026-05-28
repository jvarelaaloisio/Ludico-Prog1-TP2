using UnityEngine;
using UnityEngine.Events; // con esto manejamos eventos
using Core;
using VarelaAloisio.Core;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float furyReward = 0.1f;
    private IFuryManager _furyManager;

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
        // Intentamos buscar y usar el servicio todo en el mismo movimiento
        if (CompareTag("Enemy"))
        {
            if (Service.TryGet(out IFuryManager furyService))
            {
                Debug.Log($"[HEALTH - {gameObject.name}] ¡Servicio encontrado! Pasando {furyReward} de furia...");
                furyService.AddFury(furyReward);
            }
            else
            {
                Debug.LogWarning($"[HEALTH - {gameObject.name}] Alerta: El servicio IFuryManager no está listo en la escena todavía.");
            }
        }
    }
}
