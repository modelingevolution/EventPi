namespace EventPi.Pwm.Ui
{
    public class MotorClient(string url)
    {
        readonly record struct Args(float Target, float Actual);
        // HttpProxy
        private readonly HttpClient _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(url)
        };


        public async Task Steer(string motorName, float target, float actual)
        {
            var response = await _httpClient.PostAsJsonAsync($"Motor/{motorName}", new Args(target,actual));
            response.EnsureSuccessStatusCode();
        }
    }
}
