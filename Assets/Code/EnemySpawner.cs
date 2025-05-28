using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs por color (3 por grupo)")]
    public GameObject[] yellowEnemies;
    public GameObject[] redEnemies;
    public GameObject[] blueEnemies;
    public GameObject[] greenEnemies;

    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public int totalEnemies = 5;
    public Transform[] waypoints;

    private GameObject currentEnemy;
    private string lastColor = "";

    private Dictionary<string, GameObject[]> enemyGroups;

    void Start()
    {
        Debug.Log("Spawn Interval actual: " + spawnInterval + " segundos");

        // Agrupar prefabs por color
        enemyGroups = new Dictionary<string, GameObject[]>()
        {
            { "Yellow", yellowEnemies },
            { "Red", redEnemies },
            { "Blue", blueEnemies },
            { "Green", greenEnemies }
        };

        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < totalEnemies; i++)
        {
            // Esperar hasta que el enemigo anterior haya sido destruido
            while (currentEnemy != null)
                yield return null;

            yield return new WaitForSeconds(spawnInterval);

            // Obtener una lista de colores disponibles, excluyendo el último
            List<string> availableColors = new List<string>(enemyGroups.Keys);
            availableColors.Remove(lastColor);

            // Elegir un color aleatorio distinto al anterior
            string selectedColor = availableColors[Random.Range(0, availableColors.Count)];
            lastColor = selectedColor;

            // Elegir un prefab aleatorio dentro del color
            GameObject[] selectedGroup = enemyGroups[selectedColor];
            GameObject prefabToSpawn = selectedGroup[Random.Range(0, selectedGroup.Length)];

            // Instanciar el enemigo
            currentEnemy = Instantiate(prefabToSpawn, spawnPoint.position, prefabToSpawn.transform.rotation);

            // Asignar waypoints
            EnemyMovement movement = currentEnemy.GetComponent<EnemyMovement>();
            if (movement != null)
                movement.waypoints = waypoints;
        }
    }
}
