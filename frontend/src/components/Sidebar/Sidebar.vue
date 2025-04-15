<template>
  <nav class="sidebar">
    <!-- Логотип -->
    <img
        src="@/assets/CollageLogo.png"
        alt="Логотип приложения"
        class="sidebar__logo"
    />

    <div class="sidebar__content">
      <!-- Меню -->
      <ul class="sidebar__menu">
        <li v-for="item in items" :key="item.title" class="menu-item">
          <div
              class="menu-item__header"
              :aria-expanded="item.isOpen"
              @click="item.children ? toggleItem(item) : null"
          >
            <span v-if="item.icon" class="menu-item__icon">{{ item.icon }}</span>
            <!-- Если элемент не имеет вложенных пунктов -->
            <template v-if="!item.children">
              <router-link
                  :to="item.path"
                  class="menu-item__link"
                  :class="{ 'menu-item__link--active': isActive(item.path) }"
                  exact-active-class="menu-item__link--active"
              >
                {{ item.title }}
              </router-link>
            </template>
            <!-- Если элемент имеет вложения -->
            <template v-else>
              <span class="menu-item__title">{{ item.title }}</span>
              <span class="menu-item__arrow">▶</span>
            </template>
          </div>

          <!-- Подменю с анимацией -->
          <transition name="menu-slide">
            <ul v-if="item.children && item.isOpen" class="submenu">
              <li v-for="child in item.children" :key="child.title" class="submenu__item">
                <router-link
                    :to="child.path"
                    class="submenu__link"
                    :class="{ 'submenu__link--active': isActive(child.path) }"
                    exact-active-class="submenu__link--active"
                >
                  {{ child.title }}
                </router-link>
              </li>
            </ul>
          </transition>
        </li>
      </ul>

      <!-- Кнопка выхода -->
      <button class="sidebar__logout" @click="handleLogout">
        <span class="logout__icon">🚪</span>
        <span class="logout__text">Выход</span>
      </button>
    </div>
  </nav>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import itemsData from '@/router/menuData.js'

const router = useRouter()
const route = useRoute()
const items = ref(itemsData)

const toggleItem = (item) => {
  if (item.children) {
    item.isOpen = !item.isOpen
  }
}

const handleLogout = () => {
  localStorage.removeItem('token')
  router.push('/authentication')
}

const isActive = (path) => route.path === path
</script>

<!-- Стили можно оставить без изменений, если они соответствуют дизайну -->
<style scoped>
.sidebar {
  --sidebar-item-height: 2.5rem;
  --sidebar-icon-size: 1.25rem;
  width: var(--sidebar-width);
  background: var(--secondary-background-color);
  border-right: var(--secondary-color);
  height: 100vh;
  padding: 1.25rem;
  display: flex;
  flex-direction: column;
}

.sidebar__logo {
  width: 80%;
  margin: 0 auto 2rem;
  object-fit: contain;
}

.sidebar__content {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  gap: 1rem;
}

.sidebar__menu {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.menu-item {
  display: flex;
  flex-direction: column;
}

.menu-item__header {
  display: flex;
  align-items: center;
  padding: 0 0.75rem;
  min-height: var(--sidebar-item-height);
  border-radius: var(--border-radius);
  cursor: pointer;
  transition: all var(--transition-duration) ease;
  background-color: var(--secondary-background-color);
  gap: 0.625rem;
}

.menu-item__header:hover {
  background-color: var(--hover-color);
}

.menu-item__link,
.menu-item__title {
  flex: 1;
  padding: 0.375rem 0;
  font-size: 0.9375rem;
  line-height: 1.3;
  color: var(--text-color);
}

.menu-item__link {
  display: flex;
  align-items: center;
  text-decoration: none;
  transition: all var(--transition-duration) ease;
}

.menu-item__link--active,
.menu-item__header[aria-expanded="true"] {
  background-color: var(--active-bg-color);
  color: var(--primary-color);
  font-weight: 500;
}

.menu-item__icon {
  width: var(--sidebar-icon-size);
  text-align: center;
  flex-shrink: 0;
}

.menu-item__arrow {
  transition: transform var(--transition-duration) ease;
  flex-shrink: 0;
}

.menu-item__header[aria-expanded="true"] .menu-item__arrow {
  transform: rotate(90deg);
}

.submenu {
  list-style: none;
  padding-left: 1.75rem;
  margin: 0.25rem 0 0;
  display: flex;
  flex-direction: column;
  gap: 0.125rem;
}

.submenu__link {
  display: flex;
  align-items: center;
  padding: 0 0.75rem;
  min-height: 2rem;
  text-decoration: none;
  color: var(--text-color);
  border-radius: var(--border-radius);
  transition: all var(--transition-duration) ease;
  font-size: 0.875rem;
}

.submenu__link:hover {
  background-color: var(--hover-color);
}

.submenu__link--active {
  background-color: var(--active-bg-color);
  color: var(--primary-color);
  font-weight: 500;
}

.sidebar__logout {
  display: flex;
  align-items: center;
  width: 100%;
  padding: 0 0.75rem;
  min-height: var(--sidebar-item-height);
  border: none;
  border-radius: var(--border-radius);
  background-color: var(--secondary-background-color);
  cursor: pointer;
  color: var(--text-color);
  transition: all var(--transition-duration) ease;
  gap: 0.625rem;
}

.sidebar__logout:hover {
  background-color: var(--error-hover-color);
  color: var(--error-color);
}

.logout__icon {
  width: var(--sidebar-icon-size);
  text-align: center;
}

.menu-slide-enter-active,
.menu-slide-leave-active {
  transition: all var(--transition-duration) ease;
  overflow: hidden;
}

.menu-slide-enter-from,
.menu-slide-leave-to {
  max-height: 0;
  opacity: 0;
  transform: translateY(-0.5rem);
}

.menu-slide-enter-to,
.menu-slide-leave-from {
  max-height: 20rem;
  opacity: 1;
  transform: translateY(0);
}
</style>
