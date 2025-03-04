using SimulatorEPL.Events;
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
            Messenger<Match>.AddListener(AppEvent.MatchStarted, OnGameStarted);
            Messenger<Match>.AddListener(AppEvent.MatchFinished, OnGameFinished);
        }

        private void OnDestroy()
        {
            Messenger<Match>.RemoveListener(AppEvent.MatchStarted, OnGameStarted);
            Messenger<Match>.RemoveListener(AppEvent.MatchFinished, OnGameFinished);
        }

        private void OnGameStarted(Match game)
        {
            MatchView view = Instantiate(gameViewPrefab, viewsHolder);
            view.Init(game);
            gameViews.Add(view);
            view.SetIsStarted(true);
        }

        private void OnGameFinished(Match game)
        {
            var view = gameViews.FirstOrDefault(view => view.Match == game);

            if (!view)
                return;

            Destroy(view.gameObject);
            gameViews.Remove(view);
        }
    }
}
