using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f; // cantidad de daño a infligir
    [SerializeField] private string targetTag; // "Player" o "Enemy" - las caps importan!

    private void OnTriggerEnter2D(Collider2D other)
    {
        // sólo hace daño si tiene el tag correcto
        if (other.gameObject.CompareTag(targetTag))
        {
            // busca el componente en el collider
            if (other.TryGetComponent<Health>(out Health targetHealth))
            {
                targetHealth.TakeDamage(damageAmount); // inflige daño
            }
        }
    }
}