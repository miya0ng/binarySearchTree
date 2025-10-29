using System;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueueTest : MonoBehaviour
{

    public void Start()
    {
        RunTest();
    }
    public void RunTest()
    {
        BasicOperationsTest();
    }

    static void BasicOperationsTest()
    {
        var scheduler = new PriorityQueue<GameTask, int>();

        scheduler.Enqueue(new GameTask("Render"), 2);
        scheduler.Enqueue(new GameTask("Physics"), 1);
        scheduler.Enqueue(new GameTask("AI"), 3);

        Debug.Log($"�۾� ��: {scheduler.Count}"); // ���: 3

        GameTask task = scheduler.Dequeue();
        Debug.Log($"ù ��° �۾�: {task}"); // ���: Physics

        task = scheduler.Peek();
        Debug.Log($"���� �۾�: {task}"); // ���: Render

        scheduler.Clear();
        Debug.Log($"Clear �� �۾� ��: {scheduler.Count}"); // ���: 0
    }

}