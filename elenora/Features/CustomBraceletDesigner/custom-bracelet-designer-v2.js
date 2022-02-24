var container;
var currentItems = [];

$(function () {
    InitializeVariables();
    AddNewMessage(CBD_V2_SALUTATION);
    AddNewMessage(CBD_V2_WELCOME);
    AddNewMessage(CBD_V2_WELCOME2);
    AddNewMessage(CBD_V2_EXAMPLE_IMAGES);
    AddExampleImagesWidget();
    AddNewMessage(CBD_V2_START_DESCRIPTION);
})

function InitializeVariables() {
    container = $('#cbd-v2-container');
}

function AddNewMessage(messageText) {
    var message = $('<div class="cbd-v2-message-container"><div class= "cbd-v2-message">' + messageText + '</div></div>');
    currentItems.push({ message });
    container.append(message);
}

function AddExampleImagesWidget() {
    var carousel = $('<div id="cbd-v2-example-images-container" class="owl-carousel owl-theme">');
    for (var i = 0; i < exampleImages.length; i++) {
        carousel.append($('<img class="cbd-v2-example-images-item" src="' + exampleImages[i].src + '" alt="' + exampleImages[i].title + '" title="' + exampleImages[i].title + '">'));
    }
    currentItems.push({ carousel });
    container.append(carousel);
    carousel.owlCarousel({
        stagePadding: 0,
        loop: true,
        autoplay: false,
        autoplayTimeout: 3000,
        autoplayHoverPause: true,
        items: 2
    });
    carousel.trigger('play.owl.autoplay', [3000]);
}