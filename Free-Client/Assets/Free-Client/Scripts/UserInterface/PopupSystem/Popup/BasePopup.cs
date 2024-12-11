using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assambra.FreeClient.Utilities;

namespace Assambra.FreeClient.UserInterface.PopupSystem.Popup
{
    public abstract class BasePopup : MonoBehaviour
    {
        [SerializeField] private bool _showAsError;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _informationText;
        [SerializeField] private Button _buttonClose;

        public virtual void Setup(string title, string information, Delegate primaryCallback = null, Delegate secondaryCallback = null)
        {
            _titleText.text = title;
            _informationText.text = information;

            if (!ValidateComponents(new (UnityEngine.Object, string)[]
            {
                (_buttonClose, "Close button")
            }))
                return;

            _buttonClose.onClick.RemoveAllListeners();
            _buttonClose.onClick.AddListener(() => { OnButtonClose(); });
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
