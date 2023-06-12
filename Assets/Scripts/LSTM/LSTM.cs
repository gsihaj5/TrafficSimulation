using System;

public class LSTM
{
    private int inputSize;
    private int hiddenSize;
    private int outputSize;

    private float[,] weights;
    private float[] biases;

    private float[] hiddenState;
    private float[] cellState;

    public LSTM(int inputSize, int hiddenSize, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenSize = hiddenSize;
        this.outputSize = outputSize;

        InitializeWeights();
        InitializeBiases();

        hiddenState = new float[hiddenSize];
        cellState = new float[hiddenSize];
    }

    private void InitializeWeights()
    {
        weights = new float[hiddenSize, inputSize + hiddenSize];
    }

    private void InitializeBiases()
    {
        biases = new float[hiddenSize];
    }

    public float[] Calculate( float[] input)
    {
        float[] concatInputHidden = ConcatenateArrays(input, hiddenState);

        float[] gates = MultiplyMatrixByVector(weights, concatInputHidden);
        AddBiasesToVector(gates, biases);
        ApplySigmoidActivation(gates);

        float[] inputGate = GetSliceFromVector(gates, 0, hiddenSize);
        float[] forgetGate = GetSliceFromVector(gates, hiddenSize, 2 * hiddenSize);
        float[] outputGate = GetSliceFromVector(gates, 2 * hiddenSize, 3 * hiddenSize);
        float[] candidateCellState = GetSliceFromVector(gates, 3 * hiddenSize, 4 * hiddenSize);

        MultiplyVectorsElementwise(forgetGate, cellState);
        MultiplyVectorsElementwise(inputGate, candidateCellState);
        AddVectors(cellState, candidateCellState);

        ApplyTanhActivation(cellState);
        MultiplyVectorsElementwise(outputGate, cellState);
        ApplyTanhActivation(hiddenState);

        float[] output = MultiplyMatrixByVector(weights, hiddenState);
        AddBiasesToVector(output, biases);
        ApplySigmoidActivation(output);

        return output;
    }

    // Helper methods for vector and matrix operations
    private float[] ConcatenateArrays(float[] arr1, float[] arr2)
    {
        float[] result = new float[arr1.Length + arr2.Length];
        Array.Copy(arr1, result, arr1.Length);
        Array.Copy(arr2, 0, result, arr1.Length, arr2.Length);
        return result;
    }

    private float[] MultiplyMatrixByVector(float[,] matrix, float[] vector)
    {
        int numRows = matrix.GetLength(0);
        int numCols = matrix.GetLength(1);

        float[] result = new float[numRows];
        for (int i = 0; i < numRows; i++)
        {
            float sum = 0;
            for (int j = 0; j < numCols; j++)
            {
                sum += matrix[i, j] * vector[j];
            }

            result[i] = sum;
        }

        return result;
    }

    private void AddBiasesToVector(float[] vector, float[] biases)
    {
        for (int i = 0; i < vector.Length; i++)
        {
            vector[i] += biases[i];
        }
    }

    private void ApplySigmoidActivation(float[] vector)
    {
        for (int i = 0; i < vector.Length; i++)
        {
            vector[i] = (float)(1 / (1 + Math.Exp(-vector[i])));
        }
    }

    private float[] GetSliceFromVector(float[] vector, int start, int end)
    {
        int length = end - start;
        float[] slice = new float[length];
        Array.Copy(vector, start, slice, 0, length);
        return slice;
    }

    private void MultiplyVectorsElementwise(float[] vector1, float[] vector2)
    {
        for (int i = 0; i < vector1.Length; i++)
        {
            vector1[i] *= vector2[i];
        }
    }

    private void AddVectors(float[] vector1, float[] vector2)
    {
        for (int i = 0; i < vector1.Length; i++)
        {
            vector1[i] += vector2[i];
        }
    }

    private void ApplyTanhActivation(float[] vector)
    {
        for (int i = 0; i < vector.Length; i++)
        {
            vector[i] = (float)Math.Tanh(vector[i]);
        }
    }
}