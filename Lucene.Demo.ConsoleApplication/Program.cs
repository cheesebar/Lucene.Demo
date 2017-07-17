using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucene.Demo.ConsoleApplication
{
    public enum Country
    {
        魏 = 1,
        蜀 = 2,
        吴 = 3,
        默认 = 4
    }
    class Program
    {
        static Program()
        {
            LunceneDBPathProviders.SetLunceneDBPathPrivider(new ConsoleAPPLuceneDBPathPrivider());
        }
        static void Main(string[] args)
        {
            CreateNew();

            string key;
            while (true)
            {
                string str;
                Country country;
                while (true)
                {
                    Console.WriteLine("请输入国家:1.魏    2.蜀    3吴    4.默认");

                    str = Console.ReadLine();

                    if (Enum.TryParse(str, out country))
                    {
                        break;
                    }
                }

                while (true)
                {
                    Console.WriteLine($"请输入查询关键字:(当前国家:{country.ToString()})");
                    Console.WriteLine();
                    key = Console.ReadLine();
                    if (!string.IsNullOrEmpty(key))
                    {
                        break;
                    }
                }

                Input(key, country);
            }
        }

        private static void Input(string key, Country country)
        {
            Console.WriteLine("---------------------------------------------------------");

            Console.WriteLine("开始查询..." + "(关键字为" + key + ")");

            var seacher = new Figure();

            var result = seacher.Search(key, 1, 100, country != Country.默认 ? new { Country = country.ToString() } : null);
            Console.WriteLine("查询结果如下:");

            Console.WriteLine("合计:" + result.Count());
            int index = 1;
            foreach (var item in result)
            {
                Console.WriteLine(index++ + ":");
                Console.WriteLine("ID:\t" + item.ID);
                Console.WriteLine("人物姓名:\t" + item.FigureName);
                Console.WriteLine("头衔:\t" + item.Appellation);
                Console.WriteLine("国家:\t" + item.Country);
                Console.WriteLine("关键字:\t" + item.KeyWords);
                Console.WriteLine("简介:\t" + item.Synopsis);
                Console.WriteLine();
            }
            Console.WriteLine("---------------------------------------------------------");
        }

        private static void CreateNew()
        {
            Console.WriteLine("是否需要更新索引(默认不更新)?输入 yes or no");

            string update = Console.ReadLine();

            if (update.ToLower() == "yes" || (!System.IO.Directory.Exists(LunceneDBPathProviders.Current.Get(typeof(Figure)))))
            {
                if (update.ToLower() != "yes")
                {
                    Console.WriteLine("第一次运行,需要从网络下载数据...");
                }
                Console.WriteLine("开始从网络上获取数据...");
                FigureProvider.Init();
                Console.WriteLine("数据获取完成...");
                Console.WriteLine();

                Console.WriteLine("开始创建索引...");

                using (var luceneDB = new LuceneDBIndexer())
                {
                    luceneDB.AddRange(FigureProvider.Figures);
                }

                Console.WriteLine("索引创建完成...");
                Console.WriteLine();
            }
        }
    }
}
