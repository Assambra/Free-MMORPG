using Assambra.FreeClient.Managers;
using UnityEngine;
using Crest;

namespace Assambra.FreeClient.Integrations
{
    public class SetFieldsOceanRenderer : MonoBehaviour
    {
        [SerializeField] OceanRenderer oceanRenderer;

        void Start()
        {
            oceanRenderer.Viewpoint = GameManager.Instance.MainCamera.transform;
            oceanRenderer.ViewCamera = GameManager.Instance.MainCamera;
            oceanRenderer._primaryLight = GameManager.Instance.DirectionalLight;
        }
    }
}
