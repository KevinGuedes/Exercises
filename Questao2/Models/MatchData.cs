using Newtonsoft.Json;

namespace Questao2.Models;

internal class MatchData
{
    [JsonProperty("team1goals")]
    public int Team1Goals { get; set; }

    [JsonProperty("team2goals")]
    public int Team2Goals { get; set; }
}
