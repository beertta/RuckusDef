using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [Header("Prefab y movimiento")]
    public GameObject prefabToSpawn;
    public float speed = 3f;

    [Header("Waypoints")]
    public Transform[] waypoints;

    private GameObject spawnedObject;
    private int currentWaypointIndex = 0;
    private bool movementActive = true;

    void Start()
    {
        if (waypoints.Length == 0 || prefabToSpawn == null)
        {
            Debug.LogWarning("Faltan waypoints o prefab.");
            return;
        }

        // Instanciar el prefab en el primer waypoint
        spawnedObject = Instantiate(prefabToSpawn, waypoints[0].position, Quaternion.identity);
    }

    void Update()
    {
        if (!movementActive || spawnedObject == null || waypoints.Length == 0)
            return;

        Transform target = waypoints[currentWaypointIndex];
        Vector3 direction = target.position - spawnedObject.transform.position;
        spawnedObject.transform.position += direction.normalized * speed * Time.deltaTime;

        if (direction.magnitude < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                movementActive = false; // Detener movimiento
            }
        }
    }
}