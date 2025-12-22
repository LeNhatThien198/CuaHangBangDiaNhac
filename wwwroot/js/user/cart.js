$(document).ready(function () {
    updateCartBadge();

    // --- FIX: Restore Button State on Back Button (BFCache) ---
    window.addEventListener('pageshow', function (event) {
        // If loaded from bfcache or just standard navigation
        // Reset any buttons that were left in loading state
        $('.btn-loading').each(function () {
            const btn = $(this);
            const originalHtml = btn.data('original-html');
            if (originalHtml) {
                btn.html(originalHtml);
            }
            btn.prop('disabled', false);
            btn.removeClass('btn-loading');
        });

        // FIX: Recalculate totals if inputs are restored by browser
        setTimeout(function () { // Small delay to ensure DOM update
            updateGrandTotal();
        }, 100);
    });

    // --- UTILS ---
    function formatVnd(amount) {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
    }

    // --- CART SELECTION LOGIC ---
    function updateGrandTotal() {
        let total = 0;
        let selectedItemsCount = 0; // Unique products
        let totalQty = 0; // Total quantity of selected products

        $('.item-checkbox:checked').each(function () {
            const row = $(this).closest('tr');
            const rawTotal = parseFloat(row.find('.row-total').attr('data-raw')) || 0;
            const qty = parseInt(row.find('.quantity-input').val()) || parseInt($(this).attr('data-qty')) || 1;

            total += rawTotal;
            selectedItemsCount++;
            totalQty += qty;
        });

        const formatted = formatVnd(total);

        // Update Sticky Footer
        $('#sticky-total').text(formatted);
        $('#selected-count').text(selectedItemsCount);
        $('#total-qty').text(totalQty);

        // Update Button State
        if (selectedItemsCount === 0) {
            $('.btn-checkout').addClass('disabled');
        } else {
            $('.btn-checkout').removeClass('disabled');
        }
    }

    // 1. Check All Top
    $('#check-all').change(function () {
        const isChecked = $(this).is(':checked');
        toggleAll(isChecked);
    });

    // 2. Check All Bottom
    $('#check-all-bottom').change(function () {
        const isChecked = $(this).is(':checked');
        toggleAll(isChecked);
    });

    function toggleAll(isChecked) {
        // Sync triggers
        $('#check-all').prop('checked', isChecked);
        $('#check-all-bottom').prop('checked', isChecked);

        // Toggle Items and Artists
        $('.item-checkbox:not(:disabled), .artist-checkbox').prop('checked', isChecked);
        updateGrandTotal();
    }

    // 3. Artist Checkbox
    $('.artist-checkbox').change(function () {
        const isChecked = $(this).is(':checked');
        const artistId = $(this).data('artist');
        $(`.item-checkbox[data-artist="${artistId}"]:not(:disabled)`).prop('checked', isChecked);
        updateGrandTotal();
    });

    // 4. Single Item Checkbox
    $(document).on('change', '.item-checkbox', function () {
        updateGrandTotal();

        // Uncheck Masters if one unchecked
        if (!$(this).is(':checked')) {
            $('#check-all').prop('checked', false);
            $('#check-all-bottom').prop('checked', false);

            const artistId = $(this).data('artist');
            $(`.artist-checkbox[data-artist="${artistId}"]`).prop('checked', false);
        }
    });

    // BUY NOW / PRE-ORDER NOW BUTTON CLICK (Add & Redirect)
    $(document).on('click', '.btn-buy-now', function (e) {
        e.preventDefault();
        const btn = $(this);
        const productId = btn.data('id');
        let quantity = 1;

        // Check for quantity input (only on Details page)
        const qtyInput = document.getElementById('qtyInput');
        if (qtyInput) {
            quantity = parseInt(qtyInput.value) || 1;
        }

        // Store original state
        btn.data('original-html', btn.html());
        btn.addClass('btn-loading');

        // Add animation
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Loading...');
        btn.prop('disabled', true);

        $.ajax({
            url: '/Cart/AddToCart',
            type: 'POST',
            data: { productId: productId, quantity: quantity },
            success: function (response) {
                if (response.success) {
                    // Update Badge (Optional but good)
                    updateCartBadge(response.cartCount);
                    // Redirect to Cart
                    window.location.href = '/Cart';
                } else {
                    Swal.fire({
                        title: 'Lỗi',
                        text: response.message,
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                    // Reset button
                    btn.html(btn.data('original-html'));
                    btn.prop('disabled', false);
                    btn.removeClass('btn-loading');
                }
            },
            error: function () {
                Swal.fire({
                    title: 'Lỗi',
                    text: 'Đã có lỗi xảy ra. Vui lòng thử lại.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
                // Reset button
                btn.html(btn.data('original-html'));
                btn.prop('disabled', false);
                btn.removeClass('btn-loading');
            }
        });
    });

    // --- ADD TO CART (Global) ---
    $(document).on('click', '.btn-add-to-cart', function (e) {
        e.preventDefault();
        const btn = $(this);
        const productId = btn.data('id');
        let quantity = 1;

        const qtyInput = btn.closest('.d-flex').find('input[type="number"]');
        if (qtyInput.length > 0) {
            quantity = qtyInput.val();
        }

        addToCart(productId, quantity, btn);
    });

    // --- QTY STEPPERS (Cart Page) ---
    $(document).on('click', '.cart-qty-minus', function () {
        const input = $(this).siblings('.quantity-input');
        let val = parseInt(input.val()) || 1;
        if (val > 1) {
            input.val(val - 1).trigger('change');
        }
    });

    $(document).on('click', '.cart-qty-plus', function () {
        const input = $(this).siblings('.quantity-input');
        let val = parseInt(input.val()) || 1;
        let max = parseInt(input.attr('max')) || 99;
        if (val < max) {
            input.val(val + 1).trigger('change');
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

    // --- QTY CHANGE (AJAX) ---
    $(document).on('change', '.quantity-input', function () {
        const input = $(this);
        const productId = input.data('id');
        let quantity = parseInt(input.val()) || 1;
        let max = parseInt(input.attr('max')) || 99;

        if (quantity < 1) {
            quantity = 1;
            input.val(1);
        }
        if (quantity > max) {
            quantity = max;
            input.val(max);
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

        $.post('/Cart/UpdateQuantity', { productId: productId, quantity: quantity }, function (response) {
            if (response.success) {
                if (response.shouldReload) {
                    location.reload();
                    return;
                }

                // 1. Update Badge
                if (response.cartCount !== undefined) {
                    $('.badge-cart').text(response.cartCount);
                }

                // 2. Update Row Data
                const checkbox = $(`.item-checkbox[data-id="${productId}"]`);
                const rowTotalEl = $(`#total-${productId}`);

                // Update Checkbox Data attributes for calculation
                checkbox.attr('data-qty', quantity);

                // If the controller returns itemTotal, update it
                if (response.itemTotal !== undefined) {
                    checkbox.attr('data-raw', response.itemTotal);
                    rowTotalEl.attr('data-raw', response.itemTotal);
                    rowTotalEl.text(response.formattedItemTotal);
                }

                // 3. Recalculate Sidebar
                updateGrandTotal();

                // Optional: Show small success feedback (toast)
                // Swal.fire({...}) // Maybe too noisy for just qty change
            } else {
                // If failed (e.g. removed or error), reload to sync
                location.reload();
            }
        });
    });

    // --- BATCH DELETE ---
    $('#btn-delete-selected').click(function () {
        const selectedIds = [];
        $('.item-checkbox:checked').each(function () {
            selectedIds.push($(this).data('id'));
        });

        if (selectedIds.length === 0) {
            Swal.fire({
                icon: 'info',
                title: 'Chưa chọn sản phẩm',
                text: 'Vui lòng chọn ít nhất một sản phẩm để xóa.',
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 2000
            });
            return;
        }

        Swal.fire({
            title: 'Xác nhận xóa?',
            text: `Bạn có chắc muốn xóa ${selectedIds.length} sản phẩm đã chọn khỏi giỏ hàng?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#000',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Đồng ý xóa',
            cancelButtonText: 'Hủy bỏ'
        }).then((result) => {
            if (result.isConfirmed) {
                // Show loading
                $('#btn-delete-selected').prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span>');

                $.post('/Cart/RemoveMultipleItems', { productIds: selectedIds }, function (response) {
                    if (response.success) {
                        location.reload();
                    }
                });
            }
        });
    });

    // --- STICKY FOOTER COLLISION LOGIC ---
    // The bar is fixed to bottom(0). If we scroll into the Footer, we increase bottom() to push it up.
    const siteFooter = $('footer');
    const stickyBar = $('#sticky-cart-bar');

    function adjustStickyBar() {
        if (!siteFooter.length || !stickyBar.length) return;

        const footerTop = siteFooter.offset().top;
        const windowHeight = $(window).height();
        const scrollTop = $(window).scrollTop();
        const currentScrollBottom = scrollTop + windowHeight;

        const overlap = currentScrollBottom - footerTop;

        if (overlap > 0) {
            stickyBar.css('bottom', `${overlap}px`);
        } else {
            stickyBar.css('bottom', '0px');
        }
    }

    if (siteFooter.length && stickyBar.length) {
        $(window).on('scroll resize', adjustStickyBar);
        // Initial check
        adjustStickyBar();
        // Check again after all images load (layout shift)
        $(window).on('load', adjustStickyBar);
    }

    // --- CHECKOUT NAVIGATION ---
    $('#btn-process-checkout').click(function () {
        const selectedIds = [];
        $('.item-checkbox:checked').each(function () {
            selectedIds.push($(this).data('id'));
        });

        if (selectedIds.length === 0) {
            Swal.fire({
                icon: 'info',
                title: 'Chưa chọn sản phẩm',
                text: 'Vui lòng chọn ít nhất một sản phẩm để thanh toán.',
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 2000
            });
            return;
        }

        // Redirect with Query Parameters
        const queryString = selectedIds.map(id => `selectedIds=${id}`).join('&');
        window.location.href = `/Checkout?${queryString}`;
    });

    // Initial Calc
    updateGrandTotal();
});

function addToCart(productId, quantity, btnElement) {
    const originalText = btnElement.html();
    btnElement.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span>');

    $.post('/Cart/AddToCart', { productId: productId, quantity: quantity }, function (response) {
        if (response.success) {
            updateCartBadge();
            Swal.fire({
                icon: 'success',
                title: 'Đã thêm vào giỏ!',
                text: response.message,
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 3000
            });
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: 'Không thể thêm sản phẩm.',
            });
        }
    }).fail(function () {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi mạng',
            text: 'Vui lòng thử lại sau.',
        });
    }).always(function () {
        btnElement.prop('disabled', false).html(originalText);
    });
}
// check
function updateCartBadge() {
    $.get('/Cart/GetCartCount', function (data) {
        $('.badge-cart').text(data.count);
    });
}
