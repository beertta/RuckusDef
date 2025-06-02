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

    [Header("UI")]
    public Image barraDeVidaUI;

    private int enemigosActivosAtacando = 0;
    private Coroutine rutinaReducirVida;
    private bool esperandoReducirVida = false;
    private bool reduccionPausada = true;

    private List<EnemigoEnEspera> enemigosEnEspera = new List<EnemigoEnEspera>();

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

            if (vidaActual <= 0)
            {
                Debug.Log("¡Vida agotada!");
                yield break;
            }

            float tiempo = GetTiempoReduccion();
            yield return new WaitForSeconds(tiempo);
        }
    }

    void ActualizarBarra()
    {
        if (barraDeVidaUI != null)
        {
            barraDeVidaUI.fillAmount = vidaActual / vidaMaxima;
        }
    }

    //  Este lo llama el enemigo al instanciarse (por ejemplo en Start del enemigo)
    public void RegistrarEnemigoEnEspera(GameObject enemigo)
    {
        float tiempo = GetTiempoEsperaRonda();
        enemigosEnEspera.Add(new EnemigoEnEspera(enemigo, Time.time + tiempo));
    }

    // Este lo llama el enemigo cuando muere antes de atacar
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
        {
            reduccionPausada = true;
        }
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

    //  Struct para llevar el control temporal de los enemigos
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
