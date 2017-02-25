$(function () {
    var menu = $('#menu');
    menu.height($(window).height() - $("#pageoneHeader").height() - 50);
    menu.width(200);
    menu.css("top", $("#pageoneHeader").height());
    menu.css("left", -200);

    var menuStatus;
    var show = function () {
        if (menuStatus) {
            return;
        }
        menuStatus = true;
        menu.animate({ left: '+=200px' }, 800);
    };
    var hide = function () {
        if (!menuStatus) {
            return;
        }
        menuStatus = false;
        menu.animate({ left: '-=200px' }, 800);
    };
    var toggle = function () {
        if (!menuStatus) {
            show();
        } else {
            hide();
        }
        return false;
    };
    // Show/hide the menu
    $("a.showMenu").click(toggle);
});

function urlGo(url) {
    var menu = $('#menu');
    $("a.showMenu").click();
    location.href = url;
}