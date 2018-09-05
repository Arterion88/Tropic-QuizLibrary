using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EmbeddedResourceHelper;

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

    [Serializable]
    public class Question2
    {
        #region Properties
        public int Id { get; set; }

        private readonly string testName;

        public string Prompt { get; set; }

        public int CorrectAnswer { get; set; }
        public int UserAnswer { get; set; }
        public bool IsCorrect { get { return CorrectAnswer == UserAnswer; } }

        public List<Answer> Answers { get; set; }
        #endregion

        #region Constructors

        public Question2()
        {

        }

        public Question2(int id, string testName, string question, int correctAnswer)
        {
            Id = id;
            string[] items = question.Split(';');
            Prompt = items[0];
            CorrectAnswer = correctAnswer;
            UserAnswer = -1;
            this.testName = testName;
            Answers = new List<Answer>();
            for (int i = 1; i < items.Length; i += 2)
                Answers.Add(new Answer(items[i], items[i + 1]));
        }

        #endregion

        [Serializable]
        public class Answer
        {
            #region Properties and Variables

            #region AnswerType
            public enum AnswerType
            {
                Button, Image, ImageButton, None
            }

            public AnswerType GetAnswerType
            {
                get { return Text == "" ? (ImageExists ? AnswerType.Image : AnswerType.None) : (ImageExists ? AnswerType.ImageButton : AnswerType.Button); }
            }
            #endregion

            public string Text { get; set; }

            private readonly string _image;
            public Uri Image {
                get
                {
                    if (_image == string.Empty)
                        return null;

                    if (!ImageExists)
                        return new Uri(string.Format("pack://application:,,,/QuizLibrary;Component/{0}/{1}", Test.ImageFolder, "not-found.png"), UriKind.RelativeOrAbsolute);

                    return new Uri(string.Format("pack://application:,,,/QuizLibrary;Component/{0}/{1}", Test.ImageFolder,_image),UriKind.RelativeOrAbsolute);
                }
            }

            public bool ImageExists => Assembly.GetExecutingAssembly().GetManifestResourceNames().Contains("/"+Test.ImageFolder+"/" +_image);

            public int CorrectAnswerIndex { get; set; }
            #endregion

            #region Constructors
                public Answer()
                {

                }
                public Answer(string text, string image)
                {
                    this.Text = text;
                    this._image = image;
                }
            #endregion            
        }
    }

    

}
