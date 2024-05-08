using NUnit.Framework.Constraints;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lazy.Tests;

public class Tests
{
    private static readonly Random rnd = new();

    [Test]
    public void TestRaceCondition()
    {
        var lazyRand = new LazyThreadSafe<int>(() => rnd.Next());
        var mre = new ManualResetEvent(false);
        var threadsCount = Environment.ProcessorCount;
        var threads = new Thread[threadsCount];
        var results = new int[threadsCount];
        for (int i = 0; i < threads.Length; i++)
        {
            int localI = i;
            threads[i] = new Thread(() =>
            {
                mre.WaitOne();
                results[localI] = lazyRand.Get();
            });
            threads[i].Name = $"thread_{localI}";
            threads[i].Start();
        }
        mre.Set();
        for (int i = 0; i < threads.Length; i++)
        {
            threads[i].Join();
        }
        for (int i = 0; i < results.Length; i++)
        {
            Assert.That(results[i], Is.EqualTo(results[0]));
        }
    }

    [Test]
    public void TestRaceConditionWithException()
    {
        var lazyRand = new LazyThreadSafe<int>(() =>
        {
            var e = new Exception($"{rnd.Next()}");
            throw e;
        });
        var mre = new ManualResetEvent(false);
        var threadsCount = Environment.ProcessorCount;
        var threads = new Thread[threadsCount];
        var results = new int[threadsCount];
        for (int i = 0; i < threads.Length; i++)
        {
            int localI = i;
            threads[i] = new Thread(() =>
            {
                mre.WaitOne();
                try
                {
                    lazyRand.Get();
                }
                catch (Exception e)
                {
                    results[localI] = int.Parse(e.Message);
                }
            });
            threads[i].Name = $"thread_{localI}";
            threads[i].Start();
        }
        mre.Set();
        for (int i = 0; i < threads.Length; i++)
        {
            threads[i].Join();
        }
        for (int i = 0; i < results.Length; i++)
        {
            Assert.That(results[i], Is.EqualTo(results[0]));
        }
    }

    [Test]
    public void TestLazyCalculation()
    {
        var lazy = new Lazy<int>(() => rnd.Next());
        var value = lazy.Get();
        Assert.That(lazy.Get(), Is.EqualTo(value));
    }

    [Test]
    public void TestLazyCalculationWIthException()
    {
        var lazy = new Lazy<int>(() => throw new Exception($"{rnd.Next()}"));
        var result1 = -1;
        var result2 = 1;
        try
        {
            lazy.Get();
        }
        catch (Exception e)
        {
            result1 = int.Parse(e.Message);
        }
        try
        {
            lazy.Get();
        }
        catch (Exception e)
        {
            result2 = int.Parse(e.Message);
        }
        Assert.That(result1, Is.EqualTo(result2));
    }

    private static IEnumerable<TestCaseData> LazyWithNulls
        => new TestCaseData[]
        {
            new TestCaseData(new Lazy<object>(() => null)),
            new TestCaseData(new LazyThreadSafe<object>(() => null))
        };

    [TestCaseSource(nameof(LazyWithNulls))]
    public void TestCanReturnNull(ILazy<object> lazy)
    {
        Assert.That(lazy.Get(), Is.Null);
    }

    private static IEnumerable<TestCaseData> NullSuppliers
        => new TestCaseData[]
        {
            new TestCaseData(new Lazy<object>(null)),
            new TestCaseData(new LazyThreadSafe<object>(null))
        };

    [TestCaseSource(nameof(NullSuppliers))]
    public void TestNullSuppliers(ILazy<object> lazy)
    {
        Assert.Throws<NullReferenceException>(() => lazy.Get());
    }
}