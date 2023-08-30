using PlaylistGame.Services.Repositories;

namespace PlaylistGame.Models;

public class Game
{
    public string GameCode { get; set; }
    public int PlayerCount => Players.Count;
    public int SongsCount => Songs.Count;
    public int ActualSongIndex { get; set; }
    
    public List<Player> Players { get; set; }
    
    public List<SongInfo> Songs { get; set; }
    public int NumberOfSongsPerPlayer { get; set; }
    public int PointPerRightVote { get; set; }
    public int PointPerVoteFooled { get; set; }
    public int NumberOfRandomSongDeleted { get; set; }
}