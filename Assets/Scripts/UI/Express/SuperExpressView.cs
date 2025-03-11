using SimulatorEPL.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SimulatorEPL.UI.Express
{
    public class SuperExpressView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform viewsHolder;
        [SerializeField]
        private MatchExpressView expressViewPrefab;
        [SerializeField]
        private Matchmaker matchmaker;
        [Space]
        [SerializeField]
        private LayoutElement layoutElement;
        [SerializeField]
        private CanvasGroup canvasGroup;

        private readonly List<MatchExpressView> views = new List<MatchExpressView>();

        public void SetVisible(bool visible)
        {
            layoutElement.ignoreLayout = !visible;
            canvasGroup.alpha = visible ? 1f : 0f;
        }

        private void Awake()
        {
            Messenger<int, RoundState>.AddListener(AppEvent.RoundStateChanged, OnRoundStateChanged);

            for (int i = 0; i < AppConstants.RoundMatchesCount; i++)
            {
                var view = Instantiate(expressViewPrefab, viewsHolder);
                views.Add(view);
            }
        }

        private void OnDestroy()
        {
            Messenger<int, RoundState>.RemoveListener(AppEvent.RoundStateChanged, OnRoundStateChanged);
        }

        private void OnRoundStateChanged(int round, RoundState state)
        {
            if (state != RoundState.Started)
                return;

            List<Match> matches = matchmaker.NextMatches.Take(AppConstants.RoundMatchesCount).ToList();

            for (int i = 0; i < views.Count; i++)
                views[i].Init(matches[i]);
        }
    }
}
