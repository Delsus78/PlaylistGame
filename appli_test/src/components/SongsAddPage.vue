<template>
    <div>
        <div v-for="(item, index) in Array.from({ length: size }).map((_, i) => i)" :key="index">
            <text-box placeholder="youtube url here" @value-changed="handleInput(index, $event)" />
        </div>
        <ValidationButton @on-click="emitValues">Validate</ValidationButton>
    </div>
</template>

<script setup>
import { ref, defineEmits } from 'vue';
import TextBox from './TextBox.vue';
import ValidationButton from "@/components/ValidationButton.vue";

const { size } = defineProps(['size']);
const emit = defineEmits(["validate"]);

const values = ref(Array.from({ length: size }, () => ''));

const handleInput = (index, value) => {
    values.value[index] = value;
};

const emitValues = () => {
    emit('validate', values.value);
};
</script>
