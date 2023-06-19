using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField][Range(1f, 5)] private float timeScale;
    [SerializeField] private GameObject intersection_prefab;
    [SerializeField] private GameObject spawner_prefab;
    [SerializeField] private ExitCollider _exitCollider;

    private Grids grid;
    private float intervalReport = .1f;
    private float elapsedTime = 0;

    private float evaluationTime = 25f;
    private float elapsedEvaluation = 0;

    private float elapsedReset = 0;
    private float resetTimeout = 5f;

    private float episodeElapsed = 0;
    private float episodeTimeOut = 45f;

    void Start()
    {
        //create environment
        grid = new Grids(3, 5, intersection_prefab, spawner_prefab);

        //initialize Q-Network and Target Q-Network
    }

    // Update is called once per frame
    void Update()
    {
        episodeElapsed+= Time.deltaTime;

        if (elapsedReset > resetTimeout)
        {

            GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
            int carLength = cars.Length;

            if (carLength <= 0 || episodeElapsed > episodeTimeOut)
            {
                grid.RESET();

                for(int i = 0; i < carLength; i ++){
                    GameObject car = cars[i];
                    Destroy(car.gameObject);
                }

                episodeElapsed = 0;
                elapsedReset = 0;
            }
        }else{
            elapsedReset+=Time.deltaTime;
        }


        Time.timeScale = timeScale;
        elapsedTime += Time.deltaTime;
        elapsedEvaluation += Time.deltaTime;
        if (elapsedTime > intervalReport)
        {
            CalculateSTMARL();
            elapsedTime -= intervalReport;
        }

        if (elapsedEvaluation > evaluationTime)
        {
            elapsedEvaluation = 0;
            Debug.Log("passed car after 25s");
            Debug.Log(ExitCollider.successCar);
        }
    }

    private void CalculateSTMARL()
    {
        Debug.Log("Calculatting STMARL");
        foreach (TrafficLight traffic_light in grid.trafficLights)
        {
            Observer observer = traffic_light.GetComponentInChildren<Observer>();

            float vin = observer.GetInputValue()[0];
            float vout = observer.computeLSTM(observer.calculateRelationReasoning());

            observer.qNetwork.calculateReward(observer); // estimated reward 
            float action = observer.qNetwork.getAction(vin, vout);

            observer.Train();

            traffic_light.doAction((int)action);
        }
    }
}
