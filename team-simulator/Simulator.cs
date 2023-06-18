using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using Meta.Numerics.Statistics.Distributions;
using team_performance_simulator.Models;

namespace team_performance_simulator;

public static class Simulator
{
    private const int NumberOfSims = 500;
    private static ConcurrentBag<TeamSimResult>? _simResultsBag;

    public static async Task SimulateTeamPerformanceAsync()
    {
        _simResultsBag = new ConcurrentBag<TeamSimResult>();
        var sw = new Stopwatch();
        sw.Start();

        var megaTeam = new Team
        {
            NumberOfDevelopers = 2,
            NumberOfProjectManagers = 0,
            NumberOfQaTesters = 1
        };

        var gigaTeam = new Team
        {
            NumberOfDevelopers = 1,
            NumberOfProjectManagers = 1,
            NumberOfQaTesters = 1
        };

        var petaTeam = new Team
        {
            NumberOfDevelopers = 3,
            NumberOfProjectManagers = 1,
            NumberOfQaTesters = 0
        };
                                        
        //We use our initial team projections and simulate over N simulations
        var teamProjections = new List<double> { megaTeam.GetWeeklyProjection(), gigaTeam.GetWeeklyProjection(), petaTeam.GetWeeklyProjection() };
       
        //Simulate pool of lineups for each trial
        var trialNumbers = Enumerable.Range(1, NumberOfSims - 1).ToArray();
        Console.WriteLine($"{sw.Elapsed} Running Simulation...");
        var simTasks = trialNumbers.Select(trial => Task.Run(() => SimulateAllTeamProjections(teamProjections)));
        await Task.WhenAll(simTasks).ConfigureAwait(false);
        Console.WriteLine($"{sw.Elapsed} Simulation Complete");

        //Output our simulations results to a CSV file (Excel)
        Console.WriteLine($"{sw.Elapsed} Writing csv to file system...");                         
        WriteCsv($"teamSimResults-{Guid.NewGuid()}.csv", _simResultsBag);              
        Console.WriteLine($"{sw.Elapsed} Writing csv to file system Complete");
    }

    //This function does the actual simulation
    /// <summary>
    /// 
    /// </summary>
    /// <param name="teamProjections"></param>
    private static void SimulateAllTeamProjections(List<double> teamProjections)
    {
        var simulationResult = new TeamSimResult();
        var trialSimulatedResults = new List<double>();
        foreach (var projection in teamProjections)
        {
            //we simulate each trial over a normal distribution curve
            var stdDev = Convert.ToDouble(0.5);
            var normalDistribution = new NormalDistribution(projection, stdDev);
            var randomDouble = GetRandomNumber();
            var simulatedProjection = normalDistribution.InverseLeftProbability(randomDouble);
            trialSimulatedResults.Add(Math.Round(simulatedProjection, 2));
        }

        simulationResult.MegaTeamTime = trialSimulatedResults[0];
        simulationResult.GigaTeamTime = trialSimulatedResults[1];
        simulationResult.PetaTeamTime = trialSimulatedResults[2];
        _simResultsBag?.Add(simulationResult);
    }

    private static double GetRandomNumber()
    {
        var random = new Random();
        var next = random.NextDouble();

        return 0.01 + (next * (0.99 - 0.01));
    }

    private static void WriteCsv(string fileName, IEnumerable<TeamSimResult> teamSimResults)
    {
        using var writer = new StreamWriter(fileName);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(teamSimResults);
    }

}