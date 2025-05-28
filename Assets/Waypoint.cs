using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public int index; // El orden del waypoint dentro de su forma
    public FormaType forma; // Cuadrado, Círculo, etc.
}

public enum FormaType
{
    Cuadrado,
    Circulo,
    Triangulo,
    Rombo
}

