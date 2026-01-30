using GraphNS;

namespace CycleCheckerTest;

public class Tests
{
    [Test]
    public void TreeTest()
    {
        var graph = new List<int>[4];
        graph[0] = new List<int> { 1, 2 };
        graph[1] = new List<int> { 3 };
        graph[2] = new List<int>();
        graph[3] = new List<int>();

        bool hasCycle = CycleChecker.IsCyclic(graph);
        Assert.That(CycleChecker.IsCyclic(graph), Is.False);
    }

    [Test]
    public void GraphWithCycleTest()
    {
        var graph = new List<int>[3];
        graph[0] = new List<int> { 1 };
        graph[1] = new List<int> { 2 };
        graph[2] = new List<int> { 0 };
        Assert.That(CycleChecker.IsCyclic(graph), Is.True);
    }

    [Test]
    public void EmptyGraphTest()
    {
        var graph = new List<int>[0];
        Assert.That(CycleChecker.IsCyclic(graph), Is.False);
    }
}
