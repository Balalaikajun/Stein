import {createRouter, createWebHistory} from "vue-router";
import TestPage from "@/views/TestPage.vue";
import Authentication from "@/views/Authentication.vue";
import DepartmentPage from '@/views/DepartmentPage.vue'
import Specialization from '@/views/Specialization.vue'


const routes = [
    {
        path: "/",
        name: "test",
        component: TestPage,
    },
    {
        path: "/authentication",
        name: "authentication",
        component: Authentication,
    },
    {
        path: "/departments",
        name: "departments",
        component: DepartmentPage,
    },
    {
        path: "/specializations",
        name: "specializations",
        component: Specialization,
    }

]

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes
})

export default router