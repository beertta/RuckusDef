using System.Collections.Generic;
using UnityEngine;

public class FormaRecognizer : MonoBehaviour
{
    public List<Forma> formas;

    void Start()
    {
        foreach (var forma in formas)
        {
            forma.completada = false;
            foreach (var waypoint in forma.waypoints)
            {
                waypoint.OnWaypointActivado += (wp) => OnWaypointActivado(forma);
                waypoint.ResetVisual();
            }
        }
    }

    private void OnWaypointActivado(Forma forma)
    {
        if (forma.completada) return;

        // Verificar si todos los waypoints est�n activados (esfera verde)
        bool todosActivados = true;
        foreach (var wp in forma.waypoints)
        {
            if (!wpIsActivado(wp))
            {
                todosActivados = false;
                break;
            }
        }

        if (todosActivados)
        {
            forma.completada = true;
            InstanciarObjeto(forma);
        }
    }

    // M�todo para saber si un waypoint est� activado (puedes hacerlo p�blico en Waypoint para mejor acceso)
    private bool wpIsActivado(Waypoint wp)
    {
        // Acceso directo a bool activado no permitido? Si no, puedes crear getter en Waypoint
        return wp != null && wpIsActivadoGetter(wp);
    }

    private bool wpIsActivadoGetter(Waypoint wp)
    {
        // Aqu� asumiremos que Waypoint tiene un m�todo p�blico IsActivado
        return wp.IsActivado();
    }

    void InstanciarObjeto(Forma forma)
    {
        if (forma.objetoAInstanciar != null && forma.puntoSpawn != null)
        {
            Instantiate(forma.objetoAInstanciar, forma.puntoSpawn.position, Quaternion.identity);
            Debug.Log($"�Forma {forma.tipo} completada! Objeto instanciado.");
        }
    }
}
