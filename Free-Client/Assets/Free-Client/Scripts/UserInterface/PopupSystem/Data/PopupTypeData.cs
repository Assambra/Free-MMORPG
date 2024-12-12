using Assambra.FreeClient.UserInterface.PopupSystem.Enum;
using System;
using UnityEngine;

namespace Assambra.FreeClient.UserInterface.PopupSystem.Data
{
    [Serializable]
    public class PopupTypeData
    {
        public PopupType Type;
        public Color TitleColor;
        public Sprite Icon;
        public Color IconColor;
        public AudioClip PlaySound;
    }
}
