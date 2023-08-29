using Microsoft.AspNetCore.SignalR;
using PlaylistGame.Models;

namespace PlaylistGame.Services;

public class GameHubService
{
    private readonly IHubContext<GameHub> _hubContext;

    public GameHubService(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyGameEnded(string groupName, List<Player> players)
    {
        await _hubContext.Clients.Group(groupName).SendAsync("game-ended", players);
    }
    
    public async Task NotifyNewVote(string groupName, string playerId)
    {
        await _hubContext.Clients.Group(groupName).SendAsync("new-vote", playerId);
    }
    
    public async Task NotifyEndRound(string groupeName, Game game)
    {
        await _hubContext.Clients.Group(groupeName).SendAsync("end-round", game);
    }
    
    public async Task LeaveGroup(string groupName, string playerId)
    {
        // Supprimer le membre du groupe dans la collection
        if (GameHub.GroupMembers.TryGetValue(groupName, out var members))
        {
            members.RemoveWhere(x => x.Item2 == playerId);
        }
    }

    public async Task NotifyNewSongs(string gameCode, Game game)
    {
        await _hubContext.Clients.Group(gameCode).SendAsync("new-songs", game);
    }
    
    public async Task NotifyNewPlayerNumber(string groupName, Game game)
    {
        await _hubContext.Clients.Group(groupName).SendAsync("players-changed", game);
    }

    public async Task NotifyNextRound(string groupName, object game)
    {
        await _hubContext.Clients.Group(groupName).SendAsync("next-round", game);
    }
}