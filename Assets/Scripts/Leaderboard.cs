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
            Messenger<Game, Team>.AddListener(AppEvent.TeamGoaled, OnTeamGoaled);
            Messenger<Game>.AddListener(AppEvent.GameStarted, OnGameStarted);
            Messenger<Game>.AddListener(AppEvent.GameFinished, OnGameFinished);

            Init();
        }

        private void OnDestroy()
        {
            Messenger<Game, Team>.RemoveListener(AppEvent.TeamGoaled, OnTeamGoaled);
            Messenger<Game>.RemoveListener(AppEvent.GameStarted, OnGameStarted);
            Messenger<Game>.RemoveListener(AppEvent.GameFinished, OnGameFinished);
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

        private void OnTeamGoaled(Game game,Team teamGoaled)
        {
            TeamStatistic statsGoaledTeam = statistics[teamGoaled];
            TeamStatistic statsMissedGoalTeam = statistics[game.teamHome == teamGoaled ? game.teamAway : game.teamHome];

            statsGoaledTeam.OnGoal();
            statsMissedGoalTeam.OnMissedGoal();

            UpdateView(statsGoaledTeam);
            UpdateView(statsMissedGoalTeam);
        }

        private void OnGameStarted(Game game)
        {
            statistics[game.teamHome].OnGameStarted();
            statistics[game.teamAway].OnGameStarted();

            UpdateView(statistics[game.teamHome]);
            UpdateView(statistics[game.teamAway]);
        }

        private void OnGameFinished(Game game)
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
