using SimulatorEPL.Events;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace SimulatorEPL
{
    public class Game
    {
        public readonly Team teamHome;
        public readonly Team teamAway;

        private readonly Queue<Team> nextStepsScoredTeams = new Queue<Team>();

        private int gameTime;

        public Game(Team teamHome, Team teamAway)
        {
            this.teamHome = teamHome;
            this.teamAway = teamAway;

            for (int i = 0; i < 3; i++)
                nextStepsScoredTeams.Enqueue(AddNextStepScoredTeamOrNull());
        }

        public event Action<int> GameTimeChanged;

        public GameCoefs Coefs { get; private set; }

        public int GameTime
        {
            get => gameTime;
            set
            {
                if (value < 0)
                    return;

                gameTime = value;
                GameTimeChanged?.Invoke(gameTime);
            }
        }

        public Score Score { get; private set; }

        public override string ToString()
        {
            return $"{teamHome.Title} - {teamAway.Title}";
        }

        public void SimulateMinute()
        {
            // TODO: просчитывать и следующее событие тоже. И записывать его. Если будет гол - показывать три стрелки
            
            Team scoredTeam = nextStepsScoredTeams.Dequeue();

            if (scoredTeam != null)
                AddScore(scoredTeam);

            AddNextStepScoredTeamOrNull();
            GameTime++;
        }

        public bool HasAnyTeamFromGame(Game anotherGame)
        {
            return anotherGame.teamHome.Id == teamHome.Id || anotherGame.teamHome.Id == teamAway.Id
                || anotherGame.teamAway.Id == teamHome.Id || anotherGame.teamAway.Id == teamAway.Id;
        }

        private Team AddNextStepScoredTeamOrNull()
        {
            int randomTeamId = Random.Range(0, 2);
            Team team = randomTeamId == 0 ? teamHome : teamAway;

            bool hasGoal = Random.Range(0, 100) < 10;

            if (hasGoal)
            {
                nextStepsScoredTeams.Enqueue(team);
                Messenger<Game, Team>.Broadcast(AppEvent.AfterThreetStepsGoalCalculated, this, team);
                return team;
            }

            nextStepsScoredTeams.Enqueue(null);
            return null;
        }

        private void AddScore(Team scoredTeam)
        {
            if (scoredTeam.Id == teamHome.Id)
                Score = new Score(Score.home + 1, Score.away);
            else
                Score = new Score(Score.home, Score.away + 1);

            Messenger<Game>.Broadcast(AppEvent.ScoreChanged, this);
            Messenger<Team>.Broadcast(AppEvent.TeamGoaled, scoredTeam);
        }
    }
}
