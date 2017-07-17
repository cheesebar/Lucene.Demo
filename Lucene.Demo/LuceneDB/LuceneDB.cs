using System;
using System.Collections.Generic;

namespace Lucene.Demo
{
    public abstract class LuceneDB:IDisposable
    {
        private ILunceneDBPathProvider _dbPathProvider = LunceneDBPathProviders.Current;
        private string _dbPath;
        protected System.IO.DirectoryInfo _sysDirectory;
        protected Lucene.Net.Store.Directory _luDirectory;
        protected Lucene.Net.Analysis.Standard.StandardAnalyzer _analyzer;
        protected Lucene.Net.Index.IndexWriter _indexWriter;
        protected bool _isOpen = false;
        public LuceneDB(string path)
        {
            _dbPath = path;
        }
        public LuceneDB() { }
        public void SetDBPath(Type type)
        {
            if (null == _dbPath)
            {
                _dbPath = _dbPathProvider.Get(type);
            }
        }

        public string DBPath
        {
            get
            {
                return _dbPath;
            }
        }

        protected abstract void Open();
        public  virtual void Dispose()
        {
            _analyzer.Close();
            _analyzer.Dispose();
        }
    }
}
