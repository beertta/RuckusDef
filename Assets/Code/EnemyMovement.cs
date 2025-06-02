using UnityEngine;
using System;

public class EnemyMovement : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    public float speed = 2f;
    public Animator animator;

    public static event Action OnEnemigoLlegoAlFinal; // Evento estático

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (currentWaypointIndex >= waypoints.Length)
        {
            animator.SetBool("isWalking", false);
            return;
        }

        Transform target = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        animator.SetBool("isWalking", true);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                OnEnemigoLlegoAlFinal?.Invoke(); // Notificar
            }
        }
    }
}
