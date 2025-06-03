using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    public static BarraDeVida Instance;

    [Header("Vida")]
    public float vidaMaxima = 100f;
    public float vidaActual = 100f;

    [Header("Daño por segundo según ronda")]
    public float dañoRonda1 = 5f;
    public float dañoRonda2 = 10f;
    public float dañoRonda3 = 25f;

    [Header("Tiempo base entre ticks")]
    public float tiempoBaseReduccion = 1f;

    [Header("Tiempos de espera antes de reducir la vida según ronda")]
    public float tiempoEsperaRonda1 = 5f;
    public float tiempoEsperaRonda2 = 2.5f;
    public float tiempoEsperaRonda3 = 1f;

    [Header("Ronda actual (1 a 3)")]
    public int rondaActual = 1;

    [Header("Referencias externas")]
    public Image barraDeVidaUI;
    public GameObject gameOverPanel;
    public EnemySpawner enemySpawner; // Referencia a EnemySpawner

    private int enemigosActivosAtacando = 0;
    private Coroutine rutinaReducirVida;
    private bool esperandoReducirVida = false;
    private bool reduccionPausada = true;
    private bool vidaAgotada = false;

    private List<EnemigoEnEspera> enemigosEnEspera = new List<EnemigoEnEspera>();

    void Start()
    {
        Instance = this;
        vidaActual = vidaMaxima;
        ActualizarBarra();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        EnemyMovement.OnEnemigoLlegoAlFinal += EnemigoLlegadoAlFinal;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        EnemyMovement.OnEnemigoLlegoAlFinal -= EnemigoLlegadoAlFinal;
    }

    void EnemigoLlegadoAlFinal()
    {
        enemigosActivosAtacando++;

        if (rutinaReducirVida == null)
            rutinaReducirVida = StartCoroutine(ReducirVida());

        if (reduccionPausada)
            StartCoroutine(EsperarYReanudar());
    }

    IEnumerator EsperarYReanudar()
    {
        esperandoReducirVida = true;
        float tiempoEspera = GetTiempoEsperaRonda();
        yield return new WaitForSeconds(tiempoEspera);
        esperandoReducirVida = false;
        reduccionPausada = false;
    }

    IEnumerator ReducirVida()
    {
        while (vidaActual > 0)
        {
            if (enemigosActivosAtacando == 0 || esperandoReducirVida || reduccionPausada)
            {
                yield return null;
                continue;
            }

            float daño = GetDañoRonda();
            vidaActual -= daño;
            vidaActual = Mathf.Clamp(vidaActual, 0f, vidaMaxima);
            ActualizarBarra();

            if (vidaActual <= 0 && !vidaAgotada)
            {
                vidaAgotada = true;
                StartCoroutine(ReiniciarPartida());
                yield break;
            }

            float tiempo = GetTiempoReduccion();
            yield return new WaitForSeconds(tiempo);
        }
    }

    // Nuevo método público para reiniciar desde fuera
    public void ReiniciarPartidaPublica()
    {
        StartCoroutine(ReiniciarPartida());
    }

    // Ahora privado para controlar el acceso
    private IEnumerator ReiniciarPartida()
    {
        Debug.Log("Vida agotada. Reiniciando...");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        yield return new WaitForSeconds(3f);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Resetear vida y variables
        vidaActual = vidaMaxima;
        ActualizarBarra();
        rondaActual = 1;
        enemigosActivosAtacando = 0;
        reduccionPausada = true;
        esperandoReducirVida = false;
        vidaAgotada = false;
        enemigosEnEspera.Clear();
        rutinaReducirVida = null;

        // Notificar al EnemySpawner que reinicie
        if (enemySpawner != null)
            enemySpawner.ReiniciarPartida();
    }

    void ActualizarBarra()
    {
        if (barraDeVidaUI != null)
            barraDeVidaUI.fillAmount = vidaActual / vidaMaxima;
    }

    public void RegistrarEnemigoEnEspera(GameObject enemigo)
    {
        float tiempo = GetTiempoEsperaRonda();
        enemigosEnEspera.Add(new EnemigoEnEspera(enemigo, Time.time + tiempo));
    }

    public void EnemigoMuertoAntesDeAtacar(GameObject enemigo)
    {
        var item = enemigosEnEspera.Find(e => e.enemigo == enemigo);
        if (item != null && Time.time < item.expiraEn)
        {
            float cantidad = GetDañoRonda();
            vidaActual += cantidad;
            vidaActual = Mathf.Clamp(vidaActual, 0f, vidaMaxima);
            ActualizarBarra();
            enemigosEnEspera.Remove(item);
        }
    }

    public void EnemigoAtacandoMuerto()
    {
        enemigosActivosAtacando = Mathf.Max(0, enemigosActivosAtacando - 1);

        if (enemigosActivosAtacando == 0)
            reduccionPausada = true;
    }

    float GetDañoRonda()
    {
        return rondaActual switch
        {
            1 => dañoRonda1,
            2 => dañoRonda2,
            3 => dañoRonda3,
            _ => dañoRonda1
        };
    }

    float GetTiempoEsperaRonda()
    {
        return rondaActual switch
        {
            1 => tiempoEsperaRonda1,
            2 => tiempoEsperaRonda2,
            3 => tiempoEsperaRonda3,
            _ => tiempoEsperaRonda1
        };
    }

    float GetTiempoReduccion()
    {
        return rondaActual switch
        {
            2 => tiempoBaseReduccion / 2f,
            3 => tiempoBaseReduccion / 5f,
            _ => tiempoBaseReduccion
        };
    }

    private class EnemigoEnEspera
    {
        public GameObject enemigo;
        public float expiraEn;

        public EnemigoEnEspera(GameObject e, float tiempo)
        {
            enemigo = e;
            expiraEn = tiempo;
        }
    }
}
