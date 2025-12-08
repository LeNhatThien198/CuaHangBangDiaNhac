function confirmDelete(button) {
    Swal.fire({
        title: "Bạn có chắc chắn?",
        text: "Hành động này không thể hoàn tác!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Xóa ngay",
        cancelButtonText: "Hủy bỏ"
    }).then((result) => {
        if (result.isConfirmed) {
            button.closest('form').submit();
        }
    });
}