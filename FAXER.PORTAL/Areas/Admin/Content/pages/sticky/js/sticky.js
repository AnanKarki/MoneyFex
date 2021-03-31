'use strict';
$(document).ready(function() {
    var $window = $(window);
    if ($window.width() <= 767) {
        setWindowSticky();
    } else {
        setSticky();
    }
});
$(window).on('resize', function() {
    var $window = $(window);
    if ($window.width() <= 767) {
        setWindowSticky();
    } else {
        setSticky();
    }
});

function setWindowSticky() {
    $('#notes').postitall({
        content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.',
        width: 150,
        height: 200,
        posX: 50,
        posY: 200,
        style: {
            backgroundcolor: '#4680ff',
            textcolor: '#fff',
            fontfamily: 'Open Sans',
            fontsize: 'small',
            arrow: 'none',
        },
    });
    $('#idRunTheCode').click(function(e) {
        var a = $("#the_notes div.PIApostit").last().css("top");
        var b = $("#the_notes div.PIApostit").last().css("left");
        $.PostItAll.new({
            content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.',
            width: 150,
            height: 200,
            posX: parseInt(b, 10) + 10,
            posY: parseInt(a, 10),
            style: {
                backgroundcolor: '#4680ff',
                textcolor: '#fff',
                fontfamily: 'Open Sans',
                fontsize: 'small',
                arrow: 'none',
            },
        });
        e.preventDefault();
    });
    $.fn.postitall.globals = {
        prefix: '#PIApostit_',
        filter: 'domain',
        savable: false,
        randomColor: true,
        toolbar: true,
        autoHideToolBar: false,
        removable: true,
        askOnDelete: false,
        draggable: true,
        resizable: true,
        editable: true,
        changeoptions: false,
        blocked: false,
        minimized: true,
        expand: true,
        fixed: false,
        addNew: true,
        showInfo: false,
        pasteHtml: false,
        htmlEditor: false,
        autoPosition: true,
        export: false,
        addArrow: 'back'
    };
}

function setSticky() {
    $('#notes').postitall({
        content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.',
        width: 150,
        height: 200,
        posX: 50,
        posY: 200,
        style: {
            backgroundcolor: '#4680ff',
            textcolor: '#fff',
            fontfamily: 'Open Sans',
            fontsize: 'small',
            arrow: 'none',
        },
    });
    $('#notes1').postitall({
        content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.',
        width: 150,
        height: 200,
        posX: 230,
        posY: 200,
        style: {
            backgroundcolor: '#4680ff',
            textcolor: '#fff',
            fontfamily: 'Open Sans',
            fontsize: 'small',
            arrow: 'none',
        },
    });
    $('#idRunTheCode').click(function(e) {
        var a = $("#the_notes div.PIApostit").last().css("top");
        var b = $("#the_notes div.PIApostit").last().css("left");
        $.PostItAll.new({
            content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.',
            width: 150,
            height: 200,
            posX: parseInt(b, 10) + 10,
            posY: parseInt(a, 10),
            style: {
                backgroundcolor: '#4680ff',
                textcolor: '#fff',
                fontfamily: 'Open Sans',
                fontsize: 'small',
                arrow: 'none',
            },
        });
        e.preventDefault();
    });
    $.fn.postitall.globals = {
        prefix: '#PIApostit_',
        filter: 'domain',
        savable: false,
        randomColor: true,
        toolbar: true,
        autoHideToolBar: false,
        removable: true,
        askOnDelete: false,
        draggable: true,
        resizable: true,
        editable: true,
        changeoptions: false,
        blocked: false,
        minimized: true,
        expand: true,
        fixed: false,
        addNew: true,
        showInfo: false,
        pasteHtml: false,
        htmlEditor: false,
        autoPosition: true,
        export: false,
        addArrow: 'back'
    };
};