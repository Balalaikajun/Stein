import {createRouter, createWebHistory} from "vue-router";
import TestPage from "@/views/TestPage.vue";
import Authentication from "@/views/Authentication.vue";
import DepartmentPage from '@/views/DepartmentPage.vue'
import SpecializationPage from '@/views/SpecializationPage.vue'
import TeachersPage from '@/views/TeachersPage.vue'
import GroupsPage from '@/views/GroupsPage.vue'


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
        component: SpecializationPage,
    },
    {
        path: "/teachers",
        name: "teachers",
        component: TeachersPage,
    },
    {
        path: "/groups",
        name: "groups",
        component: GroupsPage,
    }

]

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes
})

export default router