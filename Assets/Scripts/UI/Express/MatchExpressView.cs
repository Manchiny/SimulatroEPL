using SimulatorEPL.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimulatorEPL.UI.Express
{
    public class MatchExpressView : MonoBehaviour
    {
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
        private ExpressEventSelector winHomeSelector;
        [SerializeField]
        private ExpressEventSelector drawEventSelector;
        [SerializeField]
        private ExpressEventSelector winAwaySelector;

        public Match Match { get; private set; }

        public void Init(Match match)
        {
            Match = match;

            teamHomeTitle.text = match.teamHome.Title;
            teamHomeLogo.sprite = match.teamHome.Icon;

            teamAwayTitle.text = match.teamAway.Title;
            teamAwayLogo.sprite = match.teamAway.Icon;
        }

        private void Awake()
        {
            Messenger<ExpressEventSelector>.AddListener(AppEvent.ExpressEventSelectChanged, OnEventSelectChanged);
        }

        private void OnDestroy()
        {
            Messenger<ExpressEventSelector>.RemoveListener(AppEvent.ExpressEventSelectChanged, OnEventSelectChanged);
        }

        private void OnEventSelectChanged(ExpressEventSelector selector)
        {

        }
    }
}
