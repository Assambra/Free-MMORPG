using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class InformationPopup : BasePopup
    {
        [SerializeField] Button _oKButton;

        public override void Setup(string title, string information, Delegate primaryCallback, Delegate secondaryCallback)
        {
            base.Setup(title, information, primaryCallback);

            if (!ValidateComponents(new (UnityEngine.Object, string)[]
            {
                (_oKButton, "OK button")
            }))
                return;

            _oKButton.onClick.RemoveAllListeners();
            _oKButton.onClick.AddListener(() =>
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
            if (_oKButton != null) 
                _oKButton.onClick.RemoveAllListeners();
            
            base.Destroy();
        }
    }
}
