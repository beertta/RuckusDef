using System.Collections.Generic;
using UnityEngine;

public class FormaRecognizer : MonoBehaviour
{
    public List<Forma> formas;
    private Dictionary<FormaType, List<Waypoint>> progresoJugador = new Dictionary<FormaType, List<Waypoint>>();

    public float tolerancia = 0.2f; // Distancia mínima para considerar que tocó un waypoint

    void Start()
    {
        foreach (var forma in formas)
        {
            progresoJugador[forma.tipo] = new List<Waypoint>();
        }
    }

    public void VerificarWaypoint(Vector3 posicionJugador)
    {
        foreach (var forma in formas)
        {
            if (forma.completada) continue;

            foreach (var waypoint in forma.waypoints)
            {
                if (!progresoJugador[forma.tipo].Contains(waypoint))
                {
                    if (Vector3.Distance(posicionJugador, waypoint.transform.position) < tolerancia)
                    {
                        progresoJugador[forma.tipo].Add(waypoint);

                        // Cambiar color del waypoint tocado
                        waypoint.ActivarVisual();

                        Debug.Log($"Waypoint alcanzado: {forma.tipo} - {waypoint.index}");

                        // Verificar si todos han sido alcanzados
                        if (progresoJugador[forma.tipo].Count == forma.waypoints.Count)
                        {
                            forma.completada = true;
                            InstanciarObjeto(forma);
                        }

                        return;
                    }
                }
            }
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


