namespace SimulatorEPL
{
    public struct GameCoefs
    {
        public readonly float winHome;
        public readonly float winAway;
        public readonly float draw;

        public readonly float handicapHome;
        public readonly float handicapAway;

        public readonly float totalMore;
        public readonly float totalLess;

        public GameCoefs(float winHome, float winAway, float draw, float handicapHome, float handicapAway, float totalMore, float totalLess)
        {
            this.winHome = winHome;
            this.winAway = winAway;
            this.draw = draw;

            this.handicapHome = handicapHome;
            this.handicapAway = handicapAway;

            this.totalMore = totalMore;
            this.totalLess = totalLess;
        }
    }
}
