
$(document).ready(function () {
    initExampleImagesCarousel();
    startBraceletDesignerWithDelay();
    createBeadsSelectors();

    $(".designer-style-selector:eq(0)").click(function () { setBraceletStyle(0); });
    $(".designer-style-selector:eq(1)").click(function () { setBraceletStyle(1); });
    $(".designer-style-selector:eq(2)").click(function () { setBraceletStyle(2); });
    $(".designer-style-selector:eq(3)").click(function () { setBraceletStyle(3); });
    $(".bracelet-text-input").focus(function () { deleteInitialText(); });
    $(".bracelet-text-input").click(function () { moveCursorToEnd(); });
    $(".bracelet-input-component").click(function (e) { return moveCursorToElement(e, this); });
    $(".bracelet-designer-keyboard-button").click(function () { keyboardButtonPress(this); });
    $(".bracelet-designer-keyboard-backspace").click(function () { removeElement(); });
    $(".white-letters-selection").click(function () { changeLettersColor(this, true); });
    $(".black-letters-selection").click(function () { changeLettersColor(this, false); });
    $("#designer-tab-1").click(function () { switchTab(1); });
    $("#designer-tab-2").click(function () { switchTab(2); });
    $("#designer-tab-3").click(function () { switchTab(3); });
    $("#designer-tab-bottom-1").click(function () { switchTab(1); });
    $("#designer-tab-bottom-2").click(function () { switchTab(2); });
    $("#designer-tab-bottom-3").click(function () { switchTab(3); });
    $(".switch-next-tab-2").click(function () { switchTab(2); });
    $(".switch-next-tab-3").click(function () { switchTab(3, true); });
    $(".bracelet-designer-cart-button").click(function () { addCustomBraceletToCart(cartItemId); });
});

function startBraceletDesignerWithDelay() {
    $(".name-bracelet-designer-button").html("<img src='/images/loading-white.gif' />");
    $.ajax({
        url: "/egyedi-karkoto-keszito/start",
        data: "",
        method: "POST",
    });
    window.onbeforeunload = function () { return isModified(); };
    setTimeout(function () {
        if (cartItemId > 0) {
            startBracletDesignerForExistingBracelet();
        }
        else {
            startBraceletDesigner();
        }
    }, 1000);
}

function startBracletDesignerForExistingBracelet() {
    $(".bracelet-designer-cart-button").html("Módosítás mentése");
    var selectedBeadId = cartItemSelectedBeadId;
    var selectedBead = beads.filter(function (b) { return b.id == selectedBeadId })[0];
    startBraceletDesigner(beads.indexOf(selectedBead));
    deleteInitialText();
    for (var i = 0; i < cartItemSelectedComponents.length; i++) {
        var extra = extras.filter(function (e) { return e.id == cartItemSelectedComponents[i] });
        if (extra.length > 0) {
            onExtraSelected(extra[0]);
        }
        else {
            var whiteLetter = whiteLetters.filter(function (l) { return l.id == cartItemSelectedComponents[i] });
            var blackLetter = blackLetters.filter(function (l) { return l.id == cartItemSelectedComponents[i] });
            var letter = "";
            if (whiteLetter.length > 0) {
                letter = whiteLetter[0].letter;
            }
            else if (blackLetter.length > 0) {
                changeLettersColor($(".black-letters-selection"), false);
                letter = blackLetter[0].letter;
            }
            if (letter != "") {
                var newElement = $('<div class="bracelet-input-component">' + letter + '</div>');
                newElement.click(function (e) { return moveCursorToElement(e, this); });
                $(".cursor").before(newElement);
                drawPreview();
            }
        }
    }
    $("#product-bracelet-size").val(cartItemBraceletSize);
}

braceletDesignerIsInitialized = false;
var startingData = '';
function startBraceletDesigner() {
    hideElementsOnBraceletDesignerStart();
    setBraceletStyle(selectedStyle);
    selectBead(selectedBeadId);
    if (selectedSecondaryBeadId > 0) {
        selectBead(selectedSecondaryBeadId);
    }
    for (var i = 0; i < selectedComplementaryProducts.length; i++) {
        $(".complementary-product-checkbox-" + selectedComplementaryProducts[i]).prop("checked", true);
    }
    startingData = getCartItemData(cartItemId);
    updateTotalPrice();

    braceletDesignerIsInitialized = true;
    drawPreview();
}

