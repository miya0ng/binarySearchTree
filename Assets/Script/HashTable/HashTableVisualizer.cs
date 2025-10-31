using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class HashTableVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Layout Settings")]
    [SerializeField] private float nodeSpacing = 10f;
    [SerializeField] private float nodeHeight = 60f;

    [Header("Color Settings")]
    [SerializeField] private Color emptyBucketColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color occupiedBucketColor = new Color(0.5f, 0.8f, 1f, 1f);
    [SerializeField] private Color collisionBucketColor = new Color(1f, 0.7f, 0.5f, 1f);

    private List<GameObject> nodeObjects = new List<GameObject>();

    private void Awake()
    {
        if (contentTransform == null && scrollRect != null)
        {
            contentTransform = scrollRect.content;
        }
    }

    public void Clear()
    {
        foreach (GameObject nodeObj in nodeObjects)
        {
            if (nodeObj != null)
            {
                Destroy(nodeObj);
            }
        }
        nodeObjects.Clear();
    }

    public void VisualizeChainingHashTable<TKey, TValue>(ChaningHashTable<TKey, TValue> hashTable)
    {
        Clear();

        if (hashTable == null || contentTransform == null || nodePrefab == null)
        {
            Debug.LogWarning("HashTableVisualizer: Missing required references");
            return;
        }

        // ChaningHashTable의 내부 buckets에 접근하기 위해 리플렉션 사용
        var bucketsField = typeof(ChaningHashTable<TKey, TValue>).GetField("buckets",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (bucketsField == null)
        {
            Debug.LogError("Cannot access buckets field");
            return;
        }

        var buckets = bucketsField.GetValue(hashTable) as LinkedList<KeyValuePair<TKey, TValue>>[];

        if (buckets == null)
        {
            Debug.LogError("Buckets is null");
            return;
        }

        VisualizeBuckets(buckets);
    }

    public void VisualizeOpenAddressingHashTable<TKey, TValue>(OpenAddressingHashTable<TKey, TValue> hashTable)
    {
        Clear();

        if (hashTable == null || contentTransform == null || nodePrefab == null)
        {
            Debug.LogWarning("HashTableVisualizer: Missing required references");
            return;
        }

        // OpenAddressingHashTable의 내부 table에 접근
        var tableField = typeof(OpenAddressingHashTable<TKey, TValue>).GetField("table",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (tableField == null)
        {
            Debug.LogError("Cannot access table field");
            return;
        }

        var table = tableField.GetValue(hashTable) as KeyValuePair<TKey, TValue>[];

        if (table == null)
        {
            Debug.LogError("Table is null");
            return;
        }

        VisualizeArray(table, hashTable);
    }

    private void VisualizeBuckets<TKey, TValue>(LinkedList<KeyValuePair<TKey, TValue>>[] buckets)
    {
        for (int i = 0; i < buckets.Length; i++)
        {
            var bucket = buckets[i];

            if (bucket == null || bucket.Count == 0)
            {
                // 빈 버킷
                CreateNodeUI(i, "Empty", "Empty", emptyBucketColor);
            }
            else if (bucket.Count == 1)
            {
                // 단일 항목
                var kvp = bucket.First.Value;
                CreateNodeUI(i, kvp.Key.ToString(), kvp.Value.ToString(), occupiedBucketColor);
            }
            else
            {
                // 충돌이 있는 버킷 - 체이닝으로 여러 항목
                foreach (var kvp in bucket)
                {
                    CreateNodeUI(i, kvp.Key.ToString(), kvp.Value.ToString(), collisionBucketColor);
                }
            }
        }

        UpdateContentSize();
    }

    private void VisualizeArray<TKey, TValue>(KeyValuePair<TKey, TValue>[] items, OpenAddressingHashTable<TKey, TValue> hashTable)
    {
        var occupiedField = typeof(OpenAddressingHashTable<TKey, TValue>).GetField("occupied",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        bool[] occupied = occupiedField?.GetValue(hashTable) as bool[];

        for (int i = 0; i < items.Length; i++)
        {
            if (occupied != null && !occupied[i])
            {
                // 빈 슬롯
                CreateNodeUI(i, "Empty", "Empty", emptyBucketColor);
            }
            else
            {
                var item = items[i];
                CreateNodeUI(i, item.Key.ToString(), item.Value.ToString(), occupiedBucketColor);
            }
        }

        UpdateContentSize();
    }

    private void CreateNodeUI(int index, string key, string value, Color color)
    {
        GameObject nodeObj = Instantiate(nodePrefab, contentTransform);
        HashTableNode nodeComponent = nodeObj.GetComponent<HashTableNode>();

        if (nodeComponent == null)
        {
            nodeComponent = nodeObj.AddComponent<HashTableNode>();
        }

        nodeComponent.SetData(index, key, value);
        nodeComponent.SetColor(color);

        nodeObjects.Add(nodeObj);
    }

    private void UpdateContentSize()
    {
        if (contentTransform == null) return;

        RectTransform contentRect = contentTransform as RectTransform;
        if (contentRect == null) return;

        float totalHeight = nodeObjects.Count * (nodeHeight + nodeSpacing) + nodeSpacing;
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);

        // LayoutGroup이 있다면 레이아웃 재계산
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
    }

    // 테스트용 메서드들
    public void OnTableOptions(int index)
    {
        switch(index)
        {
            case 0:
                TestOpenAddressingHashTable();
                break;
            case 1:
                TestChainingHashTable();
                break;
        }
    }
    public void TestChainingHashTable()
    {
        var hashTable = new ChaningHashTable<string, int>();
        hashTable.Add("apple", 1);
        hashTable.Add("banana", 2);
        hashTable.Add("cherry", 3);
        hashTable.Add("date", 4);
        hashTable.Add("elderberry", 5);

        VisualizeChainingHashTable(hashTable);
    }

    public void TestOpenAddressingHashTable()
    {
        var hashTable = new OpenAddressingHashTable<string, int>();
        hashTable.Add("apple", 1);
        hashTable.Add("banana", 2);
        hashTable.Add("cherry", 3);
        hashTable.Add("date", 4);
        hashTable.Add("elderberry", 5);

        VisualizeOpenAddressingHashTable(hashTable);
    }
}

