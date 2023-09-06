import { createStore } from 'vuex';

export default createStore({
    state: {
        game: {
            gameCode: null,
            playerCount: 0,
            players: [],
            songs: [],
            songsCount: 0,
            numberOfSongsPerPlayer: 0,
            actualSongIndex: 0,
            pointPerRightVote: 0,
            gamePhase: "",
            pointPerVoteFooled: 0
        },
        player: {
            id: "",
            name: "",
            imageUrl: "",
            score: 0,
            votedId: "",
            isOwner: false,
            isSongsGiven: false
        },
        missingPlayers: []
    },
    mutations: {
        // GAME MUTATIONS
        setGame(state, game) {

            // calculate missing players
            const playersId = game.players.map((player) => player.id);
            const songsPlayersId = game.songs.map((song) => song.playerId);
            const playersIdMissing = playersId.filter((playerId) => !songsPlayersId.includes(playerId));

            // get all players name from id
            state.missingPlayers = playersIdMissing.map((playerId) => {
                const player = game.players.find((player) => player.id === playerId);
                return player ? player.name : "";
            });

            state.game = game;
        },
        setGameCode(state, gameCode) {
            state.game.gameCode = gameCode;
        },
        setGamePhase(state, phase) {
            state.game.gamePhase = phase;
        },
        setPlayerCount(state, count) {
            state.game.playerCount = count;
        },
        setPlayers(state, players) {
            state.game.players = players;
        },
        // PLAYER MUTATIONS
        setPlayer(state, player) {
            state.player = player;
        },
        setPlayerIsOwner(state, isOwner) {
            state.player.isOwner = isOwner;
        },
        setPlayerIsSongsGiven(state, isSongsGiven) {
            state.player.isSongsGiven = isSongsGiven;
        },
        SET_HAS_VOTED(state, playerId) {
            console.log('SET_HAS_VOTED ', playerId);
            const player = state.game.players.find(p => p.id === playerId);
            if (player) player.hasVoted = true;
        },
        clearAll(state) {
            state.game = {
                gameCode: null,
                playerCount: 0,
                players: [],
                songs: [],
                songsCount: 0,
                numberOfSongsPerPlayer: 0,
                actualSongIndex: 0,
                pointPerRightVote: 0,
                gamePhase: "",
                pointPerVoteFooled: 0
            };
            state.player = {
                id: "",
                name: "",
                imageUrl: "",
                score: 0,
                votedId: "",
                isOwner: false
            };
        }
    },
    actions: {
        setHasVoted({ commit }, playerId) {
            commit('SET_HAS_VOTED', playerId);
        },
        resetAll({ commit }) {
            commit('clearAll');
        },
        // GAME ACTIONS
        setGame({ commit }, game) {
            commit('setGame', game);
        },
        setGameCode({ commit }, gameCode) {
            commit('setGameCode', gameCode);
        },
        setGamePhase({ commit }, phase) {
            commit('setGamePhase', phase);
        },
        setPlayerCount({ commit }, count) {
            commit('setPlayerCount', count);
        },
        setPlayers({ commit }, players) {
            commit('setPlayers', players);
        },
        // PLAYER ACTIONS
        setPlayer({ commit }, player) {
            commit('setPlayer', player);
        },
        setPlayerIsOwner({ commit }, isOwner) {
            commit('setPlayerIsOwner', isOwner);
        },
        setPlayerIsSongsGiven({ commit }, isSongsGiven) {
            commit('setPlayerIsSongsGiven', isSongsGiven);
        }
    }
});
