using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    public int vehicleCount = 0;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Car"))
        {
            vehicleCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Car"))
        {
            vehicleCount--;
        }
    }
}
