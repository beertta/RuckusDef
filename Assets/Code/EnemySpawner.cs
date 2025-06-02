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

    [Header("Formas en escena")]
    public GameObject formaCuadrado;
    public GameObject formaCirculo;
    public GameObject formaTriangulo;
    public GameObject formaRombo;

    [Header("Paneles de ronda")]
    public GameObject round1Panel;
    public GameObject round2Panel;
    public GameObject finalBossPanel;

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
        DesactivarPanelesDeRonda();
    }

    // Llamar desde VRPanelManager para empezar las rondas
    public void StartSpawning()
    {
        StartCoroutine(RunGamePhases());
    }

    IEnumerator RunGamePhases()
    {
        // ROUND 1
        yield return StartCoroutine(ActivarPanelYEsperar(round1Panel));

        List<string> round1Colors = new List<string>() { "Red", "Green", "Blue", "Yellow" };
        foreach (string color in round1Colors)
        {
            yield return EsperarHastaQueNoHayaEnemigo();
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemigo(color);
        }

        // ROUND 2
        yield return StartCoroutine(ActivarPanelYEsperar(round2Panel));

        for (int i = 0; i < 7; i++) // Cambia la cantidad de enemigos para round 2 si quieres
        {
            yield return EsperarHastaQueNoHayaEnemigo();
            yield return new WaitForSeconds(spawnInterval);

            List<string> availableColors = new List<string>(allColors);
            availableColors.Remove(lastColor);
            string selectedColor = availableColors[Random.Range(0, availableColors.Count)];
            lastColor = selectedColor;

            SpawnEnemigo(selectedColor);
        }

        // FINAL BOSS
        yield return StartCoroutine(ActivarPanelYEsperar(finalBossPanel));

        yield return EsperarHastaQueNoHayaEnemigo();
        yield return new WaitForSeconds(spawnInterval);

        SpawnFinalBoss();
    }

    IEnumerator ActivarPanelYEsperar(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(true);

        yield return new WaitForSeconds(2f);

        if (panel != null)
            panel.SetActive(false);
    }

    void SpawnEnemigo(string color)
    {
        GameObject prefab = GetRandomPrefab(color);
        Quaternion rot = CalcularRotacionInicial();
        currentEnemy = Instantiate(prefab, spawnPoint.position, rot);
        AssignWaypoints(currentEnemy);
        ActivarFormaPorColor(color);
    }

    void SpawnFinalBoss()
    {
        Quaternion rot = CalcularRotacionInicial();
        currentEnemy = Instantiate(finalBossPrefab, spawnPoint.position, rot);
        AssignWaypoints(currentEnemy);

        FinalBoss bossScript = currentEnemy.GetComponent<FinalBoss>();
        if (bossScript != null)
        {
            bossScript.spawner = this;
            bossScript.IniciarBoss();
        }
    }

    GameObject GetRandomPrefab(string color)
    {
        GameObject[] group = enemyGroups[color];
        return group[Random.Range(0, group.Length)];
    }

    Quaternion CalcularRotacionInicial()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            Vector3 target = waypoints[0].position;
            return Quaternion.LookRotation(target - spawnPoint.position) * Quaternion.Euler(0, 270, 0);
        }
        return Quaternion.identity;
    }

    IEnumerator EsperarHastaQueNoHayaEnemigo()
    {
        while (currentEnemy != null)
            yield return null;
    }

    public void OnEnemyDestroyed()
    {
        currentEnemy = null;
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
            case "Blue": formaCuadrado?.SetActive(true); break;
            case "Yellow": formaCirculo?.SetActive(true); break;
            case "Red": formaTriangulo?.SetActive(true); break;
            case "Green": formaRombo?.SetActive(true); break;
        }
    }

    public void DesactivarFormas()
    {
        formaCuadrado?.SetActive(false);
        formaCirculo?.SetActive(false);
        formaTriangulo?.SetActive(false);
        formaRombo?.SetActive(false);
    }

    void DesactivarPanelesDeRonda()
    {
        if (round1Panel != null) round1Panel.SetActive(false);
        if (round2Panel != null) round2Panel.SetActive(false);
        if (finalBossPanel != null) finalBossPanel.SetActive(false);
    }
}
