using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] private string tooltiptext;
        [SerializeField] private float delayToShowInfo = 1f;
        [SerializeField] private GameObject _prefabTooltipElement;

        private float timer;
        private bool startTimer;
        private GameObject tooltipGameObject;

        private RectTransform parentRectTransform;
        private float parentWidth;
        private float parentHeight;

        private void OnDestroy()
        {
            if (tooltipGameObject != null)
            {
                RemoveTooltip();
            }
        }

        private void Awake()
        {
            parentRectTransform = gameObject.gameObject.GetComponent<RectTransform>();
            parentHeight = parentRectTransform.sizeDelta.y;
            parentWidth = parentRectTransform.sizeDelta.x;
        }

        private void Update()
        {
            if (startTimer)
            {
                if (timer >= delayToShowInfo)
                {
                    if (tooltipGameObject == null)
                    {
                        CreateToolTip();
                    }
                    else
                    {
                        if (MouseMoved())
                        {
                            StopTimer();
                            RemoveTooltip();
                        }
                    }
                }

                timer += Time.deltaTime;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StartTimer();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopTimer();
            RemoveTooltip();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StopTimer();
            RemoveTooltip();
        }

        private void StartTimer()
        {
            startTimer = true;
        }

        private void StopTimer()
        {
            startTimer = false;
            timer = 0;
        }

        private void CreateToolTip()
        {
            tooltipGameObject = Instantiate(_prefabTooltipElement, GameManager.Instance.UIHandler.Canvas.transform);
            TooltipElement te = tooltipGameObject.GetComponent<TooltipElement>();
            te.SetTooltipText(tooltiptext);

            RectTransform rectTransform = tooltipGameObject.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

            Vector2 tooltipSize = te.GetTooltipSize();

            float newTooltipX = tooltipSize.x / 2;
            float newTooltipY = tooltipSize.y / 2;

            rectTransform.position = new Vector3(parentRectTransform.position.x - newTooltipX, parentRectTransform.position.y + newTooltipY + parentHeight, 0);
        }

        private void RemoveTooltip()
        {
            Destroy(tooltipGameObject);
        }

        private bool MouseMoved()
        {
            return Input.GetAxis("Mouse X") > 0.01f || Input.GetAxis("Mouse X") < -0.01f ||
                   Input.GetAxis("Mouse Y") > 0.01f || Input.GetAxis("Mouse Y") < -0.01f;
        }
    }
}
