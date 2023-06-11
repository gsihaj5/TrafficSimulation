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

    void Start()
    {
        grid = new Grids(3, 5, intersection_prefab, spawner_prefab);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime > intervalReport)
        {
            grid.printAllTrafficParameter();
        }
    }
}
