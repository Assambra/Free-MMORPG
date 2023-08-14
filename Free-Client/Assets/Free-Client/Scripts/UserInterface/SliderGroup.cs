using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SliderGroup : MonoBehaviour
{
    public RectTransform CreateCharacterLayoutGroup;

    [SerializeField] private Button headerGroupeButton;
    [SerializeField] private Sprite rightArrow;
    [SerializeField] private Sprite downArrow;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private GameObject prefabSliderObject;
    [SerializeField] private Transform slidersHome;

    [SerializeField] RectTransform layoutGroup;

    private bool isGroupeOpen = false;

    private List<GameObject> sliders = new List<GameObject>();

    public void OnHeaderGroupeButton()
    {
        if(!isGroupeOpen)
        {
            isGroupeOpen = true;
            headerGroupeButton.image.sprite = downArrow;
            foreach(GameObject slider in sliders)
            {
                slider.SetActive(true);
            }
            //LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);
            LayoutRebuilder.ForceRebuildLayoutImmediate(CreateCharacterLayoutGroup);
        }
        else
        {
            isGroupeOpen = false;
            headerGroupeButton.image.sprite = rightArrow;
            foreach (GameObject slider in sliders)
            {
                slider.SetActive(false);
            }
            //LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);
            LayoutRebuilder.ForceRebuildLayoutImmediate(CreateCharacterLayoutGroup);
        }
    }

    public void SetGroupName(string name)
    {
        headerText.text = name;
    }

    public void CreateSlider(string name)
    {
        GameObject go = Instantiate(prefabSliderObject, slidersHome);
        go.name = name;
        SliderObject sliderObject = go.GetComponent<SliderObject>();

        sliderObject.SetSliderName(name);

        sliders.Add(go);

        go.SetActive(false);
    }


    public void DestroySliders()
    {
        foreach(GameObject go in sliders) 
        {
            Destroy(go);
        }
    }
}
