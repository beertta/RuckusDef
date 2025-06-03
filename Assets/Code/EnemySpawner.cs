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

    [Header("Formas en escena")]
    public GameObject formaCuadrado;
    public GameObject formaCirculo;
    public GameObject formaTriangulo;
    public GameObject formaRombo;

    [Header("Paneles de ronda")]
    public GameObject round1Panel;
    public GameObject round2Panel;
    public GameObject finalRoundPanel;
    public GameObject victoryPanel;

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

    public void StartSpawning()
    {
        StartCoroutine(RunGamePhases());
    }

    IEnumerator RunGamePhases()
    {
        yield return StartCoroutine(ActivarPanelYEsperar(round1Panel));

        List<string> round1Colors = new List<string>() { "Red", "Green", "Blue", "Yellow" };
        foreach (string color in round1Colors)
        {
            yield return EsperarHastaQueNoHayaEnemigo();
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemigo(color);
        }

        yield return EsperarHastaQueNoHayaEnemigo();

        yield return StartCoroutine(ActivarPanelYEsperar(round2Panel));

        for (int i = 0; i < 2; i++)
        {
            yield return EsperarHastaQueNoHayaEnemigo();
            yield return new WaitForSeconds(spawnInterval);

            List<string> availableColors = new List<string>(allColors);
            availableColors.Remove(lastColor);
            string selectedColor = availableColors[Random.Range(0, availableColors.Count)];
            lastColor = selectedColor;

            SpawnEnemigo(selectedColor);
        }

        yield return EsperarHastaQueNoHayaEnemigo();

        yield return StartCoroutine(ActivarPanelYEsperar(finalRoundPanel));

        float finalRoundInterval = spawnInterval * 0.5f;
        for (int i = 0; i < 10; i++)
        {
            yield return EsperarHastaQueNoHayaEnemigo();
            yield return new WaitForSeconds(finalRoundInterval);

            List<string> availableColors = new List<string>(allColors);
            availableColors.Remove(lastColor);
            string selectedColor = availableColors[Random.Range(0, availableColors.Count)];
            lastColor = selectedColor;

            SpawnEnemigo(selectedColor);
        }

        yield return EsperarHastaQueNoHayaEnemigo();

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            CanvasGroup cg = victoryPanel.GetComponent<CanvasGroup>();
            if (cg != null)
                yield return StartCoroutine(FadeCanvasGroup(cg, 0f, 1f, 0.5f));
        }

        yield return new WaitForSeconds(5f);

        if (victoryPanel != null)
        {
            CanvasGroup cg = victoryPanel.GetComponent<CanvasGroup>();
            if (cg != null)
                yield return StartCoroutine(FadeCanvasGroup(cg, 1f, 0f, 0.5f));

            victoryPanel.SetActive(false);
        }

        ReiniciarPartida();
    }

    IEnumerator ActivarPanelYEsperar(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
            CanvasGroup cg = panel.GetComponent<CanvasGroup>();
            if (cg != null)
                yield return StartCoroutine(FadeCanvasGroup(cg, 0f, 1f, 0.5f));
        }

        yield return new WaitForSeconds(4f);

        if (panel != null)
        {
            CanvasGroup cg = panel.GetComponent<CanvasGroup>();
            if (cg != null)
                yield return StartCoroutine(FadeCanvasGroup(cg, 1f, 0f, 0.5f));

            panel.SetActive(false);
        }
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }

    void SpawnEnemigo(string color)
    {
        GameObject prefab = GetRandomPrefab(color);
        Quaternion rot = CalcularRotacionInicial();
        currentEnemy = Instantiate(prefab, spawnPoint.position, rot);

        AssignComponents(currentEnemy);
        ActivarFormaPorColor(color);
    }

    void AssignComponents(GameObject enemy)
    {
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null)
            movement.waypoints = waypoints;
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
        if (finalRoundPanel != null) finalRoundPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    public void ReiniciarPartida()
    {
        StopAllCoroutines();
        DesactivarFormas();
        DesactivarPanelesDeRonda();

        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
            currentEnemy = null;
        }

        string[] tags = { "Red", "Green", "Blue", "Yellow" };
        foreach (string tag in tags)
        {
            GameObject[] enemigos = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemigo in enemigos)
                Destroy(enemigo);
        }

        lastColor = "";

        StartCoroutine(RunGamePhases());
    }
}
