using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UMA;
using UMA.CharacterSystem;
using Newtonsoft.Json.Linq;
using static UnityEngine.UI.GridLayoutGroup;

public class SliderObject : MonoBehaviour
{
    [SerializeField] TMP_Text textSliderName;
    [SerializeField] Slider slider;

    private UMADnaBase owner;
    private DynamicCharacterAvatar avatar;
    private int index;
    private DNARangeAsset dNARangeAsset;

    private bool isInitialized = false;

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
    }

    public void InitializeSlider(string sliderName, string dnaName, float currentValue, int index, DynamicCharacterAvatar avatar, UMADnaBase owner)
    {
        textSliderName.text = sliderName;
        slider.value = currentValue;
        this.index = index;
        this.avatar = avatar;
        this.owner = owner;

        DNARangeAsset[] dnaRangeAssets = avatar.activeRace.data.dnaRanges;
        foreach (DNARangeAsset d in dnaRangeAssets)
        {
            if (d.ContainsDNARange(index, dnaName))
            {
                dNARangeAsset = d;
                return;
            }
        }

        isInitialized = true;
    }

    public void OnSliderValueChanged()
    {
        if(isInitialized)
        {
            float value = slider.value;

            if (dNARangeAsset == null) //No specified DNA Range Asset for this DNA
            {
                owner.SetValue(index, value);
                avatar.ForceUpdate(true, false, false);
                return;
            }

            if (dNARangeAsset.ValueInRange(index, value))
            {
                owner.SetValue(index, value);
                avatar.ForceUpdate(true, false, false);
                return;
            }
            else
            {
                Debug.LogWarning ("DNA Value out of range!");
            }
        }
    }
}
