using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GraphSearch
{
    private Graph graph;
    public List<GraphNode> path = new List<GraphNode>();

    public void Init(Graph graph)
    {
        this.graph = graph;
    }

    public void DFS(GraphNode node)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        var stack = new Stack<GraphNode>();

        visited.Add(node);
        stack.Push(node);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            path.Add(current);

            foreach (var neighbor in current.adjacents)
            {
                if (neighbor == null || !neighbor.CanVisit) continue;
                if (visited.Contains(neighbor)) continue;

                visited.Add(neighbor);
                stack.Push(neighbor);
            }
        }
    }

    public void BFS(GraphNode node)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        visited.Add(node);
        queue.Enqueue(node);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            path.Add(current);
            foreach (var neighbor in current.adjacents)
            {
                if (neighbor == null || !neighbor.CanVisit) continue;
                if (visited.Contains(neighbor)) continue;

                visited.Add(neighbor);
                queue.Enqueue(neighbor);
            }
        }
    }

    public void DFS_Recursive(GraphNode node)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        DFS_Recursive(node, visited);
    }

    public void DFS_Recursive(GraphNode node, HashSet<GraphNode> visited)
    {
        if (!node.CanVisit) return;
        if (visited.Contains(node)) return;

        path.Add(node);
        visited.Add(node);
        foreach (var neighbor in node.adjacents)
        {
            if (neighbor == null || !neighbor.CanVisit) continue;
            DFS_Recursive(neighbor, visited);
        }
    }
    public void PathFindingBFS(GraphNode start, GraphNode target)
    {
        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();
        var parentMap = new Dictionary<GraphNode, GraphNode>();
        visited.Add(start);

        queue.Enqueue(start);
        bool found = false;
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == target)
            {
                found = true;
                break;
            }

            foreach (var neighbor in current.adjacents)
            {
                if (neighbor == null || !neighbor.CanVisit) continue;
                if (visited.Contains(neighbor)) continue;

                visited.Add(neighbor);
                parentMap[neighbor] = current;
                queue.Enqueue(neighbor);
            }
        }
        path.Clear();
        if (found)
        {
            var current = target;
            while (current != null)
            {
                path.Add(current);
                parentMap.TryGetValue(current, out current);
            }
            path.Reverse();
        }
    }
}