namespace SimulatorEPL.Events
{
    public static class AppEvent
    {
        public const string ScoreChanged = nameof(ScoreChanged);
        public const string TeamGoaled = nameof(TeamGoaled);

        public const string MatchNextAdded = nameof(MatchNextAdded);
        public const string MatchNextRemoved = nameof(MatchNextRemoved);

        public const string MatchStarted = nameof(MatchStarted);
        public const string MatchFinished = nameof(MatchFinished);

        public const string GoalSoonCalculated = nameof(GoalSoonCalculated);
        public const string CoefsChanged = nameof(CoefsChanged);

        public const string RoundStateChanged = nameof(RoundStateChanged);
    }
}
