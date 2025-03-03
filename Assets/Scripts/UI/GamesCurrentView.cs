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
        private GameView gameViewPrefab;

        private readonly List<GameView> gameViews = new List<GameView>();

        private void Awake()
        {
            Messenger<Game>.AddListener(AppEvent.GameStarted, OnGameStarted);
            Messenger<Game>.AddListener(AppEvent.GameFinished, OnGameFinished);
        }

        private void OnDestroy()
        {
            Messenger<Game>.RemoveListener(AppEvent.GameStarted, OnGameStarted);
            Messenger<Game>.RemoveListener(AppEvent.GameFinished, OnGameFinished);
        }

        private void OnGameStarted(Game game)
        {
            GameView view = Instantiate(gameViewPrefab, viewsHolder);
            view.Init(game);
            gameViews.Add(view);
            view.SetIsStarted(true);
        }

        private void OnGameFinished(Game game)
        {
            var view = gameViews.FirstOrDefault(view => view.Game == game);

            if (!view)
                return;

            Destroy(view.gameObject);
            gameViews.Remove(view);
        }
    }
}
