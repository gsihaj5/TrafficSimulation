using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grids
{
    private Dictionary<int, List<int>> adjacencyList;
    public List<TrafficLight> trafficLights;
    private int row_size, col_size;
    private GameObject intersection_light_prefab;
    private GameObject spawner_prefab;

    // Start is called before the first frame update
    public Grids(int row_size, int col_size, GameObject intersection_light, GameObject spawner_prefab)
    {
        this.row_size = row_size;
        this.col_size = col_size;
        this.intersection_light_prefab = intersection_light;
        this.spawner_prefab = spawner_prefab;
        adjacencyList = new Dictionary<int, List<int>>();
        trafficLights = new List<TrafficLight>();

        InitNodes();
        InitSpawners();
        InitEdgeNodes();

        int index = 0;
        foreach (TrafficLight trafficLight in trafficLights)
        {
            Observer observer = trafficLight.GetComponentInChildren<Observer>();
            observer.neighboringObserver = GetNeighbors(index);
            index++;
        }
    }

    private void InitNodes()
    {
        for (int y = 0; y < row_size; y++)
        {
            for (int x = 0; x < col_size; x++)
            {
                AddNode(y + x);
                GameObject traffic_gameobject = Object.Instantiate(intersection_light_prefab,
                    new Vector2(15 * x, 15 * y), Quaternion.identity);
                trafficLights.Add(traffic_gameobject.GetComponent<TrafficLight>());
            }
        }
    }

    private void InitSpawners()
    {
        for (int i = 0; i < col_size * 2; i++)
        {
            if (i < col_size) //bottom to top spawner
                InstantiateSpawner(new Vector2(15 * i - .5f, -9), new Vector2(0, 1));
            else //top to bottom spawner
                InstantiateSpawner(new Vector2(15 * (i - col_size) + .5f, (row_size - 1) * 15 + 9), new Vector2(0, -1));
        }

        for (int i = 0; i < row_size * 2; i++)
        {
            if (i < row_size) //left to right spawner
                InstantiateSpawner(new Vector2(-9, 15 * i + .5f), new Vector2(1, 0));
            else //right to left
                InstantiateSpawner(new Vector2((col_size - 1) * 15 + 9, 15 * (i - row_size) - .5f), new Vector2(-1, 0));
        }
    }

    private void InstantiateSpawner(Vector2 position, Vector2 direction)
    {
        GameObject spawner_gameobject = Object.Instantiate(spawner_prefab, position, Quaternion.identity);
        Spawner spawner = spawner_gameobject.GetComponent<Spawner>();
        spawner.direction = direction;
    }

    private void InitEdgeNodes()
    {
        for (int i = 0; i < row_size; i++)
        {
            for (int j = 0; j < col_size; j++)
            {
                int currentNode = i * col_size + j;

                if (i > 0)
                {
                    int topNode = (i - 1) * col_size + j;
                    AddEdge(currentNode, topNode);
                }

                if (i < row_size - 1)
                {
                    int bottomNode = (i + 1) * col_size + j;
                    AddEdge(currentNode, bottomNode);
                }

                if (j > 0)
                {
                    int leftNode = i * col_size + (j - 1);
                    AddEdge(currentNode, leftNode);
                }

                if (j < col_size - 1)
                {
                    int rightNode = i * col_size + (j + 1);
                    AddEdge(currentNode, rightNode);
                }
            }
        }
    }

    public void AddNode(int node)
    {
        if (!adjacencyList.ContainsKey(node))
        {
            adjacencyList[node] = new List<int>();
        }
    }

    public void AddEdge(int node1, int node2)
    {
        if (adjacencyList.ContainsKey(node1) && adjacencyList.ContainsKey(node2))
        {
            adjacencyList[node1].Add(node2);
            adjacencyList[node2].Add(node1);
        }
    }

    public List<Observer> GetNeighbors(int node)
    {
        if (adjacencyList.ContainsKey(node))
        {
            List<int> neighborIndexes = adjacencyList[node];
            List<Observer> neighborTrafficLights = new();

            foreach (int neighborIndex in neighborIndexes)
            {
                neighborTrafficLights.Add(trafficLights[neighborIndex].GetComponentInChildren<Observer>());
            }

            return neighborTrafficLights;
        }

        return new List<Observer>();
    }

    public void printAllTrafficParameter()
    {
        Debug.Log("Report");
        foreach (TrafficLight traffic_light in trafficLights)
        {
            Observer observer = traffic_light.GetComponentInChildren<Observer>();
            Debug.Log("vehicle count : " + observer.GetVehicleCount());
        }
    }
}