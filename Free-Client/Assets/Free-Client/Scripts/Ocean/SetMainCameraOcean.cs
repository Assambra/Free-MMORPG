using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crest;

public class SetMainCameraOcean : MonoBehaviour
{
    [SerializeField] OceanRenderer oceanRenderer;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;

        oceanRenderer.Viewpoint = mainCam.transform;
        oceanRenderer.ViewCamera = mainCam;
    }
}
