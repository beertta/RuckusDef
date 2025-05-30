using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float amplitude = 0.02f;     // Altura del movimiento (2 cm)
    public float frequency = 1f;        // Velocidad del movimiento

    private Vector3 startPos;
    private bool isFloating = true;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (isFloating)
        {
            float offsetY = Mathf.Sin(Time.time * frequency) * amplitude;
            transform.position = startPos + new Vector3(0, offsetY, 0);
        }
    }

    public void SetFloating(bool shouldFloat)
    {
        isFloating = shouldFloat;

        // Si deja de flotar, fijamos la posición actual como base
        if (!shouldFloat)
            startPos = transform.position;
    }
}
