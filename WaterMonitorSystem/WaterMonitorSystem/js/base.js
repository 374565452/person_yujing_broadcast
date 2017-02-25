// JavaScript Document
$(function () {
    if (!placeholderSupport()) {
        $('[placeholder]').focus(function () {
            var input = $(this);
            if (input.val() == input.attr('placeholder')) {
                input.val('');
                input.removeClass('placeholder');
            }
        }).blur(function () {
            var input = $(this);
            if (input.val() == '' || input.val() == input.attr('placeholder')) {
                input.addClass('placeholder');
                input.val(input.attr('placeholder'));
            }
        }).blur();
    };
    /*
    $("#button").click(function () {
        $(".lo").fadeIn(500);
        $(".lo").children(".l").animate({
            left: -129
        },
        1300);
        $(".lo").children(".r").animate({
            right: -129
        },
        1300);
    });
    
    $("#close").click(function () {
        $(".lo").fadeOut(100);

        $(".lo").children(".l").animate({
            left: 0
        });
        $(".lo").children(".r").animate({
            right: 0
        });
    });
    */
});

function placeholderSupport() {
    return 'placeholder' in document.createElement('input');
}



