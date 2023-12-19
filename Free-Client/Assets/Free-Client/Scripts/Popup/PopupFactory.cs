using UnityEngine;

public class PopupFactory<T> : IPopupFactory where T : BasePopup
{
    private T popupInstance;

    public BasePopup CreatePopup()
    {
        if (popupInstance == null || popupInstance.gameObject == null)
        {
            var prefab = Resources.Load<T>(typeof(T).Name);
            if (prefab == null)
            {
                Debug.LogError("Prefab konnte nicht geladen werden: " + "Prefabs/" + typeof(T).Name);
                return null;
            }
            popupInstance = GameObject.Instantiate(prefab, PopupManager.Instance.UIHandler.Canvas);
        }

        return popupInstance;
    }
}