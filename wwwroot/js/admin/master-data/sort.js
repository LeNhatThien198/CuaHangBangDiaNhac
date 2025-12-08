function applySort(tableId, colIndex, type, order, element) {
    const table = document.getElementById(tableId);
    if (!table) return;

    // 1. CẬP NHẬT GIAO DIỆN NÚT SORT (UI UPDATE)
    if (element) {
        // Tìm nút dropdown cha gần nhất
        const dropdownContainer = element.closest('.dropdown');
        if (dropdownContainer) {
            const btnToggle = dropdownContainer.querySelector('.dropdown-toggle');

            // Lấy icon và text từ mục được chọn
            const iconHtml = element.querySelector('i').outerHTML;
            const text = element.innerText.trim();

            // Gán ngược lại vào nút chính (Giữ nguyên class, chỉ thay nội dung)
            btnToggle.innerHTML = `${iconHtml} ${text}`;

            // (Tùy chọn) Highlight mục đang chọn trong menu
            const allItems = dropdownContainer.querySelectorAll('.dropdown-item');
            allItems.forEach(item => item.classList.remove('active'));
            element.classList.add('active');
        }
    }

    // 2. LOGIC SẮP XẾP (GIỮ NGUYÊN)
    const tbody = table.querySelector('tbody');
    const rows = Array.from(tbody.querySelectorAll('tr'));

    rows.sort((a, b) => {
        let aVal = a.children[colIndex].innerText.trim();
        let bVal = b.children[colIndex].innerText.trim();

        if (type === 'num') {
            aVal = parseFloat(aVal.replace(/[^0-9.-]+/g, "")) || 0;
            bVal = parseFloat(bVal.replace(/[^0-9.-]+/g, "")) || 0;
            return order === 'asc' ? aVal - bVal : bVal - aVal;
        } else {
            return order === 'asc'
                ? aVal.localeCompare(bVal)
                : bVal.localeCompare(aVal);
        }
    });

    tbody.innerHTML = '';
    rows.forEach(row => tbody.appendChild(row));
}