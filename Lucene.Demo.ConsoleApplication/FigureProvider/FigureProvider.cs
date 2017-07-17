using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lucene.Demo.ConsoleApplication
{
    public class FigureProvider
    {
        public static IEnumerable<Figure> Figures { get => figures; }

        public static void Init()
        {
            weiFigures = GetFigures("魏");
            shuFigures = GetFigures("蜀");
            wuFigures = GetFigures("吴");

            Run();
        }

        static IEnumerable<string> weiFigures;
        static IEnumerable<string> shuFigures;
        static IEnumerable<string> wuFigures;

        static List<Figure> figures = new List<Figure>();
        static object syncObj = new object();

        static IEnumerable<string> GetFigures(string country)
        {
            return File.OpenText(AppDomain.CurrentDomain.BaseDirectory + $"{country}.txt")
                       .ReadToEnd()
                       .Split(',')
                       .Select(t => Regex.Match(t, "[\u4e00-\u9fa5]+").Value)
                       .Where(t => !string.IsNullOrEmpty(t));
        }

        static void Run()
        {
            var t1 = Task.Run(() =>
            {
                foreach (var item in weiFigures)
                {
                    CrawlerFigure(item, "魏");
                }
            });
            var t2 = Task.Run(() =>
            {
                foreach (var item in shuFigures)
                {
                    CrawlerFigure(item, "蜀");
                }
            });
            var t3 = Task.Run(() =>
            {
                foreach (var item in wuFigures)
                {
                    CrawlerFigure(item, "吴");
                }
            });

            Task.WaitAll(t1, t2, t3);
        }

        static void CrawlerFigure(string figureName, string conuntry)
        {
            Figure figure = new Figure();

            figure.Country = conuntry;

            HttpWebRequest request = WebRequest.Create($"http://baike.baidu.com/item/{figureName}") as HttpWebRequest;
            request.Method = "GET";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    string rawHtml = streamReader.ReadToEnd();

                    var lemmaWgt_lemmaTitle_title = CrawlerHelper.GetDom(rawHtml, "dd", "lemmaWgt-lemmaTitle-title");

                    figure.FigureName = CrawlerHelper.GetInnerHtml(CrawlerHelper.GetH1(lemmaWgt_lemmaTitle_title));
                    figure.Appellation = CrawlerHelper.GetInnerHtml(CrawlerHelper.GetH2(lemmaWgt_lemmaTitle_title));
                    figure.KeyWords = Regex.Match(rawHtml, "(?<=meta name=\"keywords\" content=\").*?(?=\")").Value;
                    figure.Synopsis = CrawlerHelper.GetInnerHtml(CrawlerHelper.GetDom(rawHtml, "div", "lemma-summary"));

                    lock (syncObj)
                    {
                        figure.ID = (figures.Count + 1).ToString();
                        figures.Add(figure);
                    }

                    figure.Appellation = CrawlerHelper.GetInnerHtml(CrawlerHelper.GetH2(lemmaWgt_lemmaTitle_title));

                    responseStream.Dispose();
                    streamReader.Dispose();
                }
            }
        }
    }
}
