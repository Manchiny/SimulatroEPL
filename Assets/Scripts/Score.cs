namespace SimulatorEPL
{
    public struct Score
    {
        public readonly int home;
        public readonly int away;

        public readonly double prob;

        public Score(int home, int away, double prob = 0)
        {
            this.home = home;
            this.away = away;

            this.prob = prob;
        }

        public override string ToString()
        {
            return $"{home}:{away}";
        }
    }
}