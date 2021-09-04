using UnityEngine;
using UnityEngine.UIElements;

namespace Samples.Runtime.Rendering
{
    public class RenderTextureBackgroundDemo : MonoBehaviour
    {
        [SerializeField] public VisualTreeAsset visualTreeAsset;
        [SerializeField] public PanelSettings panelSettings;
        [SerializeField] public StyleSheet styleSheet;

        [SerializeField] public RenderTexture cubeRt;
        [SerializeField] public RenderTexture cylinderRt;
        [SerializeField] public RenderTexture capsuleRt;

        void OnEnable()
        {
            var doc = GetComponent<UIDocument>();

            var cube = doc.rootVisualElement.Q("cube-thumbnail");
            cube.style.backgroundImage = Background.FromRenderTexture(cubeRt);

            var cylinder = doc.rootVisualElement.Q("cylinder-thumbnail");
            cylinder.style.backgroundImage = Background.FromRenderTexture(cylinderRt);


            // The capsule thumbnail texture is assigned via uss
            doc.rootVisualElement.styleSheets.Add(styleSheet);
        }
    }
}
