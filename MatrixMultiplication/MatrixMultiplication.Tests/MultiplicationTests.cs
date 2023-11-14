namespace MatrixMultiplication.Tests;

public class Tests
{
    private void CheckMultiplicationResults(Matrix leftMatrix, Matrix rightMatrix, Matrix expectedResult)
    {
        var singleTreadResult = leftMatrix.MultiplyBy(rightMatrix);
        var multiTreadResult = leftMatrix.ParalleledMultiplyBy(rightMatrix);
        Assert.That(expectedResult.IsEqualTo(singleTreadResult), Is.True);
        Assert.That(expectedResult.IsEqualTo(multiTreadResult), Is.True);
    }

    [Test]
    public void TestCorrectArbitraryMatrices()
    {
        var leftMatrix = new Matrix(new int[,]
        {
            { 1, -2, 3 },
            { 0, 1, 0 },
            { 0, 0, 1 },
            { 1, 1, 1 }
        });
        var rightMatrix = new Matrix(new int[,]
        {
            { 1, 2 },
            { 3, 4 },
            { 5, 6 }
        });
        var expectedResult = new Matrix(new int[,]
        {
            { 10, 12 },
            { 3, 4 },
            { 5, 6 },
            { 9, 12 }
        });
        CheckMultiplicationResults(leftMatrix, rightMatrix, expectedResult);
    }

    [Test]
    public void TestOneElementMatrices()
    {
        var leftMatrix = new Matrix(new int[,]
        {
            { 2 }
        });
        var rightMatrix = new Matrix(new int[,]
        {
            { 3 }
        });
        var expectedResult = new Matrix(new int[,]
        {
            { 6 }
        });
        CheckMultiplicationResults(leftMatrix, rightMatrix, expectedResult);
    }

    [Test]
    public void TestBigMatrices()
    {
        var leftMatrix = new Matrix("../../../BigMatrix.txt");
        var rightMatrix = new Matrix("../../../BigMatrix.txt");
        var singleThreadResult = leftMatrix.MultiplyBy(rightMatrix);
        var multiThreadResult = leftMatrix.ParalleledMultiplyBy(rightMatrix);
        Assert.That(singleThreadResult.IsEqualTo(multiThreadResult));
    }

    [Test]
    public void TestNonCompliantDimensions()
    {
        var leftMatrix = new Matrix(new int[,]
        {
            { 1, 2 },
            { 3, 4 }
        });
        var rightMatrix = new Matrix(new int[,]
        {
            { 1 },
            { 2 },
            { 3 }
        });
        Assert.Throws<InvalidOperationException>(() => leftMatrix.MultiplyBy(rightMatrix));
        Assert.Throws<InvalidOperationException>(() => leftMatrix.ParalleledMultiplyBy(rightMatrix));
    }

    [Test]
    public void TestReadMatrix()
    {
        var matrix = new Matrix("../../../Matrix.txt");
        var expected = new Matrix(new int[,]
        {
            { 1, 2, 3, 4 },
            { 5, 6, 7, 8 },
            { 4, 3, 2, 1 }
        });
        Assert.That(matrix.IsEqualTo(expected), Is.True);
    }

    [TestCase("../../../EmptyMatrix.txt")]
    [TestCase("../../../InvalidRowLengthMatrix.txt")]
    [TestCase("../../../IncorrectSymbolMatrix.txt")]
    public void TestIncorrectFormatMatrices(string path)
    {
        Assert.Throws<FormatException>(() => new Matrix(path));
    }
}