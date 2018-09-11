/// somthing about menu?
$(document).ready(function () {
    var $pageContent = $('.page-content');

    /// lets activate sidr menu
    var $sidrMenuOpen = $('#page-selection-menu-button', $pageContent),
        $sidr = $('#page-selection-menu', $pageContent);

    tools.menu.buildSidrMenu({
        button: $sidrMenuOpen,
        menu: $sidr
    });

    /// код относящийся к ViewPostMethod.chtml, которая требует выполнять представление данных через перезагрузки страницы,
    /// размазывается решение так себе, но на позитивной стороне это возможность сброса состояния элементов управления
    $('a[href="viewpostmethod"]', $sidr).on('click', function (e, data) {
        localStorage.removeItem('userInput');
    });
});