function createBeadsSelectors() {
    for (var i = 0; i < beads.length; i++) {
        var beadSelector = createBeadSelector(beads[i]);
        $(".designer-bead-selectors").append(beadSelector);
    }

    for (var i = 0; i < extras.length; i++) {
        var extra = extras[i];
        if (extra.visible) {
            var extraSelector = createExtraSelector(extra, 1);
            extraSelector[0].extra = extra;
            extraSelector.click(function () { onExtraSelected(this.extra); });
            $(".selectable-extras").append(extraSelector);
            if (i == Math.floor(extras.length / 2)) {
                $(".selectable-extras").append($('<div class="flex-wrap-break"></div>'));
            }
        }
    }
}

function createBeadSelector(bead) {
    var beadSelector = $('<div class="bead-option"></div>');
    if (bead.soldOut) beadSelector.addClass("sold-out");
    var innerDiv = $('<div></div>');
    var picture = $('<picture></picture>');
    picture.append($('<source srcset="/images/components/' + bead.url + '.webp" type="image/webp">'));
    picture.append($('<source srcset="/images/components/' + bead.url + '.png" type="image/png">'));
    picture.append($('<img src="/images/components/' + bead.url + '.png">'));
    innerDiv.append(picture);
    beadSelector.append(innerDiv);
    beadSelector.append($('<span>' + bead.name + '</span>'));
    if (!bead.soldOut) {
        beadSelector.click(function () { selectBead(bead.id) });
    }
    return beadSelector;
}

function initExampleImagesCarousel() {
    $('.preview-images-carousel').owlCarousel({
        stagePadding: 50,
        loop: true,
        autoplay: false,
        autoplayTimeout: 3000,
        autoplayHoverPause: true,
        responsive: {
            0: {
                items: 1,
            },
            610: {
                items: 2
            },
            1000: {
                items: 3
            },
            1220: {
                items: 4
            }
        }
    });
    $('.preview-images-carousel').trigger('play.owl.autoplay', [3000]);
}

function isModified() {
    if (cartItemId > 0) {
        var data = getCartItemData(cartItemId);
        if (JSON.stringify(data) != JSON.stringify(startingData)) {
            return "Még nem mentettel el a változtatásokat!";
        }
    }
    return undefined;
}

function hideElementsOnBraceletDesignerStart() {
    $(".name-bracelet-designer-button").hide();
    $(".name-bracelet-designer-wrapper").slideDown("slow");
    $(".bottom-buttons-navigation").css("visibility", "visible");
    $(".next-step-button").show();
    $("html, body").animate({
        scrollTop: 0
    }, 400);
}

function selectBead(beadId) {
    var bead = getBeadById(beadId);
    var element = getBeadSelector(bead);
    if (selectedStyle == 0) {
        if ($(element).hasClass("selected")) return;
        createComplementaryProductSelectors(bead);
        $(".bead-option").removeClass("selected");
        $(element).addClass("selected");
        selectedBead = bead;
    }
    else {
        if (this.selectedBead == null) {
            if (this.selectedSecondaryBead == bead) return;
            $(element).addClass("selected");
            this.selectedBead = bead;
        }
        else if (this.selectedSecondaryBead == null) {
            if (this.selectedBead == bead) return;
            $(element).addClass("selected");
            this.selectedSecondaryBead = bead;
        }
        else if (this.selectedSecondaryBead == bead) {
            this.selectedSecondaryBead = null;
            $(element).removeClass("selected");
        }
        else if (this.selectedBead == bead) {
            this.selectedBead = null;
            $(element).removeClass("selected");
        }
        else return;
    }
    drawPreview();
    showSelectedsBeadInformation();
}

function showSelectedsBeadInformation() {
    var container = $(".custom-bracelet-bead-information-container")
    container.html("");
    var beadNr = 1;
    if (this.selectedBead != null) container.append(getBeadInformation(this.selectedBead, beadNr++));
    if (this.selectedStyle > 0 && this.selectedSecondaryBead != null) container.append(getBeadInformation(this.selectedSecondaryBead, beadNr));
}

