using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private bool _isAccelerating;
    private bool _collidingWithCar;

    [SerializeField] private GameObject CarGameObject;
    private Rigidbody2D _rigidbody2D;

    [SerializeField] private float maxSpeed = 0;
    public Vector2 direction;
    [SerializeField] private float acceleration = 1;
    [SerializeField] private float deceleration = 2.5f;

    private TrafficLight _trafficLight;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = CarGameObject.GetComponent<Rigidbody2D>();
        _isAccelerating = true;
        _collidingWithCar = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isAccelerating)
        {
            if (_rigidbody2D.velocity.magnitude < maxSpeed)
            {
                float deltaSpeed = (acceleration * Time.deltaTime);
                _rigidbody2D.velocity += new Vector2(direction.x * deltaSpeed, direction.y * deltaSpeed);
            }
        }
        else
        {
            if (_trafficLight != null) CheckPermission();
            if (_rigidbody2D.velocity.magnitude > 0.2f)
            {
                float deltaSpeed = (deceleration * Time.deltaTime);

                _rigidbody2D.velocity -= new Vector2(direction.x * deltaSpeed, direction.y * deltaSpeed);
            }
            else
            {
                _rigidbody2D.velocity = new Vector2();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("TrafficCollider"))
        {
            if (!_collidingWithCar)
            {
                _trafficLight = col.transform.parent.GetComponent<TrafficLight>();
                CheckPermission();
            }
            else
            {
                _isAccelerating = false;
            }
        }
        if(col.CompareTag("FrontCar")) Debug.Log("BABLAS");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Car"))
        {
            _isAccelerating = false;
            _collidingWithCar = true;
            Debug.Log("collide with car should be decelerating");
        }
    }


    private void CheckPermission()
    {
        if (direction.y > 0)
            _isAccelerating = _trafficLight.GetPermission("top");
        if (direction.y < 0)
            _isAccelerating = _trafficLight.GetPermission("bottom");
        if (direction.x < 0)
            _isAccelerating = _trafficLight.GetPermission("left");
        if (direction.x > 0)
            _isAccelerating = _trafficLight.GetPermission("right");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Car"))
        {
            _isAccelerating = true;
            _collidingWithCar = false;
        }
    }
}