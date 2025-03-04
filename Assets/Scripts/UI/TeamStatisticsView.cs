using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimulatorEPL.UI
{
    public class TeamStatisticsView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI placeText;
        [SerializeField]
        private Image teamLable;
        [SerializeField]
        private TextMeshProUGUI teamName;
        [Space]
        [SerializeField]
        private TextMeshProUGUI gamesCountText;
        [SerializeField]
        private TextMeshProUGUI winsText;
        [SerializeField]
        private TextMeshProUGUI drawsText;
        [SerializeField]
        private TextMeshProUGUI loosesText;
        [Space]
        [SerializeField]
        private TextMeshProUGUI goalsText;
        [SerializeField]
        private TextMeshProUGUI deltaGoalsText;
        [SerializeField]
        private TextMeshProUGUI scoresText;
        [Space]
        [SerializeField]
        private TextMeshProUGUI coefPlace1Text;
        [SerializeField]
        private TextMeshProUGUI coefPlace2Text;
        [SerializeField]
        private TextMeshProUGUI coefPlace3Text;

        private TeamStatistic statistic;

        public void Init(TeamStatistic statistic)
        {
            this.statistic = statistic;

            teamLable.sprite = statistic.team.Icon;
            teamName.text = statistic.team.Title;

            UpdateValues();

            statistic.PlaceChanged += OnPlaceChanged;
            statistic.GoalsChanged += OnGoalsChanged;
            statistic.GamesChanged += OnGamesChanged;
        }

        private void OnDestroy()
        {
            if (statistic == null)
                return;

            statistic.PlaceChanged -= OnPlaceChanged;
            statistic.GoalsChanged -= OnGoalsChanged;
            statistic.GamesChanged -= OnGamesChanged;
        }

        private void UpdateValues()
        {
            OnPlaceChanged();
            OnGamesChanged();
            OnGoalsChanged();

            //coefPlace1Text.text = statistic.
            //coefPlace2Text.text = statistic.
            //coefPlace3Text.text = statistic.
        }

        private void OnPlaceChanged()
        {
            placeText.text = statistic.Place.ToString();
        }

        private void OnGamesChanged()
        {
            gamesCountText.text = statistic.GamesCount.ToString();
            winsText.text = statistic.WinsCount.ToString();
            drawsText.text = statistic.DrawsCount.ToString();
            loosesText.text = statistic.LooseCount.ToString();
        }

        private void OnGoalsChanged()
        {
            goalsText.text = $"{statistic.Goals} - {statistic.MissedGoals}";
            deltaGoalsText.text = statistic.GoalsDelta.ToString();
            scoresText.text = statistic.SeasonScore.ToString();
        }
    }
}