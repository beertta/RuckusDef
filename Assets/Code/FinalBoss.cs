//using UnityEngine;

//public class FinalBoss : MonoBehaviour
//{
//    public string colorEsperado;  // Color que debe recibir el arma para recibir daño
//    public bool esPrimerBoss = false;
//    public EnemySpawner spawner;

//    public GameObject panelFinJuego; // Panel de fin de juego (poner en inspector)

//    private int golpesRecibidos = 0;
//    private int golpesParaDerrotar = 4;
//    private bool haSidoDerrotado = false;

//    public void IniciarBoss()
//    {
//        haSidoDerrotado = false;
//        golpesRecibidos = 0;
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.CompareTag("Red") ||
//            collision.gameObject.CompareTag("Green") ||
//            collision.gameObject.CompareTag("Blue") ||
//            collision.gameObject.CompareTag("Yellow"))
//        {
//            if (collision.gameObject.tag == colorEsperado)
//            {
//                Debug.Log("Final Boss golpeado con color correcto: " + colorEsperado);

//                // Destruye el arma
//                Destroy(collision.gameObject);

//                RecibirDanio();
//            }
//            else
//            {
//                Debug.Log("Final Boss recibió un golpe de color incorrecto.");
//            }
//        }
//    }

//    private void RecibirDanio()
//    {
//        if (haSidoDerrotado) return;

//        golpesRecibidos++;

//        if (golpesRecibidos >= golpesParaDerrotar)
//        {
//            haSidoDerrotado = true;

//            if (spawner != null)
//            {
//                spawner.OnEnemyDestroyed();
//                spawner.OnFinalBossKilled(colorEsperado);
//            }

//            if (panelFinJuego != null)
//                panelFinJuego.SetActive(true);

//            Destroy(gameObject);
//        }
//    }
//}
