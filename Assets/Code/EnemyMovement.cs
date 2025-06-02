using UnityEngine;
using System;

public class EnemyMovement : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    public float speed = 2f;
    public Animator animator;

    public AudioClip gruñidoClip;      // Sonido inicial del conejo
    public AudioClip pasosClip;        // Sonido de pasos

    private AudioSource audioSource;   // Reproducirá ambos
    private bool gruñidoReproducido = false;

    public static event Action OnEnemigoLlegoAlFinal;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (gruñidoClip != null && audioSource != null && !gruñidoReproducido)
        {
            audioSource.PlayOneShot(gruñidoClip);
            gruñidoReproducido = true;
        }
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (currentWaypointIndex >= waypoints.Length)
        {
            animator.SetBool("isWalking", false);
            PararPasos();
            return;
        }

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        animator.SetBool("isWalking", true);
        ReproducirPasos();

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                OnEnemigoLlegoAlFinal?.Invoke();
            }
        }
    }

    void ReproducirPasos()
    {
        if (pasosClip == null || audioSource == null) return;

        if (!audioSource.isPlaying)
        {
            audioSource.clip = pasosClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void PararPasos()
    {
        if (audioSource != null && audioSource.isPlaying && audioSource.clip == pasosClip)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }
}
