using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAccountActivation : MonoBehaviour
{
    [SerializeField] Button _buttonSendActivationCode;
    [SerializeField] Button _buttonResendActivationEmail;
    [SerializeField] Button _buttonQuit;
    [SerializeField] TMP_InputField _inputFieldActivationCode;

    public void OnButtonSendActivationCode()
    {
        string activationCode = _inputFieldActivationCode.text;
        NetworkManagerAccount.Instance.ActivateAccount(activationCode);
    }

    public void OnButtonResendActivationEmail()
    {
        _buttonResendActivationEmail.interactable = false;
        NetworkManagerAccount.Instance.ResendActivationCodeEmail();
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
