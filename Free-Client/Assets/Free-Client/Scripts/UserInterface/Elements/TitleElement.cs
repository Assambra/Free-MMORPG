using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class TitleElement : MonoBehaviour
    {
        [Header("User interface references")]
        [SerializeField] private Button _titleButton;
        [SerializeField] private Sprite _arrowRight;
        [SerializeField] private Sprite _arrowDown;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Transform _elementsHome;

        [Space(5)]

        [Header("User interface references")]
        [SerializeField] private List<GameObject> _objectsList = new List<GameObject>();

        // Private Variables
        private bool _isOpen = false;
        private RectTransform _layout;
        private bool _isSubtitle;
        private RectTransform _parentLayout;

        public void InitializeHeaderElement(string headerText, RectTransform layout, bool isSubtitle = false, RectTransform parentLayout = null)
        {
            this._titleText.text = headerText;
            this._layout = layout;
            this._isSubtitle = isSubtitle;
            this._parentLayout = parentLayout;
        }

        public GameObject CreateObject(GameObject prefab, string name)
        {
            GameObject go = Instantiate(prefab, _elementsHome);
            go.name = name;
            _objectsList.Add(go);
            go.SetActive(false);
            return go;
        }

        public void OnHeaderButton()
        {
            if (!_isOpen)
            {
                _isOpen = true;
                _titleButton.image.sprite = _arrowDown;
                foreach (GameObject obj in _objectsList)
                {
                    obj.SetActive(true);
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(_elementsHome.GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(_layout);
                if (_isSubtitle)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_parentLayout);

            }
            else
            {
                _isOpen = false;
                _titleButton.image.sprite = _arrowRight;
                foreach (GameObject obj in _objectsList)
                {
                    obj.SetActive(false);
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(_elementsHome.GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(_layout);
                if (_isSubtitle)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_parentLayout);
            }
        }

        public void DestroyObjects()
        {
            foreach (GameObject obj in _objectsList)
            {
                Destroy(obj);
            }
            _objectsList.Clear();
        }

        public RectTransform GetParentLayout()
        {
            return _layout;
        }

        public RectTransform GetLayout()
        {
            return _elementsHome.GetComponent<RectTransform>();
        }
    }
}
