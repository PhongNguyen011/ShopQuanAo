// Dropdown click (hỗ trợ mobile)
document.addEventListener('click', function (e) {
    const isTrigger = e.target.closest('.dropdown > a');
    if (isTrigger) {
        e.preventDefault();
        const menu = isTrigger.parentElement.querySelector('.dropdown-menu');
        if (menu) menu.style.display = menu.style.display === 'block' ? 'none' : 'block';
    } else {
        // click ngoài thì đóng
        document.querySelectorAll('.dropdown .dropdown-menu').forEach(m => m.style.display = '');
    }
});

// Thu nhỏ header khi scroll
(function () {
    const header = document.querySelector('.site-header');
    if (!header) return;
    window.addEventListener('scroll', () => {
        header.classList.toggle('is-scrolled', window.scrollY > 8);
    });
})();

// Mobile menu open/close + submenu toggle
(function () {
    const btn = document.querySelector('.mobile-menu-toggle');
    const menu = document.querySelector('.hamburger_menu');
    const overlay = document.querySelector('.fs_menu_overlay');
    const closeBtn = document.querySelector('.hamburger_close');

    const open = () => {
        if (!menu || !overlay) return;
        menu.classList.add('active');
        overlay.classList.add('active');
        menu.setAttribute('aria-hidden', 'false');
        document.body.style.overflow = 'hidden';
    };
    const close = () => {
        if (!menu || !overlay) return;
        menu.classList.remove('active');
        overlay.classList.remove('active');
        menu.setAttribute('aria-hidden', 'true');
        document.body.style.overflow = '';
    };

    btn && btn.addEventListener('click', open);
    overlay && overlay.addEventListener('click', close);
    closeBtn && closeBtn.addEventListener('click', close);
    document.addEventListener('keydown', e => { if (e.key === 'Escape') close(); });

    // Submenu toggle
    document.querySelectorAll('.hamburger_menu .menu_item.has-children > a').forEach(a => {
        a.addEventListener('click', (e) => {
            e.preventDefault();
            a.parentElement.classList.toggle('open');
        });
    });
})();
