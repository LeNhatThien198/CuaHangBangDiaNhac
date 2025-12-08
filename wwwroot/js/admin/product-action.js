// ==========================================
// 1. XEM TRƯỚC ẢNH
// ==========================================
function previewCover(input) {
    const preview = document.getElementById('coverPreview');
    const placeholder = document.getElementById('coverPlaceholder');
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function(e) {
            preview.src = e.target.result;
            preview.style.display = 'block';
            if (placeholder) placeholder.style.display = 'none';
        }
        reader.readAsDataURL(input.files[0]);
    } else {
        preview.src = '#';
        preview.style.display = 'none';
        if (placeholder) placeholder.style.display = 'flex';
    }
}

function previewGallery(input) {
    const container = document.getElementById('galleryPreviewContainer');
    container.innerHTML = '';
    if (input.files) {
        Array.from(input.files).forEach(file => {
            const reader = new FileReader();
            reader.onload = function(e) {
                const div = document.createElement('div');
                div.className = 'gallery-item';
                const img = document.createElement('img');
                img.src = e.target.result;
                div.appendChild(img);
                container.appendChild(div);
            }
            reader.readAsDataURL(file);
        });
    }
}

// ==========================================
// 2. LOGIC KHỞI TẠO & XỬ LÝ SỰ KIỆN
// ==========================================
document.addEventListener('DOMContentLoaded', function() {

    // A. XỬ LÝ PRE-ORDER
    const preOrderCheck = document.getElementById('preOrderCheck');
    const preOrderBox = document.getElementById('preOrderDateBox');
    if (preOrderCheck && preOrderBox) {
        preOrderCheck.addEventListener('change', function() {
            if (this.checked) new bootstrap.Collapse(preOrderBox, { toggle: false }).show();
            else new bootstrap.Collapse(preOrderBox, { toggle: false }).hide();
        });
    }

    // B. XỬ LÝ CONDITION (HÀNG CŨ / MỚI)
    const isUsedCheck = document.getElementById('IsUsed'); // Checkbox Hàng cũ
    const conditionSelect = document.getElementById('Condition'); // Dropdown Tình trạng

    if (isUsedCheck && conditionSelect) {
        
        // Hàm xử lý logic ẩn hiện option M (Mint)
        function updateConditionState() {
            const isUsed = isUsedCheck.checked;
            
            // Tìm option "M (Mint)"
            let mintOption = null;
            for (let i = 0; i < conditionSelect.options.length; i++) {
                if (conditionSelect.options[i].text.startsWith("M (Mint)")) {
                    mintOption = conditionSelect.options[i];
                    break;
                }
            }

            if (!isUsed) {
                // HÀNG MỚI -> Set cứng M (Mint) và Disable
                conditionSelect.value = "M (Mint)";
                conditionSelect.classList.add("bg-light"); // Style xám
                conditionSelect.style.pointerEvents = "none"; // Chặn click
                conditionSelect.setAttribute("tabindex", "-1"); // Chặn tab
            } else {
                // HÀNG CŨ -> Enable
                conditionSelect.classList.remove("bg-light");
                conditionSelect.style.pointerEvents = "auto";
                conditionSelect.removeAttribute("tabindex");

                // Ẩn option Mint đi (vì hàng cũ không được chọn Mint)
                if (mintOption) {
                    mintOption.hidden = true; 
                    // Nếu đang chọn Mint mà chuyển sang cũ -> Reset về rỗng hoặc NM
                    if (conditionSelect.value === "M (Mint)") {
                        conditionSelect.value = ""; 
                    }
                }
            }
        }

        // Chạy ngay khi load trang (để set trạng thái Edit/Create ban đầu)
        updateConditionState();

        // Chạy khi bấm thay đổi
        isUsedCheck.addEventListener('change', updateConditionState);
    }

    // C. LOAD STYLE THEO GENRE
    const genreSelect = document.getElementById('GenreSelect');
    const styleSelect = document.getElementById('StyleSelect');
    if (genreSelect && styleSelect) {
        genreSelect.addEventListener('change', function() {
            loadStyles(this.value);
        });
    }

    window.loadStyles = function(genreId, selectedStyleId = null) {
        styleSelect.innerHTML = '<option value="">Đang tải...</option>';
        styleSelect.disabled = true;

        if (genreId) {
            fetch(`/Admin/Genre/GetStylesByGenreJson?genreId=${genreId}`)
                .then(response => response.json())
                .then(data => {
                    if (data.length > 0) {
                        styleSelect.innerHTML = '<option value="">-- Chọn Phong cách --</option>';
                        data.forEach(item => {
                            const isSelected = (selectedStyleId && item.id == selectedStyleId) ? 'selected' : '';
                            styleSelect.innerHTML += `<option value="${item.id}" ${isSelected}>${item.name}</option>`;
                        });
                        styleSelect.disabled = false;
                    } else {
                        styleSelect.innerHTML = '<option value="">(Không có phong cách con)</option>';
                        styleSelect.disabled = true;
                    }
                })
                .catch(err => {
                    console.error(err);
                    styleSelect.innerHTML = '<option value="">Lỗi tải dữ liệu</option>';
                });
        } else {
            styleSelect.innerHTML = '<option value="">-- Chọn Thể loại trước --</option>';
            styleSelect.disabled = true;
        }
    }
});

