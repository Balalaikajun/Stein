import {createRouter, createWebHistory} from "vue-router";
import TestPage from "@/views/TestPage.vue";
import Authentication from "@/views/Authentication.vue";
import Department from '@/configs/department.js'
import DepartmentPage from '@/views/DepartmentPage.vue'


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
    }

]

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes
})

export default router