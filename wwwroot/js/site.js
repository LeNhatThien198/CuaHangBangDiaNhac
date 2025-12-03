let lastScrollTop = 0;
const header = document.querySelector('header');

if (header) {
    window.addEventListener('scroll', function () {
        let scrollTop = window.scrollY || document.documentElement.scrollTop;

        if (scrollTop < 0) scrollTop = 0;

        if (scrollTop > lastScrollTop && scrollTop > 80) {
            header.classList.add('header-up');
        }
        else {
            header.classList.remove('header-up');
        }

        lastScrollTop = scrollTop;
    });
}