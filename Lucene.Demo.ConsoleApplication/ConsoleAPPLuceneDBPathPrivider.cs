using System;

namespace Lucene.Demo.ConsoleApplication
{
    public class ConsoleAPPLuceneDBPathPrivider : ILunceneDBPathProvider
    {
        private string _prePath = AppDomain.CurrentDomain.BaseDirectory;
        public string Get(Type type)
        {
            return _prePath + @"\Lucene\" + type.Name;
        }
    }
}
