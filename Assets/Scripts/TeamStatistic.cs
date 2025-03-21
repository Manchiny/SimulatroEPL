using System;

namespace SimulatorEPL
{
    public class TeamStatistic
    {
        public readonly Team team;

        private int place;

        public TeamStatistic(Team team, int place)
        {
            this.team = team;
            Place = place;
        }

        public event Action GoalsChanged;
        public event Action PlaceChanged;
        public event Action GamesChanged;

        public int Place 
        {
            get => place;
            set
            {
                if (value == place)
                    return;

                place = value;
                PlaceChanged?.Invoke();
            }
        }

        public int GamesCount { get; private set; }

        public int WinsCount { get; private set; }
        public int LooseCount { get; private set; }
        public int DrawsCount { get; private set; }

        public int Goals { get; private set; }
        public int MissedGoals { get; private set; }

        public int GoalsDelta => Goals - MissedGoals;
        public int SeasonPoints => WinsCount * 3 + DrawsCount * 1;

        public void OnGameStarted()
        {
            GamesCount++;
            GamesChanged?.Invoke();
        }

        public void OnGameFinished(MatchResult result)
        {
            if (result == MatchResult.Win)
                WinsCount++;
            else if (result == MatchResult.Loose)
                LooseCount++;
            else
                DrawsCount++;

            GamesChanged?.Invoke();
        }

        public void OnGoal()
        {
            Goals++;
            GoalsChanged?.Invoke();
        }

        public void OnMissedGoal()
        {
            MissedGoals++;
            GoalsChanged?.Invoke();
        }
    }
}