function getBeadInformation(bead, beadNr) {
    var beadInfo = $('<div class="custom-bracelet-bead-information"></div>');
    var beadInfoHeader = $('<div class="bead-information-header"></div>');
    beadInfoHeader.click(function () { toggleBeadDescription(beadNr); });
    var picture = $('<picture></picture>');
    picture.append($('<source srcset="/images/components/' + bead.url + '.webp" type="image/webp">'));
    picture.append($('<source srcset="/images/components/' + bead.url + '.png" type="image/png">'));
    picture.append($('<img src="/images/components/' + bead.url + '.png">'));
    beadInfoHeader.append(picture);
    var beadInfoInner = $('<div class="bead-information-header-inner"></div>');
    beadInfoInner.append($('<h3>' + bead.name + '</h3>'));
    beadInfoInner.append($('<span>' + bead.description + '</span>'));
    beadInfoHeader.append(beadInfoInner);
    beadInfoHeader.append($('<div class="bead-info-header-expand" id="bead-info-header-expand-' + beadNr + '">▼</div>'));
    beadInfo.append(beadInfoHeader);
    beadInfo.append($('<span class="bead-description" id="bead-description-' + beadNr + '">' + bead.longDescription.replace(/NewLine/g, "<br />") + '</span>'));
    return beadInfo;
}

function toggleBeadDescription(id) {
    if ($("#bead-info-header-expand-" + id).html() == "▼") {
        $("#bead-description-" + id).slideDown();
        $("#bead-info-header-expand-" + id).html("▲");
    }
    else {
        $("#bead-description-" + id).slideUp();
        $("#bead-info-header-expand-" + id).html("▼");
    }
}

function onExtraSelected(extra) {
    deleteInitialText();
    var extraComponents = getExtraComponents();
    if (extraComponents.length > 15) {
        $(".bracelet-text-input-message").html("<b>Max 15 karakter</b>");
        return;
    }
    var newElement = $(createExtraSelector(extra, 0.6, false)[0]);
    newElement.click(function (e) { return moveCursorToElement(e, this); });
    newElement.addClass("bracelet-input-component");
    $(".cursor").before(newElement);
    drawPreview();
}

function moveCursorToEnd() {
    deleteInitialText();
    $($(".bracelet-input-component").last()).after($(".cursor"));
}

function moveCursorToElement(event, element) {
    if (deleteInitialText()) return false;
    var rect = element.getBoundingClientRect();
    var x = event.clientX - rect.left;
    if (x < element.offsetWidth / 2) {
        $(element).before($(".cursor"));
    }
    else {
        $(element).after($(".cursor"));
    }
    return false;
}

function keyboardButtonPress(button) {
    deleteInitialText();
    var extraComponents = getExtraComponents();
    if (extraComponents.length > 15) {
        $(".bracelet-text-input-message").html('<b style="font-size: 1rem;">Max 15 karakter</b>');
        return;
    }
    var newElement = $('<div class="bracelet-input-component">' + $(button).html() + '</div>');
    newElement.click(function (e) { return moveCursorToElement(e, this); });
    $(".cursor").before(newElement);
    drawPreview();
}

function removeElement() {
    if (deleteInitialText()) return;
    $(".cursor").prev().remove();
    drawPreview();
}

