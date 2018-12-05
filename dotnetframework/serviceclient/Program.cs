using System;
using MagicEightBallServiceClient.ServiceReference1;

namespace MagicEightBallServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("***** Ask the Magic 8 Ball *****\n");

            using (EightBallClient ball = new EightBallClient("basicHttpBinding_IEightBall"))
            {

                Console.Write("Your question: ");
                //string question = Console.ReadLine();
                string question = "Will there be another fire?";
                string answer =
                    ball.ObtainAnswerToQuestion(question);
                Console.WriteLine("In response to {0}   8-Ball says: {1}", question, answer);
                Console.ReadLine();

            }

        }
    }
}
