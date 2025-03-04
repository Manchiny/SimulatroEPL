using SimulatorEPL.Events;
using System.Collections.Generic;
using UnityEngine;

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

        private Dictionary<Team, TeamStatistic> statistics = new Dictionary<Team, TeamStatistic>();
        private List<TeamStatisticsView> views = new List<TeamStatisticsView>();

        private void Awake()
        {
            Messenger<Match, Team>.AddListener(AppEvent.TeamGoaled, OnTeamGoaled);
            Messenger<Match>.AddListener(AppEvent.MatchStarted, OnGameStarted);
            Messenger<Match>.AddListener(AppEvent.MatchFinished, OnGameFinished);

            Init();
        }

        private void OnDestroy()
        {
            Messenger<Match, Team>.RemoveListener(AppEvent.TeamGoaled, OnTeamGoaled);
            Messenger<Match>.RemoveListener(AppEvent.MatchStarted, OnGameStarted);
            Messenger<Match>.RemoveListener(AppEvent.MatchFinished, OnGameFinished);
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
            }
        }

        private void OnTeamGoaled(Match game,Team teamGoaled)
        {
            TeamStatistic statsGoaledTeam = statistics[teamGoaled];
            TeamStatistic statsMissedGoalTeam = statistics[game.teamHome == teamGoaled ? game.teamAway : game.teamHome];

            statsGoaledTeam.OnGoal();
            statsMissedGoalTeam.OnMissedGoal();

            UpdateView(statsGoaledTeam);
            UpdateView(statsMissedGoalTeam);
        }

        private void OnGameStarted(Match game)
        {
            statistics[game.teamHome].OnGameStarted();
            statistics[game.teamAway].OnGameStarted();

            UpdateView(statistics[game.teamHome]);
            UpdateView(statistics[game.teamAway]);
        }

        private void OnGameFinished(Match game)
        {
            statistics[game.teamHome].OnGameFinished(game.GetGameResultForSide(GameSide.Home));
            statistics[game.teamAway].OnGameFinished(game.GetGameResultForSide(GameSide.Away));

            UpdateView(statistics[game.teamHome]);
            UpdateView(statistics[game.teamAway]);
        }

        private void UpdateView(TeamStatistic statistic)
        {

        }
    }
}
