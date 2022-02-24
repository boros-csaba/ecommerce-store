var trace = "";
var loadingImageDark = "/images/loading.gif";
var checkmarkImage = "/images/checkmark.png";

$(function () {
    toggleEmptyCartMessage();
    $(".coupon-input").keyup(addCoupon);

    initSearchBar();

    window.addEventListener("error", function (event, source, lineno, colno, error) {
        var message = "Javascript error happened!";
        message += "<br />EventTrace: " + trace;
        message += "<br />Url: " + window.location.href;
        if (event) message += "<br />Event: " + event.toString();
        if (source) message += "<br />Source: " + source.toString();
        if (lineno) message += "<br />LineNo: " + lineno.toString();
        if (colno) message += "<br />ColNo: " + colno.toString();
        if (error) message += "<br />Error: " + error.toString();
        if (navigator != null && navigator.userAgent != null) message += "<br />Browser: " + navigator.userAgent;
        if (error && error.stack) message += "<br />ErrorStack: " + error.stack;
        var errorObject = new Error();
        if (errorObject && errorObject.stack) message += "<br />ErrorObjStack: " + errorObject.stack;
        this.reportError(message);
        return false;
    });
    $(document).ajaxError(function (_event, request, settings, thrownError) {
        var message = "Ajax error happened!";
        message += "<br />EventTrace: " + trace;
        message += "<br />Current Url: " + window.location.href;
        if (request && request.status) message += "<br />Status: " + request.status;
        if (settings && settings.url) message += "<br />Ajax Url: " + settings.url;
        if (thrownError) message += "<br />Error: " + thrownError.toString();
        if (request != null) message += "<br />request: " + JSON.stringify(request);
        if (settings != null) message += "<br />settings: " + JSON.stringify(settings);
        if (thrownError != null) message += "<br />thrownError: " + JSON.stringify(thrownError);
        if (navigator != null && navigator.userAgent != null) message += "<br />browser: " + navigator.userAgent;
        reportError(message);
    });
    $(".footer-back-to-top").css("visibility", "hidden");
    $("body").scroll(function () {
        handleBackToTopButton();       
    });

    $(".image-with-preview").click(function () { showPreviewImageWindow(this) });
    $(".product-image-magnify").click(function () { showPreviewImageWindow($(this).prev()) });
});

function logTrace(message) {
    trace += "<br />" + message;
}

function reportError(message) {
    $.ajax({
        url: "/report-error",
        data: { message: message },
        method: "POST",
    });
}

var addToCartButtonText = "";
function addToCart(element, productId, productIdString, productName, productCategory, price) {
    logTrace("addToCart" + productId);
    var braceletSize = 3;
    var braceletSize2 = 3;
    var quantity = 1;
    if ($("#product-bracelet-size").length) {
        braceletSize = $("#product-bracelet-size").children("option:selected").val();
        braceletSize2 = null;
        if ($("#product-bracelet-size2").children("option:selected") != null) {
            braceletSize2 = $("#product-bracelet-size2").children("option:selected").val();
        }
        if ($("#product-details-quantity").length) {
            quantity = $("#product-details-quantity")[0].innerHTML;
        }
    }
    if ($(element).html().indexOf("img") > 0) {
        return;
    }
    else if (addToCartButtonText == "") {
        addToCartButtonText = $(element).text()
    }
    $(element).html('<img src="/images/loading-white.gif" width="18px" height="18px" />');
    var url = "/kosar/hozzaadas?productId=" + productId + "&quantity=" + quantity + "&braceletSize=" + braceletSize;
    if (braceletSize2 != null) {
        url = url + "&braceletSize2=" + braceletSize2;
    }

    if ($(".custom-text-bracelet-text").length > 0) {
        var customText = $(".custom-text-bracelet-text").val();
        if (customText && customText.length > 0) {
            url += "&customText=" + customText;
        }
    }

    window.dataLayer = window.dataLayer || [];
    var eventId = guid();
    url = url + "&eventId=" + eventId;
    var externalId = '';
    if (typeof fbExternalId != "undefined") {
        externalId = fbExternalId;
    }
    console.log(externalId);
    window.dataLayer.push({
        event: 'addToCart',
        eventID: eventId,
        external_id: externalId,
        ecommerce: {
            currencyCode: 'HUF',
            add: {
                products: [{
                    id: productIdString,
                    name: productName,
                    category: productCategory,
                    price: price,
                    quantity: quantity
                }]
            }
        }
    });

    $.ajax({
        url: url,
        method: "POST",
        success: function (data, _textStatus, _jqXHR) {
            $("#cart-drawer").html(data);
            updateCartItemsCountFromCart();
            showCartDrawer();
            toggleEmptyCartMessage();
            $(element).html(addToCartButtonText);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log("Error");
            console.log(jqXHR);
            console.log(textStatus);
            console.log(errorThrown);
        }
    });
    return false;
}

