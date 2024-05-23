namespace EventPi.Abstractions;

record WebHostEnv(string WwwRoot, string Content) : IWebHostingEnv
{

}