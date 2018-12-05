using System;

// The key WCF namespace.

namespace MagicEightBallServiceLib
{
    public class MagicEightBallService : IEightBall
    {
        public MagicEightBallService()
        {
            Console.WriteLine("The 8-Ball awaits your question...");
        }
        public string ObtainAnswerToQuestion(string userQuestion)
        {
            string[] answers =  { "Future Uncertain", "Yes", "No",
        "Hazy", "Ask again later", "Definitely" };

            // Return a random response.
            Random r = new Random();
            return answers[r.Next(answers.Length)];
        }
    }
}
