using Assambra.FreeClient.UserInterface.PopupSystem.Enum;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface.PopupSystem.Popup
{
    public class InfoPopup : BasePopup
    {
        [SerializeField] Button _buttonOK;

        public override void Setup(PopupType type, string title, string information, Delegate primaryCallback, Delegate secondaryCallback)
        {
            base.Setup(type, title, information, primaryCallback);

            if (!ValidateComponents(new (UnityEngine.Object, string)[]
            {
                (_buttonOK, "OK button")
            }))
                return;

            _buttonOK.onClick.RemoveAllListeners();
            _buttonOK.onClick.AddListener(() =>
            {
                (primaryCallback as Action)?.Invoke();
                Destroy();
            });
        }

        public override void OnButtonClose()
        {
            Destroy();
        }

        public override void Destroy()
        {
            if (_buttonOK != null)
                _buttonOK.onClick.RemoveAllListeners();
            
            base.Destroy();
        }
    }
}
