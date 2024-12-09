using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assambra.FreeClient
{
    public class KeyNavigation : MonoBehaviour
    {
        [SerializeField] private KeyCode _next = KeyCode.Tab;
        [SerializeField] private KeyCode _previous = KeyCode.LeftShift;
        [SerializeField] private KeyCode _ok = KeyCode.Return;
        [SerializeField] private KeyCode _cancel = KeyCode.Escape;

        [SerializeField] private List<GameObject> _uIObjects = new List<GameObject>();
        [SerializeField] private GameObject _firstFocus;

        private int _index = 1;

        private void Start()
        {
            if (_uIObjects == null || _uIObjects.Count == 0)
            {
                Debug.LogWarning("The UI objects list is empty or null.");
                return;
            }

            SetFocus(_firstFocus);
        }

        private void Update()
        {
            if (Input.GetKey(_previous) && Input.GetKeyDown(_next))
            {
                NavigateBackward();
            }
            else if (Input.GetKeyDown(_next))
            {
                NavigateForward();
            }
        }

        private void NavigateForward()
        {
            int originalIndex = _index;
            do
            {
                _index++;
                if (_index > _uIObjects.Count)
                    _index = 1;
            }
            while (!IsActive(_uIObjects[_index - 1]) && _index != originalIndex);

            SetFocus(_uIObjects[_index - 1]);
        }

        private void NavigateBackward()
        {
            int originalIndex = _index;
            do
            {
                _index--;
                if (_index < 1)
                    _index = _uIObjects.Count;
            }
            while (!IsActive(_uIObjects[_index - 1]) && _index != originalIndex);

            SetFocus(_uIObjects[_index - 1]);
        }

        private void SetFocus(GameObject focusObject)
        {
            if (focusObject == null)
            {
                Debug.LogWarning("The focus object is null.");
                return;
            }

            if (GetUIType<Button>(focusObject) is Button button)
            {
                button.Select();
                //EventSystem.current.SetSelectedGameObject(button.gameObject);
            }
            else if (GetUIType<TMP_InputField>(focusObject) is TMP_InputField inputField)
            {
                inputField.Select();
                //EventSystem.current.SetSelectedGameObject(inputField.gameObject);
            }
            else
            {
                Debug.Log($"The object {focusObject.name} is not a supported UI element. Skipping...");
            }
        }

        private bool IsActive(GameObject obj)
        {
            return obj != null && obj.activeInHierarchy;
        }

        private T GetUIType<T>(GameObject obj) where T : Component
        {
            if (obj == null)
            {
                Debug.LogWarning("The provided GameObject is null.");
                return null;
            }

            return obj.GetComponent<T>();
        }
    }
}


