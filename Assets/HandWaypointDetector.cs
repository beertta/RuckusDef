using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandWaypointDetector : MonoBehaviour
{
    public FormaRecognizer recognizer;

    void Update()
    {
        if (recognizer != null)
        {
            recognizer.VerificarWaypoint(transform.position); // Posición del lápiz
        }
    }
}

