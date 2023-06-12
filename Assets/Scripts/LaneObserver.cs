using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneObserver : MonoBehaviour
{
    public int vehicleCount = 0;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Car"))
        {
            Debug.Log("trigger small");
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