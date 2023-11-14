using System.Text.RegularExpressions;

namespace MatrixMultiplication;

/// <summary>
/// class that implements a matrix data structure
/// </summary>
public class Matrix
{
    private readonly int[,] data;

    public int Rows => data.GetLength(0);

    public int Columns => data.GetLength(1);

    private static Random rand = new();

    public Matrix(string path)
    {
        string matrixDimensionsPattern = @"^\d+ \d+$";
        using var reader = new StreamReader(path);
        var firstLine = reader.ReadLine();
        if (firstLine == null)
            throw new FormatException("file is empty");
        if (!Regex.IsMatch(firstLine, matrixDimensionsPattern))
            throw new FormatException("first line must contain two positive integers separated by space");
        string[] dimensions = firstLine.Split();
        int rows = int.Parse(dimensions[0]);
        int columns = int.Parse(dimensions[1]);
        data = new int[rows, columns];
        string rowsPattern = @"^(\d+ ?)*$";
        for (int i = 0; i < rows; i++)
        {
            var line = reader.ReadLine();
            if (line == null)
                throw new FormatException("actual number of rows is not equal to stated");
            if (!Regex.IsMatch(line, rowsPattern))
                throw new FormatException("rows must contain only numbers");
            string[] intsInStrings = line.Split();
            if (intsInStrings.Length != columns)
                throw new FormatException($"length of {i}th row ({intsInStrings.Length}) is not equal to stated number of columns ({columns})");
            for (int j = 0; j < intsInStrings.Length; j++)
            {
                data[i, j] = int.Parse(intsInStrings[j]);
            }
        }
    }

    public Matrix(int[,] data)
    {
        int rows = data.GetLength(0);
        int columns = data.GetLength(1);
        this.data = new int[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                this.data[i, j] = data[i, j];
            }
        }
    }

    /// <summary>
    /// multiplies matrix by another matrix without additional threads
    /// </summary>
    /// <param name="other">matrix to multiply by</param>
    /// <returns>result of multiplication</returns>
    /// <exception cref="InvalidOperationException">thrown if multiplication is not defined for given matrices</exception>
    public Matrix MultiplyBy(Matrix other)
    {
        if (this.Columns != other.Rows)
            throw new InvalidOperationException("matrices' dimensions are not compliant for multiplication");
        int[,] result = new int[this.Rows, other.Columns];
        for (int i = 0; i < this.Rows; i++)
        {
            for (int j = 0; j < other.Columns; j++)
            {
                for (int k = 0; k < this.Columns; k++)
                {
                    result[i, j] += this.data[i, k] * other.data[k, j];
                }
            }
        }
        return new Matrix(result);
    }

    /// <summary>
    /// multiplies matrix by another matrix using multiple threads
    /// </summary>
    /// <param name="other">matrix to multiply by</param>
    /// <returns>result of multiplication</returns>
    /// <exception cref="InvalidOperationException">thrown if multiplication is not defined for given matrices</exception>
    public Matrix ParalleledMultiplyBy(Matrix other)
    {
        if (this.Columns != other.Rows)
            throw new InvalidOperationException("matrices' dimensions are not compliant for multiplication");
        int[,] result = new int[this.Rows, other.Columns];
        int threadsNumber = Math.Min(Environment.ProcessorCount, other.Columns);
        int chunkSize = other.Columns / threadsNumber;
        int remainder = other.Columns % threadsNumber;
        Thread[] threads = new Thread[threadsNumber];
        for (int i = 0; i < threadsNumber; i++)
        {
            int localI = i;
            int startColumn;
            int endColumn;
            if (localI < remainder)
            {
                startColumn = (chunkSize + 1) * localI;
                endColumn = startColumn + chunkSize + 1;
            }
            else
            {
                startColumn = (chunkSize + 1) * remainder + chunkSize * (localI - remainder);
                endColumn = startColumn + chunkSize;
            }
            threads[i] = new Thread(() =>
            {
                for (int n = 0; n < this.Rows; n++)
                {
                    for (int m = startColumn; m < endColumn; m++)
                    {
                        for (int k = 0; k < this.Columns; k++)
                        {
                            result[n, m] += this.data[n, k] * other.data[k, m];
                        }
                    }
                }
            });
            threads[i].Start();
        }
        for (int i = 0; i < threadsNumber; i++)
        {
            threads[i].Join();
        }
        return new Matrix(result);
    }

    /// <summary>
    /// writes matrix to file
    /// </summary>
    /// <param name="path">path to file to write the matrix to</param>
    public void WriteToFile(string path)
    {
        using var streamWriter = new StreamWriter(path);
        streamWriter.WriteLine($"{Rows} {Columns}");
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns - 1; j++)
            {
                streamWriter.Write(data[i, j]);
                streamWriter.Write(" ");
            }
            streamWriter.WriteLine(data[i, Columns - 1]);
        }
    }

    /// <summary>
    /// generates matrix of random matrices of given size
    /// </summary>
    /// <param name="rows">number of rows</param>
    /// <param name="columns">number of columns</param>
    /// <returns>random matrix with given numbers of rows and columns</returns>
    public static Matrix GenerateRandomMatrix(int rows, int columns)
    {
        int[,] data = new int[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                data[i, j] = rand.Next(100);
            }
        }
        return new Matrix(data);
    }

    /// <summary>
    /// checks if two matrices are equal
    /// </summary>
    /// <param name="other">matrix to compare to</param>
    /// <returns>true if matrices are equal, false otherwise</returns>
    public bool IsEqualTo(Matrix other)
    {
        if (Rows != other.Rows || Columns != other.Columns)
            return false;
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (data[i, j] != other.data[i, j])
                    return false;
            }
        }
        return true;
    }
}
