using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabButton : MonoBehaviour
{
    [SerializeField] private GameObject tabColorizer;

    public void SetColorizerActiveState()
    {
        tabColorizer.SetActive(!tabColorizer.activeSelf);  
    }

    public bool GetColorizerActiveState()
    {
        return tabColorizer.activeSelf;
    }
}
