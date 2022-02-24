var GLSPSMap = function () { };

GLSPSMap.prototype = function () {
    var allowedLangCtrCodes = ['CZ', 'HU', 'EN', 'RO', 'SI', 'SK', 'HR'];
    var ajaxUrl = '//online.gls-hungary.com/psmap/psmap_getdata.php';

    var homeLoc = null;
    var map = null;
    var psData = [];
    var lngData = [];
    var psData_OnMap = [];
    var markers = [];
    var selectedID = '';
    var filter = '';
    var txtSearchPS = '';
    var _ctrcode = '';
    var _senderid = null;
    var _pclshopin = null;
    var _parcellockin = null;
    var _isDropOff = null;
    var _initAddress = null;
    var _codhandler = null;

    var countries = [];
    countries["HU"] = "Hungary";
    countries["HR"] = "Croatia";
    countries["CZ"] = "Czech Republic";
    countries["SK"] = "Slovakia";
    countries["SI"] = "Slovenia";
    countries["RO"] = "Romania";

    var init = function (ctrcode, containerID, address, isDropOff, senderid, pclshopin, parcellockin, codhandler) {
        // set variables to init values
        destroy();

        // container for the map
        var container = $('#' + containerID);

        // prepare start address
        if (isEmpty(address)) {
            switch (ctrcode.toUpperCase()) {
                case 'HU':
                    address = 'Budapest, HU';
                    break;
                case 'SK':
                    address = 'Bratislava, SK';
                    break;
                case 'CZ':
                    address = 'Praha, CZ';
                    break;
                case 'RO':
                    address = 'Bucuresti, RO';
                    break;
                case 'SI':
                    address = 'Ljubljana, SI';
                    break;
                case 'HR':
                    address = 'Zagreb, HR';
                    break;
            }
        }
        else {
            // remove name of recipient from address
            editedAddress = address.split(',');
            address = editedAddress.slice(1).join();
        }

        _ctrcode = ctrcode;
        _isDropOff = isDropOff;
        _initAddress = address;

        if (!isEmpty(senderid)) {
            _senderid = senderid;
        }
        if (!isEmpty(pclshopin)) {
            _pclshopin = pclshopin;
        }
        if (!isEmpty(parcellockin)) {
            _parcellockin = parcellockin;
        }
        if (!isEmpty(codhandler)) {
            _codhandler = codhandler;
        }

        switch (ctrcode.toUpperCase()) {
            case 'HU':
                ajaxUrl = '//online.gls-hungary.com/psmap/psmap_getdata.php';
                break;
            case 'SK':
                ajaxUrl = '//online.gls-slovakia.sk/psmap/psmap_getdata.php';
                break;
            case 'CZ':
                ajaxUrl = '//online.gls-czech.com/psmap/psmap_getdata.php';
                break;
            case 'RO':
                ajaxUrl = '//online.gls-romania.ro/psmap/psmap_getdata.php';
                break;
            case 'SI':
                ajaxUrl = '//online.gls-slovenia.com/psmap/psmap_getdata.php';
                break;
            case 'HR':
                ajaxUrl = '//online.gls-croatia.com/psmap/psmap_getdata.php';
                break;
        }

        // try get lng from url
        var lngFromUrl = getLanguageFromUrl();

        if ($.inArray(lngFromUrl, allowedLangCtrCodes) !== -1) {
            var lngCtrCode = lngFromUrl;
        }
        else {
            var lngCtrCode = ctrcode;
        }

        // load text "Find ParcelShop"
        crossDomainAjax({
            action: 'getLng2',
            country: lngCtrCode.toLowerCase()
        },
            function (data) {
                lngData = getLanguageArray(data);
            }
        );

        // prepare HTML
        container.append($(document.createElement('div'))
                            .prop('id', 'left-canvas')
                            .append($(document.createElement('input'))
                                    .prop('id', 'searchinput')
                                    .prop('type', 'text')
                                    .addClass('default')
                                    .val(lngData.searchPS)
                                    .on('focus', searchinput_onfocus)   // get focus
                                    .on('blur', searchinput_onblur)     // lost focus
                                    .on('keyup', searchinput_onkeyup)   // key press
                                    )
                            .append($(document.createElement('div'))
                                .prop('id', 'psitems-canvas')))
                .append($(document.createElement('div'))
                            .prop('id', 'right-canvas')
                            .append($(document.createElement('div'))
                                .prop('id', 'map-canvas')));

        // load list of ParcelShops
        crossDomainAjax({
            action: 'getList',
            dropoff: isDropOff
        },
            function (data) {
                for (var i = 0; i < data.length; i++) {
                    if (containsArrayTheParcelShop(psData, data[i]) === false) {
                        psData.push(data[i]);
                    }
                }
            }
        );

        // start map
        map = new google.maps.Map(document.getElementById('map-canvas'), { zoom: 12 });
        // set listenner for the map
        google.maps.event.addListener(map, 'dragend', updateLeftSide);
        google.maps.event.addListener(map, 'zoom_changed', updateLeftSide);

        // show parcelshops on the map
        if (!isEmpty(psData)) {
            addMarkers();
        }

        // address initializing and show on the map
        initAddress(address);
    }

    // clear all variables and set the to default values
    var destroy = function () {
        homeLoc = null;
        map = null;
        psData = [];
        lngData = [];
        psData_OnMap = [];
        markers = [];
        selectedID = '';
        filter = '';
        txtSearchPS = '';
        _ctrcode = '';
        _senderid = null;
        _pclshopin = null;
        _parcellockin = null;
        _isDropOff = null;
        _initAddress = null;
        _codhandler = null;
    }

    // initialization of address after start application
    var initAddress = function (address) {
        /* declare the google geocoder api */
        var geocoder = new google.maps.Geocoder();

        if (address.replace(" ", "") == parseInt(address.replace(" ", ""), 10))
        {            
            address += "," + countries[_ctrcode];        
        }

        address += "," + _ctrcode.toLowerCase();

        /* add the address and check that it is existing -> callback function */
        geocoder.geocode({ address: address }, addressFound);
    }

    // found address, set marker, set zoom for min. 5 visible PS
    var addressFound = function (results, status) {
        if (status === google.maps.GeocoderStatus.OK) {
            // if is set that remove old marker from map
            if (homeLoc != null) {
                homeLoc.setMap(null);
            }

            var marker = new google.maps.Marker({
                map: map,
                position: results[0].geometry.location
            });

            map.setCenter(results[0].geometry.location);
            homeLoc = marker;

            // place marker with animation
            marker.setAnimation(google.maps.Animation.DROP);

            // set zoom - 5 PS must be visible
            setVisiblyParcelShop(5, psData, psData_OnMap, map);
            updateLeftSide();
        }
    }

    // ???
    var refreshOnMapDataFilter = function () {
        if (!psData) {
            return;
        }

        var bounds = map.getBounds();
        var result = [];
        for (var ii = 0; ii < psData.length; ii++) {
            if (bounds.contains(new google.maps.LatLng(psData[ii]['geolat'], psData[ii]['geolng']))) {
                if (filter === ''
                    || checkFilter(psData[ii].name)
                    || checkFilter(psData[ii].ctrcode)
                    || checkFilter(psData[ii].zipcode)
                    || checkFilter(psData[ii].city)
                    || checkFilter(psData[ii].address)) {
                    result.push(psData[ii]);
                }
            }
        }

        psData_OnMap = result;
    }

    /* check the filtered data */
    var checkFilter = function (tocheck) {
        return prepareForFilter(tocheck).indexOf(filter) > -1;
    }

    /* typed address spec char check */
    var prepareForFilter = function (expression) {
        return expression.toLowerCase()
                        .replace(new RegExp("\\s", 'g'), "")
                        .replace(new RegExp("[àáâãäå]", 'g'), "a")
                        .replace(new RegExp("æ", 'g'), "ae")
                        .replace(new RegExp("ç", 'g'), "c")
                        .replace(new RegExp("[èéêëě]", 'g'), "e")
                        .replace(new RegExp("[ìíîï]", 'g'), "i")
                        .replace(new RegExp("ñ", 'g'), "n")
                        .replace(new RegExp("[òóôõöő]", 'g'), "o")
                        .replace(new RegExp("œ", 'g'), "oe")
                        .replace(new RegExp("[ùúûüűů]", 'g'), "u")
                        .replace(new RegExp("[ýÿ]", 'g'), "y")
                        .replace(new RegExp("š", 'g'), "s")
                        .replace(new RegExp("č", 'g'), "c")
                        .replace(new RegExp("ř", 'g'), "r")
                        .replace(new RegExp("ž", 'g'), "z")
                        .replace(new RegExp("ť", 'g'), "t")
                        .replace(new RegExp("ď", 'g'), "d")
                        .replace(new RegExp("ľ", 'g'), "l")
                        .replace(new RegExp("\\W", 'g'), "");
    }

    // add markers on the map with title
    var addMarkers = function () {
        for (var index = 0; index < psData.length; index++) {

            if (psData[index].isparcellocker === "t") {
                var iconPath = '//online.gls-hungary.com/img/icon_parcellocker_hu.png';
            }
            else {
                var iconPath = '//online.gls-hungary.com/img/icon_paketshop50x38_' + ((_ctrcode.toLowerCase() == 'hu') ? 'hu' : 'en') + '.png';
            }

            var marker = new google.maps.Marker({
                map: map,
                title: psData[index]['name']
                        + "\r" + psData[index]['address']
                        + "\r" + psData[index]['zipcode'] + ' ' + psData[index]['city']
                        + "\r" + $.trim(psData[index]['contact'] + ' ' + psData[index]['phone']),
                icon: iconPath,
                position: new google.maps.LatLng(psData[index]['geolat'], psData[index]['geolng'])
            });

            markers[psData[index]['pclshopid']] = { marker: marker };
            addMarkerEvents(marker, psData[index]['pclshopid']);
        }
    }

    // reset visibility of markers 
    var refreshMarkersVisibility = function () {
        var found = false;

        for (var index = 0; index < psData.length; index++) {
            found = false;
            for (var i = 0; i < psData_OnMap.length; i++) {
                if (psData_OnMap[i]['pclshopid'] === psData[index]['pclshopid']) {
                    found = true;
                    break;
                }
            }

            if (!isEmpty(psData[index]['pclshopid']) && !isEmpty(psData[index]['pclshopid'].marker)) {
                markers[psData[index]['pclshopid']].marker.setVisible(found);
            }
        }
    }

    // add event for marker
    var addMarkerEvents = function (marker, id) {
        google.maps.event.addListener(marker, 'click',
            function () {
                highlightMarker(marker, id, true);
            });
    }

    // change of bounds
    var updateLeftSide = function () {
        refreshOnMapDataFilter();
        psData_OnMap = getVisibleParcelShops(map, psData);

        countDistances(homeLoc, psData_OnMap);
        sortByDistance(psData_OnMap);

        loadPSDataItems();
    }

    // show data of visible ParcelShops on the left side
    var loadPSDataItems = function () {
        // clear left side
        $('#psitems-canvas').html('');

        // fill left side
        $.each(psData_OnMap, function () {
            var redTextHtml = '';
            var holidayTxt = '';

            // has holiday or not?
            if (!isEmpty(this['holidaystarts']) && !isEmpty(this['holidayends'])) {
                if (this['holidaystarts'] == this['holidayends']) {
                    holidayTxt = lngData.holiday + ": " + formatDate(_ctrcode, this['holidayends']);
                }
                else {
                    holidayTxt = lngData.holiday + ": " + formatDate(_ctrcode, this['holidaystarts']) + " - " + formatDate(_ctrcode, this['holidayends']);
                }
            }

            // show red text (holiday or dropoffpoint)?
            if (this['dropoffpoint'] == '' || this['dropoffpoint'] == 'f' || holidayTxt != '') {
                var redText = (holidayTxt != '') ? holidayTxt : lngData.dropOff;
                redTextHtml = '<br/> <span style="color: red;">' + redText + '</span>';
            }

            /* ps map left side PS data list */
            var markerDiv = $(document.createElement('div'))
                .prop('id', this.pclshopid)
                /* add the distance to the title */
                .prop('title', formatDistance(this.distance))
                .on('mouseover', { obj: this }, function (event) {
                    if (!$(this).hasClass('psSelected')) {
                        deSelectItems(event.data.obj.pclshopid);
                        if (!isEmpty(markers[event.data.obj.pclshopid]) && !isEmpty(markers[event.data.obj.pclshopid].div)) {
                            markers[event.data.obj.pclshopid].div.addClass('psOver');
                        }
                    }
                })
                .on('mouseout', { obj: this }, function (event) {
                    if (!$(this).hasClass('psSelected')) {
                        if (!isEmpty(markers[event.data.obj.pclshopid]) && !isEmpty(markers[event.data.obj.pclshopid].div)) {
                            markers[event.data.obj.pclshopid].div.removeClass('psOver');
                        }
                    }
                })
                .on('click', { obj: this }, function (event) {
                    highlightMarker(markers[event.data.obj.pclshopid].marker,
                        event.data.obj.pclshopid);
                })
                .css('padding', '3px 2px')
                .css('cursor', 'pointer')
                .html(this['name']
                        + '<br/>' + this['zipcode'] + ' ' + this['city']
                        + '<br/>' + this['address']
                        + redTextHtml);

            markers[this.pclshopid].div = markerDiv;

            /* show the left div with the ps data */
            $('#psitems-canvas').append(markerDiv)
                .append($(document.createElement('div'))
                        .html('<hr style="float:left;width:98%;margin:0;"/>'))
                        .css('margin', '0')
                .append(
                    $(document.createElement('div'))
                        .css('clear', 'both'));
        });

        //Place holder
        $('#psitems-canvas').append($(document.createElement('div')).html('<br/><br/>'));
        //refreshMarkersVisibility();
    }

    // item was unselected
    var deSelectItems = function (id) {
        /* compare the key and id, selectedID values  */
        for (var key in markers) {
            if (key !== id && key !== selectedID) {
                if (markers[key] && markers[key].marker && markers[key].marker.animation) {
                    markers[key].marker.setAnimation(null);
                }

                $(markers[key].div).removeClass();
            }
        }
    }

    // marker will be jumping
    var highlightMarker = function (marker, id, scroll) {
        var markerdiv_exists = !isEmpty(markers[id]) && !isEmpty(markers[id].div);
        selectedID = id;

        deSelectItems(id);
        if (markerdiv_exists) {
            markers[id].div.addClass('psOver').addClass('psSelected');
        }

        marker.setAnimation(google.maps.Animation.BOUNCE);

        if (scroll && markerdiv_exists) {
            $('#psitems-canvas').animate(
            {
                scrollTop: $(markers[id].div).parent().scrollTop()
                            + $(markers[id].div).offset().top
                            - $(markers[id].div).parent().offset().top
            });
        }

        if (!isEmpty(glsPSMap_OnSelected_Handler) && glsPSMap_OnSelected_Handler) {
            returnSeleceted(id);
        }
    }

    // return detail of the parcelshop
    var returnSeleceted = function (id) {
        for (var i = 0; i < psData.length; i++) {
            if (psData[i]['pclshopid'] === id) {
                if (psData[i].openings) {
                    this.glsPSMap_OnSelected_Handler(psData[i]);
                }
                else {
                    crossDomainAjax(
                        {
                            action: 'getOpenings',
                            pclshopid: id
                        },
                        $.proxy(function (data) {
                            psData[i].openings = data;
                            this.glsPSMap_OnSelected_Handler(psData[i]);
                        }, this));
                }
                break;
            }
        }
    }

    // show/hide initialization text in search textbox
    var searchinput_onfocus = function () {
        var searchinput_DefaultText = lngData.searchPS;

        if (this.value === searchinput_DefaultText) {
            this.value = '';
            $(this).removeClass('default');
        }
    }

    /* searchinput_DefaultText get the defaultsearch text and it is putted into the functions */
    var searchinput_onblur = function () {
        if (this.value === '') {
            this.value = lngData.searchPS;
            $(this).addClass('default');
        }

        filter = '';
    }

    // event for typing in textbox
    var searchinput_onkeyup = function () {
        if (filter === this.value) {
            return;
        }

        if ($.trim(this.value) === '' || this.value === lngData.searchPS) {
            filter = '';
        }
        else {
            filter = prepareForFilter(this.value);
        }

        if (filter != '') {
            initAddress(filter);
        }

        /* i set empty the filter to the */
        filter = '';
    }


    // ajax function
    function crossDomainAjax(data, successCallback) {
        var dataToSend = '?ctrcode=' + _ctrcode;
        $.each(data, function (key, value) {
            dataToSend += '&' + key + '=' + value;
        });

        // Extend list query string with optional parameters
        if (data.action == 'getList') {
            if (!isEmpty(_senderid)) {
                dataToSend += '&' + 'senderid' + '=' + _senderid;
            }
            if (!isEmpty(_pclshopin)) {
                dataToSend += '&' + 'pclshopin' + '=' + _pclshopin;
            }
            if (!isEmpty(_parcellockin)) {
                dataToSend += '&' + 'parcellockin' + '=' + _parcellockin;
            }
            if (!isEmpty(_codhandler)) {
                dataToSend += '&' + 'codhandler' + '=' + _codhandler;
            }
        }

        // IE8 & 9 only Cross domain JSON GET request
        if ('XDomainRequest' in window && window.XDomainRequest !== null) {
            var xdr = new XDomainRequest(); // Use Microsoft XDR
            xdr.open('get', ajaxUrl + dataToSend);
            xdr.onload = function () {
                var dom = new ActiveXObject('Microsoft.XMLDOM'),
                JSON = $.parseJSON(xdr.responseText);

                dom.async = false;

                if (JSON == null || typeof (JSON) == 'undefined') {
                    JSON = $.parseJSON(data.firstChild.textContent);
                }

                successCallback(JSON); // internal function
            };

            xdr.onerror = function () {
                _result = false;
            };

            xdr.send();
        }
            // IE7 and lower can't do cross domain
        else if (navigator.userAgent.indexOf('MSIE') != -1 && parseInt(navigator.userAgent.match(/MSIE ([\d.]+)/)[1], 10) < 8) {
            return false;
        }
            // Do normal jQuery AJAX for everything else          
        else {
            $.ajax({
                url: ajaxUrl + dataToSend,
                cache: false,
                dataType: 'json',
                type: 'GET',
                async: false, // must be set to false
                success: function (data, success) {
                    successCallback(data);
                }
            });
        }
    }


    function setParam(key, value) {
        switch (key) {
            case 'pclshopin':
                _pclshopin = value;
                return true;
                break;
            case 'parcellockin':
                _parcellockin = value;
                return true;
                break;
            default:
                return false;
                break;
        }
    }

    // load data and show on map via markers
    var loadList = function () {
        crossDomainAjax({
            action: 'getList',
            dropoff: _isDropOff
        },
        function (data) {
            psData = data;
            addMarkers();
        });

        initAddress(_initAddress);
    }

    return {
        init: init,
        initAddress: initAddress,
        setParam: setParam,
        reloadList: loadList
    };
}();

