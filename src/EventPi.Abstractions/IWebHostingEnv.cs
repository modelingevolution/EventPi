namespace EventPi.Abstractions;

public interface IWebHostingEnv
{
    string WwwRoot { get; }
    string Content { get; }
}