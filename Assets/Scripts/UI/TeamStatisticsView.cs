using DG.Tweening;
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
        private TextMeshProUGUI firstPlace;
        [SerializeField]
        private TextMeshProUGUI topFourPlace;
        [SerializeField]
        private TextMeshProUGUI lastThreePlace;

        private RectTransform rectTransform;
        private Tween positionAnimation;
        private float height;

        public TeamStatistic Statistic { get; private set; }

        public void Init(TeamStatistic statistic)
        {
            rectTransform = GetComponent<RectTransform>();
            height = rectTransform.sizeDelta.y;

            this.Statistic = statistic;

            teamLable.sprite = statistic.team.Icon;
            teamName.text = statistic.team.Title;

            UpdateValues();

            statistic.PlaceChanged += OnPlaceChanged;
            statistic.GoalsChanged += OnGoalsChanged;
            statistic.GamesChanged += OnGamesChanged;
        }

        public void SetOutrights(double firstPlace, double topFourPlace, double lastThreePlace)
        {
            this.firstPlace.text = GetOutrightsString(firstPlace);
            this.topFourPlace.text = GetOutrightsString(topFourPlace);
            this.lastThreePlace.text = GetOutrightsString(lastThreePlace);
        }

        private string GetOutrightsString(double value) 
        {
            return value == 0 ? "-" : value.ToString("N2");
        }

        private void OnDestroy()
        {
            if (Statistic == null)
                return;

            Statistic.PlaceChanged -= OnPlaceChanged;
            Statistic.GoalsChanged -= OnGoalsChanged;
            Statistic.GamesChanged -= OnGamesChanged;
        }

        private void UpdateValues()
        {
            OnPlaceChanged();
            OnGamesChanged();
            OnGoalsChanged();
        }

        private void OnPlaceChanged()
        {
            placeText.text = Statistic.Place.ToString();
            SetPositionY(-(Statistic.Place - 1) * height);
        }

        private void SetPositionY(float positionY)
        {
            if (Statistic == null)
                return;

            positionAnimation?.Kill();

            positionAnimation = rectTransform
                .DOLocalMoveY(positionY, 0.8f)
                .SetLink(gameObject)
                .SetEase(Ease.Linear);
        }

        private void OnGamesChanged()
        {
            gamesCountText.text = Statistic.GamesCount.ToString();
            winsText.text = Statistic.WinsCount.ToString();
            drawsText.text = Statistic.DrawsCount.ToString();
            loosesText.text = Statistic.LooseCount.ToString();
            scoresText.text = Statistic.SeasonScore.ToString();
        }

        private void OnGoalsChanged()
        {
            goalsText.text = $"{Statistic.Goals} - {Statistic.MissedGoals}";
            deltaGoalsText.text = Statistic.GoalsDelta.ToString();
        }
    }
}