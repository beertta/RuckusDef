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
        if (bossRenderer == null)
        {
            bossRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            if (bossRenderer == null)
            {
                Debug.LogError("No se encontró SkinnedMeshRenderer en FinalBoss ni hijos.");
            }
            else
            {
                Debug.Log("SkinnedMeshRenderer asignado automáticamente en FinalBoss.");
            }
        }

        colorToMaterial = new Dictionary<string, Material>
    {
        { "Red", redMaterial },
        { "Green", greenMaterial },
        { "Blue", blueMaterial },
        { "Yellow", yellowMaterial }
    };

        if (colorSequence == null || colorSequence.Count == 0)
        {
            IniciarBoss();
        }
    }


    public void IniciarBoss()
    {
        // Primer color fijo (por ejemplo "Red")
        colorSequence = new List<string> { "Red", "Green", "Blue", "Yellow" };

        // Barajamos solo los colores a partir del índice 1 (sin tocar el primero)
        List<string> subList = colorSequence.GetRange(1, colorSequence.Count - 1);
        Shuffle(subList);

        // Volvemos a colocar el rojo fijo al inicio
        colorSequence = new List<string> { "Red" };
        colorSequence.AddRange(subList);

        currentColorIndex = 0;
        AplicarColorActual();
        Debug.Log("FinalBoss iniciado con color inicial fijo y resto aleatorio.");
    }

    void AplicarColorActual()
    {
        string color = colorSequence[currentColorIndex];
        if (colorToMaterial.TryGetValue(color, out Material mat) && bossRenderer != null)
        {
            bossRenderer.material = mat;
            Debug.Log($"FinalBoss color aplicado: {color}");
        }
        else
        {
            Debug.LogWarning("Material o Renderer no asignado en FinalBoss.");
        }

        spawner?.ActivarFormaPorColor(color);
        gameObject.tag = color; // Para que el arma correcta pueda detectarlo
    }

    void SiguienteColor()
    {
        currentColorIndex++;
        if (currentColorIndex >= colorSequence.Count)
        {
            Debug.Log("FinalBoss muerto, secuencia completada.");
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
