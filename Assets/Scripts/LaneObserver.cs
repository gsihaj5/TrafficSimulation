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
            vehicleCount++;
            _cars.Add(col.GetComponent<Car>());
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Car"))
        {
            vehicleCount--;
            _cars.Remove(col.GetComponent<Car>());
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
        if (vehicleCount == 0) return 0;
        float result = nn.Process(new[] { vehicleCount, AverageSpeed() })[0];
        return result;
    }

    public float[] GetObservation()
    {
        if (vehicleCount == 0) return new float[] { 0, 0 };
        return new[] { vehicleCount, AverageSpeed() };
    }
}