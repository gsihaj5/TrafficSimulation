using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    [SerializeField] private LaneObserver topObserver;
    [SerializeField] private LaneObserver bottomObserver;
    [SerializeField] private LaneObserver leftObserver;
    [SerializeField] private LaneObserver rightObserver;

    private List<Car> _cars = new();

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Car"))
        {
            Debug.Log("trigger large");
            _cars.Add(col.gameObject.GetComponent<Car>());
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Car"))
        {
            _cars.Remove(col.gameObject.GetComponent<Car>());
        }
    }

    public int GetVehicleCount()
    {
        return topObserver.vehicleCount + bottomObserver.vehicleCount + leftObserver.vehicleCount +
               rightObserver.vehicleCount;
    }

    public List<LaneObserver> GetLaneObservers()
    {
        return new List<LaneObserver> { topObserver, leftObserver, bottomObserver, rightObserver };
    }

    public float AverageSpeed()
    {
        int vehicleCount = GetVehicleCount();
        float speeds = 0;

        foreach (Car car in _cars)
        {
            speeds += car.getVelocity();
        }

        return speeds / vehicleCount;
    }
}