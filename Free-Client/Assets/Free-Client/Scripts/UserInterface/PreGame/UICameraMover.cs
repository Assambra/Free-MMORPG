using UnityEngine;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class UICameraMover : MonoBehaviour
    {
        [SerializeField] private Button buttonCameraUp;
        [SerializeField] private Button buttonCameraDown;
        [SerializeField] private Toggle toggleAutoFocus;

        [SerializeField] private float cameraMoveSpeed = 0.25f;
        [SerializeField] private float cameraMinOffset = 0.27f;
        [SerializeField] private float cameraMaxOffset = 2.75f;

        private UICreateCharacter uICreateCharacter;

        private ButtonPressedChecker checkerButtonCameraUp;
        private ButtonPressedChecker checkerButtonCameraDown;

        private void Awake()
        {
            checkerButtonCameraUp = buttonCameraUp.gameObject.GetComponent<ButtonPressedChecker>();
            checkerButtonCameraDown = buttonCameraDown.gameObject.GetComponent<ButtonPressedChecker>();
        }

        private void Start()
        {
            uICreateCharacter = FindObjectOfType<UICreateCharacter>();
        }

        void Update()
        {
            if (checkerButtonCameraUp.IsButtonPressed)
                OnButtonCameraUpPressed();
            if (checkerButtonCameraDown.IsButtonPressed)
                OnButtonCameraDownPressed();

            uICreateCharacter.UseCameraAutoFocus = toggleAutoFocus.isOn;
        }

        private void OnButtonCameraUpPressed()
        {
            Vector3 currentCameraOffset = GameManager.Instance.CameraController.GetCameraOffset();
            float newOffset = ClampOffset(currentCameraOffset.y + (cameraMoveSpeed * Time.deltaTime));
            GameManager.Instance.CameraController.SetCameraOffset(new Vector3(currentCameraOffset.x, newOffset, currentCameraOffset.z));
        }

        private void OnButtonCameraDownPressed()
        {
            Vector3 currentCameraOffset = GameManager.Instance.CameraController.GetCameraOffset();
            float newOffset = ClampOffset(currentCameraOffset.y + (-cameraMoveSpeed * Time.deltaTime));
            GameManager.Instance.CameraController.SetCameraOffset(new Vector3(currentCameraOffset.x, newOffset, currentCameraOffset.z));
        }

        private float ClampOffset(float value)
        {
            return Mathf.Clamp(value, cameraMinOffset, cameraMaxOffset);
        }
    }
}
