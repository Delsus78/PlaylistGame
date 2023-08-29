<script setup>
import HelloWorld from './components/HelloWorld.vue'
import ConnexionPage from './components/ConnexionPage.vue'
import Logo from "@/components/icons/Logo.vue";
import {computed} from "vue";
import { useStore } from "vuex";
import GamePage from "@/components/GamePage.vue";  // Ajouté pour accéder au store
import axios from 'axios';
import config from '@/config.js';
import GameInfo from "@/components/GameInfo.vue";

const store = useStore();
const game = computed(() => store.state.game);
const player = computed(() => store.state.player);
const missingPlayers = computed(() => store.state.missingPlayers);

const handleCodeRetrieved = async (code, playerName) => {
    // join game
    try {
        code = code.toUpperCase();
        const response = await axios.post(config.apiUrl + "Game/" + code + "/join?playerName=" + playerName);

        if (response.status === 200) {
            await store.dispatch('setPlayerIsOwner', false);
            await assignGameStoreAtJoining(response.data);
        }
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const handleGameCreated = async (gameParams, playerName) => {
    try {
        const url = `${config.apiUrl}Game?numberOfSongPerPlayer=${gameParams.numberOfSongsPerPlayer}&pointsPerRightVote=${gameParams.pointsPerRightVote}&pointsPerVoteFooled=${gameParams.pointsPerVoteFooled}`;
        const gameInfo = await axios.post(url);

        //join it
        const response = await axios.post(config.apiUrl + "Game/" + gameInfo.data.gameCode + "/join?playerName=" + playerName);

        await assignGameStoreAtJoining(response.data);

        // set player as owner
        await store.dispatch('setPlayerIsOwner', true);

    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const handleVote = async (playerIdToVote) => {
    try {
        const url = config.apiUrl + "Game/"+ game.value.gameCode + "/"+ player.value.id + "/vote/" + playerIdToVote;
        await axios.post(url);

    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const handleLeaveGame = async (gamePhase) => {
    try {
        if (gamePhase !== "end-result") {
            await axios.post(config.apiUrl + "Game/" + game.value.gameCode + "/leave?playerId=" + player.value.id);

            if (player.value.isOwner) {
                await handleEndGame();
            }
        }

        console.log("Player is leaving the game : "+ game.value.gameCode);
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    } finally {
        await store.dispatch('resetAll');
    }
}

const handleEndGame = async () => {
    try {
        if (player.value.isOwner) {
            await axios.post(config.apiUrl + "Game/" + game.value.gameCode + "/end");

            console.log("Owner is ending the game : "+ game.value.gameCode);
        }
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const handleNextRound = async () => {
    try {
        if (player.value.isOwner) {
            await axios.post(config.apiUrl + "Game/" + game.value.gameCode + "/next");

            console.log("Next Round of game : "+ game.value.gameCode);
        }
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const handleEndRound = async () => {
    try {
        if (player.value.isOwner) {
            await axios.post(config.apiUrl + "Game/" + game.value.gameCode + "/results");

            console.log("End Round of game : "+ game.value.gameCode);
        }
    } catch (error) {
        console.error('Erreur lors de l\'appel API:', error);
    }
}

const handleAddSongs = async (songsToAdd) => {
    try {
        await axios.post(config.apiUrl + "Game/" + game.value.gameCode + "/"+ player.value.id +"/addSongs", songsToAdd);

        await store.dispatch('setPlayerIsSongsGiven', true);
    } catch (error) {
        if (error.response.data.toString().includes("Song already added")) {
            // get song url from error message (|| url ||)
            const songUrl = error.response.data.toString().split("||")[1];
            alert("Song already added = " + songUrl);
        }
        else
          console.error('Erreur lors de l\'appel API:', error);
    }
}

const assignGameStoreAtJoining = async (joinGameDTO) => {
    await store.dispatch('setGame', joinGameDTO.game);
    await store.dispatch('setPlayer', joinGameDTO.player);
}

</script>
<template>
  <header>
    <div class="right-main">
      <Logo class="logo"/>
      <div class="wrapper">
          <HelloWorld msg="PLAYLIST GAME"/>
      </div>
    </div>
    <GameInfo v-if="game.gameCode" :game="game" :missing-players="missingPlayers"/>
  </header>

  <main>
    <ConnexionPage v-if="game.gameCode == null"
       @code-retrieved="handleCodeRetrieved"
       @game-created="handleGameCreated"/>
    <GamePage v-else
              @leave="handleLeaveGame"
              @end-round="handleEndRound"
              @next-round="handleNextRound"
              @add-songs="handleAddSongs"
              @vote="handleVote"/>
  </main>

  <footer>
      <span v-if="game.gameCode" @click="handleEndGame" class="stop-button">
          Stop
      </span>
      <span v-else class="footer-text">
          © 2021 - PLAYLIST GAME v0.3.1 - All rights reserved to Team UNC
      </span>

  </footer>
</template>

<style scoped>
header {
  line-height: 1.5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
  color: var(--color-logo);
  width: 125px;
  height: 125px;
}

.stop-button {
    font-size: 1.2rem;
    font-weight: bold;
    color: var(--vt-c-black-soft);
    position: fixed;
    bottom: 20px;
    right: 20px;
    padding: 10px 20px;
    border: none;
    cursor: pointer;
    transition: 0.1s;
    z-index: 1000; /* Pour s'assurer qu'il est au-dessus des autres éléments */
}

.footer-text {
    font-size: 1rem;
    font-weight: bold;
    color: var(--vt-c-black-soft);
    margin-top: 10rem;
    bottom: 0;
    left: 10px;
    transition: 0.1s;
    z-index: 1000;
}

.footer-text:hover {
    text-shadow: 0px 0px 5px var(--vt-c-green-1);
}

.stop-button:hover {
    text-shadow: 0px 0px 5px var(--vt-c-red-1);
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    flex-direction: column;
    padding-right: calc(var(--section-gap) / 2);
  }

  .right-main {
      display: flex;
      place-items: center;
      padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
    color: var(--color-logo);
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }

  .footer-text {
      font-size: 1.2rem;
      font-weight: bold;
      color: var(--vt-c-black-soft);
      position: fixed;
      bottom: 20px;
      left: 20px;
      padding: 10px 20px;
      transition: 0.1s;
      z-index: 1000;
  }
}
</style>