// function return distance between two points on the map (air distance)
var getDistance = function (pointA, pointB) {
    var rad = function (x) {
        return x * Math.PI / 180;
    };

    var R = 6378137;
    var dLat = rad(pointB.lat() - pointA.lat());
    var dLong = rad(pointB.lng() - pointA.lng());
    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) + Math.cos(rad(pointA.lat())) * Math.cos(rad(pointB.lat())) * Math.sin(dLong / 2) * Math.sin(dLong / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var distance = R * c;

    return distance; // result in meters
};

// used bubble sort for sort array by distance
var sortByDistance = function (listOfPs) {
    var swapped, i, temp;

    do {
        swapped = false;
        for (i = 0; i < listOfPs.length - 1; i++) {
            if (listOfPs[i]['distance'] > listOfPs[i + 1]['distance']) {
                temp = listOfPs[i];
                listOfPs[i] = listOfPs[i + 1];
                listOfPs[i + 1] = temp;
                swapped = true;
            }
        }
    } while (swapped);
}

// count distance for all PS in list
var countDistances = function (marker, listOfPS) {
    /* count air distance from address to PS for each PS */
    if (marker != null) {
        var pointA = new google.maps.LatLng(marker.getPosition().lat(), marker.getPosition().lng());
        var distance;

        for (var i = 0; i < listOfPS.length; i++) {
            if (!isEmpty(listOfPS[i]['geolat']) && !isEmpty(listOfPS[i]['geolng'])) {
                var pointB = new google.maps.LatLng(listOfPS[i]['geolat'], listOfPS[i]['geolng']);

                listOfPS[i]['distance'] = getDistance(pointA, pointB);
            }
        }
    }
}

