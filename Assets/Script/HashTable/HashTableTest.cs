using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HashTableTest : MonoBehaviour
{
    private void Start()
    {
        var hashTable = new ChaningHashTable<string, int>();

        hashTable.Add("one", 1);
        hashTable.Add("two", 2);
        hashTable.Add("three", 3);

        foreach (var key in hashTable.Keys)
        {
            Debug.Log($"Key: {key}, Value: {hashTable[key]}");
        }

        Debug.Log($"Contains key 'two': {hashTable.ContainsKey("two")}");
        hashTable.Remove("two");
        Debug.Log($"Contains key 'two' after removal: {hashTable.ContainsKey("two")}");

        var array = new KeyValuePair<string, int>[5];
        hashTable.CopyTo(array, 0);
        Debug.Log("Copied to array:");
        foreach (var kvp in array)
        {
            if (kvp.Key != null)
                Debug.Log($"Copied Key: {kvp.Key}, Copied Value: {kvp.Value}");
        }
        hashTable.Clear();
        Debug.Log($"Count after clear: {hashTable.Count}");

        for (int i = 0; i < 100; i++)
        {
            hashTable.Add($"key{i}", i);
        }
        Debug.Log($"Count after adding 100 items: {hashTable.Count}");


        foreach (var key in hashTable.Keys)
        {
            Debug.Log($"Key: {key}, Value: {hashTable[key]}");
        }
    }
}