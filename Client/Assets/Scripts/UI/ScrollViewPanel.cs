using System;
using System.Collections;
using System.Collections.Generic;
using GameEngine;
using GameEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewPanel : MonoBehaviour
{
    //public GameObject[] scrollViewArr;
    public GameObject scrollViewGroup;
    public List<IScrollView<QuestCellModel>> scrollViewArr = new();
    private int index = 0;
    private float deltaTime;
    public TextMeshProUGUI text;
    public TextMeshProUGUI title;
    
    private void Start()
    {
        foreach(Transform transform in scrollViewGroup.transform)
        {
            scrollViewArr.Add(transform.GetComponent<IScrollView<QuestCellModel>>());
        }
        
        List<QuestCellModel> list = new();
        for (int i = 1; i <= 200; i++)
            list.Add(new() { title = $"TITLE_{i}", desc = $"DESC_{i}" });

        for (int i = 0; i < scrollViewArr.Count; i++)
        {
            scrollViewArr[i].Initialize();
            scrollViewArr[i].UpdateContent(list);
        }
        
        OnClickLeft();
    }

    public void OnClickLeft()
    {
        scrollViewArr[index].SetVisible(false);
        index = Mathf.Max(index - 1, 0);
        scrollViewArr[index].SetVisible(true);
        title.text = scrollViewArr[index].GetType().Name;
    }

    public void OnClickRight()
    {
        scrollViewArr[index].SetVisible(false);
        index = Mathf.Min(index + 1, scrollViewArr.Count - 1);
        scrollViewArr[index].SetVisible(true);
        title.text = scrollViewArr[index].GetType().Name;
    }

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / Time.deltaTime;

        // 텍스트 업데이트
        text.text = $"FPS: {fps:0.}";
    }
}
