import {createRouter, createWebHistory} from "vue-router";
import Authentication from "@/views/Authentication.vue";
import DepartmentPage from '@/views/DepartmentPage.vue'
import SpecializationPage from '@/views/SpecializationPage.vue'
import TeachersPage from '@/views/TeachersPage.vue'
import GroupsPage from '@/views/GroupsPage.vue'
import StudentsPage from '@/views/StudentsPage.vue'
import OrderPage from '@/views/OrderPage.vue'
import PerformancePage from '@/views/PerformancePage.vue'


const routes = [
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
    },
    {
        path: "/students",
        name: "students",
        component: StudentsPage,
    },
    {
        path: "/orders",
        name: "orders",
        component: OrderPage,
    },
    {
        path: "/Performance",
        name: "performance",
        component: PerformancePage,
    }


]

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes
})

export default router