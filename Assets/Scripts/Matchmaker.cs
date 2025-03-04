using SimulatorEPL.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimulatorEPL
{
    public class Matchmaker : MonoBehaviour
    {
        [SerializeField]
        private TeamsDb teamsDb;

        private const int matchesInRoundCount = 10;

        private readonly WaitForSeconds waitSecond = new WaitForSeconds(1f);

        private readonly Queue<Match> currentMatches = new Queue<Match>();
        private readonly Queue<Match> nextMatches = new Queue<Match>();

        private void Start()
        {
            Init();
            StartCoroutine(SimulateNewRound());
        }

        private void Init()
        {
            GenerateSeasonMatches(teamsDb.Teams);
        }

        private void GenerateSeasonMatches(IReadOnlyList<Team> teams)
        {
            var seasonMatches = new Dictionary<int, List<Match>>();

            int halfMatches = teams.Count / 2;

            List<Team> homeTeams = teams.Take(halfMatches).ToList();
            List<Team> awayTeams = teams.Skip(halfMatches).ToList();

            for (int i = 0; i < teams.Count - 1; i++)
            {
                seasonMatches[i] = new List<Match>();
                seasonMatches[teams.Count + i] = new List<Match>();

                for (int j = 0; j < halfMatches; j++)
                {
                    seasonMatches[i].Add(new Match(homeTeams[j], awayTeams[j]));
                    seasonMatches[teams.Count + i].Add(new Match(awayTeams[j], homeTeams[j]));
                }

                Team pop = homeTeams.Last();

                homeTeams = new List<Team> { homeTeams[0], awayTeams[0] }.Concat(homeTeams.Skip(1).Take(homeTeams.Count - 2)).ToList(); ;

                awayTeams = awayTeams.Skip(1).ToList();
                awayTeams.Add(pop);
            }

            foreach (var pair in seasonMatches)
            {
                List<Match> matches = pair.Value;

                foreach (var match in matches)
                {
                    nextMatches.Enqueue(match);
                    Messenger<Match>.Broadcast(AppEvent.MatchNextAdded, match);
                }
            }
        }

        private void AddNewRoundGames()
        {
            for (int i = 0; i < matchesInRoundCount; i++)
                TryAddCurrentMatch();
        }

        private IEnumerator SimulateNewRound()
        {
            Messenger<RoundState>.Broadcast(AppEvent.RoundStateChanged, RoundState.FirstTime);

            AddNewRoundGames();
            int counter = AppConstants.GameDurationSeconds;

            while (counter > 0)
            {
                counter--;

                foreach (var game in currentMatches)
                    game.SimulateMinute();

                if (counter == AppConstants.GameDurationSeconds / 2)
                {
                    Messenger<RoundState>.Broadcast(AppEvent.RoundStateChanged, RoundState.HalfTime);

                    for (int i = 0; i < 5; i++)
                        yield return waitSecond;

                    Messenger<RoundState>.Broadcast(AppEvent.RoundStateChanged, RoundState.SecondTime);
                }
                else
                {
                    yield return waitSecond;
                }
            }

            while (currentMatches.Count > 0)
            {
                var match = currentMatches.Dequeue();
                match.Finish();
                Messenger<Match>.Broadcast(AppEvent.MatchFinished, match);
            }

            yield return null;
            Debug.Log("Games finished");

            if (nextMatches.Count > 0)
                StartCoroutine(ShowTimeOut());
            else
                Debug.Log("All games finished");
        }

        private IEnumerator ShowTimeOut()
        {
            Messenger<RoundState>.Broadcast(AppEvent.RoundStateChanged, RoundState.FullTime);

            int timer = 5;

            while (timer > 0)
            {
                yield return waitSecond;
                timer--;
            }

            yield return null;
            Messenger<RoundState>.Broadcast(AppEvent.RoundStateChanged, RoundState.None);
            yield return null;

            StartCoroutine(SimulateNewRound());
        }

        private void TryAddCurrentMatch()
        {
            if (nextMatches.Count <= 0)
                return;

            var game = nextMatches.Dequeue();
            Messenger<Match>.Broadcast(AppEvent.MatchNextRemoved, game);

            currentMatches.Enqueue(game);
            Messenger<Match>.Broadcast(AppEvent.MatchStarted, game);
        }
    }
}
