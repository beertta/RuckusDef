using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    [Header("Texturas por color")]
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public Material yellowMaterial;

    [Header("Renderizador del conejo")]
    public Renderer bossRenderer;

    [Header("Sistema de partículas al morir")]
    public ParticleSystem onDeathParticles;
    public float effectDuration = 2f;

    [HideInInspector] public EnemySpawner spawner;

    private List<string> colorSequence;
    private int currentColorIndex = 0;

    private Dictionary<string, Material> colorToMaterial;

    void Start()
    {
        colorToMaterial = new Dictionary<string, Material>
        {
            { "Red", redMaterial },
            { "Green", greenMaterial },
            { "Blue", blueMaterial },
            { "Yellow", yellowMaterial }
        };
    }

    public void IniciarBoss()
    {
        colorSequence = new List<string> { "Red", "Green", "Blue", "Yellow" };
        Shuffle(colorSequence);
        currentColorIndex = 0;
        AplicarColorActual();
    }

    void AplicarColorActual()
    {
        string color = colorSequence[currentColorIndex];
        bossRenderer.material = colorToMaterial[color];
        spawner?.ActivarFormaPorColor(color);
        gameObject.tag = color; // Para que el arma correcta pueda detectarlo
    }

    void SiguienteColor()
    {
        currentColorIndex++;

        if (currentColorIndex >= colorSequence.Count)
        {
            // Muerte del jefe
            StartCoroutine(DeathSequence());
        }
        else
        {
            AplicarColorActual();
        }
    }

    private IEnumerator DeathSequence()
    {
        if (onDeathParticles != null)
        {
            onDeathParticles.transform.parent = null;
            onDeathParticles.gameObject.SetActive(true);
            onDeathParticles.Play();
            Destroy(onDeathParticles.gameObject, effectDuration);
        }

        yield return new WaitForSeconds(effectDuration);
        Destroy(gameObject);
        spawner?.DesactivarFormas();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Red") ||
            collision.gameObject.CompareTag("Green") ||
            collision.gameObject.CompareTag("Blue") ||
            collision.gameObject.CompareTag("Yellow"))
        {
            if (collision.gameObject.tag == gameObject.tag)
            {
                Destroy(collision.gameObject);
                SiguienteColor();
            }
        }
    }

    void Shuffle(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            string temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }
}
