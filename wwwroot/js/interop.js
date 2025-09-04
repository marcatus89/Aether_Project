// Bọc tất cả các hàm interop vào một đối tượng window
// để đảm bảo chúng có thể được truy cập toàn cục
window.siteInterop = {
    initializeScrollAnimation: function () {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    observer.unobserve(entry.target);
                }
            });
        }, {
            threshold: 0.1
        });

        const elements = document.querySelectorAll('.animate-on-scroll');
        elements.forEach(el => observer.observe(el));
    }
};

