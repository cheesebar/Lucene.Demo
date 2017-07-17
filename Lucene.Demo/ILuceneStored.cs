namespace Lucene.Demo
{
    public interface ILuceneStored
    {
         string ID { get; set; }  
        /// <summary>
        /// 非表数据关键字,可以自定义
        /// </summary>
         string _Customer { get; set; }
        /// <summary>
        /// 类别,定向搜索
        /// </summary>
         string _Category { get; set; }
    }
}
