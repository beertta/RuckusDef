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
    public GameObject finalBossPrefab;

    [Header("Formas en escena (prefabs desactivados por defecto)")]
    public GameObject formaCuadrado;   // Se activa para Blue
    public GameObject formaCirculo;    // Se activa para Yellow
    public GameObject formaTriangulo;  // Se activa para Red
    public GameObject formaRombo;      // Se activa para Green

    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public Transform[] waypoints;

    private GameObject currentEnemy;
    private string lastColor = "";

    private Dictionary<string, GameObject[]> enemyGroups;
    private List<string> allColors = new List<string>() { "Red", "Green", "Blue", "Yellow" };

    void Start()
    {
        enemyGroups = new Dictionary<string, GameObject[]>()
        {
            { "Yellow", yellowEnemies },
            { "Red", redEnemies },
            { "Blue", blueEnemies },
            { "Green", greenEnemies }
        };

        DesactivarFormas();

        StartCoroutine(RunGamePhases());
    }

    IEnumerator RunGamePhases()
    {
        // ROUND 1
        List<string> round1Colors = new List<string>() { "Red", "Green", "Blue", "Yellow" };

        foreach (string color in round1Colors)
        {
            while (currentEnemy != null)
                yield return null;

            yield return new WaitForSeconds(spawnInterval);

            GameObject prefab = GetRandomPrefab(color);

            Quaternion lookRotation = Quaternion.identity;
            if (waypoints != null && waypoints.Length > 0)
            {
                Vector3 lookTarget = waypoints[0].position;
                lookRotation = Quaternion.LookRotation(lookTarget - spawnPoint.position) * Quaternion.Euler(0, 270, 0);
            }

            currentEnemy = Instantiate(prefab, spawnPoint.position, lookRotation);
            AssignWaypoints(currentEnemy);
            ActivarFormaPorColor(color);
        }

        // ROUND 2
        for (int i = 0; i < 1; i++)
        {
            while (currentEnemy != null)
                yield return null;

            yield return new WaitForSeconds(spawnInterval);

            List<string> availableColors = new List<string>(allColors);
            availableColors.Remove(lastColor);

            string selectedColor = availableColors[Random.Range(0, availableColors.Count)];
            lastColor = selectedColor;

            GameObject prefab = GetRandomPrefab(selectedColor);

            Quaternion lookRotation = Quaternion.identity;
            if (waypoints != null && waypoints.Length > 0)
            {
                Vector3 lookTarget = waypoints[0].position;
                lookRotation = Quaternion.LookRotation(lookTarget - spawnPoint.position) * Quaternion.Euler(0, 270, 0);
            }

            currentEnemy = Instantiate(prefab, spawnPoint.position, lookRotation);
            AssignWaypoints(currentEnemy);
            ActivarFormaPorColor(selectedColor);
        }

        // FINAL BOSS
        while (currentEnemy != null)
            yield return null;

        yield return new WaitForSeconds(spawnInterval);

        Quaternion bossLookRotation = Quaternion.identity;
        if (waypoints != null && waypoints.Length > 0)
        {
            Vector3 lookTarget = waypoints[0].position;
            bossLookRotation = Quaternion.LookRotation(lookTarget - spawnPoint.position) * Quaternion.Euler(0, 270, 0);
        }

        currentEnemy = Instantiate(finalBossPrefab, spawnPoint.position, bossLookRotation);
        AssignWaypoints(currentEnemy);

        FinalBoss bossScript = currentEnemy.GetComponent<FinalBoss>();
        if (bossScript != null)
        {
            bossScript.spawner = this;
            bossScript.IniciarBoss();
            Debug.Log("FinalBoss instanciado y secuencia iniciada.");
        }
    }

    GameObject GetRandomPrefab(string color)
    {
        GameObject[] group = enemyGroups[color];
        return group[Random.Range(0, group.Length)];
    }

    void AssignWaypoints(GameObject enemy)
    {
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null)
            movement.waypoints = waypoints;
    }

    public void ActivarFormaPorColor(string color)
    {
        DesactivarFormas();

        switch (color)
        {
            case "Blue":
                if (formaCuadrado != null) formaCuadrado.SetActive(true);
                break;
            case "Yellow":
                if (formaCirculo != null) formaCirculo.SetActive(true);
                break;
            case "Red":
                if (formaTriangulo != null) formaTriangulo.SetActive(true);
                break;
            case "Green":
                if (formaRombo != null) formaRombo.SetActive(true);
                break;
        }
    }

    public void DesactivarFormas()
    {
        if (formaCuadrado != null) formaCuadrado.SetActive(false);
        if (formaCirculo != null) formaCirculo.SetActive(false);
        if (formaTriangulo != null) formaTriangulo.SetActive(false);
        if (formaRombo != null) formaRombo.SetActive(false);
    }

    public void OnEnemyDestroyed()
    {
        currentEnemy = null;
    }
}
