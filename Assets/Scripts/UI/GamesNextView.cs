using SimulatorEPL.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SimulatorEPL.UI
{
    public class GamesNextView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform viewsHolder;
        [SerializeField]
        private MatchView matchViewPrefab;
        [SerializeField]
        private LayoutElement layoutElement;
        [SerializeField]
        private CanvasGroup canvasGroup;

        private readonly List<MatchView> matchViews = new List<MatchView>();

        public void SetVisible(bool visible)
        {
            layoutElement.ignoreLayout = !visible;
            canvasGroup.alpha = visible ? 1f : 0f;
        }

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
