using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public ParticleSystem onDeathParticles; // Referencia al sistema de partículas hijo
    public float effectDuration = 3f;       // Tiempo de vida del sistema de partículas (independiente del enemigo)

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Red") ||
            collision.gameObject.CompareTag("Green") ||
            collision.gameObject.CompareTag("Blue") ||
            collision.gameObject.CompareTag("Yellow"))
        {
            if (collision.gameObject.tag == gameObject.tag)
            {
                Debug.Log("Enemigo destruido, colores coinciden");

                // Destruye el arma inmediatamente
                Destroy(collision.gameObject);

                // Activa y reproduce partículas
                if (onDeathParticles != null)
                {
                    onDeathParticles.gameObject.SetActive(true);
                    onDeathParticles.transform.parent = null;
                    onDeathParticles.Play();

                    // Las partículas se eliminan solas después de unos segundos
                    Destroy(onDeathParticles.gameObject, effectDuration);
                }

                // AVISA a la barra de vida que un enemigo atacando murió
                if (BarraDeVida.Instance != null)
                {
                    BarraDeVida.Instance.EnemigoAtacandoMuerto();
                }

                // Destruye el enemigo inmediatamente
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Colores no coinciden, enemigo no destruido");
            }
        }
    }
}
