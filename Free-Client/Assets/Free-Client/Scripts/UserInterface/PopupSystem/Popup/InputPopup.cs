using Assambra.FreeClient.UserInterface.PopupSystem.Enum;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface.PopupSystem.Popup
{
    public class InputPopup : BasePopup
    {
        [SerializeField] private Button _buttonOK;
        [SerializeField] private Button _buttonCancel;
        [SerializeField] private TMP_InputField _inputFieldUserInput;

        private Delegate _secondaryCallback;

        public override void Setup(PopupType type, string title, string information, Delegate primaryCallback, Delegate secondaryCallback)
        {
            base.Setup(type, title, information, primaryCallback, secondaryCallback);

            _secondaryCallback = secondaryCallback;

            if (!ValidateComponents(new (UnityEngine.Object, string)[]
            {
                (_buttonOK, "OK button"),
                (_buttonCancel, "Cancel button"),
                (_inputFieldUserInput, "Input Field")
            }))
                return;
            
            _buttonOK.onClick.RemoveAllListeners();
            _buttonOK.onClick.AddListener(() =>
            {
                (primaryCallback as Action<string>)?.Invoke(_inputFieldUserInput.text);
                Destroy();
            });

            _buttonCancel.onClick.RemoveAllListeners();
            _buttonCancel.onClick.AddListener(() =>
            {
                (secondaryCallback as Action)?.Invoke();
                Destroy();
            });
        }

        public override void OnButtonClose()
        {
            (_secondaryCallback as Action)?.Invoke();
            Destroy();
        }

        public override void Destroy()
        {
            if (_buttonOK != null) 
                _buttonOK.onClick.RemoveAllListeners();
            if (_buttonCancel != null) 
                _buttonCancel.onClick.RemoveAllListeners();
            
            base.Destroy();
        }
    }
}

