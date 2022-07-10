using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chatbubble : MonoBehaviour
{
    GameObject background;
    TextMeshPro textMeshPro;
    List<string> textList = new List<string>();
    int textIndex = 0;
    [SerializeField]
    TextAsset textAsset;

    [SerializeField]
    bool testSkipText;

    private void Awake()
    {
        background = transform.Find("Background").GetComponent<GameObject>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        if (textAsset != null)
        {
            foreach (string line in textAsset.text.Split('#'))
            {
                textList.Add(line);
            }
            SetText();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (testSkipText)
        {
            NextText();
            testSkipText = false;
        }
    }

    private void SetText()
    {
        textMeshPro.text = "";
        textMeshPro.text = textList[textIndex];
    }

    public void NextText()
    {
        if (textIndex == textList.Count-2)
        {
            gameObject.SetActive(false);
        }
        else
        {
            textIndex++;
            SetText();
        }

    }
}
