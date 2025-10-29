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
        // TODO: 구현
        // 1. 새 요소를 리스트 끝에 추가
        // 2. HeapifyUp으로 힙 속성 복구

        heap.Add((element, priority));
        HeapifyUp(Count - 1);
    }

    public TElement Dequeue()
    {
        // TODO: 구현
        // 1. 빈 큐 체크 및 예외 처리
        // 2. 루트 요소 저장
        // 3. 마지막 요소를 루트로 이동
        // 4. HeapifyDown으로 힙 속성 복구
        // 5. 저장된 루트 요소 반환

        if (heap.Count == 0) throw new InvalidOperationException(" heap 이 비어있음");

        var root = heap[0];
        heap[0] = heap[Count - 1];
        heap.RemoveAt(Count - 1);
        HeapifyDown(0);
        return root.Element;
    }

    public TElement Peek()
    {
        // TODO: 구현
        // 1. 빈 큐 체크 및 예외 처리
        // 2. 루트 요소 반환

        if (heap.Count == 0) throw new InvalidOperationException("heap 이 비어있음");

        return heap[0].Element;
    }

    public void Clear()
    {
        // TODO: 구현
        heap.Clear();
    }

    private void HeapifyUp(int index)
    {
        // TODO: 구현
        // 현재 노드가 부모보다 작으면 교환하며 위로 이동

        if (index < 0 || index >= heap.Count) throw new InvalidOperationException("유효한 인덱스가 아닙니다.");

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
        // TODO: 구현
        // 현재 노드가 자식보다 크면 더 작은 자식과 교환하며 아래로 이동

        ///-왼쪽 자식: `2 * i + 1`
        ///-오른쪽 자식: `2 * i + 2`
        ///-부모: `(i - 1) / 2` (정수 나눗셈)

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