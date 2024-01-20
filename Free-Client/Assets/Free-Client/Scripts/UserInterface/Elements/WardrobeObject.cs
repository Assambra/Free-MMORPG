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

    private DynamicCharacterAvatar avatar;

    private Dictionary<string, int> options = new Dictionary<string, int>();
    
    private Dictionary<UMATextRecipe, int> recipies = new Dictionary<UMATextRecipe, int>();

    private UMATextRecipe lastRecipe;

    private void Awake()
    {
        dropdownWardrobe.ClearOptions();
    }

    public void InitializeWardrobe(DynamicCharacterAvatar avatar, string slotname)
    {
        textWardrobeName.text = slotname;
        this.avatar = avatar;

        CreateOptions(slotname);
    }

    private void CreateOptions(string slotname)
    {
        List<UMATextRecipe> slotRecipes = avatar.AvailableRecipes[slotname];
        
        options.Add("None", 0);

        int i = 1;
        foreach (UMATextRecipe utr in slotRecipes)
        {
            string name = GetRecipeNameOrDisplayValue(utr);
            
            recipies.Add(utr, i);
            options.Add(name, i);

            i++;
        }

        dropdownWardrobe.AddOptions(options.Keys.ToList());
        PreselectionOption(options);

        dropdownWardrobe.onValueChanged.AddListener(delegate {
            OnDropdownValueChanged(dropdownWardrobe);
        });
    }

    private void PreselectionOption(Dictionary<string, int> options)
    {
        Dictionary<string, UMATextRecipe> avatarRecipies = avatar.WardrobeRecipes;

        foreach (KeyValuePair<string, UMATextRecipe> recipe in avatarRecipies)
        {
            string name = GetRecipeNameOrDisplayValue(recipe.Value);

            if (options.ContainsKey(name))
            {
                dropdownWardrobe.SetValueWithoutNotify(options[name]);
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
        foreach(KeyValuePair<UMATextRecipe, int> recipe in recipies)
        {
            if(change.value != 0)
            {
                if (change.value == recipe.Value)
                {
                    avatar.SetSlot(recipe.Key);
                    avatar.BuildCharacter(true);
                    lastRecipe = recipe.Key;
                }
            }
            else
            {
                avatar.ClearSlot(lastRecipe.wardrobeSlot);
                avatar.BuildCharacter(true);
            }
        }
    }
}
