using PlaylistGame.Services.Repositories;

namespace PlaylistGame.Models;

public class Player : MongoBaseModel
{
    public string Name { get; set; }
    public HashSet<string> VotersNames { get; set; }
    public string ImageUrl { get; set; }
    public int score { get; set; }
}