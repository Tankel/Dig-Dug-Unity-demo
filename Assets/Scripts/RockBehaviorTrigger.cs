using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehaviorTrigger : MonoBehaviour
{
    public GameObject rockObject; // Referencia al objeto de la roca
    private bool hasTriggered = false; // Variable para controlar si el evento ha sido activado

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered) // Asegúrate de que el evento solo ocurra si no se ha activado antes
        {
            Animator rockAnimator = rockObject.GetComponent<Animator>();
            if (rockAnimator != null)
            {
                rockAnimator.SetBool("isTouched", true);
                rockObject.SendMessage("RockTouched", 0.83f, SendMessageOptions.RequireReceiver);
                hasTriggered = true; // Marca el evento como activado
            }
            else
            {
                Debug.LogWarning("El objeto de la roca no tiene un componente Animator.");
            }
        }
    }
}
