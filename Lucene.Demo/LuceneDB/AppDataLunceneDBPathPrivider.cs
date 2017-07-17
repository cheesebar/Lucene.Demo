using System;

namespace Lucene.Demo
{
    public class AppDataLunceneDBPathProvider : ILunceneDBPathProvider
    {
        private string _prePath = AppDomain.CurrentDomain.BaseDirectory;
        public string Get(Type type)
        {
            return _prePath + @"\App_Data\Index\" + type.Name;
        }
    }
}
