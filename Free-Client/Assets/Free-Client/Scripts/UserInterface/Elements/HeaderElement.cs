using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeaderElement : MonoBehaviour
{
    [field: SerializeField] public List<GameObject> ObjectsList {get; private set;}

    [Space(5)]

    [Header("User interface references")]
    [SerializeField] private Button headerButton;
    [SerializeField] private Sprite arrowRight;
    [SerializeField] private Sprite arrowDown;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private Transform objectHome;

    // Private Variables
    
    private Dictionary<string, GameObject> prefabs;

    private bool isOpen = false;
    private RectTransform layoutGroup;


    private void OnEnable()
    {
        ObjectsList = new List<GameObject>();
    }

    public void InitializeHeaderElement(string headerText, Dictionary<string, GameObject> prefabs, RectTransform layoutGroup)
    {
        this.headerText.text = headerText;
        this.prefabs = prefabs;
        this.layoutGroup = layoutGroup;
    }

    public GameObject CreateObject(string key, string name)
    {
        foreach (KeyValuePair<string, GameObject> obj in prefabs)
        {
            if (obj.Key == key) 
            {
                GameObject go = Instantiate(obj.Value, objectHome);
                go.name = name;
                ObjectsList.Add(go);
                go.SetActive(false);
                return go;
            }
        }
        return null;
    }

    public void OnHeaderButton()
    {
        if (!isOpen)
        {
            isOpen = true;
            headerButton.image.sprite = arrowDown;
            foreach (GameObject obj in ObjectsList)
            {
                obj.SetActive(true);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(objectHome.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);
        }
        else
        {
            isOpen = false;
            headerButton.image.sprite = arrowRight;
            foreach (GameObject obj in ObjectsList)
            {
                obj.SetActive(false);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(objectHome.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);
        }
    }

    public void DestroyObjects()
    {
        foreach (GameObject obj in ObjectsList)
        {
            Destroy(obj);
        }
        ObjectsList.Clear();
    }
}
