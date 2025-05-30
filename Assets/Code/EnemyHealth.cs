using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Red") ||
            collision.gameObject.CompareTag("Green") ||
            collision.gameObject.CompareTag("Blue") ||
            collision.gameObject.CompareTag("Yellow"))
        {
            // Solo destruye si el tag coincide con el propio tag (color)
            if (collision.gameObject.tag == gameObject.tag)
            {
                Debug.Log("Enemigo destruido, colores coinciden");

                // Destruye el enemigo (este)
                Destroy(gameObject);

                // También destruye el objeto que lo golpeó
                Destroy(collision.gameObject);
            }
            else
            {
                Debug.Log("Colores no coinciden, enemigo no destruido");
            }
        }
    }
}
