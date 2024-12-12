using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assambra.FreeClient.Utilities;
using Assambra.FreeClient.UserInterface.PopupSystem.Enum;
using Assambra.FreeClient.UserInterface.PopupSystem.Manager;

namespace Assambra.FreeClient.UserInterface.PopupSystem.Popup
{
    public abstract class BasePopup : MonoBehaviour
    {
        [SerializeField] private Image _imageWindowIcon;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private Button _buttonClose;
        [SerializeField] private AudioSource _audioSource;

        public virtual void Setup(PopupType type, string title, string information, Delegate primaryCallback = null, Delegate secondaryCallback = null)
        {
            ApplyPopupType(type);

            _titleText.text = title;
            _infoText.text = information;

            if (!ValidateComponents(new (UnityEngine.Object, string)[]
            {
                (_buttonClose, "Close button")
            }))
                return;

            _buttonClose.onClick.RemoveAllListeners();
            _buttonClose.onClick.AddListener(() => { OnButtonClose(); });
        }

        private void ApplyPopupType(PopupType type)
        {
            var titlecolor = PopupManager.Instance.GetTitleColor(type);
            if(_titleText != null && titlecolor != null)
            {
                _titleText.color = titlecolor;
            }

            var icon = PopupManager.Instance.GetIcon(type);
            if (_imageWindowIcon != null && icon != null)
            {
                _imageWindowIcon.sprite = icon;
            }
            
            var color = PopupManager.Instance.GetIconColor(type);
            if (_imageWindowIcon != null && color != null)
            {
                _imageWindowIcon.color = color;
            }

            var audioclip = PopupManager.Instance.GetPlaySound(type);
            if(_audioSource != null && audioclip != null)
            {
                _audioSource.clip = audioclip;
                _audioSource.Play();
            }
        }

        public virtual bool ValidateComponents((UnityEngine.Object component, string name)[] components)
        {
            bool hasErrors = false;

            foreach (var (component, name) in components)
            {
                if (component == null)
                {
                    CustomLogger.LogWarning($"{GetType().Name}: The {name} is not assigned.");
                    hasErrors = true;
                }
            }

            return !hasErrors;
        }

        public abstract void OnButtonClose();

        public virtual void Destroy()
        {
            if (_buttonClose != null)
                _buttonClose.onClick.RemoveAllListeners();

            Destroy(gameObject);
        }
    }
}
