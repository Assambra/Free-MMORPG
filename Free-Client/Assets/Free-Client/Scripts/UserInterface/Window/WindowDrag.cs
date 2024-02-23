using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDrag : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] private RectTransform DragRectTransform = null;
	
	public void OnDrag(PointerEventData eventData)
	{
		DragRectTransform.anchoredPosition += eventData.delta;
	}

    public void OnPointerDown(PointerEventData eventData)
    {			
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}
