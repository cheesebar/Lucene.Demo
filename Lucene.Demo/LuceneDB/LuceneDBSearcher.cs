using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lucene.Demo
{
    public class LuceneDBSearcher: LuceneDB
    {
        private Type _searchType;
        public LuceneDBSearcher(string path) : base(path)
        {
        }
        public LuceneDBSearcher(Type type)
        {
            SetDBPath(type);
        }
        public LuceneDBSearcher() { }
        public Type SearchType
        {
            set
            {
                //判断该类型是否实现 某 约定
                _searchType = value;
            }
            get { return _searchType; }
        }
        public IEnumerable<T> Search<T>(string searchText, IEnumerable<string> fields, int page, int pageSize, Dictionary<string,string> condition= null) where T : new()
        {
            return GetModels<T>(SearchText(searchText, fields, page, pageSize, condition));
        }
  

        private IEnumerable<Document> SearchText(string searchText,IEnumerable<string> fields,int page,int pageSize, Dictionary<string,string> condition)
        {
            StringBuilder conditionWhere = new StringBuilder();

            foreach (var item in condition)
            {
                conditionWhere.Append(" +" + item.Key + ":" + item.Value);
            }

            Open();

            var parser = new Lucene.Net.QueryParsers.MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, fields.ToArray(), _analyzer);
            var search = new Lucene.Net.Search.IndexSearcher(_luDirectory, true);

            var query = parser.Parse("+" + searchText + conditionWhere.ToString());

            var searchDocs = search.Search(query, 100).ScoreDocs;

            return searchDocs.Select(t => search.Doc(t.Doc));
        }

        protected override void Open()
        {
            _luDirectory = Lucene.Net.Store.FSDirectory.Open(new System.IO.DirectoryInfo(DBPath));
            if (Lucene.Net.Index.IndexWriter.IsLocked(_luDirectory))
            {
                Lucene.Net.Index.IndexWriter.Unlock(_luDirectory);
            }
            var lockFilePath = System.IO.Path.Combine(DBPath, "write.lock");
            
            if (System.IO.File.Exists(lockFilePath))
            {
                System.IO.File.Delete(lockFilePath);
            }

            _analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        }
        private IEnumerable<T> GetModels<T>(IEnumerable<Document> documents) where T:new()
        {
            var type = typeof(T);
            var props = type.GetProperties().Where(prop => null != prop.GetCustomAttribute(typeof(IndexerAttribute), true));
            var objs = new List<T>();
            foreach (var document in documents)
            {
                var obj = new T();
                foreach (var prop in props)
                {
                    var attr = prop.GetCustomAttribute<IndexerAttribute>();

                    if (null != attr && attr.Store)
                    {
                        object v = Convert.ChangeType(document.Get(prop.Name), prop.PropertyType);
                        prop.SetValue(obj, v);
                    }
                }
                objs.Add(obj);
            }
            return objs;
        }
        public T GetModel<T>(Document document) where T : new()
        {
            var type = typeof(T);
            var props = type.GetProperties().Where(prop => null != prop.GetCustomAttribute(typeof(IndexerAttribute), true));

            var obj = new T();
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<IndexerAttribute>();

                if (null != attr && attr.Store)
                {
                    object v = Convert.ChangeType(document.Get(prop.Name), prop.PropertyType);
                    prop.SetValue(obj, v);
                }
            }
            return obj;
        }
        public override void Dispose()
        {
            _analyzer.Dispose();
            base.Dispose();
        }
    }
}