function showCartDrawer() {
    logTrace("showCartDrawer");
    $("#wrapper").addClass("cart-drawer-open");
    $("body").css("overflow-y", "hidden");
    $("#overlay").css("display", "block");
    $("#cart-drawer").addClass("cart-drawer-opened");
}

function hideCartDrawer() {
    logTrace("hideCartDrawer");
    $("#wrapper").removeClass("cart-drawer-open");
    $("body").css("overflow-y", "auto");
    $("#overlay").css("display", "none");
    $("#cart-drawer").removeClass("cart-drawer-opened");
}

function toggleWishlisted(element, productId, productIdString, color, removeElement) {
    logTrace("toggleWishlisted");
    removeElement = removeElement || false;
    var currentImage = $(element).attr("src");
    var isAdded = currentImage.indexOf("filled") > 0;

    $(element).attr("src", "/images/loading.gif");
    var urlBase = "/kivansag-lista/torles"
    if (!isAdded) {
        urlBase = "/kivansag-lista/hozzaadas";
        fbq('track', 'AddToWishlist', {
            content_ids: productIdString,
            content_type: 'product',
        });
    }

    $.ajax({
        url: urlBase + "?productId=" + productId,
        method: "POST",
        success: function (_data, _textStatus, _jqXHR) {
            if (isAdded) {
                var imageUrl = "/images/heart.png";
                if (color) imageUrl = "/images/heart-color.png";
                $(element).attr("src", imageUrl);
                if (removeElement) {
                    $(element).parents(".product-container").remove();
                }
            }
            else {
                var imageUrl = "/images/heart-filled.png";
                if (color) imageUrl = "/images/heart-color-filled.png";
                $(element).attr("src", imageUrl);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log("Error");
            console.log(jqXHR);
            console.log(textStatus);
            console.log(errorThrown);
        }
    });
    return false;
}

function removeCartItem(cartItemId) {
    logTrace("removeCartItem");
    var elements = $(".cart-item-quantity-" + cartItemId);
    if (elements == null || elements.length <= 0) return;
    $(".cart-item-" + cartItemId).hide("fast", function () {
        $(".cart-item-" + cartItemId).remove();
        toggleEmptyCartMessage();
    });
    $.ajax({
        url: "/kosar/torles?cartItemId=" + cartItemId,
        method: "POST",
        success: function (data, _textStatus, _jqXHR) {
            if (data.success) updateCart(data);
            updateCartItemsCountFromCart();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log("Error");
            console.log(jqXHR);
            console.log(textStatus);
            console.log(errorThrown);
        }
    });
}

function removeCouponFromCart() {
    logTrace("removeCouponCart");
    $.ajax({
        url: "/kosar/kupon-torles",
        method: "POST",
        success: function (data, _textStatus, _jqXHR) {
            if (data.success) {
                updateCart(data);
                var input = $(".coupon-input");
                input.val("");
                var statusIndicator = $($(input).next()[0]);
                statusIndicator.html("");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log("Error");
            console.log(jqXHR);
            console.log(textStatus);
            console.log(errorThrown);
        }
    });
}

function changeCartItemSize(element, cartItemId, isSize2) {
    logTrace("changeCartItemSize");
    var value = $(element).val();
    $.ajax({
        url: "/kosar/meret?cartItemId=" + cartItemId + "&size=" + value + "&isSize2=" + isSize2,
        method: "POST",
        success: function (data, _textStatus, _jqXHR) {
            if (data.success) updateCart(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log("Error");
            console.log(jqXHR);
            console.log(textStatus);
            console.log(errorThrown);
        }
    });
}

// #region Cart Item Quantity Handling

function changeCartItemQuantity(cartItemId, change, unitPrice) {
    logTrace("changeCartItemQuantity");
    var elements = $(".cart-item-quantity-" + cartItemId);
    if (elements == null || elements.length <= 0) return;
    var value = parseInt(elements[0].innerHTML, 10);
    if (!value || isNaN(value)) return;
    var newValue = change + value;
    
    if (newValue == 0) {
        removeCartItem(cartItemId);
        return;
    }

    elements.each(function () {
        $(this).html("");
        $(this).addClass("number-input-value-loading");
    });

    $.ajax({
        url: "/kosar/mennyiseg?cartItemId=" + cartItemId + "&quantity=" + newValue,
        method: "POST",
        success: function (data, _textStatus, _jqXHR) {
            if (data.success) updateCart(data);
            updateCartItemsCountFromCart();

            elements.each(function () {
                $(this).html(newValue);
                $(this).removeClass("number-input-value-loading");
                if (newValue == 1) {
                    $(this).prev().html("");
                    $(this).prev().addClass("number-imput-delete");
                }
                else {
                    $(this).prev().html("-");
                    $(this).prev().removeClass("number-imput-delete");
                }
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log("Error");
            console.log(jqXHR);
            console.log(textStatus);
            console.log(errorThrown);
        }
    });

    addAmountToMoney("#cart-drawer-total", change, unitPrice);
    addAmountToMoney("#cart-page-item-total-" + cartItemId, change, unitPrice);
    addAmountToMoney("#cart-page-total", change, unitPrice);
}

function changeQuantitySelectorValue(element, change) {
    logTrace("changeQuantitySelectorValue");
    var newValue = change + parseInt($("#" + element)[0].innerHTML, 10);
    if (newValue < 1) return;
    $("#" + element).html(newValue);
}

// #endregion

function addAmountToMoney(selector, change, unitPrice) {
    logTrace("addAmountToMoney");
    var items = $(selector);
    if (items == null || items.length < 1) return;
    var value = parseInt(getValueFromMoneyString(items[0].innerHTML), 10);
    value += unitPrice * change;
    $(items[0]).html(getFormattedMoneyString(value) + " Ft");
}

function getValueFromMoneyString(stringValue) {
    logTrace("getValueFromMoneyString");
    return stringValue.replace(/ /g, "").replace("Ft", "");
}

function getFormattedMoneyString(value) {
    logTrace("getFormattedMoneyString");
    return value.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1 ')
}

function toggleEmptyCartMessage() {
    logTrace("toggleEmptyCartMessage");
    if ($(".cart-drawer-item:visible:not(.coupon):not(.cart-drawer-item-extra):not(.cart-drawer-item-promotion)").length > 0) {
        $(".empty-cart-message").hide();
        $(".cart-checkout-button").removeClass("disabled");
    }
    else {
        if ($(".cart-drawer-item:visible:not(.coupon)").length <= 0) {
            $(".empty-cart-message").show();
        }
        else {
            $(".empty-cart-message").hide();
        }
        $(".cart-checkout-button").addClass("disabled");
    }
    logTrace("exit toggleEmptyCartMessage");
}

var addCouponTimeout = null;
function addCoupon(e) {
    logTrace("addCoupon");
    if (addCouponTimeout != null) {
        clearTimeout(addCouponTimeout);
    }
    addCouponTimeout = setTimeout(function () {
        var input = $(e.target);
        var value = input.val();
        var statusIndicator = $($(input).next()[0]);
        var errorContainer = $(statusIndicator.next()[0]);
        statusIndicator.html("");
        errorContainer.html("");
        if (value == null || value.length < 1) {
            return;
        }

        statusIndicator.html('<img src="' + loadingImageDark + '" />');
        logTrace("Coupon code: " + value);
        $.ajax({
            url: "/kosar/kupon?code=" + value,
            method: "POST",
            success: function (data, _textStatus, _jqXHR) {
                if (data.success) {
                    statusIndicator.html('<img src="' + checkmarkImage + '" />');
                    errorContainer.html("");
                    updateCart(data);
                }
                else {
                    statusIndicator.html("");
                    errorContainer.html(data.errorMessage);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log("Error");
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
            }
        });
    }, 1000);
}

function updateCart(cart) {
    logTrace("updateCart");
    $(".cart-total").html(getFormattedMoneyString(cart.total) + " Ft");

    if (cart.shippingPrice > 0) $(".cart-shipping-total").html(getFormattedMoneyString(cart.shippingPrice) + " Ft");
    else $(".cart-shipping-total").html("INGYENES");

    if (cart.hasCoupon) {
        $(".coupon-input").val(cart.CouponCode);
        $(".coupon-input").attr("disabled", true);
        $(".coupon-name").html(cart.couponName);
        $(".coupon-description").html(cart.couponDescription);
        if (cart.couponPercentage != null) $(".coupon-percentage").html("-" + cart.couponPercentage + "%");
        else $(".coupon-percentage").html("");
        $(".coupon-amount").html(getFormattedMoneyString(cart.couponAmount) + " Ft");
        $(".coupon").show("fast");
    }
    else {
        $(".coupon").hide("fast");
        $(".coupon-input").attr("disabled", false);
    }
    if (typeof updateFreeLavaBraceletPromotion === 'function') {
        updateFreeLavaBraceletPromotion(cart.total);
    }
    if (typeof updateFreeLavaBraceletPromotionCartPage === 'function') {
        updateFreeLavaBraceletPromotionCartPage(cart.total);
    }
    cart.cartItems.forEach(function (item) {
        //console.log(item);
    })
}

function updateCartItemsCount(count) {
    logTrace("updateCartItemsCount");
    if (count > 0) {
        $(".cart-items-count").html(count);
        $(".cart-items-count").show("fast");
    }
    else {
        $(".cart-items-count").hide("fast");
    }
    logTrace("exit updateCartItemsCount");
}

function updateCartItemsCountFromCart() {
    logTrace("updateCartItemsCountFromCart");
    var cartItems = $("#cart-drawer").find(".cart-drawer-item");
    var count = 0;
    for (var i = 0; i < cartItems.length; i++) {
        var quantityElement = $(cartItems[i]).find(".number-input-value");
        if (quantityElement && quantityElement.html()) {
            count += parseInt(quantityElement.html());
        }
    }
    updateCartItemsCount(count);
}

var isBackToTopButtonVisible = false;
function handleBackToTopButton() {
    var buttonTopOffset = $(".footer-back-to-top").offset().top;
    var viewPortSize = $(window).height();
    var pageScrollPosition = $("body").scrollTop();
    if (pageScrollPosition < 200) return;

    if (buttonTopOffset + 100 < viewPortSize && !isBackToTopButtonVisible) {
        $(".footer-back-to-top").css("visibility", "visible").hide().fadeIn();
        isBackToTopButtonVisible = true;
        $(".footer-back-to-top").on("click", function (_event) {
            $("html, body").animate({
                scrollTop: 0
            }, 800)
        });
    }
}

function showPreviewImageWindow(element) {
    console.log($(element).find("img")[0]);
    var image = $($(element).find("img")[0]);
    var imageSource = image.attr("src");
    console.log(imageSource);
    var elements = [];
    if (imageSource.indexOf("&s=") > 0) {
        var imgWithoutSize = imageSource.substring(0, imageSource.indexOf("&s="));
        console.log(imgWithoutSize);
        elements = [
            {
                src: imgWithoutSize + "&s=1280",
                w: 1280,
                h: 1280
            }
        ];
    }
    showGallery(0, elements);
}

function showPopupWindow(content) {
    $("#popup-window-content-inner").html(content);
    $("#popup-window").css("display", "flex");
    $("#popup-window").unbind();
    $("#popup-window").click(function (e) {
        if (e.target !== this) return;
        $("#popup-window").fadeOut("fast");
    });
    $("#popup-header-close").click(function (e) {
        $("#popup-window").fadeOut("fast");
    });
}

// #region Search
function initSearchBar() {
    $(".search-bar-container").hide();
    $("#search-input").keypress(function (e) {
        if (e.which == 13) startSearch();
    });

    $("#search-input").keyup(function () {
        var searchTerm = $("#search-input").val();
        if (!searchTerm || searchTerm.length == 0) {
            hideSearchSuggestions();
            return;
        }
        $.ajax({
            url: "/kereses-ajanlat/" + encodeURI(searchTerm),
            method: "POST",
            success: function (data, _textStatus, _jqXHR) {
                updateSuggestionsList(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log("Error");
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
            }
        });
    });
}

function showSearchBar() {
    $("#overlay").css("display", "block");
    $(".search-bar-container").show("fast");
    $("#search-input").focus();
}

function hideSearchBar() {
    $("#overlay").css("display", "none");
    $('.search-term').removeClass("validation-error");
    $(".search-bar-container").hide("fast");
    hideSearchSuggestions();
}

function hideSearchSuggestions() {
    $(".search-suggestions-container").html("");
    $(".search-suggestions-container").css("display", "none");
}

function startSearch() {
    var searchTerm = $("#search-input").val();
    if (searchTerm == null || searchTerm.length < 3) {
        $("#search-input").addClass("validation-error");
        return;
    }
    searchTerm = searchTerm.replace(" ", "-");
    window.location.href = "/kereses/" + searchTerm;
}

function updateSuggestionsList(suggestions) {
    if (!suggestions || suggestions.length == 0) {
        hideSearchSuggestions();
        return;
    }
    $(".search-suggestions-container").css("display", "block");
    $(".search-suggestions-container").html("");
    for (var i = 0; i < suggestions.length; i++) {
        var content = "";
        if (suggestions[i].type == 1) content = getMineralSuggestionTemplate(suggestions[i]);
        $(".search-suggestions-container").append(content);
    }
}

function getMineralSuggestionTemplate(suggestion) {
    var innerLink = $('<a href="/kereses/' + suggestion.searchLinkUrl + '"></a>');
    var container = $('<div class="search-suggestion-mineral"></div>');
    var picture = $('<picture></picture>');
    picture.append($('<source srcset="' + suggestion.imageUrl + '" type="image/webp">'));
    picture.append($('<source srcset="' + suggestion.imageUrl.replace(".webp", ".png") + '" type="image/png">'));
    picture.append($('<img src="' + suggestion.imageUrl.replace(".webp", ".png") + '">'));
    container.append(picture);
    var innerContainer = $('<div class="search-suggestion-mineral-inner"></div>');
    innerContainer.append($('<h3 class="search-suggestion-title">' + suggestion.title + '</h3>'));
    innerContainer.append($('<span class="search-suggestion-description">' + suggestion.description + '</div>'));
    container.append(innerContainer);
    innerLink.append(container);
    return innerLink;
}
// #endregion

function guid() {
    var buf = [],
        chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789',
        charlen = chars.length;
    for (var i = 0; i < 32; i++) {
        buf[i] = chars.charAt(Math.floor(Math.random() * charlen));
    }
    return buf.join('');
}