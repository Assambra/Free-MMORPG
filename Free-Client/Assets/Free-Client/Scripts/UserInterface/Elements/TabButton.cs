using UnityEngine;

namespace Assambra.FreeClient.UserInterface
{
    public class TabButton : MonoBehaviour
    {
        [SerializeField] private GameObject tabColorizer;

        public void SetColorizerActiveState()
        {
            tabColorizer.SetActive(!tabColorizer.activeSelf);
        }

        public bool GetColorizerActiveState()
        {
            return tabColorizer.activeSelf;
        }
    }
}
