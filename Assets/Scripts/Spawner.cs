using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private float interval;
    [SerializeField] private int carCount;
    [SerializeField] public Vector2 direction;

    private int spawnedCar = 0;

    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        elapsedTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > interval && spawnedCar < carCount)
        {
            print("spawn");
            elapsedTime -= interval;
            Spawn();
            spawnedCar++;
        }
    }

    private void Spawn()
    {
        Object newCar = Instantiate(
            carPrefab,
            new Vector3(
                transform.position.x, 
                transform.position.y, 
                -5f
            ),
            new());
        if (direction.x == -1)
        {
            newCar.GetComponent<Transform>().Rotate(0, 0, 90);
        }

        if (direction.x == 1)
        {
            newCar.GetComponent<Transform>().Rotate(0, 0, -90);
        }

        if (direction.y == -1)
        {
            newCar.GetComponent<Transform>().Rotate(0, 0, -180);
        }

        Car carScript = newCar.GetComponentInChildren<Car>();
        carScript.direction = direction;
    }
}