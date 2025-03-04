using SimulatorEPL.Events;
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
        private MatchView matchViewPrefab;

        private readonly List<MatchView> matchViews = new List<MatchView>();

        private void Awake()
        {
            Messenger<Match>.AddListener(AppEvent.MatchNextAdded, OnMatchAdded);
            Messenger<Match>.AddListener(AppEvent.MatchNextRemoved, OnMatchRemoved);
        }

        private void OnDestroy()
        {
            Messenger<Match>.RemoveListener(AppEvent.MatchNextAdded, OnMatchAdded);
            Messenger<Match>.RemoveListener(AppEvent.MatchNextRemoved, OnMatchRemoved);
        }

        private void OnMatchAdded(Match game)
        {
            MatchView view = Instantiate(matchViewPrefab, viewsHolder);
            view.Init(game);
            matchViews.Add(view);
            view.SetIsStarted(false);
        }

        private void OnMatchRemoved(Match match) 
        {
            var view = matchViews.FirstOrDefault(view => view.Match == match);

            if (!view)
                return;

            Destroy(view.gameObject);
            matchViews.Remove(view);
        }
    }
}
