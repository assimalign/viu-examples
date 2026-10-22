<template>
    <p>{{ Message }}</p>
</template>
<script setup lang="csharp">
    public string Message => "C# in a .vue container";
</script>
<style scoped>
    p {
        color: teal;
    }
</style>
