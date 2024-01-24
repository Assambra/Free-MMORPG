using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UMA;
using UMA.CharacterSystem;

public class SliderElement : MonoBehaviour
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

    public void InitializeSlider(string sliderName, string dnaName, float currentValue, int index, DynamicCharacterAvatar avatar, UMADnaBase owner, bool disableName = false, float width = 220f)
    {
        SetWidth(width);

        textSliderName.text = sliderName;
        slider.value = currentValue;
        this.index = index;
        this.avatar = avatar;
        this.owner = owner;

        if(disableName)
            DisableName();

        dNARangeAsset = GetRangeAsset(dnaName);

        if(dNARangeAsset.ContainsDNARange(index, dnaName))
        {
            slider.minValue = dNARangeAsset.means[index] - dNARangeAsset.spreads[index];
            slider.maxValue = dNARangeAsset.means[index] + dNARangeAsset.spreads[index];
        }

        isInitialized = true;
    }

    private void DisableName()
    {
        textSliderName.gameObject.SetActive(false);
    }

    private void SetWidth(float width)
    {
        RectTransform sliderObjectRectTransform = this.gameObject.GetComponent<RectTransform>();
        sliderObjectRectTransform.sizeDelta = new Vector2(width, sliderObjectRectTransform.rect.height);
        RectTransform sliderRectTransform = slider.GetComponent<RectTransform>();
        sliderRectTransform.sizeDelta = new Vector2(width, sliderRectTransform.rect.height);
    }

    private DNARangeAsset GetRangeAsset(string dnaName)
    {
        DNARangeAsset[] dnaRangeAssets = avatar.activeRace.data.dnaRanges;
        foreach (DNARangeAsset d in dnaRangeAssets)
        {
            if (d.ContainsDNARange(index, dnaName))
            {
                return d;
            }
        }
        return null;
    }

    private void OnSliderValueChanged()
    {
        if(isInitialized)
        {
            owner.SetValue(index, slider.value);
            avatar.ForceUpdate(true, false, false);
        }
    }
}
