<script setup>
import {computed, ref} from "vue";
import { useStore } from "vuex";
import * as signalR from "@microsoft/signalr";
import config from "@/config";
import ResultPage from "@/components/ResultPage.vue";
import WaitPage from "@/components/WaitPage.vue";
import InGamePage from "@/components/InGamePage.vue";

const store = useStore();
const emit = defineEmits(["leave","vote","endRound", "nextRound", "addSongs"]);
const gamePhase = ref("not-started");
const game = computed(() => store.state.game);
const player = computed(() => store.state.player);
const selectedYoutubeUrl = ref("");

const setGamePhase = (newVal) => {
    gamePhase.value = newVal;
}

// SIGNALR PART
const connection = new signalR.HubConnectionBuilder()
    .withUrl(config.apiUrl + "hubs/GameHub")
    .configureLogging(signalR.LogLevel.Information) // configurer le niveau de log
    .build();

connection.start().catch(err => console.error(err.toString())).then(() => {
    connection.invoke("JoinGroup", game.value.gameCode, player.value.id).catch(err => console.error(err.toString()));
});

const handleLeave = async () => {
    connection.invoke("LeaveGroup", game.value.gameCode).catch(err => console.error(err.toString())).then(() => {
    }).finally(() => emit('leave', gamePhase.value));
}

connection.on("next-round", (gameMsg) => {
    // reset votes
    gameMsg.players.forEach(player => {
        player.hasVoted = false;
    });

    assignGame(gameMsg);
    selectedYoutubeUrl.value = game.value.songs[game.value.actualSongIndex].songUrl;

    setGamePhase("started");
});

connection.on("end-round", (gameMsg) => {
    assignGame(gameMsg);
    setGamePhase("result");
});

connection.on("game-ended", (playersReceived) => {
    console.log("game-ended")
    assignPlayers(playersReceived);
    setGamePhase("end-result");
});

connection.on("new-vote", (votingPlayerId) => {
    console.log("new-vote");
    store.dispatch('setHasVoted', votingPlayerId);
});

connection.on("players-changed", (gameMsg) => {
    console.log("players-changed");
    assignGame(gameMsg);
});

connection.on("new-songs", (gameMsg) => {
    assignGame(gameMsg);
});

// END SIGNALR PART

const handleNextRound = async () => {
    emit('nextRound');
}

const assignPlayers = (players) => {
    store.dispatch('setPlayers', players);
}

const assignGame = (game) => {
    store.dispatch('setGame', game);
}

</script>
<template>
    <div v-if="gamePhase === 'not-started'">
        <WaitPage :game="game" :isOwner="player.isOwner" :displayAddSongsSection="!player.isSongsGiven"
                  @leave="handleLeave" @start="handleNextRound" @add-songs="emit('addSongs', $event)"></WaitPage>
    </div>

    <div v-else-if="gamePhase === 'started'">
        <InGamePage :isOwner="player.isOwner" :youtubeUrl="selectedYoutubeUrl" :players="game.players"
                    @endRound="emit('endRound')" @vote="emit('vote', $event)"></InGamePage>
    </div>

    <div v-else-if="gamePhase === 'result'">
        <ResultPage :isOwner="player.isOwner" :players="game.players" :impostor-id="game.songs[game.actualSongIndex].playerId"
                    @nextRound="handleNextRound"></ResultPage>
    </div>

    <div v-else-if="gamePhase === 'end-result'">
        <ResultPage :isOwner="false" :players="game.players"
                    isEndScreen @leave="handleLeave"></ResultPage>
    </div>
</template>
<style scoped>

</style>