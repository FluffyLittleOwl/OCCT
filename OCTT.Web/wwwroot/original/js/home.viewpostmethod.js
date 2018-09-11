$(document).ready(function () {
    /// same reasoning as in site.js
    if (window.location.search == "") {
        localStorage.removeItem('userInput');
    }

    /// base div
    var $pageView = $('div.page-rendered-view')

    /// to keep user position in between we will use localStorage
    /// obviously it'll break if you abuse this one
    var safe = {
        set: function (key, obj) {
            var serialObj = JSON.stringify(obj);
            localStorage.setItem(key, serialObj);
        },
        get: function (key) {
            var obj = JSON.parse(localStorage.getItem(key));
            return obj !== null ? obj : undefined;
        }
    }

    /// setup sort order
    var $sortOrderRadio = $('div.dataTable-sort-order', $pageView);
    $sortOrderRadio.one('configue-sort-order', function (e, data) {
        var $this = $(this);
        $this.setValue = function (value) {
            var $radioButton = $('input[value=' + value + ']', $sortOrderRadio);
            $radioButton.prop('checked', true);
            $this.data('chosenSortOrder', value);
        }
        if (data !== undefined && data.column !== undefined) {
            $this.setValue(data.column);
        } else {
            $this.setValue('PubDate');
        }
        $('input[name="sortOrder"]', this).on('click', function (e, data) {
            $this.data('chosenSortOrder', $(e.currentTarget).val());
            $this.trigger('collectUserInput');
        });
    });

    /// primitive pager, should be good as long nobody overexert it
    var $pageChoice = $('select#page-choice', $pageView);
    $pageChoice.one('configue-page-choice', function (e, data) {
        var $this = $(this);
        $this.setValue = function (value) {
            $this.val(value);
            $this.data('chosenPage', value);
        }
        if (data !== undefined && data.page !== undefined && $('option[value=' + data.page + ']', $this).length != 0) {
            $this.setValue(data.page);
        } else {
            $this.setValue(1);   /// по умолчанию
        }
        $this.on('change', function (e, data) {
            var $this = $(this);
            $this.data('chosenPage', $this.val());
            $this.trigger('collectUserInput');
        });
    });

    /// setup sort criteria (which feed are we using in this case)
    var $feedSource = $('select#feed-source', $pageView);
    $feedSource.one('configue-feed-sorting', function (e, data) {
        var $this = $(this);
        $this.setValue = function (value) {
            $this.val(value);
            $this.data('chosenFeed', value);
        }
        if (data !== undefined && data.feed !== undefined && $('option[value=' + data.feed + ']', $this).length != 0) {
            /// actually, here should be a check if this option is present too;
            /// shouldn't cause trouble if db is static but if you go full crud...
            $this.setValue(data.feed);
        } else {
            $this.setValue(0);   /// по умолчанию
        }
        $this.on('change', function (e, data) {
            var $this = $(this);
            $this.data('chosenFeed', $this.val());
            $this.trigger('collectUserInput');
        });
    });

    /// still useful
    var addQueryParamsToUrl = function (url, data) {
        var paramArr = new Array();
        for (var key in data) {
            paramArr.push(key + '=' + data[key]);
        }
        var paramString = paramArr.join('&');
        return [url, '?', paramString].join('');
    };

    /// what event says, at this point comments are just some sugar for the eyes
    $pageView.one('prepareUserControls', function (e, data) {
        var userInput = safe.get('userInput');
        $sortOrderRadio.trigger('configue-sort-order', userInput); /// { column: }
        $feedSource.trigger('configue-feed-sorting', userInput); /// { feed: }
        $pageChoice.trigger('configue-page-choice', userInput); /// { page: }
    }).trigger('prepareUserControls');

    $pageView.on('collectUserInput', function (e, data) {
        e.stopPropagation();
        var $this = $(this);
        var userInput = {
            column: $sortOrderRadio.data('chosenSortOrder'),
            feed: $feedSource.data('chosenFeed'),
            page: $pageChoice.data('chosenPage')
        };
        /// $this.data('userInput', userInput); /// would have fired only once anyway
        safe.set('userInput', userInput);
        /// FLY AWAY TO ANOTHER PAGE
        var href = window.location.href.replace(window.location.search, "");
        window.location.href = addQueryParamsToUrl(href, userInput);
    });

    /// подготовка к инициализации таблицы
    var $rssData = $('#rssData', $pageView);
    $rssData.one('configue-rss-table', function (e, data) {
        /// конфигурация таблицы, за исключением ajax 
        var rssTableOptions = {
            columns: [
                { title: "Id" },
                { title: "Источник" },
                { title: "Название новости" },
                { title: "Описание новости" },
                { title: "Дата публикации" }
            ],
            columnDefs: [
                { "targets": [0], "visible": false }, /// прячем Id
                { "targets": [1], "width": "115px" },
                { "targets": [4], "width": "160px" }
            ],
            "paging": false,
            "ordering": false,
            "searching": false,
            fnServerParams: function (aoData) { /// отключаем сортировку и поиск через таблицу
                delete aoData.columns;
                delete aoData.order;
                delete aoData.search;
            },
            processing: true,
            info: false
        };
        /// initialize table 
        $rssData.DataTable(rssTableOptions);
        /// and make rows kind-of-selectable
        $('tbody', $rssData).on('click', 'tr', function () {
            var $this = $(this);
            if (!$this.hasClass('selected')) {
                $rssData.find('tr.selected').removeClass('selected');
                $this.addClass('selected');
            }
        });

        /// dataTables_wrapper is not using 100% for whatever reason
        $rssData.closest('div.dataTables_wrapper').css('width', '100%');

    });
    $rssData.trigger('configue-rss-table');
});