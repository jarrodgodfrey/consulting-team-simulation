namespace team_performance_simulator.Models;

internal class Team
{
    public Team()
    {
        TotalPoints = 100;
    }

    public int NumberOfDevelopers { get; init; }
    public int NumberOfQaTesters { get; init; }
    public int NumberOfProjectManagers { get; init; }
    public int TotalPoints { get; init; }

    /// <summary>
    /// This method contains a simple model for
    /// projecting the time in weeks for a team made up of
    /// any permuation of developer, project manager and QA Tester
    /// </summary>
    /// <returns>Projection for duration of project in weeks</returns>
    internal double GetWeeklyProjection()
    {
        var random = new Random();
        var hoursInWorkWeek = 40;
        var existingProjectManagerFactor = .45;
        var lackOfProjectManagerFactor = 1.3;
        var qaTesterFactor = 0.5;
        var developerPerformance = new List<(double weeklyVelocity, double pointsPerHours)>();

        //Get the random weekly velocities for each dev (factor this out)
        var developerCounter = NumberOfDevelopers;
        while (developerCounter > 0)
        {
            //using a random base point value between 3 and 8 points
            var weeklyVelocity = (double)random.Next(3, 8);
            var pointsPerHour = weeklyVelocity / 40;
            developerPerformance.Add((weeklyVelocity, pointsPerHour));
            developerCounter--;
        }

        var pointsCounter = 0.0;
        var developerHoursForProject = 0;
        while (pointsCounter < TotalPoints)
        {
            pointsCounter += developerPerformance.Sum(dp => dp.pointsPerHours);
            developerHoursForProject++;
        }
        
        //TODO: cleanup if pointsCounter is slightly over TotalPoints -> convert to hours and subtract from devHoursCounter

        //add random drag hours for project
        var totalProjectHours = developerHoursForProject * GetRandomVariance(random);

        //Estimating having a project manager would reduce hours by 45% and without one would delay project by 30%
        if (NumberOfProjectManagers > 0)
        {
            totalProjectHours -= (totalProjectHours * existingProjectManagerFactor);
        }
        else
        {
            totalProjectHours += (totalProjectHours * lackOfProjectManagerFactor);
        }

        if (NumberOfQaTesters > 0)
        {
            totalProjectHours += developerHoursForProject * qaTesterFactor;
        }

        return totalProjectHours / hoursInWorkWeek;
    }

    private static double GetRandomVariance(Random random)
    {
        var next = random.NextDouble();
        return 0.75 + (next * (1.2 - 0.75));
    }

}