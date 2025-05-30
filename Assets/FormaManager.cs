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

            // Reiniciar los waypoints para volver a usar la forma en el futuro
            foreach (var wp in forma.waypoints)
            {
                wp.ResetVisual();
            }

            // Marcar como incompleta para volver a activarla en el futuro
            forma.completada = false;

            // Desactivar el prefab de forma en pantalla
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
