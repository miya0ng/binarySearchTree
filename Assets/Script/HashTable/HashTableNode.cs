using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HashTableNode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image backgroundImage;

    private void Awake()
    {
        if (text == null)
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
    }

    public void SetData(int index, string key, string value)
    {
        if (text != null)
        {
            text.text = $"Index: {index} Key: {key} Value: {value}";
        }
    }

    public void SetData<TKey, TValue>(int index, TKey key, TValue value)
    {
        if (text != null)
        {
            text.text = $"Index: {index} Key: {key} Value: {value}";
        }
    }

    public void SetColor(Color color)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = color;
        }
    }

    public void Clear()
    {
        if (text != null)
        {
            text.text = "Index: -1 Key: -1 Value: -1";
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = Color.white;
        }
    }
}
