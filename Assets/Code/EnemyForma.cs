using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyForma : MonoBehaviour
{
    private FormaType formaAsignada;

    public void SetForma(FormaType forma)
    {
        formaAsignada = forma;
        Debug.Log($"Forma asignada al enemigo: {formaAsignada}");

        // Aqu� puedes a�adir la l�gica para que el enemigo "sepa" qu� forma tiene
        // Por ejemplo, cambiar color, activar UI, etc.
    }

    // Opcional: M�todo para obtener la forma asignada
    public FormaType GetForma()
    {
        return formaAsignada;
    }
}

