using SimulatorEPL.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimulatorEPL.UI
{
    public class GamesCurrentView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform viewsHolder;
        [SerializeField]
        private MatchView gameViewPrefab;
        [SerializeField]
        private LayoutElement layoutElement;
        [SerializeField]
        private CanvasGroup canvasGroup;

        private readonly List<MatchView> matchViews = new List<MatchView>();

        public void SetVisible(bool visible)
        {
            layoutElement.ignoreLayout = !visible;
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = visible;
        }

        private void Awake()
        {
            Messenger<int, RoundState>.AddListener(AppEvent.RoundStateChanged, OnRoundStateChanged);
            Messenger<Match>.AddListener(AppEvent.MatchStateChanged, OnMatchStateChanged);
        }

        private void OnDestroy()
        {
            Messenger<int, RoundState>.RemoveListener(AppEvent.RoundStateChanged, OnRoundStateChanged);
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

        private void OnRoundStateChanged(int round, RoundState state)
        {
            if (state != RoundState.None)
                return;

            foreach (var view in matchViews)
                Destroy(view.gameObject);

            matchViews.Clear();
        }
    }
}
