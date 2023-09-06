using System.Text.Json.Serialization;
using PlaylistGame.Services.Repositories;

namespace PlaylistGame.Models;

public class Game
{
    public string GameCode { get; set; }
    public int PlayerCount => Players.Count;
    public int SongsCount => Songs.Count;
    public int ActualSongIndex { get; set; }
    
    [JsonIgnore]
    private string _gamePhase = "not-started";
    public string GamePhase
    {
        get => _gamePhase;
        set => _gamePhase = Gamephases.Contains(value) ? value : throw new Exception("Invalid game phase");
    }

    public List<Player> Players { get; set; }
    
    public List<SongInfo> Songs { get; set; }
    public int NumberOfSongsPerPlayer { get; set; }
    public int PointPerRightVote { get; set; }
    public int PointPerVoteFooled { get; set; }
    public int NumberOfRandomSongDeleted { get; set; }
    
    [JsonIgnore]
    private static readonly List<string> Gamephases = new()
    {
        "not-started",
        "started",
        "result",
        "end-result"
    };
}