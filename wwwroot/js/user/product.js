document.addEventListener("DOMContentLoaded", function () {
    // 1. View Mode Toggling (Grid vs List)
    const gridBtn = document.getElementById("btnGridView");
    const listBtn = document.getElementById("btnListView");
    const productContainer = document.getElementById("productGridContainer");

    if (gridBtn && listBtn && productContainer) {
        gridBtn.addEventListener("click", () => {
            productContainer.classList.remove("product-list-view");
            productContainer.classList.add("row-cols-2", "row-cols-md-3", "row-cols-lg-4"); // Restore Grid Classes
            gridBtn.classList.add("active");
            listBtn.classList.remove("active");
        });

        listBtn.addEventListener("click", () => {
            productContainer.classList.add("product-list-view");
            productContainer.classList.remove("row-cols-2", "row-cols-md-3", "row-cols-lg-4"); // Remove Grid sizing
            listBtn.classList.add("active");
            gridBtn.classList.remove("active");
        });
    }

    // 2. Image Gallery & Lightbox Logic
    const mainImg = document.getElementById("mainDetailImg");
    const mainImgContainer = document.querySelector(".main-image-container");
    const pageThumbs = document.querySelectorAll(".thumb-img");

    // Overlay Elements
    const overlay = document.getElementById("imgViewerOverlay");
    const viewerImg = document.getElementById("viewerImage");
    const viewerContent = document.querySelector(".viewer-content-wrapper");
    const closeBtn = document.getElementById("viewerCloseBtn");
    const viewerThumbs = document.querySelectorAll(".viewer-thumb");

    // Helper: Update All Images + Active States
    const updateAllImages = (src) => {
        if (!src) return;

        if (mainImg) mainImg.setAttribute("src", src);
        if (viewerImg) viewerImg.setAttribute("src", src);

        // Sync Active Classes (Both Page Thumbs and Viewer Thumbs)
        const allThumbs = [...pageThumbs, ...viewerThumbs];
        allThumbs.forEach(t => {
            if (t.getAttribute("src") === src) {
                t.classList.add("active");
            } else {
                t.classList.remove("active");
            }
        });
    };

    // A. Page Thumbnails Click
    if (pageThumbs.length > 0) {
        pageThumbs.forEach(thumb => {
            thumb.addEventListener("click", function () {
                updateAllImages(this.getAttribute("src"));
            });
        });
    }

    // B. Viewer Thumbnails Click
    if (viewerThumbs.length > 0) {
        viewerThumbs.forEach(thumb => {
            thumb.addEventListener("click", function (e) {
                e.stopPropagation(); // Prevent bubbling to overlay click
                updateAllImages(this.getAttribute("src"));
            });
        });
    }

    // C. Open Lightbox (Click Main Image Container)
    if (mainImgContainer && overlay && viewerImg && mainImg) {
        mainImgContainer.addEventListener("click", (e) => {
            // Ensure viewer has current main image
            const currentSrc = mainImg.getAttribute("src");
            viewerImg.setAttribute("src", currentSrc);

            // Show Overlay
            overlay.style.display = "flex";
            document.body.style.overflow = "hidden"; // Prevent scrolling
        });

        // Toggle zoom on viewer image
        viewerImg.addEventListener("click", (e) => {
            e.stopPropagation();
            viewerImg.classList.toggle("viewer-zoomed");
            viewerContent?.classList.toggle("viewer-zooming");
        });

        // Close Logic
        const closeViewer = () => {
            overlay.style.display = "none";
            document.body.style.overflow = ""; // Restore scrolling
        };

        if (closeBtn) closeBtn.addEventListener("click", closeViewer);

        overlay.addEventListener("click", (e) => {
            // Close if clicking background (not image or thumbs)
            if (e.target === overlay || e.target.classList.contains("viewer-content-wrapper")) {
                closeViewer();
            }
        });

        // ESC key
        document.addEventListener("keydown", (e) => {
            if (e.key === "Escape" && overlay.style.display === "flex") {
                closeViewer();
            }
        });
    }

    // Qty Stepper Logic
    const qtyInput = document.getElementById('qtyInput');
    const btnMinus = document.getElementById('btnMinus');
    const btnPlus = document.getElementById('btnPlus');

    if (qtyInput && btnMinus && btnPlus) {
        btnMinus.addEventListener('click', () => {
            let val = parseInt(qtyInput.value) || 1;
            if (val > 1) {
                qtyInput.value = val - 1;
            }
        });

        btnPlus.addEventListener('click', () => {
            let val = parseInt(qtyInput.value) || 1;
            let max = parseInt(qtyInput.getAttribute('max')) || 99;
            if (val < max) {
                qtyInput.value = val + 1;
            } else {
                Swal.fire({
                    icon: 'warning',
                    title: 'Giới hạn số lượng',
                    text: `Xin lỗi, chỉ còn ${max} sản phẩm trong kho!`,
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000
                });
            }
        });

        // Manual Input Validation
        qtyInput.addEventListener('change', () => {
            let val = parseInt(qtyInput.value) || 1;
            let max = parseInt(qtyInput.getAttribute('max')) || 99;

            if (val < 1) {
                qtyInput.value = 1;
            } else if (val > max) {
                qtyInput.value = max;
                Swal.fire({
                    icon: 'warning',
                    title: 'Giới hạn số lượng',
                    text: `Xin lỗi, chỉ còn ${max} sản phẩm trong kho!`,
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000
                });
            }
        });
    }
    // D. Keyboard Navigation (Arrow Keys)
    document.addEventListener("keydown", (e) => {
        // Only trigger if we have thumbnails to cycle through
        if (viewerThumbs.length <= 1) return;

        if (e.key === "ArrowLeft" || e.key === "ArrowRight") {
            // Find currently active index
            // We use viewerThumbs as the source of truth for the list order
            let currentIndex = -1;
            viewerThumbs.forEach((thumb, index) => {
                if (thumb.classList.contains("active")) {
                    currentIndex = index;
                }
            });

            // If no active class found (fallback), try matching src of mainImg
            if (currentIndex === -1 && mainImg) {
                const currentSrc = mainImg.getAttribute("src");
                viewerThumbs.forEach((thumb, index) => {
                    if (thumb.getAttribute("src") === currentSrc) {
                        currentIndex = index;
                    }
                });
            }

            // Default to 0 if still not found
            if (currentIndex === -1) currentIndex = 0;

            // Calculate new index
            let newIndex;
            if (e.key === "ArrowLeft") {
                newIndex = (currentIndex - 1 + viewerThumbs.length) % viewerThumbs.length;
            } else {
                newIndex = (currentIndex + 1) % viewerThumbs.length;
            }

            // Update
            const newSrc = viewerThumbs[newIndex].getAttribute("src");
            updateAllImages(newSrc);
        }
    });
});
