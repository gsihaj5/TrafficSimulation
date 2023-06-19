using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [SerializeField] private float intervalHorizontal;
    [SerializeField] private float intervalVertical;
    private bool isHorizontal;
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
        interval = intervalHorizontal;
        isHorizontal = true;

        // initial MLP for edgeEmbedding 
        // 2 input is for vehicle count and avg speed
        int[] networkShape = { 2, 20, 20, 2 };
    }

    // Update is called once per frame
    void Update()
    {
        UpdateColor(LightUp, _top);
        UpdateColor(LightBottom, _bottom);
        UpdateColor(LightLeft, _left);
        UpdateColor(LightRight, _right);
    }

    private void UpdateForFixedInterval()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > interval)
        {
            if (isHorizontal)
            {
                interval = intervalVertical;
                isHorizontal = false;
            }
            else
            {
                interval = intervalHorizontal;
                isHorizontal = true;
            }

            elapsedTime = 0;


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

    public void RESET(){

    }

    public void doAction(int actionIndex)
    {
        if (actionIndex == 0)
        {
            _top = false;
            _bottom = false;
            _left = true;
            _right = true;
        }
        else
        {
            _top = true;
            _bottom = true;
            _left = false;
            _right = false;
        }
    }
}
