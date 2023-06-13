using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] private GameObject intersection_prefab;
    [SerializeField] private GameObject spawner_prefab;

    private Grids grid;
    private float intervalReport = 1f;
    private float elapsedTime = 0;

    private QNetwork _qNetwork;

    void Start()
    {
        //create environment
        grid = new Grids(3, 5, intersection_prefab, spawner_prefab);

        //initialize Q-Network and Target Q-Network
        _qNetwork = new QNetwork(2, 2, 40, .01f, 0);
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
            _qNetwork.calculateReward(observer);
            float action = _qNetwork.getAction(vin, vout);

            traffic_light.doAction((int)action);
        }
    }
}