using Assambra.GameFramework.MouseHandler;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assambra.FreeClient.UserInterface
{
    public class ButtonPressedChecker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool IsButtonPressed = false;

        private EventSystem eventSystem;
        private MouseHandler mouseHandler;

        private void Awake()
        {
            mouseHandler = GameObject.Find("MouseHandler").GetComponent<MouseHandler>();
            eventSystem = mouseHandler.GetEventSystem();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsButtonPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsButtonPressed = false;
        }
    }
}
