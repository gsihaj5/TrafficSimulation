using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] GameObject intersection_prefab;
    [SerializeField] GameObject spawner_prefab;
    // Start is called before the first frame update
    void Start()
    {
        Grids grid = new Grids(3, 3, intersection_prefab, spawner_prefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
