var buttonManyStringsType = $("#many-strings-type-button");
var buttonBraidedType = $("#braided-type-button");
var activeColorSelector = null;

$(() => {
    $(".many-strings-marker-flaps").click(() => toggleColorSelectors("many-strings-flaps"));
    $(".many-strings-marker-knot").click(() => toggleColorSelectors("many-strings-knot"));
    $(".many-strings-marker-lines").click(() => toggleColorSelectors("many-strings-lines"));
    $(".braided-marker-flaps").click(() => toggleColorSelectors("braided-flaps"));
    $(".braided-marker-knot").click(() => toggleColorSelectors("braided-knot"));
    $(".braided-marker-lines").click(() => toggleColorSelectors("braided-lines"));
    $(".color-selector").on("click", (event) => selectColor(event));
    buttonManyStringsType = $("#many-strings-type-button");
    buttonBraidedType = $("#braided-type-button");
    buttonManyStringsType.click(() => switchToManyStringsBracelet());
    buttonBraidedType.click(() => switchToBraidedBracelet());
    $(".add-to-cart-button-string-bracelet").click(() => addToCartStringBracelet());
    $("#designer-braided-svg").css("height", "0px");
    toggleColorSelectors("many-strings-lines");
});

function StringBracelet() {
    this.colors = [];
    this.braceletType = 1;

    this.highlightSelectedColors = function(colorSelector) {
        if (this.colors[colorSelector] == null) return;
        let colorSelectors = $(".color-selector");
        for (let i = 0; i < this.colors[colorSelector].length; i++) {
            for (let j = 0; j < colorSelectors.length; j++) {
                if ($(colorSelectors[j]).css("background-color") == this.colors[colorSelector][i]) {
                    $(colorSelectors[j]).addClass("color-selector-selected");
                }
            }
        }
    }

    this.addColor = function(type, color) {
        if (this.colors[type] == null) {
            this.colors[type] = [];
        }
        if (this.colors[type].filter(c => c == color).length == 0) {
            this.colors[type].push(color);
        }
    }

    this.removeColor = function(type, color) {
        if (this.colors[type] == null) return;
        this.colors[type] = this.colors[type].filter(c => c != color);
    }

    this.applyColors = function() {
        this.applyKnotColor();
        this.applyLineColors();
        this.applyFlapsColors();
    }

    this.applyKnotColor = function() {
        let braceletTypeString = this.braceletType == 2 ? "braided" : "many-strings";
        let elementId = braceletTypeString + "-knot";
        let color = "#ffffff";
        if (this.colors[elementId] != null && this.colors[elementId].length > 0) {
            color = this.colors[elementId][0];
        }
        this.applyElementColor(braceletTypeString, ["knot"], color);
    }

    this.applyLineColors = function() {
        let braceletTypeString = this.braceletType == 2 ? "braided" : "many-strings";
        let colorsList = this.colors[braceletTypeString + "-lines"];
        if (this.braceletType == 2) {
            if (colorsList == null || colorsList.length == 0) {
                this.applyElementColor(braceletTypeString, ["line-1", "line-2", "line-3"], "#ffffff");
            }
            else if (colorsList.length == 1) {
                this.applyElementColor(braceletTypeString, ["line-1", "line-2", "line-3"], colorsList[0]);
            }
            else if (colorsList.length == 2) {
                this.applyElementColor(braceletTypeString, ["line-1"], colorsList[1]);
                this.applyElementColor(braceletTypeString, ["line-2", "line-3"], colorsList[0]);
            }
            else {
                this.applyElementColor(braceletTypeString, ["line-1"], colorsList[2]);
                this.applyElementColor(braceletTypeString, ["line-2"], colorsList[0]);
                this.applyElementColor(braceletTypeString, ["line-3"], colorsList[1]);
            }
        }
        else {
            if (colorsList == null || colorsList.length == 0) {
                this.applyElementColor(braceletTypeString, ["line-1", "line-2", "line-3", "line-4", "line-5", "line-6"], "#ffffff");
            }
            else if (colorsList.length == 1) {
                this.applyElementColor(braceletTypeString, ["line-1", "line-2", "line-3", "line-4", "line-5", "line-6"], colorsList[0]);
            }
            else if (colorsList.length == 2) {
                this.applyElementColor(braceletTypeString, ["line-1", "line-4", "line-5"], colorsList[1]);
                this.applyElementColor(braceletTypeString, ["line-2", "line-3", "line-6"], colorsList[0]);
            }
            else {
                this.applyElementColor(braceletTypeString, ["line-1", "line-5"], colorsList[2]);
                this.applyElementColor(braceletTypeString, ["line-2", "line-6"], colorsList[0]);
                this.applyElementColor(braceletTypeString, ["line-3", "line-4"], colorsList[1]);
            }
        }
    }

    this.applyFlapsColors = function() {
        let braceletTypeString = this.braceletType == 2 ? "braided" : "many-strings";
        let colorsList = this.colors[braceletTypeString + "-flaps"];
        if (colorsList == null || colorsList.length == 0) {
            this.applyElementColor(braceletTypeString, ["flap-1", "flap-2"], "#ffffff");
        }
        else if (colorsList.length == 1) {
            this.applyElementColor(braceletTypeString, ["flap-1", "flap-2"], colorsList[0]);
        }
        else if (colorsList.length == 2) {
            this.applyElementColor(braceletTypeString, ["flap-2"], colorsList[1]);
            this.applyElementColor(braceletTypeString, ["flap-1"], colorsList[0]);
        }
    }

    this.applyElementColor = function(idPrefix, ids, color) {
        for (let i = 0; i < ids.length; i++) {
            let element = document.getElementById("designer-" + idPrefix + "-" + ids[i]);
            element.setAttribute("fill", color);
        }
    }

    this.getColor = function(type, number) {
        var defaultValue = "#ffffff";
        if (number > 0) defaultValue = null;
        if (this.colors[type] && this.colors[type].length > number) return this.colors[type][number];
        return defaultValue;
    }
}