// format date
var formatDistance = function (distance) {
    if (distance > 1000) {
        return Math.round((distance / 1000) * 10) / 10 + " km";
    }
    else {
        return Math.round(distance) + " m";
    }
}

// edit zoom for visibility selected number of Shops
var setVisiblyParcelShop = function (count, listOfParcelShops, visibleParcelShops, map) {
    var zoom = 20; // max zoom
    var position;

    visibleParcelShops = [];
    map.setZoom(zoom);

    do {
        map.setZoom(--zoom);
        visibleParcelShops = getVisibleParcelShops(map, listOfParcelShops);
    } while (visibleParcelShops.length < count && zoom > 5);
}

// return parcelshops which are visible on the map
var getVisibleParcelShops = function (map, listOfPS) {
    result = [];

    for (var i = 0; i < listOfPS.length; i++) {
        position = new google.maps.LatLng(listOfPS[i]['geolat'], listOfPS[i]['geolng']); // get position for next using

        // is PS visible on the map?
        if (map.getBounds().contains(position)) {
            // is in the array of visible PS?
            if (containsArrayTheParcelShop(result, listOfPS[i]) === false) {
                result.push(listOfPS[i]);
            }
        }
    }

    return result;
}

var containsArrayTheParcelShop = function (arrayObjects, testObject) {
    for (var i = 0; i < arrayObjects.length; i++) {
        if (arrayObjects[i].pclshopid === testObject.pclshopid) {
            return true;
            break;
        }
    }

    return false;
}

