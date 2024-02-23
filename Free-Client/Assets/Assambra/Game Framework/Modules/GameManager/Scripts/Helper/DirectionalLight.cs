using UnityEngine;

namespace Assambra.GameFramework.GameManager
{
    public class DirectionalLight : MonoBehaviour
    {
        public bool IsPersistentDirectionalLight;

        private void Awake()
        {
            if (!IsPersistentDirectionalLight)
                gameObject.SetActive(false);
        }
    }
}

