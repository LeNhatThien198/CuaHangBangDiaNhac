function searchTable(inputId, tableId) {
    var input = document.getElementById(inputId);
    var filter = input.value.toUpperCase();
    var table = document.getElementById(tableId);
    var tr = table.getElementsByTagName("tr");

    // Duyệt qua các dòng (bỏ qua header)
    for (var i = 1; i < tr.length; i++) {
        // Lấy toàn bộ nội dung text của dòng (bao gồm cả các cột ẩn)
        var rowContent = tr[i].textContent || tr[i].innerText;

        if (rowContent) {
            if (rowContent.toUpperCase().indexOf(filter) > -1) {
                tr[i].style.display = "";
            } else {
                tr[i].style.display = "none";
            }
        }
    }
}