using Assambra.FreeClient.UserInterface.PopupSystem;
using Assambra.FreeClient.UserInterface.PopupSystem.Enum;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface.PopupSystem.Popup
{
    public class YesNoPopup : BasePopup
    {
        [SerializeField] private Button _buttonYes;
        [SerializeField] private Button _buttonNo;

        private Delegate _primaryCallback;

        public override void Setup(PopupType type, string title, string information, Delegate primaryCallback, Delegate secondaryCallback = null)
        {
            base.Setup(type, title, information, primaryCallback, secondaryCallback);
            
            _primaryCallback = primaryCallback;

            if (!ValidateComponents(new (UnityEngine.Object, string)[]
            {
                (_buttonYes, "Yes button"),
                (_buttonNo, "No button")
            }))
                return;

            _buttonYes.onClick.RemoveAllListeners();
            _buttonYes.onClick.AddListener(() =>
            {
                (_primaryCallback as Action<bool>)?.Invoke(true);
                Destroy();
            });

            _buttonNo.onClick.RemoveAllListeners();
            _buttonNo.onClick.AddListener(() =>
            {
                (_primaryCallback as Action<bool>)?.Invoke(false);
                Destroy();
            });
        }

        public override void OnButtonClose()
        {
            (_primaryCallback as Action<bool>)?.Invoke(false);
            Destroy();
        }

        public override void Destroy()
        {
            if (_buttonYes != null) 
                _buttonYes.onClick.RemoveAllListeners();
            if (_buttonNo != null) 
                _buttonNo.onClick.RemoveAllListeners();

            base.Destroy();
        }
    }
}