using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneUISet", menuName = "Assambra/SceneUISet", order = 1)]
[System.Serializable]
public class SceneUISet : ScriptableObject
{
    public GameObject[] UIElementPrefabs;


    public void EnableUIElement(string name)
    {
        bool found = false;
        for (int i = 0; i < UIElementPrefabs.Length; i++)
        {
            if (UIElementPrefabs[i].name == name)
            {
                found = true;
                UIElement uIElement = UIElementPrefabs[i].GetComponent<UIElement>();
                if (!uIElement.IsActive())
                    uIElement.ChangeActiveState();
            }
        }
        if(!found)
        { Debug.LogError("Error: Dont find any UIElement with name: " + name); }
    }

    public void EnableUIElement(int index)
    {
        if (index <= UIElementPrefabs.Length)
        {
            UIElement uIElement = UIElementPrefabs[index].GetComponent<UIElement>();
            if (!uIElement.IsActive())
                uIElement.ChangeActiveState();
        }
        else
            Debug.LogError("Error: index: " + index + " is greater then array.Length");
    }

    public void DisableUIElement(string name)
    {
        bool found = false;
        for (int i = 0; i < UIElementPrefabs.Length; i++)
        {
            if (UIElementPrefabs[i].name == name)
            {
                found = true;
                UIElement uIElement = UIElementPrefabs[i].GetComponent<UIElement>();
                if (uIElement.IsActive())
                    uIElement.ChangeActiveState();
            }
        }
        if (!found)
        { Debug.LogError("Error: Dont find any UIElement with name: " + name); }
    }

    public void DisableUIElement(int index)
    {
        if (index <= UIElementPrefabs.Length)
        {
            UIElement uIElement = UIElementPrefabs[index].GetComponent<UIElement>();
            if (uIElement.IsActive())
                uIElement.ChangeActiveState();
        }
        else
            Debug.LogError("Error: index: " + index + " is greater then array.Length");
    }

    public string UIElementName(int index)
    {
        string retstr = "";
        if(index <= UIElementPrefabs.Length)
            retstr =  UIElementPrefabs[index].name;
        else
            Debug.LogError("Error: index: " + index + " is greater then array.Length");

        return retstr;
    }

    public void StartWithDisabledUIElements()
    {
        if(UIElementPrefabs.Length > 0)
        {
            for (int i = 0; i < UIElementPrefabs.Length; i++)
            {
                UIElement uIElement = UIElementPrefabs[i].GetComponent<UIElement>();
                if (!uIElement.StartActivated && uIElement.IsActive())
                {
                    uIElement.ChangeActiveState();
                }
            }
        }
    }
}
