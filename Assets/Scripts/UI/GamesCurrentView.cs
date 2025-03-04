using SimulatorEPL.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimulatorEPL.UI
{
    public class GamesCurrentView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform viewsHolder;
        [SerializeField]
        private MatchView gameViewPrefab;

        private readonly List<MatchView> gameViews = new List<MatchView>();

        private void Awake()
        {
            Messenger<RoundState>.AddListener(AppEvent.RoundStateChanged, OnRoundStateChanged);
            Messenger<Match>.AddListener(AppEvent.MatchStarted, OnGameStarted);
        }

        private void OnDestroy()
        {
            Messenger<RoundState>.RemoveListener(AppEvent.RoundStateChanged, OnRoundStateChanged);
            Messenger<Match>.RemoveListener(AppEvent.MatchStarted, OnGameStarted);
        }

        private void OnGameStarted(Match game)
        {
            MatchView view = Instantiate(gameViewPrefab, viewsHolder);
            view.Init(game);
            gameViews.Add(view);
            view.SetIsStarted(true);
        }

        private void OnRoundStateChanged(RoundState state)
        {
            if (state != RoundState.None)
                return;

            foreach (var view in gameViews)
                Destroy(view.gameObject);

            gameViews.Clear();
        }
    }
}
