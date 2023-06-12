using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    [SerializeField] private LaneObserver topObserver;
    [SerializeField] private LaneObserver bottomObserver;
    [SerializeField] private LaneObserver leftObserver;
    [SerializeField] private LaneObserver rightObserver;

    public List<Observer> neighboringObserver;

    private bool FirstProcess = true;

    private NeuralNetwork embeddingNN;
    private NeuralNetwork vInNN;

    private LSTM lstm;
    private int LSTMInputSize = 2;
    private int LSTMHiddenSize = 10;
    private int LSTMOutputSize = 1;

    private float currentInitValue = 0;
    private float prevNodeVector = 0;
    private float[] Vhat;


    private void Start()
    {
        // initial MLP for edgeEmbedding 
        // 2 input is for vehicle count and avg speed
        int[] networkShape = { 2, 20, 20, 2 };
        embeddingNN = new NeuralNetwork(networkShape, .01f, 100, "Relu");

        vInNN = new NeuralNetwork(new[] { 9, 20, 1 }, .01f, 100, "Relu");

        //initialize LSTM
        lstm = new LSTM(LSTMInputSize, LSTMHiddenSize, LSTMOutputSize);
    }

    public int GetVehicleCount()
    {
        return topObserver.vehicleCount + bottomObserver.vehicleCount + leftObserver.vehicleCount +
               rightObserver.vehicleCount;
    }

    public List<LaneObserver> GetLaneObservers()
    {
        return new List<LaneObserver> { topObserver, leftObserver, bottomObserver, rightObserver };
    }

    public float GetEmbedNodeValue()
    {
        float sum = 0;
        sum += topObserver.GetEmbbedObservation(embeddingNN);
        sum += bottomObserver.GetEmbbedObservation(embeddingNN);
        sum += leftObserver.GetEmbbedObservation(embeddingNN);
        sum += rightObserver.GetEmbbedObservation(embeddingNN);

        return sum;
    }

    public float[] GetInputValue()
    {
        float[] topObservation = topObserver.GetObservation();
        float[] leftObservation = leftObserver.GetObservation();
        float[] bottomObservation = bottomObserver.GetObservation();
        float[] rightObservation = rightObserver.GetObservation();

        return vInNN.Process(new[]
            {
                GetEmbedNodeValue(),
                topObservation[0],
                topObservation[1],
                leftObservation[0],
                leftObservation[1],
                bottomObservation[0],
                bottomObservation[1],
                rightObservation[0],
                rightObservation[1]
            }
        );
    }

    public float[] GetVhat()
    {
        return new[]
        {
            GetInputValue()[0],
            prevNodeVector
        };
    }

    public float computeLSTM()
    {
        return lstm.Calculate(GetInputValue())[0];
    }

    private float EluActivation(float x)
    {
        if (x >= 0)
        {
            return x;
        }

        return 1 * (MathF.Exp(x) - 1);
    }
}