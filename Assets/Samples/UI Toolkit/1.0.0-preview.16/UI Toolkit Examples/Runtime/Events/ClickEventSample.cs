using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Samples.Runtime.Events
{
    [RequireComponent(typeof(UIDocument))]
    public class ClickEventSample : MonoBehaviour
    {
        private enum GameState
        {
            Waiting,
            Active
        }

        private const string ActiveClassName = "game-button--active";

        [SerializeField] private PanelSettings panelSettings = default;
        [SerializeField] private VisualTreeAsset sourceAsset = default;
        [SerializeField] private StyleSheet styleSheet = default;

        private List<Button> gameButtons;
        private Label scoreLabel;

        private GameState currentState = GameState.Waiting;
        private int activeButtonIndex = -1;
        private float delay = 3f;
        private int score;

        public void SetPanelSettings(PanelSettings newPanelSettings)
        {
            panelSettings = newPanelSettings;
            var uiDocument = GetComponent<UIDocument>();
            uiDocument.panelSettings = panelSettings;
        }

        void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            uiDocument.panelSettings = panelSettings;
            uiDocument.visualTreeAsset = sourceAsset;
        }

        void OnEnable()
        {
            if (scoreLabel == null)
            {
                score = 0;
                //After a domain reload, we need to re-cache our VisualElements and hook our callbacks
                InitializeVisualTree(GetComponent<UIDocument>());
            }
        }

        private void InitializeVisualTree(UIDocument doc)
        {
            var root = doc.rootVisualElement;

            scoreLabel = root.Q<Label>(className: "score-number");
            scoreLabel.text = score.ToString();

            gameButtons = root.Query<Button>(className: "game-button").ToList();
            var gameBoard = root.Q<VisualElement>(className: "board");
            gameBoard.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.target is Button targetButton && targetButton.ClassListContains(ActiveClassName))
                {
                    score = score + 1;
                    scoreLabel.text = score.ToString();
                    targetButton.RemoveFromClassList(ActiveClassName);
                    evt.StopImmediatePropagation();
                }
            });

            root.styleSheets.Add(styleSheet);
        }

        void Update()
        {
            delay -= Time.deltaTime;

            if (delay < 0f)
            {
                switch (currentState)
                {
                    case GameState.Waiting:
                        activeButtonIndex = Random.Range(0, gameButtons.Count);
                        gameButtons[activeButtonIndex].AddToClassList(ActiveClassName);
                        currentState = GameState.Active;
                        delay = Random.Range(0.5f, 2f);
                        break;
                    case GameState.Active:
                        gameButtons[activeButtonIndex].RemoveFromClassList(ActiveClassName);
                        currentState = GameState.Waiting;
                        delay = Random.Range(1f, 4f);
                        break;
                }
            }
        }
    }
}
