namespace SimulatorEPL
{
    public struct Score
    {
        public readonly int home;
        public readonly int away;

        public Score(int home, int away)
        {
            this.home = home;
            this.away = away;
        }

        public override string ToString()
        {
            return $"{home}:{away}";
        }
    }
}