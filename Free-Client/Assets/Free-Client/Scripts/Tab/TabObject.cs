using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabObject : MonoBehaviour
{
    [SerializeField] private TMP_Text _textTitle;
    [SerializeField] private RectTransform _windowRectTransform;
    [SerializeField] public RectTransform _topBorderLeft;
    [SerializeField] public RectTransform _topBorderRight;
    [SerializeField] private float _windowBorder = 3f;
    [SerializeField] private RectTransform _center;

    private ContentSizeFitter[] _fitters;

    private bool _unconstrained;
    private bool _preferred;

    
    private RectTransform _tabRectTransform;
    
    private float _windowWidth;
    private float _tabWidth;
    private float _topBorderLeftWidth;

    private RectTransform _parentRectTransform;
    private float _parentLeft;
    private float _parentRight;

    private bool _doOnce = false;

    private void Awake()
    {
        _tabRectTransform = gameObject.GetComponent<RectTransform>();
        _parentRectTransform = transform.parent.GetComponent<RectTransform>();

        _parentLeft = _parentRectTransform.offsetMin.x;
        _parentRight = _parentRectTransform.offsetMax.x;

        _fitters = gameObject.GetComponentsInChildren<ContentSizeFitter>();
        SetContentSizeFitterUnconstrained();
    }

    void Update()
    {
        _windowWidth = _windowRectTransform.sizeDelta.x;

        if (_windowWidth < _textTitle.preferredWidth - _parentRight + _parentLeft && !_unconstrained)
            SetContentSizeFitterUnconstrained();
        else if (_windowWidth > _textTitle.preferredWidth - _parentRight + _parentLeft && !_preferred)
            SetContentSizeFitterPreferred();

        if(!_doOnce)
        {
            _doOnce = true;
            SetupTopBorder();
        }
    }

    private void SetupTopBorder()
    {
        _tabWidth = _tabRectTransform.sizeDelta.x;
        _topBorderLeftWidth = _topBorderLeft.sizeDelta.x;

        _topBorderRight.sizeDelta = new Vector2(_windowWidth - (_windowBorder * 2) - _topBorderLeftWidth - _tabWidth, _topBorderRight.sizeDelta.y);
    }

    private void SetContentSizeFitterPreferred()
    {
        _preferred = true;
        _unconstrained = false;

        foreach (ContentSizeFitter fitter in _fitters)
        {
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    private void SetContentSizeFitterUnconstrained()
    {
        _unconstrained = true;
        _preferred = false;

        foreach (ContentSizeFitter fitter in _fitters)
        {
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
    }
}
