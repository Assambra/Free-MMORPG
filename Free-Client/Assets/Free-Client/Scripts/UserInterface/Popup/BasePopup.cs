using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assambra.FreeClient.Utilities;

namespace Assambra.FreeClient.UserInterface
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

            if (_buttonClose == null)
                CustomLogger.LogWarning("BasePopup: The close button is not assigned.");
            else
            {
                _buttonClose.onClick.RemoveAllListeners();
                _buttonClose.onClick.AddListener(() => { OnButtonClose(); });
            }
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
