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

    [Header("Formas en escena (prefabs desactivados por defecto)")]
    public GameObject formaCuadrado;   // Se activa para Blue
    public GameObject formaCirculo;    // Se activa para Yellow
    public GameObject formaTriangulo;  // Se activa para Red
    public GameObject formaRombo;      // Se activa para Green

    public Transform spawnPoint;
    public float spawnInterval = 2f;
    public int totalEnemies = 5;
    public Transform[] waypoints;

    private GameObject currentEnemy;
    private string lastColor = "";

    private Dictionary<string, GameObject[]> enemyGroups;

    void Start()
    {
        enemyGroups = new Dictionary<string, GameObject[]>()
        {
            { "Yellow", yellowEnemies },
            { "Red", redEnemies },
            { "Blue", blueEnemies },
            { "Green", greenEnemies }
        };

        // Asegurarse que todas las formas estén desactivadas al inicio
        if (formaCuadrado != null) formaCuadrado.SetActive(false);
        if (formaCirculo != null) formaCirculo.SetActive(false);
        if (formaTriangulo != null) formaTriangulo.SetActive(false);
        if (formaRombo != null) formaRombo.SetActive(false);

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

            // Asignar waypoints al enemigo
            EnemyMovement movement = currentEnemy.GetComponent<EnemyMovement>();
            if (movement != null)
                movement.waypoints = waypoints;

            // Activar la forma correspondiente según color
            ActivarFormaPorColor(selectedColor);
        }
    }

    private void ActivarFormaPorColor(string color)
    {
        // Primero desactivar todas
        if (formaCuadrado != null) formaCuadrado.SetActive(false);
        if (formaCirculo != null) formaCirculo.SetActive(false);
        if (formaTriangulo != null) formaTriangulo.SetActive(false);
        if (formaRombo != null) formaRombo.SetActive(false);

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

    // Método público para desactivar todas las formas (llámalo cuando se instancie el arma)
    public void DesactivarFormas()
    {
        if (formaCuadrado != null) formaCuadrado.SetActive(false);
        if (formaCirculo != null) formaCirculo.SetActive(false);
        if (formaTriangulo != null) formaTriangulo.SetActive(false);
        if (formaRombo != null) formaRombo.SetActive(false);
    }
}
