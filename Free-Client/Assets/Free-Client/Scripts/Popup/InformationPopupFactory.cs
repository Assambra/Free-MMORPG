using UnityEngine;

public class InformationPopupFactory : IPopupFactory
{
    private InformationPopup _popupInstance;

    public BasePopup CreatePopup()
    {
        if(_popupInstance == null || _popupInstance.gameObject == null)
        {
            var prefab = Resources.Load<InformationPopup>("Popups/InformationPopup");
            if (prefab == null)
            {
                Debug.LogError("Prefab konnte nicht geladen werden: Prefabs/ConfirmationPopup");
                return null;
            }
            
            _popupInstance = GameObject.Instantiate(prefab, PopupManager.Instance.UIHandler.Canvas);
        }

        return _popupInstance;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
