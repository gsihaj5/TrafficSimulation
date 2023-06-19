using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

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
    public QNetwork targetQNetwork;

    private LSTM lstm;
    private int LSTMInputSize = 1;
    private int LSTMHiddenSize = 40;
    private int LSTMOutputSize = 1;
    private int updateTargetCounter = 0;
    private int updateTargetBatch = 25;

    private float currentInitValue = 0;
    private float prevRelationReasoning = 0;
    private float[] Vhat;

    private float[] AttentionWeight;

    private void Start()
    {
        // initial MLP for edgeEmbedding 
        // 2 input is for vehicle count and avg speed
        embeddingNN = new NeuralNetwork(new[] { 20, 20, 20, 1 }, .01f, 100, "Relu");

        vInNN = new NeuralNetwork(new[] { 9, 20, 1 }, .01f, 100, "Relu");
        vOutNN = new NeuralNetwork(new[] { 3, 20, 1 }, .01f, 100, "Relu");
        qNetwork = new QNetwork(2, 2, 40, .01f, 0.99f);
        targetQNetwork = new QNetwork(2, 2, 40, .01f, 0);
        targetQNetwork.copyWeights(qNetwork);

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
        float[] observations = {
            //top
            topObserver.GetObservation()[0],
            topObserver.GetObservation()[1],
            0,
            0,
            0,
            //left
            0,
            leftObserver.GetObservation()[0],
            leftObserver.GetObservation()[1],
            0,
            0,
            //bottom
            0,
            0,
            rightObserver.GetObservation()[0],
            rightObserver.GetObservation()[1],
            0,
            //right
            0,
            0,
            0,
            rightObserver.GetObservation()[0],
            rightObserver.GetObservation()[1],
        };

        return embeddingNN.Process(observations)[0];
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
            prevRelationReasoning
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

    public float[] calculateRelationReasoning()
    {
        float[] vhat = GetVhat();
        float vbar = getVbar();
        Debug.Log("Vhat");
        if (Double.IsNaN(vhat[0])) vhat[0] = 0;
        if (Double.IsNaN(vhat[1])) vhat[1] = 0;
        if (Double.IsNaN(vbar)) vbar = 0;

        float[] relationReasoning = vOutNN.Process(new[] { vhat[0], vhat[1], vbar });
        prevRelationReasoning = relationReasoning[0];
        return relationReasoning;
    }


    public float computeLSTM(float[] value)
    {
        return lstm.Calculate(value)[0];
    }

    private float EluActivation(float x)
    {
        if (x >= 0)
        {
            return x;
        }

        return 1 * (MathF.Exp(x) - 1);
    }

    public void Train()
    {
        List<Transition> minibatch = qNetwork.getRandomSample();

        if (minibatch.Count <= 0) return;
        Debug.Log("TRAINING");

        minibatch.ForEach(delegate (Transition transition)
        {
            float[] currentQs = qNetwork.getQs(transition.state);
            float[] newQs = targetQNetwork.getQs(transition.newState);

            float reward = qNetwork.getReward(this) + qNetwork.discountFactor * MathF.Max(newQs[0], newQs[1]);

            float[] expectedOutput;
            if (transition.action == 0)
                expectedOutput = new []{ reward, currentQs[1]};
            else
                expectedOutput = new []{ currentQs[0], reward};

            qNetwork.trainModel(expectedOutput, transition.state);
        });
        updateTargetCounter++;

        if(updateTargetCounter>updateTargetBatch){
            updateTargetCounter = 0;
            targetQNetwork.copyWeights(qNetwork);
        }
    }
}
