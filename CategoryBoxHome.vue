<template>
    <router-link class="item-category" :to="{ name: 'ListProducts', params: { id: category.id } }">
        <div class="card-img">
            <img :src="category.imageUrl" class="card-img-top" alt="...">
            <h5 class="card-title">{{ category.categoryName }}</h5>
            <p class="card-text">{{ category.description }}</p>
        </div>
    </router-link>
</template>

<script>
export default {
    name: 'CategoryBoxHome',
    props: ["category"],
    methods: {},
}
</script>
<style scoped>

.item-category .card-img{
    align-items: center;
    height: 250px;
    overflow: hidden;

}
.item-category .card-img img {
    transition: all 0.3s;
}

.item-category .card-img:hover img {
    transform: scale(1.2);
}
</style>