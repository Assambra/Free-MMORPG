using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    [field: SerializeField] public bool Active { get; set; }
    [field: SerializeField] public List<CameraPreset> CameraPresets { get; private set; }
    [field: SerializeField] public Camera MainCamera { get; set; }
    [field: SerializeField] public GameObject CameraTarget { get; set; }
    [field: SerializeField] public bool IsOverUIElement { private get; set; }



    [Header("Automatic find")]
    [SerializeField] private bool autoFindPlayer = false;

    [Header("Camera rotate camera target")]
    [SerializeField] private bool cameraRotateCameraTarget = false;

    [Header("Block")]
    [SerializeField] private bool blockCameraTilt;
    [SerializeField] private bool blockCameraPan;

    [Header("Camera offset")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1.8f, 0f);

    [Header("Camera distance")]
    [SerializeField] private float cameraStartDistance = 5f;
    [SerializeField] private float cameraMinDistance = 0f;
    [SerializeField] private float cameraMaxDistance = 35f;
    [SerializeField] private float mouseWheelSensitivity = 10f;

    [Header("Camera pan and tilt")]
    [SerializeField] private float cameraPanSpeed = 9f;
    [SerializeField] private float cameraTiltSpeed = 9f;
    [SerializeField] private float cameraTiltMin = -35f;
    [SerializeField] private float cameraTiltMax = 80f;


    // Private variables
    private float fieldOfView;
    private float mouseX = 0f;
    private float mouseY = 0f;
    private float cameraDistance = 0f;
    private float mouseWheel = 0f;
    private float cameraPan = 0f;
    private float cameraTilt = 0f;

    private float lastCameraTilt;
    private float lastCameraPan;
    private float lastCameraTargetRotation = 0f;
    private float lastCameraFieldOfView;

    private void Awake()
    {
        if (MainCamera == null)
        {
            MainCamera = Camera.main;

            if (MainCamera == null)
                Debug.LogError("No Camera with Tag MainCamera found");
        }

        if (MainCamera != null)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            MainCamera.transform.position = Vector3.zero;
            MainCamera.transform.rotation = Quaternion.identity;

            MainCamera.transform.parent = gameObject.transform;
            fieldOfView = MainCamera.fieldOfView;
            lastCameraFieldOfView = fieldOfView;

            cameraDistance = cameraStartDistance;
        }
    }

    void Start()
    {
        if (CameraTarget == null && autoFindPlayer)
        {
            if (GameObject.FindGameObjectWithTag("Player"))
                CameraTarget = GameObject.FindGameObjectWithTag("Player");
            else
                Debug.LogError("No Player with Tag Player found");
        }
    }

    void Update()
    {
        if (!IsOverUIElement && Active)
        {
            GetMouseInput();

            if (mouseWheel != 0)
                HandleCameraDistance();

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                if (Input.GetMouseButton(1) && cameraRotateCameraTarget)
                    CameraTarget.transform.Rotate(new Vector3(0, mouseX * cameraPanSpeed));

                if (blockCameraPan && blockCameraTilt)
                    return;
                else if (!blockCameraPan && !blockCameraTilt)
                    CameraTiltAndPan();
                else if (!blockCameraTilt && blockCameraPan)
                    CameraTilt();
                else if (!blockCameraPan && blockCameraTilt)
                    CameraPan();
            }

            lastCameraPan = cameraPan;
            lastCameraTilt = cameraTilt;

            
        }

        if (lastCameraFieldOfView != MainCamera.fieldOfView)
            fieldOfView = MainCamera.fieldOfView;           
    }

    private void LateUpdate()
    {
        if (!Input.GetMouseButton(0) || !Input.GetMouseButton(1))
            RotateCameraWithTarget();

        lastCameraTargetRotation = CameraTarget.transform.eulerAngles.y;

        if (Active)
            LookAtCameraTarget();
    }

    private void LookAtCameraTarget()
    {
        ClampCameraDistance();
        transform.position = CameraTarget.transform.position + cameraOffset - transform.forward * cameraDistance;
    }

    private void GetMouseInput()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        mouseWheel = Input.GetAxis("Mouse ScrollWheel");
    }

    private void HandleCameraDistance()
    {
        cameraDistance -= mouseWheel * mouseWheelSensitivity;
    }

    private void ClampCameraDistance()
    {
        cameraDistance = Mathf.Clamp(cameraDistance, cameraMinDistance, cameraMaxDistance);
    }

    private void CameraTiltAndPan()
    {
        cameraPan += mouseX * cameraPanSpeed;
        cameraTilt -= mouseY * cameraTiltSpeed;
        cameraTilt = ClampCameraTilt(cameraTilt);
        transform.eulerAngles = new Vector3(cameraTilt, cameraPan, 0);
    }

    private void CameraTilt()
    {
        cameraTilt -= mouseY * cameraTiltSpeed;
        cameraTilt = ClampCameraTilt(cameraTilt);
        transform.eulerAngles = new Vector3(cameraTilt, lastCameraPan, 0);
    }

    private void CameraPan()
    {
        cameraPan += mouseX * cameraPanSpeed;
        transform.eulerAngles = new Vector3(lastCameraTilt, cameraPan, 0);
    }

    private void RotateCameraWithTarget()
    {
        float rotDiff = lastCameraTargetRotation - CameraTarget.transform.eulerAngles.y;
        cameraPan -= rotDiff;
        transform.eulerAngles = new Vector3(cameraTilt, cameraPan, 0);
    }

    private float ClampCameraTilt(float tilt)
    {
        return Mathf.Clamp(tilt, cameraTiltMin, cameraTiltMax);
    }

    public void SetCameraPanAbsolutAngle(float angle)
    {

        transform.eulerAngles = new Vector3(lastCameraTilt, angle, 0);
        cameraPan = angle;
    }

    public float GetCameraPanAngle()
    {
        return cameraPan;
    }

    public void SetCameraTiltAbsolutAngle(float angle)
    {
        transform.eulerAngles = new Vector3(angle, lastCameraPan, 0);
        cameraTilt = angle;
    }

    public float GetCameraTiltAngle()
    {
        return cameraTilt;
    }

    public void ResetCameraAngles()
    {
        transform.eulerAngles = new Vector3(0, 0, 0);
        cameraTilt = 0;
        cameraPan = 0;
    }

    public void BlockCameraPan(bool value)
    {
        blockCameraPan = value;
    }

    public void BlockCameraTilt(bool value)
    {
        blockCameraTilt = value;
    }

    public Vector3 GetCameraOffset()
    {
        return cameraOffset;
    }

    public float GetCameraDistance()
    {
        return cameraDistance;
    }

    public float GetCameraFieldOfView()
    {
        return fieldOfView;
    }

    public void SetCameraDistance(float cameraDistance)
    {
        this.cameraDistance = cameraDistance;
    }

    public void SetCameraOffset(Vector3 cameraOffset)
    {
        this.cameraOffset = cameraOffset;
    }

    public void ChangeCameraPreset(string name)
    {
        CameraPreset camerapreset = GetCameraPreset(name);

        this.Active = camerapreset.Active;

        this.autoFindPlayer = camerapreset.autofindPlayer;

        this.cameraRotateCameraTarget = camerapreset.cameraRotateCameraTarget;

        this.blockCameraTilt = camerapreset.blockCameraTilt;
        this.blockCameraPan = camerapreset.blockCameraPan;

        this.cameraOffset = camerapreset.cameraOffset;

        this.cameraStartDistance = camerapreset.cameraStartDistance;
        this.cameraMinDistance = camerapreset.cameraMinDistance;
        this.cameraMaxDistance = camerapreset.cameraMaxDistance;
        this.mouseWheelSensitivity = camerapreset.mouseWheelSensitivity;

        this.cameraPanSpeed = camerapreset.cameraPanSpeed;
        this.cameraTiltSpeed = camerapreset.cameraTiltSpeed;
        this.cameraTiltMin = camerapreset.cameraTiltMin;
        this.cameraTiltMax = camerapreset.cameraTiltMax;

        cameraDistance = cameraStartDistance;
    }

    private CameraPreset GetCameraPreset(string name)
    {
        foreach (CameraPreset cameraPreset in CameraPresets)
        {
            if (cameraPreset.name == name)
                return cameraPreset;
        }

        return null;
    }
}
