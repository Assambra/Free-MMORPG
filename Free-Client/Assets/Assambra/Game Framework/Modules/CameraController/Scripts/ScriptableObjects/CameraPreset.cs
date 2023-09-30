using UnityEngine;

[CreateAssetMenu(fileName = "CameraPreset", menuName = "Assambra/CameraPreset", order = 1)]
public class CameraPreset : ScriptableObject
{
    [Header("Active")]
    public bool Active = true;

    [Header("Automatic find")]
    public bool autofindPlayer = false;

    [Header("Camera rotate camera target")]
    public bool cameraRotateCameraTarget = false;

    [Header("Block Camera Pan/Tilt")]
    public bool blockCameraTilt = false;
    public bool blockCameraPan = false;

    [Header("Camera offset")]
    public Vector3 cameraOffset = new Vector3(0f, 1.8f, 0f);

    [Header("Camera distance")]
    public float cameraStartDistance = 5f;
    public float cameraMinDistance = 0f;
    public float cameraMaxDistance = 35f;
    public float mouseWheelSensitivity = 10f;

    [Header("Camera pan and tilt")]
    public float cameraPanSpeed = 9f;
    public float cameraTiltSpeed = 9f;
    public float cameraTiltMin = -35f;
    public float cameraTiltMax = 80f;
}
