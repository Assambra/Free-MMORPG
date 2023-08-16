using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGameCameraController : MonoBehaviour
{
    private Camera mainCamera;


    private void Awake()
    {
        mainCamera = Camera.main;
        mainCamera.transform.position = new Vector3(0, 1, 2);
        mainCamera.transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
