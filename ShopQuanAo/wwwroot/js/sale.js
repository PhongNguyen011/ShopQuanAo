// wwwroot/js/sale.js
(function ($) {
    // ----- Khởi tạo Isotope -----
    var $grid = $('.product-grid').isotope({
        itemSelector: '.product-item',
        layoutMode: 'fitRows',
        getSortData: {
            original: function (el) {
                return parseInt($(el).attr('data-original-order')) || 0;
            },
            price: function (el) {
                return parseFloat($(el).attr('data-price')) || 0;
            },
            name: function (el) {
                return ($(el).attr('data-name') || '').toLowerCase();
            }
        },
        sortBy: 'original',
        sortAscending: true
    });

    // ----- State -----
    var pageSize = parseInt(localStorage.getItem('promo_pageSize') || '10', 10); // mặc định khớp option 10 của bạn
    var currentPage = 1;

    var $quickSize = $('#quickSize');
    var $pageNums = $('#pageNums');
    var $totalCountEl = $('#totalCount');

    // Đồng bộ giá trị ban đầu cho dropdown
    (function syncQuickSize() {
        var val = String(pageSize);
        if (['1', '5', '10', '15'].includes(val)) $quickSize.val(val);
        else $quickSize.val(''); // nếu không thuộc preset
    })();

    // ----- Helpers -----
    function clamp(n, min, max) { return Math.max(min, Math.min(max, n)); }
    function getItemsInCurrentOrder() { return $grid.isotope('getFilteredItemElements'); }

    function renderPageControls(totalPages) {
        $pageNums.empty();
        var windowSize = 7;
        var start = Math.max(1, currentPage - Math.floor(windowSize / 2));
        var end = Math.min(totalPages, start + windowSize - 1);
        start = Math.max(1, end - windowSize + 1);

        if (start > 1) {
            $pageNums.append('<a class="btn btn-outline-secondary btn-sm" href="javascript:void(0)" data-page="1">1</a>');
            if (start > 2) $pageNums.append('<span class="btn btn-sm disabled">…</span>');
        }

        for (var i = start; i <= end; i++) {
            var cls = (i === currentPage) ? 'btn-dark' : 'btn-outline-secondary';
            $pageNums.append('<a class="btn ' + cls + ' btn-sm" href="javascript:void(0)" data-page="' + i + '">' + i + '</a>');
        }

        if (end < totalPages) {
            if (end < totalPages - 1) $pageNums.append('<span class="btn btn-sm disabled">…</span>');
            $pageNums.append('<a class="btn btn-outline-secondary btn-sm" href="javascript:void(0)" data-page="' + totalPages + '">' + totalPages + '</a>');
        }
    }

    function applyPaging() {
        var items = getItemsInCurrentOrder();
        var total = items.length;
        $totalCountEl.text(total);

        pageSize = parseInt(pageSize, 10);
        if (isNaN(pageSize) || pageSize < 1) pageSize = 1;
        if (pageSize > 60) pageSize = 60;

        var totalPages = Math.max(1, Math.ceil(total / pageSize));
        currentPage = clamp(currentPage, 1, totalPages);

        var startIdx = (currentPage - 1) * pageSize;
        var endIdx = Math.min(currentPage * pageSize, total) - 1;

        items.forEach(function (el, idx) {
            if (idx >= startIdx && idx <= endIdx) el.classList.remove('paged-out');
            else el.classList.add('paged-out');
        });

        renderPageControls(totalPages);

        $('#btnFirst, #btnPrev').prop('disabled', currentPage <= 1);
        $('#btnNext, #btnLast').prop('disabled', currentPage >= totalPages);

        $grid.isotope('layout');
    }

    // ----- Sự kiện -----
    // Sắp xếp
    $('.type_sorting_btn').on('click', function () {
        var sortByValue = $(this).data('sortby');
        var sortAsc = $(this).data('sortasc') === true || $(this).data('sortasc') === "true";
        var sortText = $(this).data('sorttext');

        $('#currentSortText').text(sortText);
        $('.type_sorting_btn').removeClass('active-sort');
        $(this).addClass('active-sort');

        $grid.isotope({ sortBy: sortByValue, sortAscending: sortAsc });
    });

    // Re-apply khi Isotope arrange xong
    $grid.on('arrangeComplete', applyPaging);

    // Nút chuyển trang
    $('#btnFirst').on('click', function () { currentPage = 1; applyPaging(); });
    $('#btnPrev').on('click', function () { currentPage = Math.max(1, currentPage - 1); applyPaging(); });
    $('#btnNext').on('click', function () { currentPage = currentPage + 1; applyPaging(); });
    $('#btnLast').on('click', function () {
        var total = getItemsInCurrentOrder().length;
        currentPage = Math.max(1, Math.ceil(total / pageSize));
        applyPaging();
    });

    // Chọn số SP/trang
    $quickSize.on('change', function () {
        var val = parseInt($(this).val() || '0', 10);
        if (val > 0) {
            pageSize = val;
            localStorage.setItem('promo_pageSize', String(pageSize));
            currentPage = 1;
            applyPaging();
        }
    });

    // Click số trang
    $pageNums.on('click', 'a[data-page]', function () {
        currentPage = parseInt($(this).data('page'), 10) || 1;
        applyPaging();
    });

    // ----- Khởi tạo -----
    $(document).ready(function () {
        applyPaging(); // chạy lần đầu
    });

})(jQuery);
