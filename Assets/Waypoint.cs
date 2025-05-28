using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public int index; // El orden del waypoint dentro de su forma
    public FormaType forma; // Cuadrado, Círculo, etc.
    private Renderer rend;
    private Color colorOriginal;
    public Color colorTocado = Color.green;

    private void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            colorOriginal = rend.material.color;
    }

    public void ActivarVisual()
    {
        if (rend != null)
            rend.material.color = colorTocado;
    }

    public void ResetVisual()
    {
        if (rend != null)
            rend.material.color = colorOriginal;
    }
}

public enum FormaType
{
    Cuadrado,
    Circulo,
    Triangulo,
    Rombo
}

