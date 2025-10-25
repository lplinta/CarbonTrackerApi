using Reqnroll;

namespace CarbonTrackerApi.BDDTests.Hooks
{
    [Binding]
    public class Hooks(ScenarioContext scenarioContext)
    {
        private static HttpClient Client { get; set; } = null!;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            Client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5001")
            };
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            Console.WriteLine($"Iniciando cenário: {scenarioContext.ScenarioInfo.Title}");
        }

        [AfterScenario]
        public void AfterScenario()
        {
            Console.WriteLine($"Finalizando cenário: {scenarioContext.ScenarioInfo.Title}");
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            Client.Dispose();
        }
    }
}