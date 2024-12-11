using Assambra.FreeClient.Utilities;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class ErrorPopup : BasePopup
    {
        [SerializeField] Button _oKButton;

        public override void Setup(string title, string information, Delegate primaryCallback, Delegate secondaryCallback)
        {
            base.Setup(title, information, primaryCallback);
            
            if(_oKButton == null)
                CustomLogger.LogWarning("ErrorPopup: The OK button is not assigned.");
            else
            {
                _oKButton.onClick.RemoveAllListeners();
                _oKButton.onClick.AddListener(() =>
                {
                    (primaryCallback as Action)?.Invoke();
                    Destroy();
                });
            }
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
