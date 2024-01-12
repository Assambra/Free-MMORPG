using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAccountActivation : MonoBehaviour
{
    [SerializeField] Button _buttonSendActivationCode;
    [SerializeField] Button _buttonQuit;
    [SerializeField] TMP_InputField _inputFieldActivationCode;

    public void OnButtonSendActivationCode()
    {
        NetworkManagerAccount.Instance.ActivateAccount(_inputFieldActivationCode.text);
    }

    public void OnButtonQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