function createComplementaryProductSelectors(bead) {
    if (bead.complementaryProducts.length <= 0) {
        $(".complementary-product-info").hide();
    }
    else {
        $(".complementary-product-info").show();
    }
    $(".complementary-products-inner-container").html("");
    for (var i = 0; i < bead.complementaryProducts.length; i++) {
        var complementaryProduct = bead.complementaryProducts[i];
        var container = $('<div class="complementary-product">');
        var checkbox = $('<input type="checkbox" class="complementary-product-checkbox complementary-product-checkbox-' + complementaryProduct.id + '" value="' + complementaryProduct.id + '" name="' + complementaryProduct.price + '" />');
        container.append(checkbox);
        checkbox.change(function () { updateTotalPrice(); });
        var picture = $('<picture class="image-with-preview complementary-product-image"></picture>');
        picture.append($('<source class="complementary-product-image" srcset="' + complementaryProduct.imageUrl + '.webp" type="image/webp">'));
        picture.append($('<source class="complementary-product-image" srcset="' + complementaryProduct.imageUrl + '.jpg" type="image/jpeg">'));
        picture.append($('<img class="complementary-product-image" src="' + complementaryProduct.imageUrl + '.jpg">'));
        container.append(picture);
        var magnifyImage = $('<img src="/images/search-white.png" class="product-image-magnify" />');
        container.append(magnifyImage);
        picture.click(function () { showPreviewImageWindow(this) });
        magnifyImage.click(function () { showPreviewImageWindow($(this).prev()) });
        var textContainer = $('<div></div>');
        textContainer.append('<span class="complementary-product-title">' + complementaryProduct.name + '</h2>');
        textContainer.append('<span class="complementary-product-price">+ ' + getFormattedMoneyString(complementaryProduct.price) + ' Ft </span>');
        container.append(textContainer);
        $(".complementary-products-inner-container").append(container);
    }
}

function updateTotalPrice() {
    var total = 6990;
    var productCheckboxes = $(".complementary-product-checkbox");
    for (var i = 0; i < productCheckboxes.length; i++) {
        if ($(productCheckboxes[i]).is(":checked")) {
            total += parseInt($(productCheckboxes[i]).attr("name"));
        }
    }
    $(".custom-bracelet-total").html(getFormattedMoneyString(total) + " Ft");
}

function setBraceletStyle(style) {
    if (style == 0) {
        $(".bead-option").removeClass("selected");
        selectedSecondaryBead = null;
        if (selectedBead) {
            var element = getBeadSelector(selectedBead);
            $(element).addClass("selected");
        }
    }
    var element = $(".designer-style-selector")[style];
    if ($(element).hasClass("selected")) return;
    $(".designer-style-selector").removeClass("selected");
    $(element).addClass("selected");
    selectedStyle = style;
    drawPreview();
}

function getBeadById(beadId) {
    return this.beads.filter(function (b) { return b.id == beadId })[0];
}

function getBeadSelector(bead) {
    var elementIndex = this.beads.indexOf(bead);
    return $(".bead-option")[elementIndex];
}

var initialTextModified = false;
function deleteInitialText() {
    if (initialTextModified) return false;
    initialTextModified = true;
    $(".bracelet-input-component").remove();
    drawPreview();
    return true;
}

function changeLettersColor(element, changeToWhiteLetters) {
    if (showWhiteLetters && changeToWhiteLetters) return;
    $(".selected-color").removeClass("selected-color");
    $(element).addClass("selected-color");
    showWhiteLetters = changeToWhiteLetters;
    drawPreview();
}

function switchTab(tab) {
    if (!isInputValid) return;
    if (selectedTab == tab) return;
    $("#tab-" + selectedTab).hide();
    selectedTab = tab;
    $(".active-tab").removeClass("active-tab");
    $("#designer-tab-" + tab).addClass("active-tab");
    $("#designer-tab-bottom-" + tab).addClass("active-tab");
    $("#tab-" + tab).show();

    if (tab < 3) {
        $(".next-step-button").click(function () { switchTab(tab + 1); });
        $(".next-step-button").html("Következő lépés");
        $(".complementary-products-container").css("display", "none");
        $(".next-step-button").addClass("mobile-only");
        $(".product-bracelet-size-wrapper").css("display", "none");
    }
    else {
        $(".complementary-products-container").css("display", "block");
        $(".next-step-button").click(function () { addCustomBraceletToCart(cartItemId); });
        $(".next-step-button").removeClass("mobile-only");
        if (cartItemId > 0) {
            $(".next-step-button").html("Módosítás mentése");
        }
        else {
            $(".next-step-button").html(addToCartText);
        }
        $(".product-bracelet-size-wrapper").css("display", "flex");
    }

    $("html, body").animate({
        scrollTop: 0
    }, 400);
}

