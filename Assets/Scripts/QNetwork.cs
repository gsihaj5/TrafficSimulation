using System;

public class QNetwork
{
    private int inputSize;
    private int outputSize;
    private float hiddenSize;
    private float learningRate;
    private float discountFactor;
    public float[,] weights;
    public float[] biases;

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
                learningRate, 100, "Relu");
    }

    public float[] getQValues(float[] state)
    {
        return _neuralNetwork.Process(state);
    }
    
    
}