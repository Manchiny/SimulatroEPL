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

            RecalculateCoefs();

            for (int i = 0; i < 3; i++)
                AddNextStepScoredTeamOrNull();
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
            Team scoredTeam = nextStepsScoredTeams.Dequeue();

            if (scoredTeam != null)
                AddScore(scoredTeam);

            AddNextStepScoredTeamOrNull();
            RecalculateCoefs();
            GameTime++;
        }

        public bool HasAnyTeamFromGame(Game anotherGame)
        {
            return anotherGame.teamHome.Id == teamHome.Id || anotherGame.teamHome.Id == teamAway.Id
                || anotherGame.teamAway.Id == teamHome.Id || anotherGame.teamAway.Id == teamAway.Id;
        }

        private void AddNextStepScoredTeamOrNull()
        {
            if (nextStepsScoredTeams.Count > 0 && nextStepsScoredTeams.Peek() != null)
            {
                nextStepsScoredTeams.Enqueue(null);
                return;
            }

            int randomTeamId = Random.Range(0, 2);
            Team team = randomTeamId == 0 ? teamHome : teamAway;

            bool hasGoal = Random.Range(0, 100) < 10;

            if (hasGoal)
            {
                nextStepsScoredTeams.Enqueue(team);
                Messenger<Game, Team>.Broadcast(AppEvent.GoalSoonCalculated, this, team);
                return;
            }

            nextStepsScoredTeams.Enqueue(null);
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

        private void RecalculateCoefs()
        {
            float winHome = Random.Range(0.01f, 4);
            float winAway = Random.Range(0.01f, 4);
            float draw = Random.Range(0.01f, 4);

            float handicapHome = Random.Range(0.01f, 4);
            float handicapAway = Random.Range(0.01f, 4);

            float totalMore = Random.Range(0.01f, 4);
            float totalLess = Random.Range(0.01f, 4);

            Coefs = new GameCoefs(winHome, winAway, draw, handicapHome, handicapAway, totalMore, totalLess);
            Messenger<Game, GameCoefs>.Broadcast(AppEvent.CoefsChanged, this, Coefs);
        }
    }
}
