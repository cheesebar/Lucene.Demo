namespace Lucene.Demo
{
    public interface ICondition { }
    public interface ILanguage: ICondition
    {
        string Language { get; set; }
    }
}
