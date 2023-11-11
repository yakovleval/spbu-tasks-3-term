using System.Text.RegularExpressions;

namespace MatrixMultiplication;

internal class Matrix
{
    private int[,] data;

    public int Rows => data.GetLength(0);

    public int Columns => data.GetLength(1);

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
                throw new FormatException($"length of row {i} is not equal to stated number of columns");
            for (int j = 0; j < intsInStrings.Length; j++)
            {
                data[i, j] = int.Parse(intsInStrings[j]);
            }
        }
    }

    public Matrix(int[,] data)
    {
        this.data = (int[,])data.Clone();
    }

    public Matrix Multiply(Matrix other)
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
}
