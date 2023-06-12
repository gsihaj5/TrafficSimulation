using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private bool isStopped = false;
    [SerializeField] private float detectionDistance = 2f;

    [SerializeField] private GameObject CarGameObject;
    private Rigidbody2D _rigidbody2D;

    [SerializeField] private float maxSpeed = 0;
    public Vector2 direction;
    [SerializeField] private float brakePower = 5f;

    private TrafficLight _trafficLight;
    public LayerMask obstacleLayer;
    public Transform frontSensor;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = CarGameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckPermission() && !CheckFrontCar())
        {
            //accelerate
            _rigidbody2D.velocity += direction * maxSpeed * Time.deltaTime;
            _rigidbody2D.velocity = Vector3.ClampMagnitude(_rigidbody2D.velocity, maxSpeed);
        }
        else
        {
            //decelerate
            _rigidbody2D.velocity -= _rigidbody2D.velocity.normalized * brakePower * Time.deltaTime;
        }
    }

    public float getVelocity()
    {
        return _rigidbody2D.velocity.magnitude;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("TrafficCollider"))
        {
            Debug.Log("entering traffic collider");
            _trafficLight = col.transform.parent.parent.GetComponent<TrafficLight>();
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("TrafficCollider"))
        {
            _trafficLight = null;
        }
    }

    private bool CheckPermission()
    {
        if (_trafficLight == null) return true;
        if (direction.y > 0)
            return _trafficLight.GetPermission("top");
        if (direction.y < 0)
            return _trafficLight.GetPermission("bottom");
        if (direction.x < 0)
            return _trafficLight.GetPermission("left");
        if (direction.x > 0)
            return _trafficLight.GetPermission("right");
        return true;
    }

    private bool CheckFrontCar()
    {
        // Raycast in front of the car to detect obstacles
        RaycastHit2D hit = Physics2D.Raycast(
            new Vector2(frontSensor.position.x, frontSensor.position.y),
            direction,
            detectionDistance,
            obstacleLayer
        );
        if (!hit) return false;
        if (hit.collider)
        {
            Debug.Log("DETECT CAR");
            // An obstacle is detected
            return true;
        }

        return false;
    }
}