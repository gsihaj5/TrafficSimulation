using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneObserver : MonoBehaviour
{
    public int vehicleCount = 0;
    private List<Car> _cars = new();

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

    public float AverageSpeed()
    {
        float speeds = 0;

        foreach (Car car in _cars)
        {
            speeds += car.getVelocity();
        }

        return speeds / vehicleCount;
    }

    public float GetEmbbedObservation(NeuralNetwork nn)
    {
        return nn.Process(new[] { vehicleCount, AverageSpeed() })[0];
    }
}