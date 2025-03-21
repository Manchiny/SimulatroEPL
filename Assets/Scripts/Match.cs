using SimulatorEPL.Events;
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SimulatorEPL
{
    public class Match
    {
        public readonly Team teamHome;
        public readonly Team teamAway;

        private readonly Queue<Team> nextStepsScoredTeams = new Queue<Team>();

        private int gameTime;

        public Match(Team teamHome, Team teamAway)
        {
            this.teamHome = teamHome;
            this.teamAway = teamAway;

            RecalculateCoefs();

            for (int i = 0; i < 3; i++)
                AddNextStepScoredTeamOrNull();
        }

        public event Action<int> MatchTimeChanged;

        public MatchCoefs Coefs { get; private set; }

        public int CurrentTime
        {
            get => gameTime;
            private set
            {
                if (value < 0)
                    return;

                gameTime = value;
                MatchTimeChanged?.Invoke(gameTime);
            }
        }

        public int MinutesRemaining => AppConstants.RoundDurationSeconds - CurrentTime;

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
            CurrentTime++;
        }

        public void Finish()
        {
            CurrentTime = AppConstants.RoundDurationSeconds;
            RecalculateCoefs();
        }

        public MatchResult GetGameResultForSide(GameSide side)
        {
            if (Score.away == Score.home)
                return MatchResult.Draw;

            if (side == GameSide.Home)
                return Score.home > Score.away ? MatchResult.Win : MatchResult.Loose;

            return Score.away > Score.home ? MatchResult.Win : MatchResult.Loose;
        }

        private void AddNextStepScoredTeamOrNull()
        {
            if (nextStepsScoredTeams.Count > 0 && nextStepsScoredTeams.Peek() != null)
            {
                nextStepsScoredTeams.Enqueue(null);
                return;
            }

            double randomValue = Random.Range(0f, 1f);

            if (randomValue <= teamHome.Power + AppConstants.HomeAdvantageCoef)
                nextStepsScoredTeams.Enqueue(teamHome);
            else if (randomValue >= 1 - teamAway.Power + AppConstants.HomeAdvantageCoef)
                nextStepsScoredTeams.Enqueue(teamAway);
            else
                nextStepsScoredTeams.Enqueue(null);
        }

        private void AddScore(Team scoredTeam)
        {
            if (scoredTeam.Id == teamHome.Id)
                Score = new Score(Score.home + 1, Score.away);
            else
                Score = new Score(Score.home, Score.away + 1);

            Messenger<Match>.Broadcast(AppEvent.ScoreChanged, this);
            Messenger<Match, Team>.Broadcast(AppEvent.TeamGoaled, this, scoredTeam);
        }

        private void RecalculateCoefs()
        {
            double winHome = 0;
            double winAway = 0;
            double draw = 0;

            double avg1 = Score.home + (teamHome.Power + AppConstants.HomeAdvantageCoef) * MinutesRemaining;
            double avg2 = Score.away + (teamAway.Power - AppConstants.HomeAdvantageCoef) * MinutesRemaining;

            double handyHome = 0;
            double handyAway = 0;
            double handyHomeAvg = 0;

            double totalSmallAdv = (int)(avg1 + avg2) + 0.5;

            double totalSmallUnder = 0;
            double totalBigUnder = 0;

            var scores = new List<Score>();

            // Для правой таблицы - RemainingMinuts = Max всегда, для матчей, которые не сыграны
            // Score = 0:0

            for (int i = Score.home; i < Score.home + 5; i++) 
            {
                for (int j = Score.away; j < Score.away + 5; j++)
                    scores.Add(new Score(i, j, GetScoreProb(i, j)));
            }

            foreach (var score in scores)
            {
                if (score.home > score.away)
                    winHome += score.prob;
                else if (score.away > score.home)
                    winAway += score.prob;
                else
                    draw += score.prob;

                if (score.home + score.away < (int)(avg1 + avg2) + 0.5f)
                    totalSmallUnder += score.prob;
                if (score.home + score.away < (int)(avg1 + avg2) + 1.5f)
                    totalBigUnder += score.prob;

                if (avg1 >= avg2)
                {
                    if (score.home - 1.5f > score.away)
                        handyHome += score.prob;
                    else
                        handyAway += score.prob;

                    handyHomeAvg = -1.5f;
                }
                else
                {
                    if (score.away - 1.5f > score.home)
                        handyHome += 1f - score.prob;
                    else
                        handyAway += 1f - score.prob;

                    handyHomeAvg = 1.5f;
                }
            }

            double totalSmallOver = 1f - totalSmallUnder;
            double totalBigOver = 1f - totalBigUnder;

            double nextScoreHome = (teamHome.Power + AppConstants.HomeAdvantageCoef) / (teamHome.Power + teamAway.Power);
            double nextScoreAway = (teamAway.Power - AppConstants.HomeAdvantageCoef) / (teamHome.Power + teamAway.Power);

            Coefs = new MatchCoefs(
                winHome: 0.95f / winHome, 
                winAway: 0.95f / winAway, 
                draw: 0.95f / draw, 

                handyHome: 0.95f / handyHome, 
                handyAway: 0.95f / handyAway,
                handyHomeAvg: handyHomeAvg,

                nextScoreHome: MinutesRemaining > 0 ? 0.95f / nextScoreHome : 0f,
                nextScoreAway: MinutesRemaining > 0 ? 0.95f / nextScoreAway : 0f,

                totalSmallUnder: 0.95f / totalSmallUnder,
                totalSmallOver: 0.95f / totalSmallOver,

                totalBigUnder: 0.95f / totalBigUnder,
                totalBigOver: 0.95f / totalBigOver,

                totalSmallAdv: totalSmallAdv);

            Messenger<Match, MatchCoefs>.Broadcast(AppEvent.CoefsChanged, this, Coefs);
        }

        private double GetScoreProb(int homeToScore, int awayToScore)
        {
            if (Score.home > homeToScore || Score.away > awayToScore)
                return 0;

            double drawScoreProb = 1f - teamHome.Power - teamAway.Power;
            int drawScore = MinutesRemaining - homeToScore - awayToScore + Score.home + Score.away;

            double binomialCoef = (double)(Factorial(MinutesRemaining) / (Factorial(homeToScore - Score.home) * Factorial(awayToScore - Score.away) * Factorial(drawScore)));

            double probability = binomialCoef
                * Math.Pow(teamHome.Power + AppConstants.HomeAdvantageCoef, homeToScore - Score.home)
                * Math.Pow(teamAway.Power - AppConstants.HomeAdvantageCoef, awayToScore - Score.away)
                * Math.Pow(drawScoreProb, drawScore);

            return probability;
        }

        private BigInteger Factorial(int n)
        {
            if (n <= 0)
                return 1;

            return n * Factorial(n - 1);
        }
    }
}
