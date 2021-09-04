using UnityEngine.UIElements;
using UnityEngine;

namespace Samples.Runtime.General
{
    [RequireComponent(typeof(UIDocument))]
    public class RootSelector : MonoBehaviour
    {
        [SerializeField] private PanelSettings panelSettings = default;
        [SerializeField] private VisualTreeAsset sourceAsset = default;

        private void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            uiDocument.panelSettings = panelSettings;
            uiDocument.visualTreeAsset = sourceAsset;
        }
    }
}
