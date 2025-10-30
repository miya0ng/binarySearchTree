using System.Collections.Generic;
using UnityEngine;
public class GraphTest : MonoBehaviour
{
    public UIGraphNode nodePrefab;
    public List<UIGraphNode> uiNodes;

    public Transform uiNodeRoot;

    private Graph graph;

    public enum AlgorithmType
    {
        DFS,
        BFS,
        DFS_Recursive,
        PathFindingBFS,
    }

    private void Start()
    {
        int[,] map = new int[10, 10]
        {
            {1,-1,1,1,1,1,1,1,1,1 },
            {1,-1,1,1,1,1,1,1,1,1 },
            {1,-1,1,-1,1,1,1,1,1,1 },
            {1,-1,1,1,1,1,1,1,1,1 },
            {1,-1,1,1,1,1,1,1,1,1 },
            {1,-1,1,1,1,1,1,1,1,1 },
            {1,-1,1,1,1,1,1,1,1,1 },
            {1,1,1,1,1,1,1,1,1,1 },
            {1,1,1,1,1,1,-1,1,1,1 },
            {1,1,1,1,1,1,-1,1,1,1 }
        };

        graph = new Graph();
        graph.Init(map);
        InitUINodes(graph);
    }

    public AlgorithmType algorithmType;
    public int startIndex;
    public int endIndex;

    [ContextMenu("Search")]
    public void Search()
    {
        var search = new GraphSearch();
        search.Init(graph);
        switch (algorithmType)
        {
            case AlgorithmType.DFS:
                search.DFS(graph.nodes[startIndex]);
                break;
            case AlgorithmType.BFS:
                search.BFS(graph.nodes[startIndex]);
                break;
            case AlgorithmType.DFS_Recursive:
                search.DFS_Recursive(graph.nodes[startIndex]);
                break;
            case AlgorithmType.PathFindingBFS:
                search.PathFindingBFS(graph.nodes[startIndex], graph.nodes[endIndex]);
                break;
        }
        ResetUINodes();
        Debug.Log(search.path.Count);
        for (int i = 0; i < search.path.Count; i++)
        {
            var node = search.path[i];
            var uiNode = uiNodes[node.id];
            uiNode.SetColor(Color.Lerp(Color.red, Color.green, (float)i / search.path.Count));
            uiNode.SetText($"ID: {node.id.ToString()} \nWeight: {node.weight} \nPath: {i}");
        }
    }

    private void InitUINodes(Graph graph)
    {
        uiNodes = new List<UIGraphNode>();
        foreach (var node in graph.nodes)
        {
            var uiNode = Instantiate(nodePrefab, uiNodeRoot);
            uiNode.SetNode(node);
            uiNode.Reset();
            uiNodes.Add(uiNode);
        }
    }

    private void ResetUINodes()
    {
        foreach (var uiNode in uiNodes)
        {
            uiNode.Reset();
        }
    }
}