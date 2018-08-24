using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizLibrary
{
    public class Question
    {
        public string TestName { get; private set; }
        public string Prompt { get; private set; }

        public int CorrectAnswer { get; private set; }
        public string CorrectAnswerString { get { return Answers[CorrectAnswer]; } }

        public int TestAnswer { get; set; }
        public string TestAnswerString { get { return Answers[TestAnswer]; } }

        public bool IsCorrectAnswer { get { return TestAnswer == CorrectAnswer; } }

        public List<string> Answers { get; set; }

        public Question(string testName, string prompt, int correctAnswer, List<string> answers)
        {
            this.Prompt = prompt;
            this.CorrectAnswer = correctAnswer;
            this.Answers = answers;
            this.TestName = testName;
            this.TestAnswer = -1;
        }

        public static List<Question> GetQuestionsFromResource(List<string> testResource, bool randomize = false)
        {
            List<string> strings = testResource;
            List<Question> questions = new List<Question>();

            for (int i = 1; i < strings.Count; i++)
            {
                List<string> q = new List<string>(strings[i].Split(';'));
                List<string> answers = q.GetRange(2, q.Count - 2);
                questions.Add(new Question(strings[0], q[0], int.Parse(q[1]) - 1, answers));
            }

            return randomize ? ShuffleList<Question>(questions) : questions;
        }


        public static List<E> ShuffleList<E>(List<E> inputList)
        {
            List<E> randomList = new List<E>();

            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }

        public static List<Question> GetMissingAnswers(List<Question> questions)
        {
            return questions.FindAll(x => x.TestAnswer == -1);
        }
    }
}
