using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lucene.Demo
{
    public abstract class LuceneEntityBase:ILuceneStored
    {
        #region private
        private Dictionary<string, PropertyInfo> _propertiesCache;
        #endregion

        #region IndexerFields
        #region ILuceneStored
        [Indexer(false, true)]
        public string ID { get; set; }
        [Indexer(true, false)]
        public string _Customer { get; set; }
        [Indexer(true, false)]
        public string _Category { get; set; }
        #endregion

        /// <summary>
        /// 图片
        /// </summary>
        [Indexer(false, true)]
        public string Picture { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [Indexer(true, true)]
        public string Title { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        [Indexer(true, true)]
        public string Synopsis { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        [Indexer(false, true)]
        public string Url { get; set; }
        #endregion

        public LuceneEntityBase()
        {

        }
        

        protected IEnumerable<T> Search<T>(string searchText, int page, int pageSize, object condition = null) where T:new ()
        {
            var ConditionDictionary = null != condition ?  InitConditionSearchFields(condition) : new Dictionary<string, string>();

            var fullTextSearchFields = from propName in PropertiesCache.Select(t => t.Key)
                                       where !ConditionDictionary.ContainsKey(propName)
                                       select propName;

            using (var luceneDB = new LuceneDBSearcher(GetType()))
            {
                return luceneDB.Search<T>(searchText, fullTextSearchFields, page, pageSize, ConditionDictionary);
            }
        }
        /// <summary>
        /// 属性缓存
        /// </summary>
        protected Dictionary<string, PropertyInfo> PropertiesCache
        {
            get
            {
                if(null == _propertiesCache)
                {
                    _propertiesCache = new Dictionary<string, PropertyInfo>();
                    foreach (var prop in GetType().GetProperties())
                    {
                        var attr = prop.GetCustomAttribute<IndexerAttribute>(true);

                        if (null != attr && attr.Index)
                        {
                            _propertiesCache.Add(prop.Name, prop);
                        }
                    }
                }
                return _propertiesCache;
            }
        }

        /// <summary>
        /// 初始化 且 条件
        /// </summary>
        protected virtual Dictionary<string, string> InitConditionSearchFields(object andCondition)
        {
            var _conditionDictionary = new Dictionary<string, string>();

            var type = GetType();
            var andConditionType = andCondition.GetType();
            var conditions = type.GetInterfaces().Where(t => typeof(ICondition).IsAssignableFrom(t) && t!= typeof(ICondition))
                                                 .SelectMany(t => t.GetProperties() /*t.GetInterfaceMap(t).InterfaceMethods*/)
                                                 .Select(t => t.Name);
            foreach (var condition in conditions)
            {
                if (!_conditionDictionary.ContainsKey(condition))
                {
                    _conditionDictionary.Add(condition, andConditionType.GetProperty(condition).GetValue(andCondition)?.ToString() ?? string.Empty);
                }
            }

            return _conditionDictionary;
        }
    }
}
