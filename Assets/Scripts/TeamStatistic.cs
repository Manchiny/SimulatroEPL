namespace SimulatorEPL
{
    public class TeamStatistic
    {
        public readonly Team team;

        public TeamStatistic(Team team)
        {
            this.team = team;
        }

        public int GamesCount { get; private set; }

        public int WinsCount { get; private set; }
        public int LooseCount { get; private set; }
        public int DrawsCount { get; private set; }

        public int Goals { get; private set; }
        public int MissedGoals { get; private set; }

        public int GoalsDelta => Goals - MissedGoals;
        public int SeasonScore => WinsCount * 3 + DrawsCount * 1;

        public void AddResult(GameResult result, int goals, int missedGoals) 
        {
            GamesCount++;

            if (result == GameResult.Win)
                WinsCount++;
            else if (result == GameResult.Loose)
                LooseCount++;
            else
                DrawsCount++;

            Goals += goals;
            MissedGoals += missedGoals;
        }
    }
}