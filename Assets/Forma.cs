using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Forma
{
    public FormaType tipo;
    public List<Waypoint> waypoints;
    public GameObject objetoAInstanciar;
    public Transform puntoSpawn; // Donde se genera el objeto al completar la forma
    public bool completada = false;
}

