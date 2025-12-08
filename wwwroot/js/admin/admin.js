window.addEventListener('DOMContentLoaded', event => {

    // ===== 1. KHỞI TẠO & KHÔI PHỤC SIDEBAR =====
    const sidebarWrapper = document.getElementById('sidebar-wrapper');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const productParent = document.getElementById('productMenuParent');
    const productSubmenu = document.getElementById('submenuProduct');

    const BREAKPOINT = 992;
    const isMobile = window.innerWidth < BREAKPOINT;
    const storedState = localStorage.getItem('sb|sidebar-toggle');

    // Khôi phục trạng thái sidebar
    if (storedState === 'true' || (isMobile && storedState === null)) {
        document.body.classList.add('sb-sidenav-toggled');
    } else {
        document.body.classList.remove('sb-sidenav-toggled');
    }

    // ===== 2. XỬ LÝ TAB "SẢN PHẨM" =====
    if (productParent && productSubmenu) {
        productParent.addEventListener('click', function (e) {
            const isActive = this.classList.contains('active');
            const isMiniMode = document.body.classList.contains('sb-sidenav-toggled');

            // Mini: Click sẽ mở sidebar và chuyển trang
            if (isMiniMode) {
                localStorage.setItem('sb|sidebar-toggle', 'false');
            }
            // Đang mở to & tab đang active → chỉ toggle submenu
            else if (isActive) {
                e.preventDefault();
                const bsCollapse = bootstrap.Collapse.getOrCreateInstance(productSubmenu, { toggle: false });
                bsCollapse.toggle();
                this.classList.toggle('collapsed');
                this.setAttribute('aria-expanded', this.getAttribute('aria-expanded') !== 'true');
            }
        });
    }

    // ===== 3. ĐÓNG SIDEBAR SAU KHI CHỌN LINK (MOBILE) =====
    const allSidebarLinks = document.querySelectorAll('#sidebar-wrapper .list-group-item');

    allSidebarLinks.forEach(link => {
        link.addEventListener('click', function () {
            if (window.innerWidth >= BREAKPOINT) return;

            const isParent = this.id === 'productMenuParent';
            const isActive = this.classList.contains('active');
            const isMini = document.body.classList.contains('sb-sidenav-toggled');

            if ((isParent && isActive) || (isParent && isMini)) return;

            document.body.classList.add('sb-sidenav-toggled');
            localStorage.setItem('sb|sidebar-toggle', 'true');
        });
    });

    // ===== 4. NÚT 3 GẠCH =====
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', e => {
            e.preventDefault();
            document.body.classList.toggle('sb-sidenav-toggled');
            localStorage.setItem('sb|sidebar-toggle', document.body.classList.contains('sb-sidenav-toggled'));
        });
    }

    // ===== 5. CLICK RA NGOÀI ĐỂ ĐÓNG SIDEBAR (MOBILE) =====
    document.addEventListener('click', function (event) {
        const overlay = window.innerWidth < BREAKPOINT;
        const isOpen = !document.body.classList.contains('sb-sidenav-toggled');

        if (overlay && isOpen) {
            if (!sidebarWrapper.contains(event.target) && !sidebarToggle.contains(event.target)) {
                document.body.classList.add('sb-sidenav-toggled');
                localStorage.setItem('sb|sidebar-toggle', 'true');
            }
        }
    });

    // ===== 6. TỰ THU GỌN KHI RESIZE =====
    window.addEventListener('resize', function () {
        if (window.innerWidth < BREAKPOINT &&
            !document.body.classList.contains('sb-sidenav-toggled')) {
            document.body.classList.add('sb-sidenav-toggled');
            localStorage.setItem('sb|sidebar-toggle', 'true');
        }
    });

});
