using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using System.Collections;

namespace Lucene.Demo
{

    public class LunceneDBPathProviders
    {
        private static ILunceneDBPathProvider _instance;
        static LunceneDBPathProviders()
        {
            _instance = new AppDataLunceneDBPathProvider();
        }

        public static ILunceneDBPathProvider Current { get { return _instance; }  }

        public static void SetLunceneDBPathPrivider(ILunceneDBPathProvider current)
        {
            _instance = current;
        }
    }
}
