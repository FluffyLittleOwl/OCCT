$(document).ready(function () {
    /// base div
    var $pageView = $('div.page-rendered-view')
    
    /// setup sort order
    var $sortOrderRadio = $('div.dataTable-sort-order', $pageView);
    $sortOrderRadio.one('configue-sort-order', function (e, data) {
        e.stopPropagation();
        $('input[name="sortOrder"]', this).on('click', function (e, data) {
            var $this = $(this);
            $this.closest('div.dataTable-sort-order').data('chosenSortOrder', $(e.currentTarget).val());
            $this.trigger('collectUserInput');
        });
        $('input[name="sortOrder"]', this).first().click();
    });

    /// setup sort criteria (which feed are we using in this case)
    var $feedSource = $('select#feed-source', $pageView);
    $feedSource.one('configue-feed-sorting', function (e, data) {
        e.stopPropagation();
        var $this = $(this);
        $this.on('change', function (e, data) {
            var $this = $(this);
            $this.data('chosenFeed', $this.val());
            $this.trigger('collectUserInput');
        });
        $this.trigger('change');
        /// get the rest of the options
        var rssFeedUrl = "/api/rssfeed/";
        $.ajax({
            method: "POST",
            url: rssFeedUrl,
            dataType: "json",
            success: function (result) {
                var data = result.data;     
                for (var key in data) {
                    var html = ["<option value='", key, "'>", data[key], "</option>"].join("");
                    $this.append(html);
                }
                $this.trigger('configuring-feed-sorting-complete');
            }
        });
    });

    /// подготовка к инициализации таблицы
    var $rssData = $('#rssData', $pageView);
    $rssData.one('configue-rss-table', function (e, data) {
        /// for .DataTable().ajax.url() part
        var addQueryParamsToUrl = function (url, data) {
            var paramArr = new Array();
            for (var key in data) {
                paramArr.push(key + '=' + data[key]);
            }
            var paramString = paramArr.join('&');
            return [url, '?', paramString].join('');
        };
        /// to keep things clear, if in future we do need to pass smth else
        var userInput = data; 
        /// конфигурация таблицы, за исключением ajax 
        var rssRecordUrl = "/api/rssrecord"; 
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
            "paging": true,
            "ordering": false,
            "searching": false,
            fnServerParams: function (aoData) { /// отключаем сортировку и поиск через таблицу
                delete aoData.columns;
                delete aoData.order;
                delete aoData.search;
            },
            processing: true,
            serverSide: true,
            ajax: {
                url: addQueryParamsToUrl(rssRecordUrl, userInput),
                type: "POST"
            }
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
        /// update url when change sort order of feed source
        $rssData.on('perform-rss-table-query-update', function (e, data) {
            var $this = $(this);
            var userInput = data;
            $this.DataTable().ajax.url(addQueryParamsToUrl(rssRecordUrl, userInput)).load(function () {
                $this.DataTable().columns.adjust();  // иначе Firefox & Vivaldi ведут себя странно
            });
        });
        /// to keep things a bit more uniform, hiding "records on page" selector
        $rssData.closest('div.dataTables_wrapper').find('div.dataTables_length').hide()
    });

    /// every time we touch the "control" area update the memorized input
    /// если было бы время, я наверное отвязался от излишней конкретики при определении userInput
    $pageView.on('collectUserInput', function (e, data) {
        e.stopPropagation();
        var $this = $(this);
        var userInput = {
            column: $sortOrderRadio.data('chosenSortOrder'),
            feed: $feedSource.data('chosenFeed')
        };
        $this.data('userInput', userInput);
        $rssData.trigger('perform-rss-table-query-update', userInput);
    });
    /// we have finished uploading options and can safely build userInput for the first time
    $pageView.one('configuring-feed-sorting-complete', function (e, data) {
        var $this = $(this);
        $this.trigger('collectUserInput');
        /// scince we don't have a table yet, first time we do a small walkaround
        var userInput = $this.data('userInput');
        $rssData.trigger('configue-rss-table', userInput);
    });

    $sortOrderRadio.trigger('configue-sort-order');
    /// нельзя вызывать до того, как развешаны прочие обработчики
    /// кривовато и неочевидно ¯\_(ツ)_/¯ 
    $feedSource.trigger('configue-feed-sorting');
});