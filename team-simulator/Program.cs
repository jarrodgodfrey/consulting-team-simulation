namespace team_performance_simulator;

class Program
{
    static async Task Main(string[] args)
    {
        await Simulator.SimulateTeamPerformanceAsync().ConfigureAwait(false);
        Environment.Exit(0);

    }

}