// return ctrcode of set language from url
var getLanguageFromUrl = function () {
    var getVariables = $(location).attr('search');  // get all variables "?example=value&example2=value2
    var langStart = getVariables.search("lang=");   // search start position of variable lang in the string

    // return substring - country code only
    return getVariables.substring(langStart + 5, 8);
}

// formatting date by country
var formatDate = function (country, date) {
    if (country.toUpperCase() === 'HU') {
        return date.replace(/\-/g, '.');
    }
    else {
        var arr = date.split('-').reverse();
        return arr.join('.');
    }
}

//
var getLanguageArray = function (data) {
    var defaultTexts = {
        holiday: "Holiday",
        searchPS: "Search ParcelShop ...",
        dropOff: "Only dispatch!"
    };

    if (isEmpty(data) || isEmpty(data[0]) || data == false) {
        return defaultTexts;
    }
    else {
        $.each(data, function () {
            switch (this['id']) {
                case 'lbl_psmap_search':
                    defaultTexts.searchPS = this['txt'];
                    break;
                case 'txt_holiday':
                    defaultTexts.holiday = this['txt'];
                    break;
                case 'txt_dropOffPoint':
                    defaultTexts.dropOff = this['txt'];
                    break;
            }
        });

        return defaultTexts;
    }
}

// don't remove!!! is necessary for customers solution
var isEmpty = function (obj) {
    if (typeof obj == 'undefined' || obj === null || obj === '') return true;
    if (typeof obj == 'number' && isNaN(obj)) return true;
    if (obj instanceof Date && isNaN(Number(obj))) return true;
    return false;
}