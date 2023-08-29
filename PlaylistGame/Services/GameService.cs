using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using PlaylistGame.Models;
using PlaylistGame.Services.Repositories;

namespace PlaylistGame.Services;

public class GameService
{
    private readonly ILogger<GameService> _logger;
    private readonly MongoDbRepository _gamesRepository;
    
    private readonly List<Game> _games;
    
    private readonly GameHubService _gameHubService;

    public GameService(
        ILogger<GameService> logger, 
        IOptions<ConnectionSetting> connectionSetting,
        GameHubService gameHubService)
    {
        _logger = logger;
        _gamesRepository = new MongoDbRepository(new MongoClient(), connectionSetting.Value.DatabaseName);
        _games = new List<Game>();
        _gameHubService = gameHubService;
    }

    public string GenerateId(int length = 5)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0987654321";
        var stringChars = new char[length];
        var random = new Random();
    
        for (var i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }
    
        var finalString = new String(stringChars);
    
        return finalString;
    }

    public async Task<Game> CreateGame(int numberOfSongPerPlayer, int pointPerRightVote, int pointPerVoteFooled)
    {
        var gameCode = GenerateId();
        
        var game = new Game
        {
            GameCode = gameCode,
            ActualSongIndex = -1,
            Players = new List<Player>(),
            Songs = new List<SongInfo>(),
            NumberOfSongsPerPlayer = numberOfSongPerPlayer,
            PointPerRightVote = pointPerRightVote,
            PointPerVoteFooled = pointPerVoteFooled
        };

        // register the game
        _games.Add(game);
        
        return game;
    }
    
    public async Task<JoinGameDTO> JoinGame(string gameCode, string playerName)
    {
        var game = await GetGame(gameCode);
        var playerId = GenerateId();
        var player = new Player
        {
            Id = playerId,
            Name = playerName,
            VotersNames = new HashSet<string>(),
            ImageUrl = "https://www.cc-cln.fr/build/images/huchet/pictos/icon-user.png",
            score = 0
        };

        game.Players.Add(player);
        
        await _gameHubService.NotifyNewPlayerNumber(gameCode, game);
        
        return new JoinGameDTO
        {
            Game = game,
            Player = player
        };
    }
    
    public async Task LeaveGame(string gameCode, string playerId)
    {
        var game = _games.FirstOrDefault(g => g.GameCode == gameCode);
        
        if (game == null)
            throw new Exception("Game not found");
        
        game.Players.RemoveAll(p => p.Id == playerId);
        game.Songs.RemoveAll(s => s.PlayerId == playerId);
        
        await _gameHubService.LeaveGroup(gameCode, playerId);
        await _gameHubService.NotifyNewPlayerNumber(gameCode, game);
    }
    
    public async Task<Game> GetGame(string gameCode)
    {
        var game = _games.FirstOrDefault(g => g.GameCode == gameCode);
        if (game == null)
            throw new Exception("Game not found");
        
        return game;
    }
    
    public IEnumerable<Game> GetGames() => _games;

    public async Task EndGame(string gameCode)
    {
        var game = await GetGame(gameCode);
        
        await _gameHubService.NotifyGameEnded(gameCode, game.Players);
        
        
        _games.RemoveAll(g => g.GameCode == gameCode);
    }

    public async Task Vote(string gameCode, string votantId, string voteId)
    {
        var game = await GetGame(gameCode);
        
        var votedPlayer = game.Players.FirstOrDefault(p => p.Id == voteId);
        
        var votant = game.Players.FirstOrDefault(p => p.Id == votantId);
        
        if (votedPlayer == null || votant == null)
            throw new Exception("Player not found");

        var impostorId = game.Songs[game.ActualSongIndex].PlayerId;
        
        // impostors can't score
        // scoring
        if (impostorId != votantId)
        {
            if (votedPlayer.Id == impostorId)
            {
                votant.score += game.PointPerRightVote;
            }
            else
            {
                var impostors = game.Players.Where(p => p.Id == impostorId).ToList();
                impostors.ForEach(i => i.score += game.PointPerVoteFooled);
            }
            
            votedPlayer.VotersNames.Add(votant.Name);
        
        }
        await _gameHubService.NotifyNewVote(gameCode, votantId);
    }

    public async Task NextRound(string gameCode)
    {
        var game = await GetGame(gameCode);
        
        // shuffle songs if index is -1
        if (game.ActualSongIndex == -1)
        {
            game.Songs = game.Songs.OrderBy(x => new Random().Next()).ToList();
        }
        
        game.ActualSongIndex++;
        
        // reset players votes
        game.Players.ForEach(p => p.VotersNames = new HashSet<string>());
        
        if (game.ActualSongIndex == game.SongsCount)
        {
            await EndGame(gameCode);
            return;
        }
        
        await _gameHubService.NotifyNextRound(game.GameCode, game);
    }

    public async Task EndRound(string gameCode)
    {
        var game = await GetGame(gameCode);
        
        await _gameHubService.NotifyEndRound(gameCode, game);
    }

    public async Task AddSongs(string gameCode, string playerId, List<string> songsUrls)
    {
        var game = await GetGame(gameCode);
        
        var player = game.Players.FirstOrDefault(p => p.Id == playerId);
        
        if (player == null)
            throw new Exception("Player not found");
        
        if (songsUrls.Count != game.NumberOfSongsPerPlayer)
            throw new Exception("Number of songs is not correct");

        foreach (var songUrl in songsUrls)
        {
            if (game.Songs.Any(s => s.SongUrl == songUrl))
            {
                game.Songs.RemoveAll(s => s.PlayerId == playerId);
                throw new Exception("Song already added ||" + songUrl + "||");
            }
            
            game.Songs.Add(new SongInfo { SongUrl = songUrl, PlayerId = playerId});
        }
        
        await _gameHubService.NotifyNewSongs(gameCode, game);
    }
}