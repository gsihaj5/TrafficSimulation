using System;

public static class MatrixOperations
{
    public static float[,] DotProduct(float[,] matrixA, float[,] matrixB)
    {
        int rowsA = matrixA.GetLength(0);
        int colsA = matrixA.GetLength(1);
        int rowsB = matrixB.GetLength(0);
        int colsB = matrixB.GetLength(1);

        if (colsA != rowsB)
        {
            throw new ArgumentException("Invalid matrix dimensions");
        }

        float[,] result = new float[rowsA, colsB];

        for (int i = 0; i < rowsA; i++)
        {
            for (int j = 0; j < colsB; j++)
            {
                float sum = 0.0f;
                for (int k = 0; k < colsA; k++)
                {
                    sum += matrixA[i, k] * matrixB[k, j];
                }
                result[i, j] = sum;
            }
        }

        return result;
    }

    public static float[,] Transpose(float[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        float[,] result = new float[cols, rows];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[j, i] = matrix[i, j];
            }
        }

        return result;
    }

    public static float[,] ApplyActivationFunction(float[,] matrix, ActivationFunction activationFunction)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        float[,] result = new float[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                float value = matrix[i, j];
                result[i, j] = ApplyActivationFunction(value, activationFunction);
            }
        }

        return result;
    }

    public static float[,] ApplyActivationFunctionDerivative(float[,] matrix, ActivationFunction activationFunction)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        float[,] result = new float[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                float value = matrix[i, j];
                result[i, j] = ApplyActivationFunctionDerivative(value, activationFunction);
            }
        }

        return result;
    }

    public static float ApplyActivationFunction(float value, ActivationFunction activationFunction)
    {
        switch (activationFunction)
        {
            case ActivationFunction.ReLU:
                return Math.Max(0.0f, value);
            default:
                throw new ArgumentException("Invalid activation function");
        }
    }

    public static float ApplyActivationFunctionDerivative(float value, ActivationFunction activationFunction)
    {
        switch (activationFunction)
        {
            case ActivationFunction.ReLU:
                return value >= 0.0f ? 1.0f : 0.0f;
            default:
                throw new ArgumentException("Invalid activation function");
        }
    }

    public static float[,] Add(float[,] matrixA, float[,] matrixB)
    {
        int rowsA = matrixA.GetLength(0);
        int colsA = matrixA.GetLength(1);
        int rowsB = matrixB.GetLength(0);
        int colsB = matrixB.GetLength(1);

        if (rowsA != rowsB || colsA != colsB)
        {
            throw new ArgumentException("Invalid matrix dimensions");
        }

        float[,] result = new float[rowsA, colsA];

        for (int i = 0; i < rowsA; i++)
        {
            for (int j = 0; j < colsA; j++)
            {
                result[i, j] = matrixA[i, j] + matrixB[i, j];
            }
        }

        return result;
    }

    public static float[,] Multiply(float[,] matrix, float scalar)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        float[,] result = new float[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = matrix[i, j] * scalar;
            }
        }

        return result;
    }

    public static float[,] ElementWiseMultiply(float[,] matrixA, float[,] matrixB)
    {
        int rowsA = matrixA.GetLength(0);
        int colsA = matrixA.GetLength(1);
        int rowsB = matrixB.GetLength(0);
        int colsB = matrixB.GetLength(1);

        if (rowsA != rowsB || colsA != colsB)
        {
            throw new ArgumentException("Invalid matrix dimensions");
        }

        float[,] result = new float[rowsA, colsA];

        for (int i = 0; i < rowsA; i++)
        {
            for (int j = 0; j < colsA; j++)
            {
                result[i, j] = matrixA[i, j] * matrixB[i, j];
            }
        }

        return result;
    }

    public static float[,] Subtract(float[,] matrixA, float[,] matrixB)
    {
        int rowsA = matrixA.GetLength(0);
        int colsA = matrixA.GetLength(1);
        int rowsB = matrixB.GetLength(0);
        int colsB = matrixB.GetLength(1);

        if (rowsA != rowsB || colsA != colsB)
        {
            throw new ArgumentException("Invalid matrix dimensions");
        }

        float[,] result = new float[rowsA, colsA];

        for (int i = 0; i < rowsA; i++)
        {
            for (int j = 0; j < colsA; j++)
            {
                result[i, j] = matrixA[i, j] - matrixB[i, j];
            }
        }

        return result;
    }
}