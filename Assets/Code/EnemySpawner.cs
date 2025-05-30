using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyFormMapping
{
    public string colorName; // Ejemplo: "Yellow", "Red", "Blue", "Green"
    public FormaType forma;  // Tipo de forma asignada a ese color
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs por color (3 por grupo)")]
    public GameObject[] yellowEnemies;
    public GameObject[] redEnemies;
    public GameObject[] blueEnemies;
    public GameObject[] greenEnemies;

    [Header("Mapeo Color - Forma")]
    public List<EnemyFormMapping> enemyFormMappings;

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
            while (currentEnemy != null)
                yield return null;

            yield return new WaitForSeconds(spawnInterval);

            List<string> availableColors = new List<string>(enemyGroups.Keys);
            availableColors.Remove(lastColor);

            string selectedColor = availableColors[Random.Range(0, availableColors.Count)];
            lastColor = selectedColor;

            GameObject[] selectedGroup = enemyGroups[selectedColor];
            GameObject prefabToSpawn = selectedGroup[Random.Range(0, selectedGroup.Length)];

            currentEnemy = Instantiate(prefabToSpawn, spawnPoint.position, prefabToSpawn.transform.rotation);

            // Asignar waypoints
            EnemyMovement movement = currentEnemy.GetComponent<EnemyMovement>();
            if (movement != null)
                movement.waypoints = waypoints;

            // Buscar la forma asignada para el color seleccionado
            FormaType formaAsignada = FormaType.Cuadrado; // Valor por defecto
            foreach (var mapping in enemyFormMappings)
            {
                if (mapping.colorName == selectedColor)
                {
                    formaAsignada = mapping.forma;
                    break;
                }
            }

            // Pasar la forma al enemigo (si tiene el componente EnemyForma)
            EnemyForma enemyForma = currentEnemy.GetComponent<EnemyForma>();
            if (enemyForma != null)
            {
                enemyForma.SetForma(formaAsignada);
            }
        }
    }
}
