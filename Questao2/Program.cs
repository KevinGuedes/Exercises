using Newtonsoft.Json;
using Questao2.Models;
using System.Web;

namespace Questao2;

public class Program
{
    public async static Task Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = await GetTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await GetTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static async Task<int> GetTotalScoredGoals(string team, int year)
    {
        using var client = new HttpClient();

        var playingAsHomeTeamTask = GetGoalsByTeamAsync(client, team, year, true);
        var playAsAwayTeamTask = GetGoalsByTeamAsync(client, team, year, false);
        var results = await Task.WhenAll(playingAsHomeTeamTask, playAsAwayTeamTask);

        return results[0] + results[1];
    }

    private static async Task<int> GetGoalsByTeamAsync(
        HttpClient client, 
        string team, 
        int year, 
        bool isHomeTeam)
    {
        var totalGoals = 0;
        var currentPage = 1;
        var totalPages = 1;
        var playingAs = isHomeTeam ? "team1" : "team2";

        var uriBuilder = new UriBuilder("https://jsonmock.hackerrank.com/api/football_matches");
        
        var query = HttpUtility.ParseQueryString(string.Empty);
        query.Set("year", year.ToString());
        query.Set(playingAs, team);

        while (currentPage <= totalPages)
        {
            query.Set("page", currentPage.ToString());
            uriBuilder.Query = query.ToString();

            var response = await client.GetAsync(uriBuilder.Uri);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException("Failed to get team data from API");

            var content = await response.Content.ReadAsStringAsync();
            var teamData = JsonConvert.DeserializeObject<TeamData>(content) ??
                throw new InvalidOperationException("Invalid payload received");

            foreach (var match in teamData.Data)
            {
                totalGoals += isHomeTeam ? match.Team1Goals : match.Team2Goals;
            }

            totalPages = teamData.TotalPages;
            currentPage++;
        }

        return totalGoals;
    }
}
