using System;

namespace Lucene.Demo
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class IndexerAttribute : Attribute
    {
        public bool Index;
        public bool Store;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">是否为其创建索引</param>
        /// <param name="store">是否存储原始数据</param>
        public IndexerAttribute(bool index,bool store)
        {
            Index = index;
            Store = store;
        }
    }
}
