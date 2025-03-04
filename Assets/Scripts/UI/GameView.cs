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
        private TextMeshProUGUI timerText;
        [Space]
        [SerializeField]
        private TextMeshProUGUI scoreText;
        [SerializeField]
        private GoalNotifier goalNotifier;
        [SerializeField]
        private AttackNotifier attackNotifier;

        public Game Game { get; private set; }

        public void Init(Game game)
        {
            Game = game;

            gameNameText.text = game.ToString();
            coefsView.SetGameCoef(game.Coefs);

            OnGameTimeChanged(game.GameTime);
            OnScoreChanged(game);

            Messenger<Game>.AddListener(AppEvent.ScoreChanged, OnScoreChanged);
            Messenger<Game, Team>.AddListener(AppEvent.GoalSoonCalculated, OnGoalSoonCalculated);
            Messenger<Game, GameCoefs>.AddListener(AppEvent.CoefsChanged, OnGameCoefsChanged);

            game.GameTimeChanged += OnGameTimeChanged;
        }

        public void SetIsStarted(bool isStarted)
        {
            timerText.gameObject.SetActive(isStarted);
            scoreText.gameObject.SetActive(isStarted);
        }

        private void OnDestroy()
        {
            if (Game != null)
                Game.GameTimeChanged -= OnGameTimeChanged;

            Messenger<Game>.RemoveListener(AppEvent.ScoreChanged, OnScoreChanged);
            Messenger<Game, Team>.RemoveListener(AppEvent.GoalSoonCalculated, OnGoalSoonCalculated);
            Messenger<Game, GameCoefs>.RemoveListener(AppEvent.CoefsChanged, OnGameCoefsChanged);
        }

        private void OnGameTimeChanged(int time)
        {
            timerText.text = $"{time}'";
            attackNotifier.Simulate();
        }

        private void OnScoreChanged(Game game)
        {
            if (game != Game)
                return;

            scoreText.text = game.Score.ToString();

            if (game.Score.away > 0 || game.Score.home > 0)
                goalNotifier.Show();

            attackNotifier.SetTargetAttackSide(null, force: true);
        }

        private void OnGoalSoonCalculated(Game game, Team goaledTeam)
        {
            if (game != Game)
                return;

            GameSide attackSide = goaledTeam == game.teamHome ? GameSide.Home : GameSide.Away;
            attackNotifier.SetTargetAttackSide(attackSide, force: false);
        }

        private void OnGameCoefsChanged(Game game, GameCoefs coefs)
        {
            if (game != Game)
                return;

            coefsView.SetGameCoef(coefs);
        }
    }
}