function drawPreview() {
    if (!braceletDesignerIsInitialized) return;

    $(".bracelet-preview-beads").html(' ');
    $(".bracelet-preview-shadow").html(' ');

    var extraComponents = getExtraComponents();
    var beadsCount = previewBeadsMaxCount - extraComponents.length;
    if (beadsCount.length % 2 == 1) beadsCount--;

    var beadsToAdd = [];
    var patternCounter = 0;
    for (var i = 0; i < beadsCount / 2; i++) {
        if (this.selectedStyle == 0) {
            beadsToAdd.unshift(this.selectedBead);
            beadsToAdd.push(this.selectedBead);
        }
        else if (this.selectedBead == null) {
            beadsToAdd.unshift(this.selectedSecondaryBead);
            beadsToAdd.push(this.selectedSecondaryBead);
        }
        else if (this.selectedSecondaryBead == null) {
            beadsToAdd.unshift(this.selectedBead);
            beadsToAdd.push(this.selectedBead);
        }
        else if (this.selectedStyle == 1) {
            if (patternCounter == 0) {
                beadsToAdd.unshift(this.selectedBead);
                beadsToAdd.push(this.selectedBead);
                patternCounter = 1;
            }
            else {
                beadsToAdd.unshift(this.selectedSecondaryBead);
                beadsToAdd.push(this.selectedSecondaryBead);
                patternCounter = 0;
            }
        }
        else if (this.selectedStyle == 2) {
            if (patternCounter == 0 || patternCounter == 1) {
                beadsToAdd.unshift(this.selectedBead);
                beadsToAdd.push(this.selectedBead);
            }
            else {
                beadsToAdd.unshift(this.selectedSecondaryBead);
                beadsToAdd.push(this.selectedSecondaryBead);
            }
            patternCounter++;
            if (patternCounter >= 4) patternCounter = 0;
        }
        else {
            beadsToAdd.unshift(this.selectedBead);
            beadsToAdd.push(this.selectedSecondaryBead);
        }
    }

    for (var i = 0; i < beadsToAdd.length / 2; i++) {
        addBeadWithShadow(beadsToAdd[i], beadSize);
    }

    for (var i = 0; i < extraComponents.length; i++) {
        var shadowSize = 0;
        var shadowHeight = "";
        if (extraComponents[i].isLetter) {
            addLetter(extraComponents[i]);
            shadowSize = letterSize;
        }
        else {
            addExtra(extraComponents[i]);
            shadowSize = extraComponents[i].width;
            if (shadowSize <= 10) {
                shadowHeight = "height: 9px";
            }
        }
        var shadowImage = "";
        if (shadowSize <= 20) {
            shadowImage = "-xs";
        }
        else if (shadowSize <= 32) {
            shadowImage = "-s";
        }
        $(".bracelet-preview-shadow").append('<img src="/images/shadow' + shadowImage + '.png" style="width:' + shadowSize + 'px;' + shadowHeight + '"/>');
    }

    for (var i = beadsToAdd.length / 2; i < beadsToAdd.length; i++) {
        addBeadWithShadow(beadsToAdd[i], beadSize);
    }
}

function addCustomBraceletToCart(cartItemId) {
    debugger;
    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push({
        event: 'addToCart',
        ecommerce: {
            currencyCode: 'HUF',
            add: {
                products: [{
                    id: 'custom-bracelet',
                    name: 'Egyedi karkötő',
                    category: 'Egyedi karkötő',
                    price: '6990'
                }]
            }
        }
    });

    window.onbeforeunload = undefined;
    var data = getCartItemData(cartItemId);
    $.ajax({
        url: "/egyedi-karkoto-keszito/kosarba",
        method: "POST",
        data: data,
        success: function (_data, _textStatus, _jqXHR) {
            window.location.href = "/rendeles/kosar";
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log("Error");
            console.log(jqXHR);
            console.log(textStatus);
            console.log(errorThrown);
        }
    });
}

function getCartItemData(cartItemId) {
    var complementaryProducts = [];
    var checkedComplementaryProducts = $(".complementary-product-checkbox:checked");
    for (var i = 0; i < checkedComplementaryProducts.length; i++) {
        complementaryProducts.push(parseInt($(checkedComplementaryProducts[i]).val()));
    }
    var data = {
        cartItemId: cartItemId,
        beadTypeId: this.selectedBead != null ? selectedBead.id : 0,
        secondaryBeadTypeId: this.selectedSecondaryBead != null ? selectedSecondaryBead.id : 0,
        styleType: this.selectedStyle,
        componentIds: getExtraComponents().map(c => c.id),
        braceletSize: $("#product-bracelet-size").children("option:selected").val(),
        complementaryProducts: complementaryProducts
    };
    return data;
}

