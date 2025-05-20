using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;         // Los 4 prefabs
    public Transform spawnPoint;
    public float spawnInterval = 2f;          // Tiempo entre enemigos (en segundos)
    public int totalEnemies = 5;
    public Transform[] waypoints;

    void Start()
    {
        Debug.Log("Spawn Interval actual: " + spawnInterval + " segundos");
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < totalEnemies; i++)
        {
            // Elegir un prefab aleatorio
            GameObject prefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // Instanciar el enemigo en el punto de aparición con la rotación original del prefab
            GameObject enemy = Instantiate(prefabToSpawn, spawnPoint.position, prefabToSpawn.transform.rotation);

            // Asignar waypoints si tiene movimiento
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.waypoints = waypoints;
            }

            // Esperar antes de generar el siguiente enemigo
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
