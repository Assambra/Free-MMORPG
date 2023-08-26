using UnityEngine;
using UnityEngine.UI;

public class UICameraMover : MonoBehaviour
{
    [SerializeField] private Button buttonCameraUp;
    [SerializeField] private Button buttonCameraDown;

    [SerializeField] private float cameraMoveSpeed = 0.25f;
    [SerializeField] private float cameraMinOffset = 0.27f;
    [SerializeField] private float cameraMaxOffset = 2.75f;

    private ButtonPressedChecker checkerButtonCameraUp;
    private ButtonPressedChecker checkerButtonCameraDown;

    private void Awake()
    {
        checkerButtonCameraUp = buttonCameraUp.gameObject.GetComponent<ButtonPressedChecker>();
        checkerButtonCameraDown = buttonCameraDown.gameObject.GetComponent<ButtonPressedChecker>();
    }

    void Update()
    {
        if (checkerButtonCameraUp.IsButtonPressed)
            OnButtonCameraUpPressed();
        if (checkerButtonCameraDown.IsButtonPressed)
            OnButtonCameraDownPressed();
    }

    private void OnButtonCameraUpPressed()
    {
        Vector3 currentCameraOffset = GameManager.Instance.cameraController.GetCameraOffset();
        float newOffset = ClampOffset(currentCameraOffset.y + (cameraMoveSpeed * Time.deltaTime));
        GameManager.Instance.cameraController.SetCameraOffset(new Vector3(currentCameraOffset.x, newOffset , currentCameraOffset.z));
    }

    private void OnButtonCameraDownPressed()
    {
        Vector3 currentCameraOffset = GameManager.Instance.cameraController.GetCameraOffset();
        float newOffset = ClampOffset(currentCameraOffset.y + (-cameraMoveSpeed * Time.deltaTime));
        GameManager.Instance.cameraController.SetCameraOffset(new Vector3(currentCameraOffset.x, newOffset , currentCameraOffset.z));
    }

    private float ClampOffset(float value)
    {
        return Mathf.Clamp(value, cameraMinOffset, cameraMaxOffset);
    }
}
