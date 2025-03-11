using SimulatorEPL.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimulatorEPL.UI
{
    public class MatchView : MonoBehaviour
    {
        [SerializeField]
        private GameCoefsView coefsView;
        [SerializeField]
        private TextMeshProUGUI teamHomeTitle;
        [SerializeField]
        private Image teamHomeLogo; 
        [SerializeField]
        private TextMeshProUGUI teamAwayTitle;
        [SerializeField]
        private Image teamAwayLogo;
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

        public Match Match { get; private set; }

        public void Init(Match match)
        {
            Match = match;
            attackNotifier.Init(match);

            teamHomeTitle.text = match.teamHome.Title;
            teamHomeLogo.sprite = match.teamHome.Icon;

            teamAwayTitle.text = match.teamAway.Title;
            teamAwayLogo.sprite = match.teamAway.Icon;

            coefsView.SetGameCoef(match.Coefs);

            OnMatchTimeChanged(match.CurrentTime);
            OnScoreChanged(match);

            Messenger<Match>.AddListener(AppEvent.ScoreChanged, OnScoreChanged);
            Messenger<Match, Team>.AddListener(AppEvent.GoalSoonCalculated, OnGoalSoonCalculated);
            Messenger<Match, MatchCoefs>.AddListener(AppEvent.CoefsChanged, OnMatchCoefsChanged);

            match.MatchTimeChanged += OnMatchTimeChanged;
        }

        public void SetIsStarted(bool isStarted)
        {
            timerText.gameObject.SetActive(isStarted);
            scoreText.gameObject.SetActive(isStarted);
        }

        private void OnDestroy()
        {
            if (Match != null)
                Match.MatchTimeChanged -= OnMatchTimeChanged;

            Messenger<Match>.RemoveListener(AppEvent.ScoreChanged, OnScoreChanged);
            Messenger<Match, Team>.RemoveListener(AppEvent.GoalSoonCalculated, OnGoalSoonCalculated);
            Messenger<Match, MatchCoefs>.RemoveListener(AppEvent.CoefsChanged, OnMatchCoefsChanged);
        }

        private void OnMatchTimeChanged(int time)
        {
            timerText.text = $"{time}'";
            attackNotifier.Simulate();
        }

        private void OnScoreChanged(Match match)
        {
            if (match != Match)
                return;

            scoreText.text = match.Score.ToString();

            if (match.Score.away > 0 || match.Score.home > 0)
                goalNotifier.Show();

            attackNotifier.SetTargetAttackSide(null, force: true);
        }

        private void OnGoalSoonCalculated(Match match, Team goaledTeam)
        {
            if (match != Match)
                return;

            GameSide attackSide = goaledTeam == match.teamHome ? GameSide.Home : GameSide.Away;
            attackNotifier.SetTargetAttackSide(attackSide, force: false);
        }

        private void OnMatchCoefsChanged(Match match, MatchCoefs coefs)
        {
            if (match != Match)
                return;

            coefsView.SetGameCoef(coefs);
        }
    }
}
