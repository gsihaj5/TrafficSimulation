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
    private NeuralNetwork vOutNN;
    public QNetwork qNetwork;

    private LSTM lstm;
    private int LSTMInputSize = 1;
    private int LSTMHiddenSize = 40;
    private int LSTMOutputSize = 1;

    private float currentInitValue = 0;
    private float prevNodeVector = 0;
    private float[] Vhat;

    private float[] AttentionWeight;


    private void Start()
    {
        // initial MLP for edgeEmbedding 
        // 2 input is for vehicle count and avg speed
        int[] networkShape = { 2, 20, 20, 2 };
        embeddingNN = new NeuralNetwork(networkShape, .01f, 100, "Relu");

        vInNN = new NeuralNetwork(new[] { 9, 20, 1 }, .01f, 100, "Relu");
        vOutNN = new NeuralNetwork(new[] { 3, 20, 1 }, .01f, 100, "Relu");
        qNetwork = new QNetwork(2, 2, 40, .01f, 0);

        //initialize LSTM
        lstm = new LSTM(LSTMInputSize, LSTMHiddenSize, LSTMOutputSize);
        double mean = 0.0;
        double standardDeviation = 5.0;

        //init attentionWeight
        AttentionWeight = new float[4];
        RandomNormal rand = new RandomNormal();
        for (int i = 0; i < AttentionWeight.Length; i++)
        {
            //AttentionWeight[i] = (float)rand.Next(-100, 100) / 100f;
            AttentionWeight[i] = (float)rand.NextNormal(mean, standardDeviation);
            //AttentionWeight[i] = 0;
        }
    }

    public int GetVehicleCount()
    {
        return topObserver.vehicleCount + bottomObserver.vehicleCount + leftObserver.vehicleCount +
               rightObserver.vehicleCount;
    }

    public float GetAverageSpeed()
    {
        return (topObserver.AverageSpeed() * topObserver.vehicleCount +
                leftObserver.AverageSpeed() * leftObserver.vehicleCount +
                bottomObserver.AverageSpeed() * bottomObserver.vehicleCount +
                rightObserver.AverageSpeed() * rightObserver.vehicleCount) / GetVehicleCount();
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

    public float getVbar()
    {
        float[] selfVhat = GetVhat();


        float sum = 0;
        foreach (Observer observer in neighboringObserver)
        {
            float calcWeight = 0;
            float[] neighborVhat = observer.GetVhat();
            float[] concatVhat =
            {
                selfVhat[0],
                selfVhat[1],
                neighborVhat[0],
                neighborVhat[1],
            };
            for (int i = 0; i < 4; i++)
            {
                calcWeight += AttentionWeight[i] * concatVhat[i];
            }

            sum += Mathf.Exp(EluActivation(calcWeight));
        }

        float vbar = 0;
        foreach (Observer observer in neighboringObserver)
        {
            float calcWeight = 0;
            float[] neighborVhat = observer.GetVhat();
            float[] concatVhat =
            {
                selfVhat[0],
                selfVhat[1],
                neighborVhat[0],
                neighborVhat[1],
            };
            for (int i = 0; i < 4; i++)
            {
                calcWeight += AttentionWeight[i] * concatVhat[i];
            }

            vbar += (Mathf.Exp(EluActivation(calcWeight)) / sum) * neighborVhat[0] * neighborVhat[1];
        }

        return vbar;
    }

    public float calculateVout()
    {
        float[] vhat = GetVhat();
        float vbar = getVbar();
        Debug.Log("Vhat");
        if (Double.IsNaN(vhat[0])) vhat[0] = 0;
        if (Double.IsNaN(vhat[1])) vhat[1] = 0;
        if (Double.IsNaN(vbar)) vbar = 0;

        float vout = vOutNN.Process(new[] { vhat[0], vhat[1], vbar })[0];
        prevNodeVector = vout;
        return vout;
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
