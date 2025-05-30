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

        // Aquí puedes añadir la lógica para que el enemigo "sepa" qué forma tiene
        // Por ejemplo, cambiar color, activar UI, etc.
    }

    // Opcional: Método para obtener la forma asignada
    public FormaType GetForma()
    {
        return formaAsignada;
    }
}

