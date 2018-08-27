using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
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

    public struct Question2
    {
        public int Id { get; set; }

        private readonly string testName;

        public string Prompt { get; set; }

        public string PromptImage { get { return String.Format("{0}.Images.{1}_0", testName, Id); } }

        public int CorrectAnswer { get; set; }

        public List<Answer> Answers { get; set; }

        public Question2(int id,string testName, string question,int correctAnswer)
        {
            Id = id;
            string[] items = question.Split(';');
            Prompt = items[0];
            CorrectAnswer = correctAnswer;
            this.testName = testName;
            Answers = new List<Answer>();
            for (int i = 0; i < items.Length; i++)
                Answers.Add(new Answer() { Text = items[i], Image= String.Format("{0}.Images.{1}_{2}", testName, Id,i+1) });
        }                   

        public struct Answer
        {
            public enum AnswerType
            {
                Button, Image, ImageButton, None
            }

            public AnswerType GetAnswerType
            {
                get
                {
                    AnswerType result;
                    result = 
                        (Text == "")
                        ?
                        (ImageExists) ? AnswerType.Image : AnswerType.None
                        :
                        (ImageExists) ? AnswerType.ImageButton : AnswerType.Button;
                    return result;
                }
            }



            public string Text { get; set; }
            public string Image { get; set; }

            public bool ImageExists {
                get
                {
                    return false; //TODO:Implement Image Exist Check
                }
            }
        }
    }

    public class Test
    {
        public string Name { get; set; }

        /// <summary>
        /// Index is language code (cs,en...)
        /// </summary>
        public Dictionary<string, List<Question2>> Questions { get; set; }

        public Test(string testName)
        {

            foreach (string fileName in GetAllTxt(testName))
            {
                if (fileName == "answers")
                {
                    //TODO: Load correct answers from file
                    continue;
                }
                Console.WriteLine(fileName.Split('.')[3]);

                Questions.Add(fileName.Split('.')[3], GetQuestionFromTxt(fileName));
            }
            Console.ReadKey();
        }
        
        public List<Question2> GetQuestionFromTxt(string fileName)
        {
            List<Question2> questions = new List<Question2>();
            int index =0;
            var lines = File.ReadLines(fileName);
            foreach (var line in lines)
            {
                questions.Add(new Question2(index,Name,line,0)); //TODO:Implement correct answer
                index++;
            }

                return questions;
        }

        private string[] GetAllTxt(string testName)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            string folderName = string.Format("{0}.Tests.{1}", executingAssembly.GetName().Name, testName);
            return executingAssembly
                .GetManifestResourceNames()
                .Where(r => r.StartsWith(folderName) && r.EndsWith(".txt"))
                //.Select(r => r.Substring(constantResName.Length + 1))
                .ToArray();
        }
    }

}
