using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light torchLight;
    public float baseIntensity = 1f;
    public float flickerAmount = 0.2f;
    public float flickerSpeed = 5f;

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
        torchLight.intensity = baseIntensity + (noise - 0.5f) * flickerAmount;
    }
}