var bracelet = new StringBracelet();

function toggleColorSelectors(colorSelector) {
    if (activeColorSelector == colorSelector) return;
    activeColorSelector = colorSelector;
    $(".marker-dot-selected").removeClass("marker-dot-selected");
    $("." + colorSelector).addClass("marker-dot-selected");
    if ($(".color-selectors-wrapper").css("visibility") == "hidden") {
        $(".color-selectors-wrapper").css("visibility", "visible");
    }
    const selectorDescriptionElement = $(".color-selectors-description");
    if (activeColorSelector == "many-strings-flaps" || activeColorSelector == "braided-flaps") {
        selectorDescriptionElement.html("Válasszd ki az állítható szálak színét!<br />1 vagy 2 szín");
    }
    else if (activeColorSelector == "many-strings-knot" || activeColorSelector == "braided-know") {
        selectorDescriptionElement.html("Válasszd ki a csomó színét!<br /> 1 szín");
    }
    else if (activeColorSelector == "many-strings-lines" || activeColorSelector == "braided-lines") {
        selectorDescriptionElement.html("Válasszd ki az fő szálak színét!<br />1, 2 vagy 3 szín!");
    }
    $(".color-selector-selected").removeClass("color-selector-selected");
    bracelet.highlightSelectedColors(colorSelector);
}

function selectColor(element) {
    let selectedColorElements = $(".color-selector-selected");
    if ($(element.target).hasClass("color-selector-selected")) {
        $(element.target).removeClass("color-selector-selected");
        bracelet.removeColor(activeColorSelector, $(element.target).css("background-color"));
    }
    else {
        if (selectedColorElements.length < getMaximumNumberOfColors(activeColorSelector)) {
            $(element.target).addClass("color-selector-selected");
            bracelet.addColor(activeColorSelector, $(element.target).css("background-color"));
        }
        else return;
    }

    bracelet.applyColors();
}

function getMaximumNumberOfColors(selectorType) {
    if (selectorType == "many-strings-flaps") return 2;
    if (selectorType == "many-strings-knot") return 1;
    if (selectorType == "many-strings-lines") return 3;
    if (selectorType == "braided-flaps") return 2;
    if (selectorType == "braided-knot") return 1;
    if (selectorType == "braided-lines") return 3;
    return 0;
}

function switchToBraidedBracelet() {
    if (bracelet.braceletType == 2) return;
    bracelet.braceletType = 2;
    buttonManyStringsType.removeClass("selected-bracelet-type");
    buttonBraidedType.addClass("selected-bracelet-type");
    $("#designer-many-strings-svg").css("height", "0px");
    $(".many-strings-marker-flaps").hide();
    $(".many-strings-marker-lines").hide();
    $(".many-strings-marker-knot").hide();
    $("#designer-braided-svg").css("height", "auto");
    $(".braided-marker-flaps").show();
    $(".braided-marker-lines").show();
    $(".braided-marker-knot").show();
    toggleColorSelectors("braided-flaps");
}

function switchToManyStringsBracelet() {
    if (bracelet.braceletType == 1) return;
    bracelet.braceletType = 1;
    buttonManyStringsType.addClass("selected-bracelet-type");
    buttonBraidedType.removeClass("selected-bracelet-type");
    $("#designer-braided-svg").css("height", "0px");
    $(".braided-marker-flaps").hide();
    $(".braided-marker-lines").hide();
    $(".braided-marker-knot").hide();
    $("#designer-many-strings-svg").css("height", "auto");
    $(".many-strings-marker-flaps").show();
    $(".many-strings-marker-lines").show();
    $(".many-strings-marker-knot").show();
    $(".color-selectors-wrapper").css("visibility", "hidden");
    toggleColorSelectors("many-strings-flaps");
}

function addToCartStringBracelet() {
var prefix = "many-strings";
    if (bracelet.braceletType == 2) prefix = "braided";
    var model = {
        braceletType: bracelet.braceletType,
        knotColor: bracelet.getColor(prefix + "-knot", 0),
        string1Color: bracelet.getColor(prefix + "-lines", 0),
        string2Color: bracelet.getColor(prefix + "-lines", 1),
        string3Color: bracelet.getColor(prefix + "-lines", 2),
        flap1Color: bracelet.getColor(prefix + "-flaps", 0),
        flap2Color: bracelet.getColor(prefix + "-flaps", 1)
    };
    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push({
        event: 'addToCart',
        ecommerce: {
            currencyCode: 'HUF',
            add: {
                products: [{
                    id: 'string-bracelet',
                    name: 'Fonott karkötő',
                    category: 'Fonott karkötő',
                    price: '3490'
                }]
            }
        }
    });

    window.onbeforeunload = undefined;
    $.ajax({
        url: "/fonott-karkoto-keszito/kosarba",
        method: "POST",
        data: model,
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


