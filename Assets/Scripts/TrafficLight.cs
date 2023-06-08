using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    private float interval;
    private float elapsedTime;

    private bool _top = false;
    private bool _bottom = false;
    private bool _left = true;
    private bool _right = true;

    [SerializeField] private SpriteRenderer LightBottom;
    [SerializeField] private SpriteRenderer LightUp;
    [SerializeField] private SpriteRenderer LightLeft;
    [SerializeField] private SpriteRenderer LightRight;

    // Start is called before the first frame update
    void Start()
    {
        elapsedTime = 0;
        interval = 3;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        UpdateColor(LightUp, _top);
        UpdateColor(LightBottom, _bottom);
        UpdateColor(LightLeft, _left);
        UpdateColor(LightRight, _right);
        if (elapsedTime > interval)
        {
            elapsedTime -= interval;

            _top = !_top;
            _bottom = !_bottom;
            _left = !_left;
            _right = !_right;
        }
    }

    private void UpdateColor(SpriteRenderer light, bool condition)
    {
        if (condition) light.color = Color.green;
        else light.color = Color.red;
    }

    public bool GetPermission(string direction)
    {
        if ("top" == direction) return _top;
        if ("bottom" == direction) return _bottom;
        if ("left" == direction) return _left;
        if ("right" == direction) return _right;

        return false;
    }
}