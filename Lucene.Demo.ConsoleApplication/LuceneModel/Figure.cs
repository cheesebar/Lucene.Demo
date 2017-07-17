using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucene.Demo.ConsoleApplication
{
    public class Figure : Lucene.Demo.LuceneEntityBase, ICountry
    {
        [Indexer(true,true)]
        public string Country { get; set; }
        [Indexer(true,true)]
        public string FigureName { get; set; }
        /// <summary>
        /// 称谓
        /// </summary>
        [Indexer(true,true)]
        public string Appellation { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        [Indexer(true,true)]
        public string KeyWords { get; set; }
        public IEnumerable<Figure> Search(string searchText, int page, int pageSize, object condition = null)
        {
            return Search<Figure>(searchText, page, pageSize, condition);
        }
    }
    public interface ICountry : ICondition
    {
        string Country { get; set; }
    }
}
