using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI;

public class ChaningHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 16;
    private const double LoadFactor = 0.75;

    private int size;
    private int count;
    private LinkedList<KeyValuePair<TKey, TValue>>[] buckets;

    public ChaningHashTable()
    {
        buckets = new LinkedList<KeyValuePair<TKey, TValue>>[DefaultCapacity];
        size = DefaultCapacity;
        count = 0;
    }
    private int GetHash(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int hash = key.GetHashCode() & 0x7FFFFFFF;
        return hash % size;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }

            throw new KeyNotFoundException();
        }
        set
        {
            if (key == null)
                throw new ArgumentNullException();

            if ((double)count / size > LoadFactor)
            {
                Resize();
            }
        }
    }
    public ICollection<TKey> Keys => buckets
                                    .Where(bucket => bucket != null)
                                    .SelectMany(bucket => bucket)
                                    .Select(kvp => kvp.Key)
                                    .ToList();
    public ICollection<TValue> Values => buckets
                                        .Where(bucket => bucket != null)
                                        .SelectMany(bucket => bucket)
                                        .Select(kvp => kvp.Value)
                                        .ToList();

    public int Count => count;

    public bool IsReadOnly => false;

    private void Resize()
    {
        int newSize = size * 2;
        var newBuckets = new LinkedList<KeyValuePair<TKey, TValue>>[newSize];

        for (int i = 0; i < size; i++)
        {
            if (buckets[i] == null) return;
            foreach (var kvp in buckets[i])
            {
                int newIndex = Math.Abs(kvp.Key.GetHashCode()) % newSize;
                if (newBuckets[newIndex] == null)
                {
                    newBuckets[newIndex] = new LinkedList<KeyValuePair<TKey, TValue>>();
                }
            }
            buckets = newBuckets;
            size = newSize;
        }
    }


    public void Add(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException();

        if ((double)count / size > LoadFactor)
        {
            Resize();
        }

        int index = GetHash(key);

        if(buckets[index] == null)
        {
            buckets[index] = new LinkedList<KeyValuePair<TKey, TValue>>();
        }

        foreach(var kvp in buckets[index])
        {
            if(kvp.Key.Equals(key))
            {
                throw new ArgumentException("Key 존재");
            }
        }

        buckets[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
        count++;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Array.Clear(buckets, 0, size);
   
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (!ContainsKey(item.Key))
            return false;

        return true;
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null) throw new ArgumentException("key 없음"); 

        int index = GetHash(key);

        if (buckets[index] == null)
        {
            return false;
        }

        foreach (var kvp in buckets[index])
        {
            if (kvp.Key.Equals(key))
            {
                return true;
            }
        }
        return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        int index = arrayIndex;
        foreach (var kvp in this)
        {
            array[index++] = kvp;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
        {
            if (buckets[i] != null)
            {
                foreach (var kvp in buckets[i])
                    yield return kvp;
            }
        }
    }

    public bool Remove(TKey key)
    {
         if(!ContainsKey(key)) return false;

         int index = GetHash(key);

        if (buckets[index] == null)
        {
            return false;
        }

        var node = buckets[index].First;
        while (node != null)
        {
            if (node.Value.Key.Equals(key))
            {
                buckets[index].Remove(node);
                count--;
                return true;
            }
            node = node.Next;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        Remove(item.Key);
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        value = default;

        int index = GetHash(key);

        foreach (var kvp in buckets[index])
        {
            if (kvp.Key.Equals(key))
            {
                value = kvp.Value;
                return true;
            }
        }
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}