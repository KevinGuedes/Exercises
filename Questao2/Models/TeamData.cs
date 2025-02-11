using Newtonsoft.Json;

namespace Questao2.Models;

internal class TeamData
{
    [JsonProperty("total_pages")]
    public int TotalPages { get; set; }

    [JsonProperty("data")]
    public IEnumerable<MatchData> Data { get; set; } = new List<MatchData>();
}
