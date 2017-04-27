/*
 * jQuery showLoading plugin v1.0
 *
 * Copyright (c) 2009 Jim Keller
 * Context - http://www.contextllc.com
 *
 * Dual licensed under the MIT and GPL licenses.
 *
 */

/*
 * jquery-show-loading v0.1.0
 *
 * Copyright (c) 2015 Slava Tyshkavets
 * https://github.com/pmstss/jquery-show-loading
 *
 * Sources initially were found here: http://stackoverflow.com/questions/15547573/why-jquery-loader-not-working
 *
 * Changes: fixed codestyle, adding AMD support, publishing as bower component, grunt for build.
 */

/*global jQuery*/

(function (factory) {
    'use strict';
    // UMD[2] wrapper for jQuery plugins to work in AMD or in CommonJS.
    //
    // [2] https://github.com/umdjs/umd

    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['jquery'], factory);
    } else if (typeof exports === 'object') {
        // Node/CommonJS
        module.exports = factory(require('jquery'));
    } else {
        // Browser globals
        factory(jQuery);
    }
}(function ($) {
    'use strict';

    $.fn.showLoading = function (options) {
        var settings = {
            indicatorID: '',
            addClass: '',
            beforeShow: null,
            afterShow: null,
            hPos: 'center',
            vPos: 'center',
            indicatorZIndex: 5001,
            overlayZIndex: 5000,
            parent: '',
            marginTop: 0,
            marginLeft: 0,
            overlayWidth: null,
            overlayHeight: null,
            overlayCSS: ''
        };

        $.extend(settings, options);

        var $loadingDiv = $('<div></div>');
        var $overlayDiv = $('<div></div>');

        // Set up ID and classes
        var indicatorID = settings.indicatorID || $(this).attr('id');

        if ($('#loading-indicator-' + indicatorID).length) {
            return this;
        }

        $loadingDiv.attr('id', 'loading-indicator-' + indicatorID);
        $loadingDiv.addClass('loading-indicator');

        if (settings.addClass) {
            $loadingDiv.addClass(settings.addClass);
        }

        // Create the overlay
        $overlayDiv.css('display', 'none');

        // Append to body, otherwise position() doesn't work on Webkit-based browsers
        $(document.body).append($overlayDiv);

        // Set overlay classes
        $overlayDiv.attr('id', 'loading-indicator-' + indicatorID + '-overlay');
        $overlayDiv.addClass('loading-indicator-overlay');
        if (settings.addClass) {
            $overlayDiv.addClass(settings.addClass + '-overlay');
        }

        // Set overlay position
        var borderTopWidth = $(this).css('border-top-width');
        var borderLeftWidth = $(this).css('border-left-width');

        // IE will return values like 'medium' as the default border, but we need a number
        borderTopWidth = isNaN(parseInt(borderTopWidth, 10)) ? 0 : borderTopWidth;
        borderLeftWidth = isNaN(parseInt(borderLeftWidth, 10)) ? 0 : borderLeftWidth;

        var overlayLeftPos = $(this).offset().left + parseInt(borderLeftWidth, 10);
        var overlayTopPos = $(this).offset().top + parseInt(borderTopWidth, 10);

        var overlayWidth = settings.overlayWidth || parseInt($(this).width(), 10) + parseInt($(this).css('padding-right'), 10) +
            parseInt($(this).css('padding-left'), 10);
        var overlayHeight = settings.overlayHeight || parseInt($(this).height(), 10) + parseInt($(this).css('padding-top'), 10) +
            parseInt($(this).css('padding-bottom'), 10);

        $overlayDiv.css('width', overlayWidth.toString() + 'px');
        $overlayDiv.css('height', overlayHeight.toString() + 'px');

        $overlayDiv.css('left', overlayLeftPos.toString() + 'px');
        $overlayDiv.css('position', 'absolute');

        $overlayDiv.css('top', overlayTopPos.toString() + 'px');
        $overlayDiv.css('z-index', settings.overlayZIndex);

        // Set any custom overlay CSS
        if (settings.overlayCSS) {
            $overlayDiv.css(settings.overlayCSS);
        }

        // We have to append the element to the body first
        // or .width() won't work in Webkit-based browsers (e.g. Chrome, Safari)
        $loadingDiv.css('display', 'none');
        $(document.body).append($loadingDiv);

        $loadingDiv.css('position', 'absolute');
        $loadingDiv.css('z-index', settings.indicatorZIndex);

        // Set top margin
        var indicatorTop = overlayTopPos;
        if (settings.marginTop) {
            indicatorTop += parseInt(settings.marginTop, 10);
        }

        var indicatorLeft = overlayLeftPos;
        if (settings.marginLeft) {
            indicatorLeft += parseInt(settings.marginTop, 10);
        }

        // set horizontal position
        if (settings.hPos.toString().toLowerCase() === 'center') {
            $loadingDiv.css('left', (indicatorLeft + ($overlayDiv.width() - parseInt($loadingDiv.width(), 10)) / 2).toString() + 'px');
        } else if (settings.hPos.toString().toLowerCase() === 'left') {
            $loadingDiv.css('left', (indicatorLeft + parseInt($overlayDiv.css('margin-left'), 10)).toString() + 'px');
        } else if (settings.hPos.toString().toLowerCase() === 'right') {
            $loadingDiv.css('left', (indicatorLeft + ($overlayDiv.width() - parseInt($loadingDiv.width(), 10))).toString() + 'px');
        } else {
            $loadingDiv.css('left', (indicatorLeft + parseInt(settings.hPos, 10)).toString() + 'px');
        }

        // set vertical position
        if (settings.vPos.toString().toLowerCase() === 'center') {
            $loadingDiv.css('top', (indicatorTop + ($overlayDiv.height() - parseInt($loadingDiv.height(), 10)) / 2).toString() + 'px');
        } else if (settings.vPos.toString().toLowerCase() === 'top') {
            $loadingDiv.css('top', indicatorTop.toString() + 'px');
        } else if (settings.vPos.toString().toLowerCase() === 'bottom') {
            $loadingDiv.css('top', (indicatorTop + ($overlayDiv.height() - parseInt($loadingDiv.height(), 10))).toString() + 'px');
        } else {
            $loadingDiv.css('top', (indicatorTop + parseInt(settings.vPos, 10)).toString() + 'px');
        }

        // Set any custom css for loading indicator
        if (settings.css) {
            $loadingDiv.css(settings.css);
        }

        // Set up callback options
        var callbackOptions = {
            'overlay': $overlayDiv,
            'indicator': $loadingDiv,
            'element': this
        };

        // beforeShow callback
        if (typeof settings.beforeShow === 'function') {
            settings.beforeShow(callbackOptions);
        }

        // Show the overlay
        $overlayDiv.show();

        // Show the loading indicator
        $loadingDiv.show();

        // afterShow callback
        if (typeof settings.afterShow === 'function') {
            settings.afterShow(callbackOptions);
        }

        var self = this;
        $loadingDiv.data('resizeAction', function () {
            $(self).hideLoading();
            $(self).showLoading();
        });

        $(window).resize($loadingDiv.data('resizeAction'));

        return this;
    };

    $.fn.hideLoading = function (options) {
        var settings = {};

        $.extend(settings, options);

        var indicatorID = settings.indicatorID || $(this).attr('id');

        var $loadingDiv = $(document.body).find('#loading-indicator-' + indicatorID);
        $(window).off('resize', $loadingDiv.data('resizeAction'));
        $loadingDiv.remove();

        $(document.body).find('#loading-indicator-' + indicatorID + '-overlay').remove();

        return this;
    };
}));