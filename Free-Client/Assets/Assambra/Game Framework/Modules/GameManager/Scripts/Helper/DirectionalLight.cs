using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalLight : MonoBehaviour
{
    public bool IsPersistentDirectionalLight;

    private void Awake()
    {
        if (!IsPersistentDirectionalLight)
            gameObject.SetActive(false);
    }
}
