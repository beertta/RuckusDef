using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    public bool esPrimerBoss = false;  // Solo indica si es el primero
    public EnemySpawner spawner;

    private bool haSidoDerrotado = false;

    public void IniciarBoss()
    {
        haSidoDerrotado = false;
    }

    public void RecibirDanio()
    {
        if (haSidoDerrotado) return;

        haSidoDerrotado = true;

        // Notificamos al spawner tras morir (sin animación ni delay)
        if (spawner != null)
        {
            spawner.OnEnemyDestroyed();
            spawner.OnFinalBossKilled(gameObject.tag);
        }

        Destroy(gameObject); // Se destruye directamente
    }
}
