using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public ParticleSystem onDeathParticles; // Referencia al sistema de partículas hijo
    public float effectDuration = 1f;       // Tiempo de vida del sistema de partículas

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

                // Si se asignó el sistema de partículas
                if (onDeathParticles != null)
                {
                    onDeathParticles.gameObject.SetActive(true); // Asegura que esté activo
                    onDeathParticles.Play();                     // Reproduce el efecto

                    // Lo separamos del enemigo antes de destruirlo
                    onDeathParticles.transform.parent = null;
                    Destroy(onDeathParticles.gameObject, effectDuration);
                }

                Destroy(gameObject); // Destruye el enemigo
                Destroy(collision.gameObject); // Destruye el objeto que lo golpeó
            }
            else
            {
                Debug.Log("Colores no coinciden, enemigo no destruido");
            }
        }
    }
}
