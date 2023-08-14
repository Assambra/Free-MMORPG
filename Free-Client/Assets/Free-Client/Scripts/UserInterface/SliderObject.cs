using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SliderObject : MonoBehaviour
{
    [SerializeField] TMP_Text sliderName;

    public void SetSliderName(string name)
    {
        sliderName.text = name;
    }
}