function createExtraSelector(extra, size, withBottomPadding = false) {
    var bottomPadding = 0;
    if (withBottomPadding) {
        bottomPadding = extra.marginTop * size / 2;
    }
    return $('<img src="/images/components/' + extra.url + '.png" style="width:' + (extra.width * size) + 'px; padding-bottom: ' + bottomPadding + 'px"> ');
}

function addExtra(extra) {
    $(".bracelet-preview-beads").append($('<img src="/images/components/' + extra.url + '.png" style="width:' + extra.width + 'px; margin-top:' + extra.marginTop + 'px">'));
}

function addLetter(letter) {
    $(".bracelet-preview-beads").append($('<img src="/images/components/' + letter.url + '.png" style="width:' + letterSize + 'px; margin-top:' + letterMarginTop + 'px">'));
}

function addBeadWithShadow(bead, size) {
    var url = bead.url;
    if (bead.imageFrequencies.length > 0) {
        var totalWeight = 0;
        for (var i = 0; i < bead.imageFrequencies.length; i++) {
            totalWeight += bead.imageFrequencies[i];
        }
        var random = Math.floor(Math.random() * (totalWeight - 1)) + 1;
        var weight = 0;
        var index = 0;
        while (weight < random) {
            weight += bead.imageFrequencies[index++];
        }
        url = bead.idString + "/" + bead.idString + "-" + index + "-256";
    }
    var picture = $('<picture></picture>');
    picture.append($('<source srcset="/images/components/' + url + '.webp" style="width:' + size + 'px;" type="image/webp">'));
    picture.append($('<source srcset="/images/components/' + url + '.png" style="width:' + size + 'px;" type="image/png">'));
    picture.append($('<img src="/images/components/' + url + '.png" style="width:' + size + 'px;">'));
    $(".bracelet-preview-beads").append(picture);
    $(".bracelet-preview-shadow").append('<img src="/images/shadow.png" style="width:' + size + 'px"/>');
}

function getExtraComponents() {
    var braceletContent = $(".bracelet-text-input").html().trim().replace(" ", "");
    var extraComponents = [];
    var inImage = false;
    var inImageUrl = false;
    var imageUrl = '';
    for (var i = 0; i < braceletContent.length; i++) {
        if (braceletContent[i] == '<') inImage = true;
        if (!inImage) {
            var letters = showWhiteLetters ? whiteLetters : blackLetters;
            if (isElementInList(braceletContent[i].toUpperCase(), validCharacters)) {
                var letter = letters.filter(function (e) { return e.letter == braceletContent[i].toUpperCase() })[0];
                extraComponents.push({ id: letter.id, url: (showWhiteLetters ? "feher-" : "fekete-") + letter.letter.toLowerCase(), isLetter: true });
            }
        }
        else {
            if (!inImageUrl && braceletContent[i] == '"') {
                inImageUrl = true;
                imageUrl = '';
            }
            else if (inImageUrl) {
                if (braceletContent[i] == '"') {
                    if (imageUrl.indexOf(".png") > 0) {
                        imageUrl = imageUrl.split("/");
                        imageUrl = imageUrl[imageUrl.length - 1].replace(".png", "");
                        var extra = getExtraByImageUrl(imageUrl);
                        extraComponents.push(extra);
                    }
                    inImageUrl = false;
                    imageUrl = '';
                }
                else {
                    imageUrl += braceletContent[i];
                }
            }
        }
        if (braceletContent[i] == '>') inImage = false;
    }
    return extraComponents;
}

function isElementInList(element, list) {
    for (var i = 0; i < list.length; i++) {
        if (list[i] == element) {
            return true;
        }
    }
    return false;
}

function getExtraByImageUrl(imageUrl) {
    for (var i = 0; i < extras.length; i++) {
        if (extras[i].url == imageUrl) {
            return extras[i];
        }
    }
}