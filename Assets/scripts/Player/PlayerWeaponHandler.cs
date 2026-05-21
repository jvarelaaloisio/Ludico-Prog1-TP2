using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    [System.Serializable]
    public struct WeaponOffset
    {
        public Vector2 position;   // desde dónde nace el arco de ataque
        public float rotationZ;    // ángulo inicial para que el arco mire al lado correcto
        public int sortingOrder;   // delante o detrás del player
    }

    [Header("Configuración del Arco de Ataque")]
    [SerializeField] private WeaponOffset offsetDown;
    [SerializeField] private WeaponOffset offsetUp;
    [SerializeField] private WeaponOffset offsetRight;
    [SerializeField] private WeaponOffset offsetLeft;

    private SpriteRenderer axeSpriteRenderer;
    private GameObject pickedUpAxe = null;

    // método para equipar el hacha recogida
    public void EquipAxe(GameObject axe)
    {
        pickedUpAxe = axe; // guarda la referencia al hacha recogida

        pickedUpAxe.transform.SetParent(transform); // el hacha ahora es hija de weaponSlot

        if (pickedUpAxe.TryGetComponent<SpriteRenderer>(out var spriteRen))
        {
            axeSpriteRenderer = spriteRen; // guarda el spriterenderer del hacha para manipularlo
            axeSpriteRenderer.enabled = false; // empieza invisible hasta que se actualice la posición
        }

        Debug.Log("¡Hacha equipada!");
    }

    public void PrepareWeaponDirection(float lastInputX, float lastInputY)
    {
        if (pickedUpAxe == null || axeSpriteRenderer == null) return; // si no hay hacha equipada, no hace nada

        if (Mathf.Abs(lastInputX) > Mathf.Abs(lastInputY))
        {
            if (lastInputX > 0)
                ApplyTransform(offsetRight); // mirando a la derecha
            else
                ApplyTransform(offsetLeft); // mirando a la izquierda
        }
        else
        {
            if (lastInputY > 0)
                ApplyTransform(offsetUp); // mirando hacia arriba
            else
                ApplyTransform(offsetDown); // mirando hacia abajo
        }
    }

    private void ApplyTransform(WeaponOffset offset)
    {
        transform.localPosition = new Vector3(offset.position.x, offset.position.y, 0f);
        transform.localRotation = Quaternion.Euler(0f, 0f, offset.rotationZ);
        axeSpriteRenderer.sortingOrder = offset.sortingOrder; // ajusta el orden de renderizado para que el arma se vea correctamente
    }

    public bool HasAxeEquipped() => pickedUpAxe != null;
    public Animator GetAxeAnimator() => pickedUpAxe.GetComponent<Animator>();
    public void ToggleAxeVisibility(bool visible)
    {
        if (axeSpriteRenderer != null) axeSpriteRenderer.enabled = visible;
    }

    public void ToggleAxeDamage(bool active)
    {
        if (pickedUpAxe != null)
        {
            if (pickedUpAxe.TryGetComponent<DamageOnTouch>(out var damageScript))
            {
                damageScript.enabled = active; // activa el daño del hacha
            }

            if (pickedUpAxe.TryGetComponent<Collider2D>(out var col))
            {
                col.enabled = active; // activa el collider del hacha para que pueda golpear a los enemigos
            }
        }
    }



    private void OnDrawGizmosSelected()
{
    // Esto se dibuja solo cuando seleccionás el WeaponSlot en la jerarquía
    // Tomamos como base la posición del personaje (el padre de este objeto)
    Vector3 basePos = transform.parent != null ? transform.parent.position : transform.position;

    // Derecha (Rojo)
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(basePos + new Vector3(offsetRight.position.x, offsetRight.position.y, 0f), 0.15f);

    // Izquierda (Azul)
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(basePos + new Vector3(offsetLeft.position.x, offsetLeft.position.y, 0f), 0.15f);

    // Arriba (Verde)
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(basePos + new Vector3(offsetUp.position.x, offsetUp.position.y, 0f), 0.15f);

    // Abajo (Amarillo)
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(basePos + new Vector3(offsetDown.position.x, offsetDown.position.y, 0f), 0.15f);
}
}
