namespace GraphNS;

/// <summary>
/// Класс для проверки цикличности графа
/// </summary>
public static class CycleChecker
{
    /// <summary>
    /// Проверяет наличие цикла в ориентированном графе.
    /// </summary>
    /// <param name="graph">Граф в виде списка смежности</param>
    /// <returns>true, если граф содержит цикл, иначе false</returns>
    public static bool IsCyclic(List<int>[] graph)
    {
        int n = graph.Length;
        bool[] visited = new bool[n];
        bool[] recursionStack = new bool[n];

        for (int i = 0; i < n; i++)
        {
            if (!visited[i])
            {
                if (DFS(i, graph, visited, recursionStack))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Реализация алгоритма DFS для поиска цикла из заданной вершины
    /// </summary>
    /// <param name="v">Вершина, из которой ищем цикл</param>
    /// <param name="graph">Список смежности графа</param>
    /// <param name="visited">Посещённые вершины</param>
    /// <param name="recursionStack">Стек рекурсии DFS</param>
    /// <returns></returns>
    private static bool DFS(int v, List<int>[] graph, bool[] visited, bool[] recursionStack)
    {
        visited[v] = true;
        recursionStack[v] = true;

        foreach (var neighbor in graph[v])
        {
            if (!visited[neighbor])
            {
                if (DFS(neighbor, graph, visited, recursionStack))
                    return true;
            }
            else if (recursionStack[neighbor])
            {
                return true;
            }
        }
        recursionStack[v] = false;
        return false;
    }
}
