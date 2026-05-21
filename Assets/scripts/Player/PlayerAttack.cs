using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    private PlayerWeaponHandler weaponHandler;
    private Animator playerAnim;

    [Header("Configuración de Ataque")]
    [SerializeField] private float attackDuration = 0.3f; // tiempo que dura el hacha visible
    private bool isAttacking = false;

    void Awake()
    {
        // referencias en el player
        weaponHandler = GetComponentInChildren<PlayerWeaponHandler>();
        playerAnim = GetComponent<Animator>();
    }

    void Update()
    {
        // si se presiona space y no se está atacando, se inicia el ataque
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            // solo se ataca si el hacha fue recogida
            if (weaponHandler != null && weaponHandler.HasAxeEquipped())
            {
                StartCoroutine(PerformAttack());
            }
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // veo hacia dónde mira el jugador
        float lastX = playerAnim.GetFloat("LastInputX");
        float lastY = playerAnim.GetFloat("LastInputY");

        // el handler acomoda el hacha
        weaponHandler.PrepareWeaponDirection(lastX, lastY);

        // muestra el hacha y activa el daño
        weaponHandler.ToggleAxeVisibility(true);
        weaponHandler.ToggleAxeDamage(true);

        // dispara animación de swing en el hacha
        Animator axeAnim = weaponHandler.GetAxeAnimator();
        if (axeAnim != null)
        {
            axeAnim.SetTrigger("TriggerSwing");
        }

        // espera a que termine el movimiento
        yield return new WaitForSeconds(attackDuration);

        // oculta el hacha y apaga el daño
        weaponHandler.ToggleAxeVisibility(false);
        weaponHandler.ToggleAxeDamage(false);

        isAttacking = false;
    }
}