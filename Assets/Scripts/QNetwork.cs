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
        public float[] getArray(){
            return new[] {vin, vout};
        }
    }

    private State latestState = null;
    private int latestAction = 0;
    private float[] latestQEstimation;

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
                learningRate, 10, "Sigmoid");

    }


    public void calculateReward(Observer observer)
    {
        if (firstEvaluation)
        {
            firstEvaluation = false;
            return;
        }

        //calculate actual reward
        float queueWeight = -1f;
        float speedWeight = 1f;

        float queueReward = (float)observer.GetVehicleCount();
        float speedReward = observer.GetAverageSpeed();

        float reward = queueWeight * queueReward + speedWeight * speedReward;
    }

    public float getAction(float vin, float vout)
    {
        State state = new State(vin, vout);

        //estimate Q values
        float[] qValues = _neuralNetwork.Process(new[] { vin, vout });

        if (double.IsNaN(qValues[0])) qValues[0] = 0;
        if (double.IsNaN(qValues[1])) qValues[1] = 0;

        latestQEstimation = qValues;
        latestState = state;
        latestAction = 0;

        if(qValues[0] < qValues[1]){
            return 1; //vertical
        }
        return 0; // horizontal
    }
}
