using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Assambra.FreeClient.UserInterface
{
    public class TabHandler : MonoBehaviour
    {
        [SerializeField] private GameObject[] tabs;
        [SerializeField] private Sprite spriteTabActive;
        [SerializeField] private Sprite spriteTabInactive;
        [SerializeField] private Color buttonActiveColor;
        [SerializeField] private Color buttonInactiveColor;
        [SerializeField] private Color textActiveColor;
        [SerializeField] private Color textInactiveColor;

        private void Awake()
        {
            foreach (GameObject tab in tabs)
            {
                Button btn = tab.GetComponent<Button>();
                btn.onClick.AddListener(() => { OnButtonTab(btn.gameObject); });
            }
        }

        public void OnButtonTab(GameObject buttonGameObject)
        {
            int i = 0;
            foreach (GameObject tab in tabs)
            {
                if (buttonGameObject == tab)
                {
                    tab.GetComponent<Image>().sprite = spriteTabActive;
                    tab.GetComponent<Button>().image.color = buttonActiveColor;

                    TabButton tb = tab.GetComponent<TabButton>();
                    if (!tb.GetColorizerActiveState())
                        tb.SetColorizerActiveState();

                    tab.GetComponentInChildren<TMP_Text>().color = textActiveColor;
                    tab.transform.SetSiblingIndex(tabs.Length);

                }
                else
                {
                    tab.GetComponent<Image>().sprite = spriteTabInactive;
                    tab.GetComponent<Button>().image.color = buttonInactiveColor;

                    TabButton tb = tab.GetComponent<TabButton>();
                    if (tb.GetColorizerActiveState())
                        tb.SetColorizerActiveState();

                    tab.GetComponentInChildren<TMP_Text>().color = textInactiveColor;
                    tab.transform.SetSiblingIndex(i);
                    i++;
                }

            }
        }

    }
}
