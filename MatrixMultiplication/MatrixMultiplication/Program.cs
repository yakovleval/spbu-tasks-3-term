using MatrixMultiplication;
using System.Diagnostics;

double StandartDeviation(double[] values)
{
    double mean = values.Average();
    double sumsOfSquares = 0;
    for (int i = 0; i < values.Length; i++)
    {
        sumsOfSquares += (values[i] - mean) * (values[i] - mean);
    }
    return Math.Sqrt(sumsOfSquares);
}

const int N = 5;
int matrixDimension = 100;
const int testsNumber = 4;
using var streamWriter = new StreamWriter("../../../statistics.txt");

streamWriter.WriteLine("matrices' size | Avg(single/multi) | StdDev(single/multi)");
streamWriter.WriteLine();

for (int i = 0; i < testsNumber; i++, matrixDimension *= 2)
{
    var leftMatrix = Matrix.GenerateRandomMatrix(matrixDimension, matrixDimension);
    var rightMatrix = Matrix.GenerateRandomMatrix(matrixDimension, matrixDimension);
    var singleThreadMeasurements = new double[N];
    var multiThreadMeasurements = new double[N];
    var stopWatch = new Stopwatch();
    for (int j = 0; j < N; j++)
    {
        stopWatch.Start();
        leftMatrix.MultiplyBy(rightMatrix);
        stopWatch.Stop();
        singleThreadMeasurements[j] = stopWatch.ElapsedMilliseconds;
        stopWatch.Restart();
        leftMatrix.ParalleledMultiplyBy(rightMatrix);
        stopWatch.Stop();
        multiThreadMeasurements[j] = stopWatch.ElapsedMilliseconds;
        stopWatch.Reset();
    }
    int precision = 2;
    double singleAverage = Math.Round(singleThreadMeasurements.Average(), precision);
    double multiAverage = Math.Round(multiThreadMeasurements.Average(), precision);
    double singleDev = Math.Round(StandartDeviation(singleThreadMeasurements), precision);
    double multiDev = Math.Round(StandartDeviation(multiThreadMeasurements), precision);

    streamWriter.WriteLine($"{matrixDimension}x{matrixDimension} | " +
        $"{singleAverage}/{multiAverage} | " +
        $"{singleDev}/{multiDev}");
    //streamWriter.WriteLine(String.Format("{0}", matrixDimension));
}
