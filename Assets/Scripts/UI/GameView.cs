using SimulatorEPL.Events;
using TMPro;
using UnityEngine;

namespace SimulatorEPL.UI
{
    public class GameView : MonoBehaviour
    {
        [SerializeField]
        private GameCoefsView coefsView;
        [SerializeField]
        private TextMeshProUGUI gameNameText;
        [Space]
        [SerializeField]
        private RectTransform isStartedObj;
        [SerializeField]
        private TextMeshProUGUI timerText;
        [Space]
        [SerializeField]
        private TextMeshProUGUI scoreText;
        [SerializeField]
        private GoalNotifier goalNotifier;

        public Game Game { get; private set; }

        public void Init(Game game)
        {
            Game = game;

            gameNameText.text = game.ToString();
            coefsView.SetGameCoef(game.Coefs);

            OnScoreChanged(game);
            UpdateTimer(game.GameTime);

            Messenger<Game>.AddListener(AppEvent.ScoreChanged, OnScoreChanged);
            game.GameTimeChanged += UpdateTimer;
        }

        public void SetIsStarted(bool isStarted)
        {
            isStartedObj.gameObject.SetActive(isStarted);
            scoreText.gameObject.SetActive(isStarted);
        }

        private void OnDestroy()
        {
            if (Game != null)
                Game.GameTimeChanged -= UpdateTimer;

            Messenger<Game>.RemoveListener(AppEvent.ScoreChanged, OnScoreChanged);
        }

        private void UpdateTimer(int time)
        {
            timerText.text = $"{time}'";
        }

        private void OnScoreChanged(Game game)
        {
            if (game == Game)
            {
                scoreText.text = game.Score.ToString();

                if (game.Score.away > 0 || game.Score.home > 0)
                    goalNotifier.Show();
            }
        }
    }
}
