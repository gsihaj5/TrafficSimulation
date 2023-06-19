using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class QNetwork
{
    private int inputSize;
    private int outputSize;
    private float hiddenSize;
    private float learningRate;
    public float discountFactor;

    private bool firstEvaluation = true;

    private State latestState = null;
    private int latestAction = 0;
    private float[] latestQEstimation;

    public NeuralNetwork _neuralNetwork;
    private List<Transition> replay_memory;

    public QNetwork(int inputSize, int outputSize, int hiddenSize, float learningRate, float discountFactor)
    {
        this.inputSize = inputSize;
        this.outputSize = outputSize;
        this.hiddenSize = hiddenSize;
        this.learningRate = learningRate;
        this.discountFactor = discountFactor;
        replay_memory = new List<Transition>();

        // Initialize the neural network
        _neuralNetwork =
            new NeuralNetwork(new[] { inputSize, hiddenSize, hiddenSize, hiddenSize, hiddenSize, outputSize },
                learningRate, 1, "Sigmoid");

    }

    public void calculateReward(Observer observer)
    {
        if (firstEvaluation)
        {
            firstEvaluation = false;
            return;
        }

        //calculate actual reward
        float reward = getReward(observer);

        State newState = new State(
            observer.GetInputValue()[0],
            observer.computeLSTM(observer.calculateRelationReasoning())
        );

        replay_memory.Add(new Transition(latestState, latestAction, reward, newState));
    }

    public float getReward(Observer observer)
    {
        float queueWeight = -1f;
        float speedWeight = 1f;

        float queueReward = (float)observer.GetVehicleCount();
        float speedReward = observer.GetAverageSpeed();

        return queueWeight * queueReward + speedWeight * speedReward;
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

        Random random = new Random();
        int action = 0;
        if (random.Next(0, 100) > 75)
        {
            Debug.Log("Exploit");
            //get MAX
            if (qValues[0] < qValues[1])
            {
                Debug.Log("VERTICAL");
                action = 1; //vertical
            }
            Debug.Log("Horizontal");
            action = 0; // horizontal
        }
        else
        {
            Debug.Log("Explore");
            action = random.Next(0, 2);
        }
        latestAction = action;
        Debug.Log("Latest action " + latestAction.ToString());
        return action;

    }

    public float[] getQs(State state)
    {
        return _neuralNetwork.Process(new[] { state.vin, state.vout });
    }

    public void copyWeights(QNetwork qnetwork)
    {
        NeuralNetwork nn = qnetwork._neuralNetwork;

        for (int i = nn.layers.Length - 1; i >= 0; i--)
        {
            Layer layer = nn.layers[i];

            this._neuralNetwork.layers[i].weightsArray = layer.weightsArray;
            this._neuralNetwork.layers[i].biasesArray = layer.biasesArray;
        }
    }

    public List<Transition> getRandomSample()
    {
        List<Transition> minibatch = new();

        Random r = new Random();
        int minibatchSize = 25;
        int memorySize = replay_memory.Count();

        if (memorySize < 25) return minibatch;

        for (int i = 0; i < minibatchSize; i++)
        {
            int randomIndex = r.Next(0, memorySize);
            minibatch.Add(replay_memory[randomIndex]);
        }

        return minibatch;
    }

    public void trainModel(float[] expectedValue, State input)
    {
        _neuralNetwork.BackPropagate(expectedValue, new[] { input.vin, input.vout });
    }
}
