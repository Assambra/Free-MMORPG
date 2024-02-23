using UnityEngine;
using TMPro;

namespace Assambra.FreeClient.UserInterface
{
    public class TabObject : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textTitle;
        [SerializeField] private RectTransform _popupRectTransform;
        [SerializeField] private RectTransform _imageBorderRectTransform;
        [SerializeField] public RectTransform _topBorderLeft;
        [SerializeField] public RectTransform _topBorderRight;
        [SerializeField] private float _windowBorder = 3f;

        private float _popupWidth;
        private float _tabWidth;
        private float _topBorderLeftWidth;


        private bool _doOnce = true;

        private void Awake()
        {
        }

        void Update()
        {
            if (_textTitle.text != "")
            {
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(this.gameObject.GetComponent<RectTransform>());
                _doOnce = false;
            }

            if (!_doOnce)
            {
                _doOnce = true;

                SetupTopBorder();
            }
        }

        private void SetupTopBorder()
        {
            _popupWidth = _popupRectTransform.sizeDelta.x;
            _tabWidth = _imageBorderRectTransform.sizeDelta.x;
            _topBorderLeftWidth = _topBorderLeft.sizeDelta.x;

            _topBorderRight.sizeDelta = new Vector2(_popupWidth - (_windowBorder * 2) - _topBorderLeftWidth - _tabWidth, _topBorderRight.sizeDelta.y);
        }
    }
}
