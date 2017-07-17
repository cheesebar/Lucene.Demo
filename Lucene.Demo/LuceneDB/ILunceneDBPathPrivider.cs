using System;

namespace Lucene.Demo
{
    public interface ILunceneDBPathProvider
    {
        string Get(Type type);
    }
}
