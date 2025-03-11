using SimulatorEPL.UI.Express;
using UnityEngine;
using UnityEngine.UI;

namespace SimulatorEPL.UI
{
    public class ViewSwitcher : MonoBehaviour
    {
        [SerializeField]
        private Button tabButton1;
        [SerializeField]
        private Button tabButton2;
        [SerializeField]
        private Button tabButton3;
        [Space]
        [SerializeField]
        private MatchesCurrentView matchesCurrentView;
        [SerializeField]
        private MatchesNextView matchesNextView;
        [SerializeField]
        private Leaderboard leaderboard;
        [SerializeField]
        private SuperExpressView expressView;
        [SerializeField]
        private AllBetsView allBetsView;

        private void Awake()
        {
            tabButton1.onClick.AddListener(() => SetTab(1));
            tabButton2.onClick.AddListener(() => SetTab(2));
            tabButton3.onClick.AddListener(() => SetTab(3));
        }

        private void Start()
        {
            SetTab(1);
        }

        private void SetTab(int tab)
        {
            matchesCurrentView.SetVisible(tab == 1);
            matchesNextView.SetVisible(tab == 2);
            expressView.SetVisible(tab == 3);
            allBetsView.SetVisible(tab == 3);
            
            leaderboard.SetVisible(tab == 1 || tab == 2);
            leaderboard.SetOutrigthsEnabled(tab == 2);
        }
    }
}
