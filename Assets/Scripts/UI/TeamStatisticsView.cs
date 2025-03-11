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
        private RectTransform outrightsHolder;
        [SerializeField]
        private TextMeshProUGUI firstPlace;
        [SerializeField]
        private TextMeshProUGUI topFourPlace;
        [SerializeField]
        private TextMeshProUGUI lastThreePlace;
        [Space]
        [SerializeField]
        private PlaceSwithPointer placeSwitchPointer;

        private RectTransform rectTransform;
        private Tween positionAnimation;
        private float height;
        private int lastPlace;

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

        public void SetOutrigthEnabled(bool enabled)
        {
            outrightsHolder.gameObject.SetActive(enabled);
        }

        public void SetOutrights(double firstPlace, double topFourPlace, double lastThreePlace)
        {
            this.firstPlace.text = GetOutrightsString(firstPlace);
            this.topFourPlace.text = GetOutrightsString(topFourPlace);
            this.lastThreePlace.text = GetOutrightsString(lastThreePlace);
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

            float targetPosY = -(Statistic.Place - 1) * height;
            SetPositionY(targetPosY);

            if (lastPlace != Statistic.Place)
                placeSwitchPointer.ShowDirection(targetPosY > rectTransform.localPosition.y);

            lastPlace = Statistic.Place;
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
            scoresText.text = Statistic.SeasonPoints.ToString();
        }

        private void OnGoalsChanged()
        {
            goalsText.text = $"{Statistic.Goals} - {Statistic.MissedGoals}";
            deltaGoalsText.text = Statistic.GoalsDelta.ToString();
        }

        private string GetOutrightsString(double value)
        {
            if (value <= 1)
                return "-";

            if (value > 100)
                return value.ToString("N0");

            return value.ToString("N2");
        }
    }
}