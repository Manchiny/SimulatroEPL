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

        private Dictionary<Team, TeamStatistic> statistics = new Dictionary<Team, TeamStatistic>();
        private List<TeamStatisticsView> views = new List<TeamStatisticsView>();

        private Dictionary<int, float> positions = new Dictionary<int, float>();

        private void Awake()
        {
            Messenger<Match, Team>.AddListener(AppEvent.TeamGoaled, OnTeamGoaled);
            Messenger<Match>.AddListener(AppEvent.MatchStarted, OnGameStarted);
            Messenger<Match>.AddListener(AppEvent.MatchFinished, OnGameFinished);
            Messenger<RoundState>.AddListener(AppEvent.RoundStateChanged, OnGameStateChanged);

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
            Messenger<Match>.RemoveListener(AppEvent.MatchStarted, OnGameStarted);
            Messenger<Match>.RemoveListener(AppEvent.MatchFinished, OnGameFinished);
            Messenger<RoundState>.RemoveListener(AppEvent.RoundStateChanged, OnGameStateChanged);
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

            for (int i = 0; i < tempPositions.Count; i++)
                positions.Add(i + 1, tempPositions[i]);
        }

        private void UpdatePlaces()
        {
            List<TeamStatistic> sortedStatistics = statistics.Values
                .OrderByDescending(stats => stats.SeasonScore)
                .ThenByDescending(stats => stats.Goals)
                .ToList();

            for (int i = 0; i < sortedStatistics.Count; i++)
            {
                var stats = sortedStatistics[i];
                stats.Place = i + 1;
            }

            //foreach (var view in views)
            //{
            //    if (positions.TryGetValue(view.Statistic.Place, out float pos))
            //        view.SetPositionY(pos);
            //}
        }

        private void OnTeamGoaled(Match game, Team teamGoaled)
        {
            TeamStatistic statsGoaledTeam = statistics[teamGoaled];
            TeamStatistic statsMissedGoalTeam = statistics[game.teamHome == teamGoaled ? game.teamAway : game.teamHome];

            statsGoaledTeam.OnGoal();
            statsMissedGoalTeam.OnMissedGoal();

            UpdatePlaces();
        }

        private void OnGameStarted(Match game)
        {
            statistics[game.teamHome].OnGameStarted();
            statistics[game.teamAway].OnGameStarted();
        }

        private void OnGameFinished(Match game)
        {
            statistics[game.teamHome].OnGameFinished(game.GetGameResultForSide(GameSide.Home));
            statistics[game.teamAway].OnGameFinished(game.GetGameResultForSide(GameSide.Away));

            UpdatePlaces();
        }

        private void OnGameStateChanged(RoundState state)
        {
            if (state == RoundState.FullTime)
            {
                foreach (var team in teamsDb.Teams)
                {
                    var stats = statistics[team];
                    team.Points = stats.SeasonScore;
                }

                GetOutright(teamsDb.Teams, matchMaker.NextMatches);
            }
        }

        // После окончания тура

        private void GetOutright(IReadOnlyList<Team> teams, IReadOnlyList<Match> seasonMatches)
        {
            var firstPlaceCount = new Dictionary<Team, int>();
            var topFourPlaces = new Dictionary<Team, int>();
            var lastThreePlaces = new Dictionary<Team, int>();

            System.Random rnd = new System.Random();

            foreach (var team in teams)
            {
                firstPlaceCount[team] = 0;
                topFourPlaces[team] = 0;
                lastThreePlaces[team] = 0;
            }

            int trials = 10000;

            for (int i = 0; i < trials; i++)
            {
                foreach (var team in teams)
                    team.TempPoints = team.Points;

                int totalCounter = 0;

                for (int roundCounter = 0; roundCounter < 5; roundCounter++)
                {
                    var randomNumbers = Enumerable.Range(0, 10).Select(x => rnd.NextDouble()).ToArray();

                    for (int matchIndex = 0; matchIndex < 10; matchIndex++)
                    {
                        Match currentMatch = seasonMatches[totalCounter];

                        if (0.95f / currentMatch.Coefs.winHome <= randomNumbers[matchIndex])
                        {
                            currentMatch.teamHome.TempPoints += 3;
                        }
                        else if (0.95f / currentMatch.Coefs.winAway >= 1 - randomNumbers[matchIndex])
                        {
                            currentMatch.teamAway.TempPoints += 3;
                        }
                        else
                        {
                            currentMatch.teamHome.TempPoints += 1;
                            currentMatch.teamAway.TempPoints += 1;
                        }

                        totalCounter++;
                    }
                }

                teams = teams.OrderByDescending(t => t.TempPoints + t.Points).ToList(); //ThenBy pointsDelta

                firstPlaceCount[teams[0]] += 1;

                for (int i1 = 0; i1 < 4; i1++)
                    topFourPlaces[teams[i1]] += 1;

                for (int i2 = 9; i2 > 6; i2--)
                    lastThreePlaces[teams[i2]] += 1;
            }

            foreach (var team in teams)
            {
                var view = views.FirstOrDefault(v => v.Statistic.team == team);

                double pairFirstPlace = firstPlaceCount[team];
                double pairtopFourPlaces = topFourPlaces[team];
                double pairlastThreePlaces = lastThreePlaces[team];

                view.SetOutrights(
                    pairFirstPlace == 0 ? 0 : 0.95f / (pairFirstPlace / (double)trials),
                    pairtopFourPlaces == 0 ? 0 : 0.95f / (pairtopFourPlaces / trials), 
                    pairlastThreePlaces == 0 ? 0 : 0.95f / (pairlastThreePlaces / trials));
            }
        }
    }
}
