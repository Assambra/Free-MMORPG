using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UMA.CharacterSystem;
using UMA;

public class SliderGroup : MonoBehaviour
{
    public RectTransform CreateCharacterLayoutGroup;

    [SerializeField] private Button headerGroupeButton;
    [SerializeField] private Sprite rightArrow;
    [SerializeField] private Sprite downArrow;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private GameObject prefabSliderObject;
    [SerializeField] private Transform slidersHome;

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
            LayoutRebuilder.ForceRebuildLayoutImmediate(CreateCharacterLayoutGroup);
        }
    }

    public void SetGroupName(string name)
    {
        headerText.text = name;
    }

    public void CreateSlider(string sliderName, string dnaName, float currentValue, int index, DynamicCharacterAvatar avatar, UMADnaBase owner)
    {
        GameObject go = Instantiate(prefabSliderObject, slidersHome);
        go.name = sliderName;
        SliderObject sliderObject = go.GetComponent<SliderObject>();

        sliderObject.InitializeSlider(sliderName, dnaName, currentValue, index, avatar, owner);

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
