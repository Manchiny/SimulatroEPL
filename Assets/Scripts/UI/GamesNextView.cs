using SimulatorEPL.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimulatorEPL.UI
{
    public class GamesNextView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform viewsHolder;
        [SerializeField]
        private GameView gameViewPrefab;

        private readonly List<GameView> gameViews = new List<GameView>();

        private void Awake()
        {
            Messenger<Game>.AddListener(AppEvent.GameNextAdded, OnGameAdded);
            Messenger<Game>.AddListener(AppEvent.GameNextRemoved, OnGameRemoved);
        }

        private void OnDestroy()
        {
            Messenger<Game>.RemoveListener(AppEvent.GameNextAdded, OnGameAdded);
            Messenger<Game>.RemoveListener(AppEvent.GameNextRemoved, OnGameRemoved);
        }

        private void OnGameAdded(Game game)
        {
            GameView view = Instantiate(gameViewPrefab, viewsHolder);
            view.Init(game);
            gameViews.Add(view);
            view.SetIsStarted(false);
        }

        private void OnGameRemoved(Game game) 
        {
            var view = gameViews.FirstOrDefault(view => view.Game == game);

            if (!view)
                return;

            Destroy(view.gameObject);
            gameViews.Remove(view);
        }
    }
}
