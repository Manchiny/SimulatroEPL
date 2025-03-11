using SimulatorEPL.Events;
using System.Collections.Generic;
using UnityEngine;

namespace SimulatorEPL.UI
{
    public class GamesCurrentView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform viewsHolder;
        [SerializeField]
        private MatchView gameViewPrefab;

        private readonly List<MatchView> matchViews = new List<MatchView>();

        private void Awake()
        {
            Messenger<RoundState>.AddListener(AppEvent.RoundStateChanged, OnRoundStateChanged);
            Messenger<Match>.AddListener(AppEvent.MatchStateChanged, OnMatchStateChanged);
        }

        private void OnDestroy()
        {
            Messenger<RoundState>.RemoveListener(AppEvent.RoundStateChanged, OnRoundStateChanged);
            Messenger<Match>.RemoveListener(AppEvent.MatchStateChanged, OnMatchStateChanged);
        }

        private void OnMatchStateChanged(Match match)
        {
            if (match.State != MatchState.FirstTime)
                return;

            MatchView view = Instantiate(gameViewPrefab, viewsHolder);
            view.Init(match);
            matchViews.Add(view);
            view.SetIsStarted(true);
        }

        private void OnRoundStateChanged(RoundState state)
        {
            if (state != RoundState.None)
                return;

            foreach (var view in matchViews)
                Destroy(view.gameObject);

            matchViews.Clear();
        }
    }
}
