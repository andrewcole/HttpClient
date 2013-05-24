namespace Illallangi
{
    public interface IHttpClient
    {
        string HttpGet(string uri, string accept = "", string proxy = "");
    }
}