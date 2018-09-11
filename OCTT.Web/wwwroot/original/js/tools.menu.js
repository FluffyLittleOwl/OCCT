var tools = (function (tools) {
    tools.menu = tools.menu || {};

    /// assumed that passed html can be processed
    var buildSidrMenu = function (options) {
        options = $.extend({
            button: null,
            menu: null
        }, options);
        if (options.button === null || options.menu === null) return;

        var $button = options.button instanceof jQuery ? options.button : $(options.button);
        var $menu = options.menu instanceof jQuery ? options.menu : $(options.menu);
        var menuId = $menu.prop("id");
        
        /// find the corresponding to the current menu item and make it active
        /// "home" is assumed to be the root view of the app
        if (window.location.pathname === "/") {
            $('a[href="home"]', $menu).closest('li').addClass("active");
        } else {
            $('a', $menu).each(function () {
                var $this = $(this),
                    href = $this.prop("href");
                if (href === window.location.href && href !== "") {
                    $this.closest('li').addClass("active");
                } else {
        
                }
            });
        }

        /// build main menu on the left
        $button.sidr({
            name: menuId,
            source: '#' + menuId,
            renaming: false // because sidr will add "sidr" prefixes otherwise
        });
        /// make sure menu has a "close" button as an addition to "Menu"'s second click
        $('.close-menu-button', $menu).on("click", function (e) {
            e.preventDefault(); // avoid change from href="#"
            $.sidr("close", menuId); // sidr will add "#" to page-selection-menu, so must be id
        });
        /// hidden class is necessary to make page to look less messy while its still building
        $menu.removeClass("hidden");
        return;
    };

    tools.menu = $.extend(tools.menu, {
        buildSidrMenu: buildSidrMenu
    });

    return tools;
})(tools || {});