// ==========================================
// 3. THÊM NHANH DANH MỤC (QUICK ADD)
// ==========================================
function quickAdd(type) {
    let labelName = type;
    let apiUrl = `/Admin/MasterData/QuickCreate${type}`;

    if (type === 'Artist') labelName = 'Nghệ sĩ';
    else if (type === 'Brand') labelName = 'Hãng phát hành';
    else if (type === 'Category') labelName = 'Định dạng';
    else if (type === 'Genre') {
        labelName = 'Thể loại';
        apiUrl = `/Admin/Genre/QuickCreateGenre`;
    }

    Swal.fire({
        title: `Thêm ${labelName} mới`,
        input: 'text',
        inputPlaceholder: `Nhập tên ${labelName}...`,
        showCancelButton: true,
        confirmButtonText: 'Lưu ngay',
        cancelButtonText: 'Hủy',
        confirmButtonColor: '#198754',
        showLoaderOnConfirm: true,
        preConfirm: (name) => {
            if (!name) {
                Swal.showValidationMessage('Vui lòng nhập tên!');
                return false;
            }
            return fetch(`${apiUrl}?name=${encodeURIComponent(name)}`, { method: 'POST' })
                .then(response => {
                    if (!response.ok) throw new Error(response.statusText);
                    return response.json();
                })
                .then(data => {
                    if (!data.success) throw new Error(data.message);
                    return data;
                })
                .catch(error => Swal.showValidationMessage(`Thất bại: ${error}`));
        }
    }).then((result) => {
        if (result.isConfirmed) {
            const data = result.value;
            const select = document.querySelector(`select[name="${type}Id"]`);
            if (select) {
                const option = new Option(data.name, data.id, true, true);
                select.add(option, undefined);
                if(type === 'Genre') {
                    const styleSelect = document.getElementById('StyleSelect');
                    if(styleSelect) {
                        styleSelect.innerHTML = '<option value="">(Chưa có phong cách nào)</option>';
                        styleSelect.disabled = true;
                    }
                }
                Swal.fire('Thành công!', `Đã thêm: ${data.name}`, 'success');
            }
        }
    });
}

function quickAddStyle() {
    const genreSelect = document.getElementById('GenreSelect');
    const genreId = genreSelect.value;
    const genreName = genreSelect.options[genreSelect.selectedIndex]?.text;

    if (!genreId) {
        Swal.fire('Chú ý', 'Vui lòng chọn <b>Thể loại chính</b> trước khi thêm phong cách!', 'warning');
        return;
    }

    Swal.fire({
        title: 'Thêm Phong cách mới',
        html: `Thêm vào thể loại: <b>${genreName}</b>`,
        input: 'text',
        inputPlaceholder: 'Nhập tên phong cách...',
        showCancelButton: true,
        confirmButtonText: 'Lưu ngay',
        showLoaderOnConfirm: true,
        preConfirm: (name) => {
            if (!name) return Swal.showValidationMessage('Nhập tên đi bạn ơi!');
            
            return fetch(`/Admin/Genre/QuickCreateStyle?name=${encodeURIComponent(name)}&genreId=${genreId}`, { method: 'POST' })
                .then(res => res.json())
                .then(data => {
                    if (!data.success) throw new Error(data.message);
                    return data;
                })
                .catch(err => Swal.showValidationMessage(err));
        }
    }).then((result) => {
        if (result.isConfirmed) {
            const data = result.value;
            window.loadStyles(genreId, data.id);
            Swal.fire('Xong!', `Đã thêm phong cách: ${data.name}`, 'success');
        }
    });
}

function deleteImage(imgId) {
    Swal.fire({
        title: 'Xóa ảnh này?',
        text: "Ảnh sẽ bị xóa vĩnh viễn khỏi hệ thống ngay lập tức!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Xóa ngay',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {

            // Gọi API Xóa
            fetch('/Admin/Product/DeleteImage', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                body: `imageId=${imgId}`
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        // Hiệu ứng xóa UI mượt mà
                        const imgElement = document.getElementById('img-' + imgId);
                        if (imgElement) {
                            imgElement.style.transition = 'all 0.3s ease';
                            imgElement.style.transform = 'scale(0)'; // Thu nhỏ
                            setTimeout(() => imgElement.remove(), 300); // Rồi mới xóa
                        }

                        // Toast thông báo
                        const Toast = Swal.mixin({
                            toast: true,
                            position: 'top-end',
                            showConfirmButton: false,
                            timer: 3000,
                            timerProgressBar: true
                        });
                        Toast.fire({ icon: 'success', title: 'Đã xóa ảnh' });

                    } else {
                        Swal.fire('Lỗi!', 'Không thể xóa ảnh. Vui lòng thử lại.', 'error');
                    }
                })
                .catch(err => {
                    console.error(err);
                    Swal.fire('Lỗi mạng!', 'Không kết nối được server.', 'error');
                });
        }
    });
}