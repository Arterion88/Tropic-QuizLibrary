using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using QuizLibrary;


namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Test test = new Test("Test0");
            List<Question2> q = test.Languages["en"];
            XmlSerializer SerializerObj = new XmlSerializer(typeof(List<Question>));

            // Create a new file stream to write the serialized object to a file
            TextWriter WriteFileStream = new StreamWriter("test.xml");
            SerializerObj.Serialize(WriteFileStream, q);


        }
    }
}
