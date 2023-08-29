<template>
    <div class="game-info">
        <ul>
            <li><span class="number-info">{{ game.playerCount }}</span> Joueurs</li>
            <li><span class="number-info">{{ game.numberOfSongsPerPlayer * game.playerCount }}</span> Manches</li>
            <li v-if="game.actualSongIndex !== -1">Manche <span class="number-info">{{ game.actualSongIndex + 1}}</span> en cours</li>
            <li>Nombre de musique au total : <span class="number-info">{{ game.songsCount }}</span></li>
            <li>
                Joueurs manquants : {{ missingPlayers.length }}
                <div v-for="playerName in missingPlayers" :key="playerName">
                    <span class="number-info">{{ playerName }}</span>
                </div>
            </li>
            <li><span class="number-info">{{ game.pointPerRightVote }}</span> Points par vote sur l'imposteur</li>
            <li><span class="number-info">{{ game.pointPerVoteFooled }}</span> Points par vote trompé en tant qu'imposteur</li>
        </ul>
    </div>
</template>

<script setup>
import { computed } from "vue";

const { game, missingPlayers } = defineProps({
    game: {
        type: Object,
        required: true,
        default: () => ({
            gameCode: null,
            playerCount: 0,
            songsCount: 0,
            numberOfSongsPerPlayer: 0,
            actualSongIndex: 0,
            pointPerRightVote: 0,
            pointPerVoteFooled: 0,
            players: [],
            songs: [],
        }),
    },
    missingPlayers: {
        type: Array,
        required: true,
        default: () => [],
    },
});

import { watch } from "vue";

watch(() => game.players, () => {
    console.log("game.players has changed, recalculating missingPlayers");
    // recalculating missingPlayers
}, { deep: true });

watch(() => game.songs, () => {
    console.log("game.songs has changed, recalculating missingPlayers");
    // recalculating missingPlayers
}, { deep: true });
</script>


<style scoped>
.game-info {
    margin-top: 3rem;
    padding: 2rem;
    border: 2px solid var(--color-border);
    border-radius: 10px;
}

.game-info h2 {
    margin-top: 0;
}

.game-info ul {
    list-style-type: none;
    padding: 0;
}

.number-info {
    font-size: 1.4rem; /* un peu plus gros que le reste du texte */
    color: var(--vt-c-red-1); /* Utiliser une couleur contrastante, comme un vert vif par exemple */
    font-weight: bold; /* Pour rendre le texte gras */
    text-shadow: 0px 0px 5px rgba(255, 0, 0, 0.6); /* une petite ombre pour le faire ressortir davantage */
    margin-top: 10px;
}
</style>
