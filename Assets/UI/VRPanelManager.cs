using System.Collections;
using TMPro;
using UnityEngine;

public class VRPanelManager : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;

    public TextMeshProUGUI countdownText1;
    public TextMeshProUGUI countdownText2;
    public TextMeshProUGUI countdownText3;

    public EnemySpawner enemySpawner; // Asignar desde el inspector

    void Start()
    {
        StartCoroutine(PanelSequence());
    }

    IEnumerator PanelSequence()
    {
        // Panel 1: 5 segundos
        panel1.SetActive(true);
        panel2.SetActive(false);
        panel3.SetActive(false);
        yield return StartCoroutine(Countdown(5, countdownText1));
        panel1.SetActive(false);

        // Panel 2: 5 segundos
        panel2.SetActive(true);
        yield return StartCoroutine(Countdown(5, countdownText2));
        panel2.SetActive(false);

        // Panel 3: 10 segundos
        panel3.SetActive(true);
        yield return StartCoroutine(Countdown(10, countdownText3));
        panel3.SetActive(false);

        // Iniciar el juego
        StartGame();
    }

    IEnumerator Countdown(int seconds, TextMeshProUGUI textElement)
    {
        for (int i = seconds; i > 0; i--)
        {
            textElement.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        textElement.text = ""; // Limpia al final si quieres
    }

    void StartGame()
    {
        Debug.Log("¡Empieza el juego!");

        // Iniciar la generación de enemigos
        if (enemySpawner != null)
        {
            StartCoroutine(enemySpawner.RunGamePhases());
        }
        else
        {
            Debug.LogError("EnemySpawner no está asignado en el Inspector.");
        }

        // Aquí puedes añadir lógica adicional para iniciar el juego en VR
    }
}
