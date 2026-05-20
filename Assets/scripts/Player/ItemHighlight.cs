using UnityEngine;

public class ItemHighlight : MonoBehaviour
{
    public enum HighlightEffect {Pulse, Wobble} // menú para el inspector

    [Header("Tipo de efecto")]
    [SerializeField] private HighlightEffect currentEffect = HighlightEffect.Pulse;

    [Header("Ajustes del efecto")]
    [SerializeField] private float speed = 3f; // velocidad
    [SerializeField] private float amplitude = 0.1f; // notoriedad

    private Vector3 originalScale;
    private float originalRotationZ;

    void Start()
    {
        // se guardanlos valores iniciales
        originalScale = transform.localScale;
        originalRotationZ = transform.localRotation.eulerAngles.z;
    }

    void Update()
    {
        // se calcula la onda senoidal (de -1 a 1 y se multiplica por la amplitud)
        float wave = Mathf.Sin(Time.time * speed) * amplitude;

        switch (currentEffect)
        {
            case HighlightEffect.Pulse:
                // se aplica el latido modificando la escala
                transform.localScale = originalScale + new Vector3(wave, wave, 0f);
                break;
            
            case HighlightEffect.Wobble:
                // se aplica el balanceo modificando la rotación
                float newRotationZ = originalRotationZ + (wave * 10f);
                transform.localRotation = Quaternion.Euler(0f, 0f, newRotationZ);
                break;
        }
    }

    private void OnDisable()
    {
        // se resetean los valores al desactivar el objeto
        transform.localScale = originalScale;
        transform.localRotation = Quaternion.Euler(0f, 0f, originalRotationZ);
    }
}
