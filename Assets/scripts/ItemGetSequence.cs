using UnityEngine;
using System.Collections;

public class ItemGetSequence : MonoBehaviour
{
    [SerializeField] private float displayDuration = 1f;
    [SerializeField] private Vector3 offsetAboveHead = new Vector3(0, 1.2f, 0);

    private bool isPickedUp = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPickedUp && other.CompareTag("Player"))
        {
            StartCoroutine(ShowItemRoutine(other.gameObject));
        }
    }

    private IEnumerator ShowItemRoutine(GameObject player)
    {
        isPickedUp = true;

        // referencias a componentes del jugador
        PlayerMovement moveScript = player.GetComponent<PlayerMovement>();
        PlayerAttack attackScript = player.GetComponent<PlayerAttack>();
        Animator playerAnim = player.GetComponent<Animator>();
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        PlayerWeaponHandler weaponHandler = player.GetComponentInChildren<PlayerWeaponHandler>();

        // congela al jugador y lo prepara para la animación
        if (moveScript) moveScript.enabled = false;
        if (attackScript) attackScript.enabled = false; // para que no pueda atacar
        if (playerRb) playerRb.linearVelocity = Vector2.zero;
        playerAnim.SetTrigger("GotItem");

        // posiciona el hacha sobre la cabeza visualmente
        transform.SetParent(player.transform);
        transform.localPosition = offsetAboveHead;
        transform.localRotation = Quaternion.identity;

        // con trygetcomponent se apagan los componentes con seguridad
        if (TryGetComponent<ItemHighlight>(out var highlight)) highlight.enabled = false;
        if (TryGetComponent<Collider2D>(out var col)) col.enabled = false;

        // se apaga el animator temporalmente para evitar conflictos
        Animator axeAnimator = GetComponent<Animator>();
        if (axeAnimator) axeAnimator.enabled = false;

        // se espera al tiempo de exhibición del item en la pantalla
        yield return new WaitForSeconds(displayDuration);

        // se reactiva el animator para la animación de equipar
        if (axeAnimator) axeAnimator.enabled = true;

        // se equipa el hacha
        if (weaponHandler != null)
        {
            transform.SetParent(weaponHandler.transform); // ahora es hijo
            transform.localPosition = Vector3.zero;
            weaponHandler.EquipAxe(gameObject); // el handler la vuelve invisible y la prepara
        }

        // devuelve el control al jugador
        if (moveScript) moveScript.enabled = true;
        if (attackScript) attackScript.enabled = true;
    }
}