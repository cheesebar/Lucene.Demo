using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace Lucene.Demo
{
    public class LuceneDBIndexer: LuceneDB
    {
        private Dictionary<Type,IEnumerable<PropertyInfo>> _tempProps;
        public LuceneDBIndexer(string path) : base(path)
        {
        }
        public LuceneDBIndexer() { }
        protected override void Open()
        {
            if (_isOpen)
            {
                Dispose();
            }
            if (!System.IO.Directory.Exists(DBPath))
            {
                System.IO.Directory.CreateDirectory(DBPath);
            }
            _analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            _luDirectory = Lucene.Net.Store.FSDirectory.Open(new System.IO.DirectoryInfo(DBPath));
            _indexWriter = new Lucene.Net.Index.IndexWriter(_luDirectory, _analyzer, true, Lucene.Net.Index.IndexWriter.MaxFieldLength.LIMITED);
            _isOpen = true;
        }
        public IEnumerable<PropertyInfo> GetProps(Type type)
        {
            if(null == _tempProps)
            {
                _tempProps = new Dictionary<Type, IEnumerable<PropertyInfo>>();
            }
            if (!_tempProps.ContainsKey(type))
            {
                _tempProps.Add(type, type.GetProperties().Where(prop => null != prop.GetCustomAttribute(typeof(IndexerAttribute), true)));
            }
            return _tempProps[type];
        }

        public void Add<T>(T obj)
        {
            SetDBPath(typeof(T));
            Open();

            Document document = new Document();
            foreach (var prop in GetProps(typeof(T)))
            {
                var value = prop.GetValue(obj)?.ToString();
                if(null != value)
                {
                    var attr = prop.GetCustomAttribute(typeof(IndexerAttribute), true) as IndexerAttribute;
                    var store = attr.Store ? Field.Store.YES : Field.Store.NO;
                    var index = attr.Index ? Field.Index.ANALYZED : Field.Index.NOT_ANALYZED;

                    document.Add(new Field(prop.Name, value, store, index));
                }
            }

            _indexWriter.AddDocument(document);
        }

        public void AddRange<T>(IEnumerable<T> objs)
        {
            SetDBPath(typeof(T));
            Open();

            foreach (var obj in objs)
            {
                Document document = new Document();
                foreach (var prop in GetProps(typeof(T)))
                {
                    var value = prop.GetValue(obj)?.ToString();
                    if (null != value)
                    {
                        var attr = prop.GetCustomAttribute<IndexerAttribute>();
                        var store = attr.Store ? Field.Store.YES : Field.Store.NO;
                        var index = attr.Index ? Field.Index.ANALYZED : Field.Index.NOT_ANALYZED;

                        document.Add(new Field(prop.Name, value, store, index));
                    }
                }
                _indexWriter.AddDocument(document);
            }
        }
        public override void Dispose()
        {
            _indexWriter.Optimize();
            _indexWriter.Dispose();
            base.Dispose();
        }
    }
}
