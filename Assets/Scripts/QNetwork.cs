using System;

public class QNetwork
{
    private int inputSize;
    private int outputSize;
    public float[,] weights;
    public float[] biases;

    public QNetwork(int inputSize, int outputSize)
    {
        this.inputSize = inputSize;
        this.outputSize = outputSize;
        InitializeParameters();
    }

    private void InitializeParameters()
    {
        weights = new float[outputSize, inputSize];
        biases = new float[outputSize];

        // Initialize weights and biases randomly or using a specific initialization method
        Random random = new Random();
        for (int i = 0; i < outputSize; i++)
        {
            for (int j = 0; j < inputSize; j++)
            {
                weights[i, j] = (float)random.NextDouble();
            }

            biases[i] = (float)random.NextDouble();
        }
    }
}