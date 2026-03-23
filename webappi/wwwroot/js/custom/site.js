/**
 * site.js
 * Common application logic (Toasts, Modals, Theme)
 */

/* --- Theme Management --- */
const ThemeManager = {
    init() {
        // Set initial theme
        const stored = localStorage.getItem('theme') || 'light';
        this.set(stored);

        // Expose global for inline HTML calls (legacy support)
        window.setTheme = (t) => this.set(t);
    },

    set(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
        this.updateUI(theme);
    },

    updateUI(theme) {
        document.querySelectorAll('.theme-selector-item').forEach(el => {
            el.classList.toggle('active', el.dataset.theme === theme);
        });
    }
};

/* --- Toast Notifications --- */
const Toast = {
    show(type, message, duration = 4000) {
        const container = document.getElementById('toast-container-custom');
        const template = document.getElementById('toast-template');
        if (!container || !template) return;

        const clone = template.content.cloneNode(true);
        const toast = clone.querySelector('.toast-custom');

        // Configure
        const config = this.getConfig(type);
        toast.classList.add(config.class);
        toast.querySelector('.toast-title').textContent = config.title;
        toast.querySelector('.toast-message').textContent = message;
        toast.querySelector('.toast-icon i').className = `lni ${config.icon}`;

        // Events
        toast.querySelector('.toast-close').addEventListener('click', () => this.remove(toast));

        // Progress
        const progress = toast.querySelector('.toast-progress');
        progress.style.transition = `width ${duration}ms linear`;

        container.appendChild(toast);

        // Animate
        requestAnimationFrame(() => progress.style.width = '100%'); // Fill or empty? CSS says width 0 to start. Let's fill.

        setTimeout(() => this.remove(toast), duration);
    },

    remove(toast) {
        toast.classList.remove('slide-in-top');
        toast.classList.add('slide-out-top');
        toast.addEventListener('animationend', () => toast.remove());
    },

    getConfig(type) {
        switch (type) {
            case 'success': return { title: 'Success', icon: 'lni-checkmark-circle', class: 'toast-success' };
            case 'error': return { title: 'Error', icon: 'lni-cross-circle', class: 'toast-error' };
            case 'warning': return { title: 'Warning', icon: 'lni-warning', class: 'toast-warning' };
            default: return { title: 'Info', icon: 'lni-information', class: 'toast-info' };
        }
    }
};

// Global Exposure
window.showToast = (type, msg) => Toast.show(type, msg);

/* --- Modals --- */
const Modal = {
    open(id) {
        const modal = document.getElementById(id);
        if (modal) modal.classList.add('active');
    },
    close(id) {
        const modal = document.getElementById(id);
        if (modal) modal.classList.remove('active');
    },
    closeAll() {
        document.querySelectorAll('.modal').forEach(m => m.classList.remove('active'));
    }
};

// Auto-init Modals (Close on click outside or close button)
document.addEventListener('DOMContentLoaded', () => {
    ThemeManager.init();

    document.querySelectorAll('.close-modal').forEach(btn => {
        btn.addEventListener('click', (e) => {
            e.target.closest('.modal').classList.remove('active');
        });
    });

    window.openModal = (id) => Modal.open(id);
    window.closeModal = (id) => Modal.close(id);

    // Bootstrap 5 Modal Fix: Move to body to prevent z-index/clipping issues
    const bootstrapModals = document.querySelectorAll('.modal');
    bootstrapModals.forEach(modal => {
        document.body.appendChild(modal);
    });
});
