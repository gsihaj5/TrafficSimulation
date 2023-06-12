using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] private GameObject intersection_prefab;
    [SerializeField] private GameObject spawner_prefab;

    private Grids grid;
    private float intervalReport = 5f;
    private float elapsedTime = 0;
    private NeuralNetwork QEstimator;

    void Start()
    {
        //create environment
        grid = new Grids(3, 5, intersection_prefab, spawner_prefab);

        //initialize Q-Network and Target Q-Network
        int inputSize = 10;
        int outputSize = 4;
        QNetwork qNetwork = new QNetwork(inputSize, outputSize);
        QNetwork targetQNetwork = new QNetwork(inputSize, outputSize);
        targetQNetwork.weights = (float[,])qNetwork.weights.Clone();
        targetQNetwork.biases = (float[])qNetwork.biases.Clone();

        QEstimator = new NeuralNetwork(new[] { 2, 20, 2 }, .01f, 100, "Relu");
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > intervalReport)
        {
            CalculateSTMARL();
            elapsedTime -= intervalReport;
        }
    }

    private void CalculateSTMARL()
    {
        Debug.Log("Calculatting STMARL");
        foreach (TrafficLight traffic_light in grid.trafficLights)
        {
            Observer observer = traffic_light.GetComponentInChildren<Observer>();
            observer.computeLSTM();
            float vin = observer.GetInputValue()[0];
            float vout = observer.calculateVout();
            float[] estimatedQValue = QEstimator.Process(new[] { vin, vout });
            Debug.Log("estimated value");
            Debug.Log(estimatedQValue);
        }
    }
}