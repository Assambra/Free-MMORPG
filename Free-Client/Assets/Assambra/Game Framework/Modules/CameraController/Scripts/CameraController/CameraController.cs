using UnityEngine;


public class CameraController : MonoBehaviour
{
    public bool IsOverUIElement { private get;  set;}

    [Header("Serialize fields")]
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private GameObject cameraTarget = null;

    [Header("Camera rotate camera target")]
    [SerializeField] private bool cameraRotateCameraTarget = false;

    [Header("Camera offset")]
    [SerializeField] private Vector3 CameraOffset = new Vector3(0f, 1.8f, 0f);
    
    [Header("Camera distance")]
    [SerializeField] private float cameraStartDistance = 5f;
    [SerializeField] float cameraMinDistance = 0f;
    [SerializeField] float cameraMaxDistance = 35f;
    [SerializeField] private float mouseWheelSensitivity = 10f;

    [Header("Camera pan and tilt")]
    [SerializeField] private float cameraPanSpeed = 9f;
    [SerializeField] private float cameraTiltSpeed = 9f;
    [SerializeField] private float cameraTiltMin = -35f;
    [SerializeField] private float cameraTiltMax = 80f;


    // Private variables
    private float mouseX = 0f;
    private float mouseY = 0f;
    private float cameraDistance = 0f;
    private float mouseWheel = 0f;
    private float cameraPan = 0f;
    private float cameraTilt = 0f;
    
    private float lastPlayerRotation = 0f;

    private void Awake()
    {
        if (mainCamera == null)
        {
            if (GameObject.FindGameObjectWithTag("MainCamera"))
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            else
                Debug.LogError("No Camera with Tag MainCamera found");
        }

        mainCamera.transform.parent = gameObject.transform;
        mainCamera.transform.position = Vector3.zero;
        mainCamera.transform.rotation = Quaternion.identity;
        cameraDistance = cameraStartDistance;
    }

    void Start()
    {
        if (cameraTarget == null)
        {
            if (GameObject.FindGameObjectWithTag("Player"))
                cameraTarget = GameObject.FindGameObjectWithTag("Player");
            else
                Debug.LogError("No Player with Tag Player found");
        }
    }


    void Update()
    {
        GetMouseInput();
        
        HandleCameraDistance();
        
        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !IsOverUIElement)
        {
            CameraTiltAndPan();

            if(Input.GetMouseButton(1) && cameraRotateCameraTarget)
                cameraTarget.transform.Rotate(new Vector3(0, mouseX * cameraPanSpeed));
        }
        else
        {
            CameraPan();
        }

        lastPlayerRotation = cameraTarget.transform.eulerAngles.y;
    }

    private void LateUpdate()
    {
        transform.position = cameraTarget.transform.position + CameraOffset - transform.forward * cameraDistance;
    }

    private void GetMouseInput()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y") ;
        mouseWheel = Input.GetAxis("Mouse ScrollWheel");
    }

    private void HandleCameraDistance()
    {
        cameraDistance -= mouseWheel * mouseWheelSensitivity;
        cameraDistance = Mathf.Clamp(cameraDistance, cameraMinDistance, cameraMaxDistance);
    }

    private void CameraTiltAndPan()
    {
        cameraPan += mouseX * cameraPanSpeed;
        cameraTilt -= mouseY * cameraTiltSpeed;

        transform.eulerAngles = new Vector3(ClampCameraTilt(cameraTilt), cameraPan, 0);
    }

    private void CameraPan()
    {
        float rotDiff = lastPlayerRotation - cameraTarget.transform.eulerAngles.y;
        cameraPan -= rotDiff;
        transform.eulerAngles = new Vector3(cameraTilt, cameraPan, 0);
    }

    private float ClampCameraTilt(float tilt)
    {
        return cameraTilt = Mathf.Clamp(cameraTilt, cameraTiltMin, cameraTiltMax);
    }
}
