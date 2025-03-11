using SimulatorEPL.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SimulatorEPL.UI
{
    public class MatchesNextView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform viewsHolder;
        [SerializeField]
        private MatchView matchViewPrefab;
        [SerializeField]
        private RoundDivider roundDivider;
        [Space]
        [SerializeField]
        private LayoutElement layoutElement;
        [SerializeField]
        private CanvasGroup canvasGroup;

        private readonly List<MatchView> matchViews = new List<MatchView>();
        private readonly Queue<RoundDivider> roundDividers = new Queue<RoundDivider>();

        public void SetVisible(bool visible)
        {
            layoutElement.ignoreLayout = !visible;
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = visible;
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
            if (matchViews.Count % AppConstants.RoundMatchesCount == 0)
            {
                var divider = Instantiate(roundDivider, viewsHolder);
                roundDividers.Enqueue(divider);
                divider.SetRoundNumber(matchViews.Count / AppConstants.RoundMatchesCount + 1);
            }

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

            if (matchViews.Count % AppConstants.RoundMatchesCount == 0)
            {
                var divider = roundDividers.Dequeue();
                Destroy(divider.gameObject);
            }
        }
    }
}
