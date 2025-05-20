using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public int totalEnemies = 5;
    public Transform[] waypoints;

    private GameObject currentEnemy;

    void Start()
    {
        Debug.Log("Spawn Interval actual: " + spawnInterval + " segundos");
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < totalEnemies; i++)
        {
            // Esperar hasta que el enemigo anterior haya sido destruido
            while (currentEnemy != null)
            {
                yield return null; // Esperar 1 frame
            }

            // Esperar un poco antes de spawnear el siguiente (opcional)
            yield return new WaitForSeconds(spawnInterval);

            // Elegir un prefab aleatorio
            GameObject prefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // Instanciar el enemigo
            currentEnemy = Instantiate(prefabToSpawn, spawnPoint.position, prefabToSpawn.transform.rotation);

            // Asignar waypoints
            EnemyMovement movement = currentEnemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.waypoints = waypoints;
            }
        }
    }
}
