using System;
using System.Collections.Generic;
using System.Linq;

class GraphDFS
{
    private int[,] incidenceMatrix;
    private bool[] visited;
    private int vertices, edges;
    private HashSet<Tuple<int, int>> existingEdges; // Для запобігання мультиребер
    private List<Tuple<int, int>> edgesList;
    private List<List<int>> adjacencyList;

    public GraphDFS(int vertices, int edges)
    {
        this.vertices = vertices;
        this.edges = edges;
        incidenceMatrix = new int[vertices, edges];
        visited = new bool[vertices];
        existingEdges = new HashSet<Tuple<int, int>>();
        edgesList = new List<Tuple<int, int>>();
        adjacencyList = new List<List<int>>(vertices);
        for (int i = 0; i < vertices; i++)
        {
            adjacencyList.Add(new List<int>());
        }
    }

    public void InputGraph()
    {
        Console.WriteLine("Введіть матрицю інцидентності:");
        for (int i = 0; i < vertices; i++)
        {
            for (int j = 0; j < edges; j++)
            {
                bool validInput = false;
                while (!validInput)
                {
                    Console.Write($"Вершина {i}, ребро {j} (0 або 1): ");
                    string input = Console.ReadLine();
                    if (int.TryParse(input, out int value) && (value == 0 || value == 1))
                    {
                        incidenceMatrix[i, j] = value;
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("❌ Введено неправильне значення. Спробуйте ще раз.");
                    }
                }
            }
        }
        ConvertToAdjacencyList();
    }

    public void RandomGraph()
    {
        Random rand = new Random();
        int generatedEdges = 0;
        while (generatedEdges < edges)
        {
            int u = rand.Next(vertices);
            int v = rand.Next(vertices);

            if (u != v)
            {
                int min = Math.Min(u, v);
                int max = Math.Max(u, v);
                Tuple<int, int> newEdge = Tuple.Create(min, max);

                if (!existingEdges.Contains(newEdge))
                {
                    incidenceMatrix[u, generatedEdges] = 1;
                    incidenceMatrix[v, generatedEdges] = 1;
                    edgesList.Add(newEdge);
                    existingEdges.Add(newEdge);
                    generatedEdges++;
                }
            }
        }
        edgesList.Sort((x, y) => x.Item1 == y.Item1 ? x.Item2.CompareTo(y.Item2) : x.Item1.CompareTo(y.Item1));
        ConvertToAdjacencyList();
    }

    private void ConvertToAdjacencyList()
    {
        adjacencyList = new List<List<int>>(vertices);
        for (int i = 0; i < vertices; i++)
        {
            adjacencyList.Add(new List<int>());
        }
        for (int j = 0; j < edges; j++)
        {
            int u = -1, v = -1;
            for (int i = 0; i < vertices; i++)
            {
                if (incidenceMatrix[i, j] == 1)
                {
                    if (u == -1)
                    {
                        u = i;
                    }
                    else
                    {
                        v = i;
                        break;
                    }
                }
            }
            if (u != -1 && v != -1)
            {
                adjacencyList[u].Add(v);
                adjacencyList[v].Add(u); // Оскільки граф неорієнтований
            }
        }

        // Видаляємо дублікати зі списків суміжності
        for (int i = 0; i < vertices; i++)
        {
            adjacencyList[i] = adjacencyList[i].Distinct().OrderBy(x => x).ToList();
        }
    }

    public void DFS(int v, ref int visitedCount)
    {
        visited[v] = true;
        visitedCount++;
        Console.Write($"{v} ");

        foreach (int neighbor in adjacencyList[v])
        {
            if (!visited[neighbor])
            {
                DFS(neighbor, ref visitedCount);
            }
        }
    }

    public void Run()
    {
        Console.Write("Початкова вершина для DFS: ");
        int start;
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out start) && start >= 0 && start < vertices)
            {
                break;
            }
            else
            {
                Console.WriteLine("❌ Невірний ввід. Спробуйте ще раз.");
            }
        }

        if (adjacencyList[start].Count == 0)
        {
            Console.WriteLine($"❌ Вершина {start} ізольована, не має суміжних вершин.");
            return;
        }

        // Запуск DFS
        Console.WriteLine("Обхід DFS з вершини " + start + ":");
        int visitedCount = 0;
        DFS(start, ref visitedCount);

        // Інформація про досяжність
        Console.WriteLine($"\n✅ Всього обійдено вершин: {visitedCount} з {vertices}");

        // Перевірка чи обійшовся весь граф
        if (visitedCount == vertices)
        {
            Console.WriteLine("Обійдено всі вершини графа.");
        }
        else
        {
            Console.WriteLine($"Обійдено не всі вершини, лише {visitedCount} з {vertices}.");
        }
    }

    public void PrintGraphInfo()
    {
        Console.WriteLine("\nКількість вершин: " + vertices);
        Console.WriteLine("Кількість ребер: " + edges);
        Console.WriteLine("Тип графа: Неорієнтований");
        Console.WriteLine("Форма подання: Матриця інцидентності");

        Console.WriteLine("\nСписок ребер:");
        for (int i = 0; i < edges; i++)
        {
            Console.WriteLine($"e{i}: ({edgesList[i].Item1},{edgesList[i].Item2})");
        }

        Console.WriteLine("\nМатриця інцидентності (" + vertices + " вершин × " + edges + " ребер)");
        Console.Write("Вершина ↓\t");
        for (int j = 0; j < edges; j++)
        {
            Console.Write($"e{j}\t");
        }
        Console.WriteLine();

        for (int i = 0; i < vertices; i++)
        {
            Console.Write($"    {i}\t\t");
            for (int j = 0; j < edges; j++)
            {
                Console.Write($"{incidenceMatrix[i, j]}\t");
            }
            Console.WriteLine();
        }

        PrintGraphAsAdjacencyList();
    }

    public void PrintGraphAsAdjacencyList()
    {
        Console.WriteLine("\nСписок суміжності:");
        for (int i = 0; i < vertices; i++)
        {
            Console.Write($"Вершина {i}: ");
            if (adjacencyList[i].Count > 0)
            {
                Console.WriteLine(string.Join(", ", adjacencyList[i]));
            }
            else
            {
                Console.WriteLine("()"); // Позначаємо ізольовану вершину
            }
        }
    }

    static void Main()
    {
        Console.Write("Введіть кількість вершин (не менше 2): ");
        int v;
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out v) && v >= 2)
            {
                break;
            }
            else
            {
                Console.WriteLine("❌ Кількість вершин повинна бути не менше 2. Спробуйте ще раз.");
            }
        }

        int minEdges = v - 1;
        int maxEdges = v * (v - 1) / 2;

        Console.Write($"Введіть кількість ребер (не менше {minEdges} та не більше {maxEdges}): ");
        int e;
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out e) && e >= minEdges && e <= maxEdges)
            {
                break;
            }
            else
            {
                Console.WriteLine($"❌ Кількість ребер повинна бути між {minEdges} та {maxEdges}. Спробуйте ще раз.");
            }
        }

        GraphDFS graph = new GraphDFS(v, e);

        Console.Write("Ввести граф вручну? (y/n): ");
        string input = Console.ReadLine().ToLower();
        if (input == "y")
        {
            graph.InputGraph();
        }
        else
        {
            graph.RandomGraph();
        }

        graph.PrintGraphInfo();
        graph.Run();

        Console.WriteLine("\n✅ Обхід завершено.");
        Console.ReadLine();
    }
}