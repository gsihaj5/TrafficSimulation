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

    private NeuralNetwork nn;

    private void Start()
    {
        // initial MLP for edgeEmbedding 
        // 2 input is for vehicle count and avg speed
        int[] networkShape = { 2, 20, 20, 2 };
        nn = new NeuralNetwork(networkShape, .01f, 100, "Relu");
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

    public float GetNodeValue()
    {
        float sum = 0;
        sum += topObserver.GetEmbbedObservation(nn);
        sum += bottomObserver.GetEmbbedObservation(nn);
        sum += leftObserver.GetEmbbedObservation(nn);
        sum += rightObserver.GetEmbbedObservation(nn);

        return sum;
    }
}