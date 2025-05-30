using UnityEngine;

public class IntroUIManager : MonoBehaviour
{
    [Header("Paneles UI")]
    public GameObject canvasRoot;         // Canvas raíz que contiene todos los paneles
    public GameObject panelStart;
    public GameObject panelMecanica;
    public GameObject panelInteraccion;

    [Header("Lógica del juego")]
    public GameObject gameplayRoot;       // Objeto principal del juego (puedes ocultarlo hasta que empiece)

    void Start()
    {
        // Mostrar solo el panel de inicio al comenzar
        panelStart.SetActive(true);
        panelMecanica.SetActive(false);
        panelInteraccion.SetActive(false);
        gameplayRoot.SetActive(false); // El juego se desactiva al principio
    }

    // Llamado por el botón "Start"
    public void OnStartPressed()
    {
        panelStart.SetActive(false);
        panelMecanica.SetActive(true);
    }

    // Llamado por el botón "Flecha"
    public void OnMecanicaNextPressed()
    {
        panelMecanica.SetActive(false);
        panelInteraccion.SetActive(true);
    }

    // Llamado por el botón "X" (cerrar)
    public void OnCloseIntroPressed()
    {
        canvasRoot.SetActive(false);        // Oculta todo el UI de introducción
        gameplayRoot.SetActive(true);       // Activa el juego
        // Aquí puedes llamar a un método que inicie el juego realmente si lo deseas
    }
}
