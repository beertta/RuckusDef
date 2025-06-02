using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public int index;
    public FormaType forma;

    private Renderer rend;
    private Color colorOriginal;
    public Color colorTocado = Color.green;

    [Tooltip("Objeto que debe detectar para cambiar color")]
    public GameObject objetoDetectar;  // Asignar en inspector

    // Evento para avisar que este waypoint fue activado
    public delegate void WaypointActivadoHandler(Waypoint wp);
    public event WaypointActivadoHandler OnWaypointActivado;

    private bool activado = false;

    private void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            colorOriginal = rend.material.color;
        else
            Debug.LogWarning("No se encontró Renderer en hijos de " + gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!activado && objetoDetectar != null && other.gameObject == objetoDetectar)
        {
            ActivarVisual();
            activado = true;

            // Avisar que se activó este waypoint
            OnWaypointActivado?.Invoke(this);
        }
    }

    public void ActivarVisual()
    {
        if (rend != null)
            rend.material.color = colorTocado;
    }

    public void ResetVisual()
    {
        StartCoroutine(ResetVisualAfterDelay());
    }

    private IEnumerator ResetVisualAfterDelay()
    {
        yield return new WaitForSeconds(3f);

        if (rend != null)
            rend.material.color = colorOriginal;

        activado = false;
    }

    public bool IsActivado()
    {
        return activado;
    }
}



public enum FormaType
{
    Cuadrado,
    Circulo,
    Triangulo,
    Rombo
}

