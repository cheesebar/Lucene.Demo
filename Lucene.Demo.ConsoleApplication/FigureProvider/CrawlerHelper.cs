using System.Text.RegularExpressions;

namespace Lucene.Demo.ConsoleApplication
{
    public class CrawlerHelper
    {
        public static string GetDom(string html, string tagName, string className)
        {
            return Regex.Match(html, $"<{tagName}.*(?={className})(.|\n)*?</{tagName}>").Value;
        }
        public static string GetInnerHtml(string html)
        {
            return Regex.Replace(html, "<[^>]*>", "");
        }
        public static string GetH1(string html)
        {
            return Regex.Match(html, $"<h1.*(?=)(.|\n)*?</h1>").Value;
        }
        public static string GetH2(string html)
        {
            return Regex.Match(html, $"<h2.*(?=)(.|\n)*?</h2>").Value;
        }
    }
}
