// wwwroot/js/cart.js
(function () {
    // Lấy AntiForgeryToken từ hidden form trong view
    const token = document.querySelector('#cart-anti input[name="__RequestVerificationToken"]')?.value;

    const fmtMoney = (n) => (n || 0).toLocaleString('vi-VN') + ' ₫';
    

    // Cập nhật số lượng và thành tiền cho 1 dòng
    function updateRowUI(id, qty, lineTotal, stock) {
        const qtyInput = document.getElementById('qty-' + id);
        const lineCell = document.getElementById('line-' + id);
        if (qtyInput) qtyInput.value = qty;
        if (lineCell) lineCell.textContent = fmtMoney(lineTotal);

        const row = document.getElementById('row-' + id);
        if (row && Number.isInteger(stock)) {
            row.dataset.stock = stock;
            const stockEl = document.getElementById('stock-' + id);
            if (stockEl) stockEl.textContent = stock;
        }

        // Disable nút nếu đạt min/max
        const minus = document.querySelector('.btn-minus[data-id="' + id + '"]');
        const plus = document.querySelector('.btn-plus[data-id="' + id + '"]');
        if (minus) minus.disabled = qty <= 1;
        if (plus) plus.disabled = Number.isInteger(stock) ? qty >= stock : false;
    }

    // Cập nhật tổng tạm tính
    function updateSubtotalUI(subtotal) {
        const subEl = document.getElementById('subtotal');
        if (subEl) subEl.textContent = fmtMoney(subtotal);
    }

    // Gọi server thay đổi quantity (delta = +1 / -1)
    async function changeQty(id, delta) {
        const res = await fetch('/Cart/ChangeQty', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8',
                'RequestVerificationToken': token
            },
            body: new URLSearchParams({ id: id, delta: delta })
        });

        if (!res.ok) return;

        const data = await res.json();

        // Nếu dòng bị xoá (hết hàng)
        if (data.removed) {
            const row = document.getElementById('row-' + id);
            if (row) row.remove();
            updateSubtotalUI(data.subtotal || 0);
            const rowsLeft = document.querySelectorAll('#cart-table tbody tr').length;
            if (rowsLeft === 0) location.reload();
            return;
        }

        if (data.ok) {
            updateRowUI(id, data.qty, data.lineTotal, data.stock);
            updateSubtotalUI(data.subtotal || 0);
        }
    }

    // Gán sự kiện click cho nút +/-
    document.addEventListener('click', function (e) {
        const minusBtn = e.target.closest('.btn-minus');
        const plusBtn = e.target.closest('.btn-plus');

        if (minusBtn) {
            const id = minusBtn.getAttribute('data-id');
            changeQty(id, -1);
        }
        if (plusBtn) {
            const id = plusBtn.getAttribute('data-id');
            changeQty(id, +1);
        }
    });

    // Thiết lập trạng thái ban đầu cho các nút
    document.querySelectorAll('tr[id^="row-"]').forEach(row => {
        const id = row.dataset.id;
        const stock = parseInt(row.dataset.stock || '0', 10);
        const qty = parseInt(document.getElementById('qty-' + id)?.value || '1', 10);
        const minus = document.querySelector('.btn-minus[data-id="' + id + '"]');
        const plus = document.querySelector('.btn-plus[data-id="' + id + '"]');
        if (minus) minus.disabled = qty <= 1;
        if (plus) plus.disabled = stock > 0 ? qty >= stock : false;
    });
})();
