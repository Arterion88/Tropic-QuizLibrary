using EmbeddedResourceHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace QuizLibrary
{
    public class Test
    {
        #region Const and Static
        public static AssemblyName GetAssembly { get { return Assembly.GetExecutingAssembly().GetName(); } }
        public const string ImageFolder = "Images";
        public const string TestsFolder = "Tests";
        public const string imageFileType = "jpg";
        public static List<string> GetAllTests
        {
            get
            {
                List<string> temp, result = new List<string>();
                temp = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x => x.Contains(correctAnswersFile)).ToList();
                foreach (var item in temp)
                    result.Add(item.Split('.')[2]);

                return result;
            }
        }
        #endregion

        #region Properties and Variables
        public string Name { get; set; }
        public Dictionary<string, List<Question2>> Languages { get; set; } = new Dictionary<string, List<Question2>>();
        public List<int> CorrectAnswers = new List<int>();
        #endregion

        #region Constructors
        public Test(string testName)
        {
            this.Name = testName;

            

            foreach (string fileName in GetAllTxt())
            {
                Languages.Add(fileName.Split('.')[3], GetQuestionFromTxt(fileName));
            }
        }
        #endregion

        #region Methods
        #region GetQuestions
        public List<Question2> GetQuestions(CultureInfo culture)
        {
            List<Question2> result = new List<Question2>();
            foreach (var item in GetAllTxt())
            {
                string fileName = item.Split('.')[3];

                if (culture.TwoLetterISOLanguageName == new CultureInfo(fileName).TwoLetterISOLanguageName)
                {
                    result = Languages[fileName];
                    break;
                }
                else continue;
            }
            return result;

        }

        #endregion

        #region GetFolderPaths
        public static string GetTestPath(string testName) => string.Format("{0}.{1}.{2}", GetAssembly.Name, TestsFolder, testName);
        public static string GetImageFolderPath(string testName) => string.Format("{0}.{1}", GetTestPath(testName), ImageFolder);
        #endregion

        private List<Question2> GetQuestionFromTxt(string fileName)//TODO: correctAnswers
        {
            List<Question2> questions = new List<Question2>();
            int index = 0;



            using (StreamReader reader = new StreamReader(EmbeddedResource.GetAsStream(Assembly.GetExecutingAssembly(), fileName)))
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        break;

                    questions.Add(new Question2(index, Name, line, CorrectAnswers[index]));
                    index++;
                }

            return questions;
        } 
        private string[] GetAllTxt()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            string folderName = GetTestPath(Name);
            return executingAssembly
                .GetManifestResourceNames()
                .Where(r => r.StartsWith(folderName) && r.EndsWith(".txt"))
                //.Select(r => r.Substring(constantResName.Length + 1))
                .ToArray();
        } 
        #endregion
    }

}
