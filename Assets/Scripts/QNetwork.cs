using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QNetwork
{
    private int inputSize;
    private int outputSize;
    private float hiddenSize;
    private float learningRate;
    private float discountFactor;
    public float[,] weights;
    public float[] biases;

    private bool firstEvaluation = true;

    private class State
    {
        public float vin { get; }
        public float vout { get; }

        public State(float vin, float vout)
        {
            this.vin = vin;
            this.vout = vout;
        }

        public override bool Equals(object obj)
        {
            // Check if two states are equal based on their values
            if (obj is State other)
            {
                return Math.Abs(vin - other.vin) < .01f && Math.Abs(vout - other.vout) < .01f;
            }

            return false;
        }
    }

    private Dictionary<State, float[]> _qTable = new();

    private State latestState = null;
    private int latestAction = 0;

    private NeuralNetwork _neuralNetwork;

    public QNetwork(int inputSize, int outputSize, int hiddenSize, float learningRate, float discountFactor)
    {
        this.inputSize = inputSize;
        this.outputSize = outputSize;
        this.hiddenSize = hiddenSize;
        this.learningRate = learningRate;
        this.discountFactor = discountFactor;

        // Initialize the neural network
        _neuralNetwork =
            new NeuralNetwork(new[] { inputSize, hiddenSize, hiddenSize, hiddenSize, hiddenSize, outputSize },
                learningRate, 100, "Sigmoid");

        initQtables();
    }

    public float[] getQValues(float[] state)
    {
        return _neuralNetwork.Process(state);
    }

    public void initQtables()
    {
        //create possible state known
        List<State> possibleStates = new List<State>();

        for (float vin = 0; vin < 1; vin += 0.1f)
        {
            for (float vout = 0; vout < 1; vout += 0.1f)
            {
                State state = new State(vin, vout);
                possibleStates.Add(state);
            }
        }

        foreach (State possibleState in possibleStates)
        {
            _qTable[possibleState] = new float[2];
            _qTable[possibleState][0] = 1.0f;
            _qTable[possibleState][1] = 0.0f;
        }
    }

    public void calculateReward(Observer observer)
    {
        if (firstEvaluation) return;

        float queueWeight = -1f;
        float speedWeight = 1f;

        float queueReward = Mathf.Clamp01(1f - (float)observer.GetVehicleCount() / 4);
        float speedReward = Mathf.Clamp01(observer.GetAverageSpeed() / 2f);

        float reward = queueWeight * queueReward + speedWeight * speedReward;

        //update q value
        _qTable[latestState][latestAction] += learningRate * (reward - _qTable[latestState][latestAction]);
    }

    public float getAction(float vin, float vout)
    {
        State state = new State(vin, vout);

        //search from tables
        foreach (KeyValuePair<State, float[]> pair in _qTable)
        {
            if (pair.Key.Equals(state))
            {
                latestState = pair.Key;
                if (pair.Value[0] > pair.Value[1])
                {
                    latestAction = 0;
                    return 0; // horizontal
                }

                latestAction = 1;
                return 1; // vertical
            }
        }

        float[] qValues = getQValues(new[] { vin, vout });

        if (double.IsNaN(qValues[0])) qValues[0] = 1;
        if (double.IsNaN(qValues[1])) qValues[1] = 0;
        _qTable[state] = new float[2];
        _qTable[state][0] = qValues[0];
        _qTable[state][1] = qValues[1];
        latestState = state;

        latestAction = 0;
        return 0; // horizontal
    }
}