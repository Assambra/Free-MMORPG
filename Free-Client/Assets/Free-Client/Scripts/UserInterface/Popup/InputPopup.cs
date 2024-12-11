using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class InputPopup : BasePopup
    {
        [SerializeField] private Button _oKButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private TMP_InputField _userInputField;

        private Delegate _secondaryCallback;

        public override void Setup(string title, string information, Delegate primaryCallback, Delegate secondaryCallback)
        {
            base.Setup(title, information, primaryCallback, secondaryCallback);

            _secondaryCallback = secondaryCallback;

            if (!ValidateComponents(new (UnityEngine.Object, string)[]
            {
                (_oKButton, "OK button"),
                (_cancelButton, "Cancel button"),
                (_userInputField, "Input Field")
            }))
                return;
            
            _oKButton.onClick.RemoveAllListeners();
            _oKButton.onClick.AddListener(() =>
            {
                (primaryCallback as Action<string>)?.Invoke(_userInputField.text);
                Destroy();
            });

            _cancelButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.AddListener(() =>
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
            if (_oKButton != null) 
                _oKButton.onClick.RemoveAllListeners();
            if (_cancelButton != null) 
                _cancelButton.onClick.RemoveAllListeners();
            
            base.Destroy();
        }
    }
}

