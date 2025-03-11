using SimulatorEPL.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.ContentSizeFitter;

namespace SimulatorEPL.UI
{
    public class Leaderboard : MonoBehaviour
    {
        [SerializeField]
        private TeamsDb teamsDb;
        [SerializeField]
        private RectTransform viewsHolder;
        [SerializeField]
        private TeamStatisticsView statisticsViewPrefab;
        [SerializeField]
        private Matchmaker matchMaker;
        [Space]
        [SerializeField]
        private LayoutElement layoutElement;
        [SerializeField]
        private CanvasGroup canvasGroup;
        [SerializeField]
        private RectTransform outrightsDescriptionHolderTop;
        [SerializeField]
        private RectTransform outrightsDescriptionHolderBottom;

        private readonly Dictionary<Team, TeamStatistic> statistics = new Dictionary<Team, TeamStatistic>();
        private readonly List<TeamStatisticsView> views = new List<TeamStatisticsView>();

        public void SetVisible(bool visible)
        {
            layoutElement.ignoreLayout = !visible;
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = visible;
        }

        public void SetOutrigthsEnabled(bool enabled) 
        {
            outrightsDescriptionHolderTop.gameObject.SetActive(enabled);
            outrightsDescriptionHolderBottom.gameObject.SetActive(enabled);

            foreach (var view in views)
                view.SetOutrigthEnabled(enabled);
        }

        private void Awake()
        {
            Messenger<Match, Team>.AddListener(AppEvent.TeamGoaled, OnTeamGoaled);
            Messenger<Match>.AddListener(AppEvent.MatchStateChanged, OnMatchStateChanged);
            Messenger<int, RoundState>.AddListener(AppEvent.RoundStateChanged, OnRoundStateChanged);

            Init();
        }

        private IEnumerator Start()
        {
            yield return null;
            SaveViewPositions();

            if (viewsHolder.TryGetComponent(out VerticalLayoutGroup holderLayout))
            {
                holderLayout.childControlHeight = false;
                holderLayout.enabled = false;
            }

            if (viewsHolder.TryGetComponent(out ContentSizeFitter contentSizeFitter))
            {
                contentSizeFitter.verticalFit = FitMode.Unconstrained;
                contentSizeFitter.enabled = false;
            }
        }

        private void OnDestroy()
        {
            Messenger<Match, Team>.RemoveListener(AppEvent.TeamGoaled, OnTeamGoaled);
            Messenger<Match>.RemoveListener(AppEvent.MatchStateChanged, OnMatchStateChanged);
            Messenger<int, RoundState>.RemoveListener(AppEvent.RoundStateChanged, OnRoundStateChanged);
        }

        private void Init()
        {
            for (int i = 0; i < teamsDb.Teams.Count; i++)
            {
                var team = teamsDb.Teams[i];
                var stats = new TeamStatistic(team, i + 1);
                statistics.Add(team, stats);

                var view = Instantiate(statisticsViewPrefab, viewsHolder);
                view.Init(stats);
                views.Add(view);
            }
        }

        private void SaveViewPositions()
        {
            List<float> tempPositions = new List<float>();

            foreach (var view in views)
                tempPositions.Add(view.transform.position.y);

            tempPositions = tempPositions.OrderByDescending(position => position).ToList();
        }

        private void UpdatePlaces()
        {
            List<TeamStatistic> sortedStatistics = statistics.Values
                .OrderByDescending(stats => stats.SeasonPoints)
                .ThenByDescending(stats => stats.Goals)
                .ToList();

            for (int i = 0; i < sortedStatistics.Count; i++)
            {
                var stats = sortedStatistics[i];
                stats.Place = i + 1;
            }
        }

        private void OnTeamGoaled(Match match, Team teamGoaled)
        {
            TeamStatistic statsGoaledTeam = statistics[teamGoaled];
            TeamStatistic statsMissedGoalTeam = statistics[match.teamHome == teamGoaled ? match.teamAway : match.teamHome];

            statsGoaledTeam.OnGoal();
            statsMissedGoalTeam.OnMissedGoal();

            UpdatePlaces();
        }

        private void OnMatchStateChanged(Match match)
        {
            if (match.State == MatchState.FirstTime)
            {
                statistics[match.teamHome].OnGameStarted();
                statistics[match.teamAway].OnGameStarted();
                return;
            }
            else if (match.IsFinished)
            {
                statistics[match.teamHome].OnGameFinished(match.GetGameResultForSide(GameSide.Home));
                statistics[match.teamAway].OnGameFinished(match.GetGameResultForSide(GameSide.Away));
            }

            UpdatePlaces();
        }

        private void OnRoundStateChanged(int round, RoundState state)
        {
            if (state == RoundState.Finished)
                RecalculateOutrights(teamsDb.Teams, matchMaker.NextMatches);
        }

        private void RecalculateOutrights(IReadOnlyList<Team> teams, IReadOnlyList<Match> seasonMatches)
        {
            var teamsTempPoints = new Dictionary<Team, int>();
            List<Team> sortedTeams = new List<Team>();

            var firstPlaceCount = new Dictionary<Team, int>();
            var topFourPlaces = new Dictionary<Team, int>();
            var lastThreePlaces = new Dictionary<Team, int>();

            System.Random rnd = new System.Random();

            foreach (var team in teams)
            {
                teamsTempPoints[team] = 0;
                firstPlaceCount[team] = 0;
                topFourPlaces[team] = 0;
                lastThreePlaces[team] = 0;
            }

            int trials = 10000;

            for (int i = 0; i < trials; i++)
            {
                foreach (var team in teams)
                    teamsTempPoints[team] = statistics[team].SeasonPoints;

                var randomNumbers = Enumerable.Range(0, seasonMatches.Count).Select(x => rnd.NextDouble()).ToArray();

                for (int matchId = 0; matchId < seasonMatches.Count; matchId++)
                {
                    Match currentMatch = seasonMatches[matchId];

                    if (1f / currentMatch.Coefs.winHome >= randomNumbers[matchId])
                    {
                        teamsTempPoints[currentMatch.teamHome] += 3;
                    }
                    else if (1f - (1f / currentMatch.Coefs.winAway) <= randomNumbers[matchId])
                    {
                        teamsTempPoints[currentMatch.teamAway] += 3;
                    }
                    else
                    {
                        teamsTempPoints[currentMatch.teamHome] += 1;
                        teamsTempPoints[currentMatch.teamAway] += 1;
                    }
                }

                sortedTeams = teamsTempPoints.OrderByDescending(x => x.Value).Select(p => p.Key).ToList();

                firstPlaceCount[sortedTeams[0]] += 1;

                for (int c = 0; c < 4; c++)
                    topFourPlaces[sortedTeams[c]] += 1;

                for (int c = 19; c > 16; c--)
                    lastThreePlaces[sortedTeams[c]] += 1;
            }

            foreach (var team in teams)
            {
                var view = views.FirstOrDefault(v => v.Statistic.team == team);

                double pairFirstPlace = firstPlaceCount[team];
                double pairtopFourPlaces = topFourPlaces[team];
                double pairlastThreePlaces = lastThreePlaces[team];

                view.SetOutrights(
                    pairFirstPlace == 0 ? 0 : Clamp(0.95f / (pairFirstPlace / (double)trials)),
                    pairtopFourPlaces == 0 ? 0 : Clamp(0.95f / (pairtopFourPlaces / trials)),
                    pairlastThreePlaces == 0 ? 0 : Clamp(0.95f / (pairlastThreePlaces / trials)));
            }

            double Clamp(double value) => value > 1000 ? 1000 : value;
        }
    }
}
