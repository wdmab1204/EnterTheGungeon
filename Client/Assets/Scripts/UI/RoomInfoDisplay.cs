using System;
using System.Collections;
using System.Collections.Generic;
using DataSequence.Tree;
using GameEngine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RoomInfoDisplay : MonoBehaviour
{
    public Text textPrefab;

    private Dictionary<string, Text> cacheText = new();

    private void Awake()
    {
        textPrefab.gameObject.SetActive(false);
    }

    public void SetUI(TreeNode treeNode)
    {
        if (treeNode == null)
            return;
        
        var position = treeNode.DungeonSize.GetCenterInt();

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(new Vector3(position.x, position.y, 0));

        string name = treeNode.Name;
        Text newText = GetText(name);
        newText.text = $"Name : {name}\nSize : {treeNode.DungeonSize.GetSize()}\nPos : {position}";
        
        RectTransform rectTransform = newText.GetComponent<RectTransform>();
        rectTransform.position = screenPosition;
        
        SetUI(treeNode?.LeftNode);
        SetUI(treeNode?.RightNode);
    }

    private Text GetText(string key)
    {
        if (cacheText.TryGetValue(key, out var text) == false)
        {
            Text newText = Instantiate(textPrefab, transform);
            cacheText.Add(key, newText);
            newText.gameObject.SetActive(true);
            newText.name = key;
            newText.color = new(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            return newText;
        }

        return text;
    }
}
