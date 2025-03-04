namespace SimulatorEPL
{
    public struct MatchCoefs
    {
        public readonly double winHome;
        public readonly double winAway;
        public readonly double draw;

        public readonly double handyHome;
        public readonly double handyAway;

        public readonly double nextScoreHome;
        public readonly double nextScoreAway;

        public readonly double totalSmallUnder;
        public readonly double totalSmallOver;

        public readonly double totalBigUnder;
        public readonly double totalBigOver;

        private readonly double totalSmallAdv;


        public MatchCoefs(double winHome, double winAway, double draw,
            double handyHome, double handyAway,
            double nextScoreHome, double nextScoreAway,
            double totalSmallUnder, double totalSmallOver,
            double totalBigUnder, double totalBigOver, double totalSmallAdv)
        {
            this.winHome = winHome;
            this.winAway = winAway;
            this.draw = draw;

            this.handyHome = handyHome;
            this.handyAway = handyAway;

            this.nextScoreHome = nextScoreHome;
            this.nextScoreAway = nextScoreAway;

            this.totalSmallUnder = totalSmallUnder;
            this.totalSmallOver = totalSmallOver;

            this.totalBigUnder = totalBigUnder;
            this.totalBigOver = totalBigOver;

            this.totalSmallAdv = totalSmallAdv;
        }

        public double TotalSmallAdv => totalSmallAdv;
        public double TotalBigAdv => totalSmallAdv + 1;
    }
}
