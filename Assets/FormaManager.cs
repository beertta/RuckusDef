using System.Collections.Generic;
using UnityEngine;

public class FormaRecognizer : MonoBehaviour
{
    public List<Forma> formas;
    public EnemySpawner enemySpawner;  // Asignar en inspector

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

        bool todosActivados = true;
        foreach (var wp in forma.waypoints)
        {
            if (!wp.IsActivado())
            {
                todosActivados = false;
                break;
            }
        }

        if (todosActivados)
        {
            forma.completada = true;
            InstanciarObjeto(forma);

            // Desactivar las formas en pantalla cuando se complete una
            if (enemySpawner != null)
                enemySpawner.DesactivarFormas();
        }
    }

    void InstanciarObjeto(Forma forma)
    {
        if (forma.objetoAInstanciar != null && forma.puntoSpawn != null)
        {
            Instantiate(forma.objetoAInstanciar, forma.puntoSpawn.position, Quaternion.identity);
            Debug.Log($"¡Forma {forma.tipo} completada! Objeto instanciado.");
        }
    }
}
