<script setup>
import WelcomeItem from './WelcomeItem.vue'
import GameCodeIcon from "@/components/icons/IconGameCode.vue";
import TextBox from "@/components/TextBox.vue";
import ValidationButton from "@/components/ValidationButton.vue";
import IconTooling from "@/components/icons/IconTooling.vue";
import {ref} from "vue";
import PseudoIcon from "@/components/icons/IconPseudo.vue";

const gameCode = ref('');
const numberOfSongsPerPlayer = ref(+localStorage.getItem('numberOfSongsPerPlayer') || 1);
const pointsPerRightVote = ref(+localStorage.getItem('pointsPerRightVote') || 1);
const pointsPerVoteFooled = ref(+localStorage.getItem('pointsPerVoteFooled') || 2);
const numberOfRandomSongDeleted = ref(+localStorage.getItem('numberOfRandomSongDeleted') || 0);
const pseudo = ref(localStorage.getItem('pseudo') || '');
const isErrored = ref(false);
const emit = defineEmits(["code-retrieved","game-created"]);

const saveLocalStorage = () =>  {
    localStorage.setItem('pseudo', pseudo.value);
    localStorage.setItem('numberOfSongsPerPlayer', numberOfSongsPerPlayer.value.toString());
    localStorage.setItem('pointsPerRightVote', pointsPerRightVote.value.toString());
    localStorage.setItem('pointsPerVoteFooled', pointsPerVoteFooled.value.toString());
    localStorage.setItem('numberOfRandomSongDeleted', numberOfRandomSongDeleted.value.toString());
}

const handleCodeValueChanged = (newValue) => {
    gameCode.value = newValue;
}

const handlePseudoValueChanged = (newValue) => {
    pseudo.value = newValue;
}

const handleCodeRetrieved = () => {
    if (pseudo.value.length === 0) {
        isErrored.value = true;
        return;
    }
    saveLocalStorage();
    emit('code-retrieved', gameCode.value, pseudo.value);
}

const handleGameCreation = () => {
    if (pseudo.value.length === 0) {
        isErrored.value = true;
        return;
    }
    saveLocalStorage();

    emit('game-created',
        {
            numberOfSongsPerPlayer: numberOfSongsPerPlayer.value,
            pointsPerRightVote: pointsPerRightVote.value,
            pointsPerVoteFooled: pointsPerVoteFooled.value,
            numberOfRandomSongDeleted: numberOfRandomSongDeleted.value
        }, pseudo.value);
}

</script>

<template>
    <WelcomeItem>
        <template #icon>
            <PseudoIcon />
        </template>
        <template #heading>Super pseudo</template>

        <TextBox :default-value="pseudo" placeholder="PSEUDO" @value-changed="handlePseudoValueChanged" :is-errored="isErrored"/>
    </WelcomeItem>
    <WelcomeItem>
        <template #icon>
            <GameCodeIcon />
        </template>
        <template #heading>Join with Game Code</template>

        <TextBox placeholder="Enter game code here" @value-changed="handleCodeValueChanged"/>
        <ValidationButton @onClick="handleCodeRetrieved"/>
    </WelcomeItem>
    <WelcomeItem>
        <template #icon>
            <IconTooling />
        </template>
        <template #heading>Create a Game</template>
        <div class="game-settings">
            <div class="number-inputs-container">
                <h4>Number of songs / player</h4>
                <h4>Points / right vote</h4>
                <h4>Points / fooled vote</h4>
                <h4>Random songs deleted</h4>
            </div>
            <div class="number-inputs-container">
                <text-box :default-value="numberOfSongsPerPlayer" placeholder="Number of songs per players" is-number-only @value-changed="numberOfSongsPerPlayer = $event" />
                <text-box :default-value="pointsPerRightVote" placeholder="Points/ right vote" is-number-only @value-changed="pointsPerRightVote = $event" />
                <text-box :default-value="pointsPerVoteFooled" placeholder="Points/ fooled vote" is-number-only @value-changed="pointsPerVoteFooled = $event" />
                <text-box :default-value="numberOfRandomSongDeleted" placeholder="Number of random songs deleted" is-number-only @value-changed="numberOfRandomSongDeleted = $event" />
            </div>
        </div>
        <ValidationButton msg="Create" @onClick="handleGameCreation"/>

    </WelcomeItem>
</template>

<style scoped>
.number-inputs-container {
    display: flex;
    justify-content: space-between;
    gap: 1rem; /* Espacement entre les éléments */
}
</style>