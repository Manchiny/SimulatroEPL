using SimulatorEPL.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimulatorEPL
{
    public class Matchmaker : MonoBehaviour
    {
        [SerializeField]
        private TeamsDb teamsDb;

        private const int teamsInGroupCount = 4;
        private const int gamesInRoundCount = 6;

        private readonly WaitForSeconds waitSecond = new WaitForSeconds(1f);

        private readonly Queue<Game> currentGames = new Queue<Game>();
        private readonly Queue<Game> nextGames = new Queue<Game>();

        private void Start()
        {
            Init();
            StartCoroutine(SimulateNewRound());
        }

        private void Init()
        {
            GenerateShedule();
        }

        private void GenerateShedule()
        {
            List<Team> sortedTeams = new List<Team>(teamsDb.Teams);

            for (int i = 0; i < sortedTeams.Count - 1; i++)
            {
                int id = Random.Range(i + 1, sortedTeams.Count);
                var tempGame = sortedTeams[id];

                sortedTeams[id] = sortedTeams[i];
                sortedTeams[i] = tempGame;
            }

            //int groupsCount = sortedTeams.Count / teamsInGroupCount;
            //List<List<Team>> teamGroups = new List<List<Team>>();
            //int teamsCounter = 0;

            //for (int i = 0; i < groupsCount; i++)
            //{
            //    List<Team> group = new List<Team>();

            //    for (int j = 0; j < teamsInGroupCount; j++)
            //    {
            //        group.Add(sortedTeams[teamsCounter]);
            //        teamsCounter++;
            //    }

            //    teamGroups.Add(group);
            //}

            List<Game> games = new List<Game>();

            for (int i = 0; i < sortedTeams.Count; i++) // TODO: Нормальный алгоритм 
            {
                Team home = sortedTeams[i];
                List<Team> availableTeams = new List<Team>(sortedTeams);
                availableTeams.Remove(home);

                for (int j = 0; j < 8; j++)
                {
                    int random = Random.Range(0, availableTeams.Count);
                    Team away = availableTeams[random];
                    availableTeams.Remove(away);

                    Game game = new Game(home, away);
                    games.Add(game);
                }
            }

            foreach (var game in games)
            {
                nextGames.Enqueue(game);
                Messenger<Game>.Broadcast(AppEvent.GameNextAdded, game);
            }

            Debug.Log("Sheldure games count: " + games.Count);
        }

        private void AddNewRoundGames()
        {
            for (int i = 0; i < gamesInRoundCount; i++)
                TryAddCurrentGame();
        }

        private IEnumerator SimulateNewRound()
        {
            AddNewRoundGames();
            Debug.Log("Round started. Games: " + currentGames.Count);

            int counter = AppConstants.GameDurationSeconds;

            while (counter > 0)
            {
                counter--;

                foreach (var game in currentGames)
                    game.SimulateMinute();

                yield return waitSecond;
            }

            while (currentGames.Count > 0)
            {
                var game = currentGames.Dequeue();
                Messenger<Game>.Broadcast(AppEvent.GameFinished, game);
            }

            yield return null;
            Debug.Log("Games finished");

            if (nextGames.Count > 0)
                StartCoroutine(ShowTimeOut());
            else
                Debug.Log("All games finished");
        }

        private IEnumerator ShowTimeOut()
        {
            int timer = 5;

            while (timer > 0)
            {
                Debug.Log("Pause: " + timer);
                yield return waitSecond;
                timer--;
            }

            yield return null;
            StartCoroutine(SimulateNewRound());
        }

        private void TryAddCurrentGame()
        {
            if (nextGames.Count > 0)
            {
                var game = nextGames.Dequeue();
                Messenger<Game>.Broadcast(AppEvent.GameNextRemoved, game);

                currentGames.Enqueue(game);
                Messenger<Game>.Broadcast(AppEvent.GameStarted, game);
                Debug.Log(game.ToString() + " Started");
            }
        }

    }
}
