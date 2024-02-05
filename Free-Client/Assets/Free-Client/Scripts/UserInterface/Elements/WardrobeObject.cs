using System.Collections.Generic;
using System.Linq;
using TMPro;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;

public class WardrobeObject : MonoBehaviour
{
    [SerializeField] TMP_Text textWardrobeName;
    [SerializeField] TMP_Dropdown dropdownWardrobe;
    [SerializeField] GameObject _prefabColorElement;
    [SerializeField] Transform _colorHome;
    private DynamicCharacterAvatar _avatar;

    private Dictionary<string, int> options = new Dictionary<string, int>();
    
    private Dictionary<UMATextRecipe, int> recipes = new Dictionary<UMATextRecipe, int>();
    private List<UMATextRecipe> _recipesToShow = new List<UMATextRecipe>();
    private UMATextRecipe lastRecipe;
    private Transform _allwaysOnTop;

    private List<GameObject> _activeColorElements = new List<GameObject>();

    private void Awake()
    {
        dropdownWardrobe.ClearOptions();
    }

    public void InitializeWardrobe(DynamicCharacterAvatar avatar, string slotname, Transform allwayOnTop, List<UMATextRecipe>[] recipesToShow = null, bool useCustomRecipesList = false)
    {
        textWardrobeName.text = slotname;
        this._avatar = avatar;
        this._allwaysOnTop = allwayOnTop;

        if(recipesToShow != null)
        {
            for (int i = 0; i < recipesToShow.Length; i++)
            {
                foreach (UMATextRecipe r in recipesToShow[i])
                {
                    _recipesToShow.Add(r);
                }
            }
        }
        
        CreateOptions(slotname, useCustomRecipesList);
    }

    private void CreateOptions(string slotname, bool useCustomRecipesList)
    {
        List<UMATextRecipe> slotRecipes = _avatar.AvailableRecipes[slotname];
        
        options.Add("None", 0);

        int i = 1;
        foreach (UMATextRecipe utr in slotRecipes)
        {
            if(_recipesToShow.Contains(utr) || !useCustomRecipesList)
            {
                string name = GetRecipeNameOrDisplayValue(utr);

                recipes.Add(utr, i);
                options.Add(name, i);

                i++;
            }
        }

        dropdownWardrobe.AddOptions(options.Keys.ToList());
        PreselectionOption(options);

        dropdownWardrobe.onValueChanged.AddListener(delegate {
            OnDropdownValueChanged(dropdownWardrobe);
        });
    }

    private void PreselectionOption(Dictionary<string, int> options)
    {
        Dictionary<string, UMATextRecipe> avatarRecipies = _avatar.WardrobeRecipes;

        foreach (KeyValuePair<string, UMATextRecipe> recipe in avatarRecipies)
        {
            string name = GetRecipeNameOrDisplayValue(recipe.Value);

            if (options.ContainsKey(name))
            {
                dropdownWardrobe.SetValueWithoutNotify(options[name]);
                OverlayColorData[] colors = recipe.Value.SharedColors;
                CreateColorSelectors(colors, _allwaysOnTop);
            }
        }
    }

    private string GetRecipeNameOrDisplayValue(UMATextRecipe recipe)
    {
        string name;

        if(string.IsNullOrEmpty(recipe.DisplayValue))
            name = recipe.name;
        else
            name = recipe.DisplayValue;

        return name;
    }

    private void OnDropdownValueChanged(TMP_Dropdown change)
    {
        foreach(KeyValuePair<UMATextRecipe, int> recipe in recipes)
        {
            if(change.value != 0)
            {
                if (change.value == recipe.Value)
                {   
                    ClearColors();
                    OverlayColorData[] colors = recipe.Key.SharedColors;
                    CreateColorSelectors(colors, _allwaysOnTop);
                    
                    _avatar.SetSlot(recipe.Key);
                    _avatar.BuildCharacter(true);
                    lastRecipe = recipe.Key;
                }
            }
            else
            {
                _avatar.ClearSlot(lastRecipe.wardrobeSlot);
                _avatar.BuildCharacter(true);
                ClearColors();
            }
        }
    }

    private void CreateColorSelectors(OverlayColorData[] sharedColors, Transform allwaysOnTop)
    {
        for (int i = 0; i < sharedColors.Length; i++)
        {
            GameObject go = Instantiate(_prefabColorElement, _colorHome);
            _activeColorElements.Add(go);
            ColorElement color = go.GetComponent<ColorElement>();
            color.Initialize(_avatar, sharedColors[i], sharedColors[i].name, allwaysOnTop);
        }
    }

    private void ClearColors()
    {
        foreach(GameObject go in _activeColorElements)
        {
            ColorElement color = go.GetComponent<ColorElement>();
            color.RemoveColorPicker();

            Destroy(go);
        }
        _activeColorElements.Clear();
    }
}
