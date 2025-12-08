function openEditModal(button) {
    var url = button.getAttribute('data-url');
    var modalContainer = document.getElementById('modalContainer');
    var modalElement = document.getElementById('editModal');

    var myModal = new bootstrap.Modal(modalElement);

    modalContainer.innerHTML = '<div class="text-center p-4"><div class="spinner-border text-primary" role="status"></div></div>';
    myModal.show();

    $.get(url, function (data) {
        modalContainer.innerHTML = data; 

        var form = $(modalContainer).find("form");
        if ($.validator && $.validator.unobtrusive) {
            $.validator.unobtrusive.parse(form);
        }
    }).fail(function () {
        modalContainer.innerHTML = '<div class="alert alert-danger m-3">Lỗi tải dữ liệu. Vui lòng thử lại.</div>';
    });
}