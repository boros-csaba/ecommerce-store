$(function () {
    initializePhoneNumberValidation();
    initializeAddressValidation();
});

function drawPoint(dataPoint) {
    var point = $('<div class="bracelet-component-point" style="left:' + dataPoint.posX + '%; top: ' + dataPoint.posY + '%"></div>');
    var line = $('<div class="bracelet-component-line" style="left:' + dataPoint.posX + '%; top: ' + dataPoint.posY + '%"></div>');
    $(".bracelet-components-animation-container").append(point);
    $(".bracelet-components-animation-container").append(line);
    line.animate({ bottom: dataPoint.endPosY + "%" }, 1000, function () {
        var text = $('<div class="bracelet-component-text" style="left:' + dataPoint.posX + '%; top: ' + (100 - dataPoint.endPosY) + '%">' + dataPoint.display + '</div>');
        $(".bracelet-components-animation-container").append(text);
    });
}

function initializePhoneNumberValidation() {
    var element = $('#phone');
    if (!element) return;
    element.keyup(function () { validatePhoneNumber(false); });
    element.change(function () { validatePhoneNumber(true); });
    element.focusout(function () { validatePhoneNumber(true); });
}

function validatePhoneNumber(fullValidation) {
    var element = $('#phone');
    if (!element) return;
    var phoneNumber = $('#phone').val().replace(/-/g, '').replace(/ /g, '').replace(/\//g, '').replace(/\\/, '');
    var isValid = phoneNumber.length == 9;
    isValid &= /^[0-9]+$/.test(phoneNumber);
    if (!phoneNumber.startsWith("20") && !phoneNumber.startsWith("30") && !phoneNumber.startsWith("31") && !phoneNumber.startsWith("50") && !phoneNumber.startsWith("70")) {
        isValid = false;
    }

    if (!isValid && !fullValidation && phoneNumber.length < 9) isValid = true;

    if (isValid) {
        $("#phone-error").hide();
        element.removeClass("checkout-validation-error");
    }
    else {
        $("#phone-error").css("display", "block");
        element.addClass("checkout-validation-error");
    }
}

function initializeAddressValidation() {
    var element = $('#address');
    if (!element) return;
    element.keyup(function () { validateAddress(false); });
    element.change(function () { validateAddress(true); });
}

function validateAddress(fullValidation) {
    var element = $('#address');
    var isValid = /[0-9]+/.test(element.val());

    if (isValid || !fullValidation) {
        $("#address-error").hide();
        element.removeClass("checkout-validation-error");
    }
    else {
        $("#address-error").css("display", "block");
        element.addClass("checkout-validation-error");
    }
}