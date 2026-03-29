using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api
{
    public class Game
    {
        public HashSet<Player> Players { get; set; } = [];
        public List<Question> Questions { get; set; } = [];
        public List<PlayerAnswer> Answers { get; set; } = [];
        public int NumberOfPlayers { get; set; }
        public int CurrentQuestion { get; set; } = 0;
        public bool IsGameOver { get; private set; } = false;
        public Dictionary<string, int> Scores { get; private set; } = [];
        public List<Player> Winners { get; private set; } = [];

        public Game(int numberOfPlayers)
        {
            NumberOfPlayers = numberOfPlayers;
        }

        public void AddPlayer(Player player)
        {
            if (Players.Count <= NumberOfPlayers)
            {
                Players.Add(player);
            }
        }

        public void StartGame()
        {
            if (Players.Count == NumberOfPlayers)
            {
                CurrentQuestion = 0;
            }
        }

        public void SubmitAnswer(Player player, string answer, int questionNumber)
        {
            if (questionNumber != CurrentQuestion)
            {
                throw new InvalidOperationException("Cannot submit answer for a question that is not currently active.");
            }


            Answers.Add(new PlayerAnswer
            {
                Player = player,
                Question = Questions[CurrentQuestion],
                Answer = answer
            });
            
            if (Answers.Count == Players.Count)
            {
                CurrentQuestion++;
                if (CurrentQuestion >= Questions.Count)
                {
                    EndGame();
                }
            }
        }

        private void EndGame()
        {
            IsGameOver = true;

            foreach (var player in Players)
            {
                var correct = Answers
                    .Count(a => a.Player == player &&
                                string.Equals(a.Answer, a.Question.Answer, StringComparison.OrdinalIgnoreCase));
                Scores[player.Name] = correct;
            }

            int highScore = Scores.Values.Max();
            Winners = [.. Players.Where(p => Scores.GetValueOrDefault(p.Name) == highScore)];
        }
    }
}