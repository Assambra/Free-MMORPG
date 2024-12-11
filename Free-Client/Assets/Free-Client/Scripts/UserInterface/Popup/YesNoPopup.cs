using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class YesNoPopup : BasePopup
    {
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;

        private Delegate _primaryCallback;

        public override void Setup(string title, string information, Delegate primaryCallback, Delegate secondaryCallback = null)
        {
            base.Setup(title, information, primaryCallback, secondaryCallback);
            
            _primaryCallback = primaryCallback;

            if (!ValidateComponents(new (UnityEngine.Object, string)[]
            {
                (_yesButton, "Yes button"),
                (_noButton, "No button")
            }))
                return;

            _yesButton.onClick.RemoveAllListeners();
            _yesButton.onClick.AddListener(() =>
            {
                (_primaryCallback as Action<bool>)?.Invoke(true);
                Destroy();
            });

            _noButton.onClick.RemoveAllListeners();
            _noButton.onClick.AddListener(() =>
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
            if (_yesButton != null) 
                _yesButton.onClick.RemoveAllListeners();
            if (_noButton != null) 
                _noButton.onClick.RemoveAllListeners();

            base.Destroy();
        }
    }
}