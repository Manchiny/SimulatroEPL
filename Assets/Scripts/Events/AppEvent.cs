namespace SimulatorEPL.Events
{
    public static class AppEvent
    {
        public const string ScoreChanged = nameof(ScoreChanged);
        public const string TeamGoaled = nameof(TeamGoaled);

        public const string GameNextAdded = nameof(GameNextAdded);
        public const string GameNextRemoved = nameof(GameNextRemoved);

        public const string GameStarted = nameof(GameStarted);
        public const string GameFinished = nameof(GameFinished);

        public const string AfterThreetStepsGoalCalculated = nameof(AfterThreetStepsGoalCalculated);
    }
}
