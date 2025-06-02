using System.Collections;
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

    [Header("Porcentaje de daño que se suma si se mata enemigo antes de que empiece a bajar la vida")]
    [Range(0f, 1f)]
    public float porcentajeVidaPorMatarEnemigo = 0.1f;

    [Header("Ronda actual (1 a 3)")]
    public int rondaActual = 1;

    [Header("UI")]
    public Image barraDeVidaUI;

    private int enemigosActivosAtacando = 0;
    private Coroutine rutinaReducirVida;
    private bool esperandoReducirVida = false;
    private bool reduccionPausada = true;

    void Start()
    {
        Instance = this;
        vidaActual = vidaMaxima;
        ActualizarBarra();
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
        {
            rutinaReducirVida = StartCoroutine(ReducirVida());
        }

        if (reduccionPausada)
        {
            StartCoroutine(EsperarYReanudar());
        }
    }

    IEnumerator EsperarYReanudar()
    {
        esperandoReducirVida = true;

        float tiempoEspera = tiempoEsperaRonda1;
        switch (rondaActual)
        {
            case 1: tiempoEspera = tiempoEsperaRonda1; break;
            case 2: tiempoEspera = tiempoEsperaRonda2; break;
            case 3: tiempoEspera = tiempoEsperaRonda3; break;
        }

        yield return new WaitForSeconds(tiempoEspera);

        esperandoReducirVida = false;
        reduccionPausada = false; // Reanudamos la reducción
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

            float dañoPorSegundo = dañoRonda1;
            switch (rondaActual)
            {
                case 1: dañoPorSegundo = dañoRonda1; break;
                case 2: dañoPorSegundo = dañoRonda2; break;
                case 3: dañoPorSegundo = dañoRonda3; break;
            }

            vidaActual -= dañoPorSegundo;
            vidaActual = Mathf.Clamp(vidaActual, 0f, vidaMaxima);
            ActualizarBarra();

            if (vidaActual <= 0)
            {
                Debug.Log("¡Vida agotada!");
                yield break;
            }

            float tiempoReal = tiempoBaseReduccion;
            switch (rondaActual)
            {
                case 2: tiempoReal /= 2f; break;
                case 3: tiempoReal /= 5f; break;
            }

            yield return new WaitForSeconds(tiempoReal);
        }
    }

    void ActualizarBarra()
    {
        if (barraDeVidaUI != null)
        {
            barraDeVidaUI.fillAmount = vidaActual / vidaMaxima;
        }
    }

    // Llamar cuando un enemigo muere antes de llegar al último waypoint
    public void EnemigoMuertoAntesDeAtacar()
    {
        if (!esperandoReducirVida && !reduccionPausada && vidaActual < vidaMaxima)
        {
            float dañoPorSegundo = dañoRonda1;
            switch (rondaActual)
            {
                case 1: dañoPorSegundo = dañoRonda1; break;
                case 2: dañoPorSegundo = dañoRonda2; break;
                case 3: dañoPorSegundo = dañoRonda3; break;
            }

            float incremento = dañoPorSegundo * porcentajeVidaPorMatarEnemigo;
            vidaActual += incremento;
            vidaActual = Mathf.Clamp(vidaActual, 0f, vidaMaxima);
            ActualizarBarra();
        }
    }

    // Llamar cuando un enemigo que estaba atacando muere
    public void EnemigoAtacandoMuerto()
    {
        enemigosActivosAtacando = Mathf.Max(0, enemigosActivosAtacando - 1);

        if (enemigosActivosAtacando == 0)
        {
            reduccionPausada = true; // Pausamos la reducción porque no hay enemigos atacando
        }
    }
}
