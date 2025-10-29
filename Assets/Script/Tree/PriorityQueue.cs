using System;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<TElement, TPriority>
    where TPriority : IComparable<TPriority>
{
    private List<(TElement Element, TPriority Priority)> heap;

    public PriorityQueue()
    {
        heap = new List<(TElement, TPriority)>();
    }

    public int Count => heap.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        // TODO: ����
        // 1. �� ��Ҹ� ����Ʈ ���� �߰�
        // 2. HeapifyUp���� �� �Ӽ� ����

        heap.Add((element, priority));
        HeapifyUp(Count - 1);
    }

    public TElement Dequeue()
    {
        // TODO: ����
        // 1. �� ť üũ �� ���� ó��
        // 2. ��Ʈ ��� ����
        // 3. ������ ��Ҹ� ��Ʈ�� �̵�
        // 4. HeapifyDown���� �� �Ӽ� ����
        // 5. ����� ��Ʈ ��� ��ȯ

        if (heap.Count == 0) throw new InvalidOperationException(" heap �� �������");

        var root = heap[0];
        heap[0] = heap[Count - 1];
        heap.RemoveAt(Count - 1);
        HeapifyDown(0);
        return root.Element;
    }

    public TElement Peek()
    {
        // TODO: ����
        // 1. �� ť üũ �� ���� ó��
        // 2. ��Ʈ ��� ��ȯ

        if (heap.Count == 0) throw new InvalidOperationException("heap �� �������");

        return heap[0].Element;
    }

    public void Clear()
    {
        // TODO: ����
        heap.Clear();
    }

    private void HeapifyUp(int index)
    {
        // TODO: ����
        // ���� ��尡 �θ𺸴� ������ ��ȯ�ϸ� ���� �̵�

        if (index < 0 || index >= heap.Count) throw new InvalidOperationException("��ȿ�� �ε����� �ƴմϴ�.");

        while (index > 0)
        {
            var parentIndex = (index - 1) / 2;
            if (heap[index].Priority.CompareTo(heap[parentIndex].Priority) >= 0) break;
            var temp = heap[index];
            heap[index] = heap[parentIndex];
            heap[parentIndex] = temp;
            index  = parentIndex;
        } 
    }

    private void HeapifyDown(int index)
    {
        // TODO: ����
        // ���� ��尡 �ڽĺ��� ũ�� �� ���� �ڽİ� ��ȯ�ϸ� �Ʒ��� �̵�

        ///-���� �ڽ�: `2 * i + 1`
        ///-������ �ڽ�: `2 * i + 2`
        ///-�θ�: `(i - 1) / 2` (���� ������)

        while (true)
        {
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;
            int smallestIndex = index;

            if (leftChildIndex <= heap.Count -1 && heap[leftChildIndex].Priority.CompareTo(heap[smallestIndex].Priority) < 0)
            {
                smallestIndex = leftChildIndex;
            }

            if (rightChildIndex <= heap.Count - 1 && heap[rightChildIndex].Priority.CompareTo(heap[smallestIndex].Priority) < 0)
            {
                smallestIndex = rightChildIndex;
            }

            if (smallestIndex == index) break;

            var temp = heap[smallestIndex];
            heap[smallestIndex] = heap[index];
            heap[index] = temp;
            index = smallestIndex;
        }
    }
}