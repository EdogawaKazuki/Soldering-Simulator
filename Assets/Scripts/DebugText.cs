using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DebugText : MonoBehaviour
{
    public TMP_Text debugText;
    List<String> debugTextList = new List<String>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text = String.Join("\n", debugTextList);
    }
    public int RegisterDebugText(String text){
        debugTextList.Add(text);
        return debugTextList.Count - 1;
    }
    public void UpdateDebugText(int index, String text){
        debugTextList[index] = text;
    }
}
