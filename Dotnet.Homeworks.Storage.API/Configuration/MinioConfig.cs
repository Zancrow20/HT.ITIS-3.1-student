namespace Dotnet.Homeworks.Storage.API.Configuration;

public class MinioConfig
{
    public static string MinioConfigString = "MinioConfig";
    public string Username { get; set; }
    public string Password { get; set; }
    public string Endpoint { get; set; }
    public int Port { get; set; }
    public bool WithSsl { get; set; }
}