// Template slider jQuery script

$(document).on('ready', function() {
    "use strict";
    $(".slider").owlCarousel({
        navigation: false, // Show next and prev buttons
        slideSpeed: 1000,
        paginationSpeed: 3000,
          autoplayTimeout:2000,
        singleItem: true,
        pagination: false,
        loop:true,
        autoPlay: true,
        navigationText: ["<i class='fa fa-angle-left'></i>", "<i class='fa fa-angle-right'></i>"],
        addClassActive: true,
        animateOut: 'fadeOut',
         animateIn: 'fadeOut'
 

    });

});


$(document).on('ready', function() {
    "use strict";
   $('.testimonialhome').owlCarousel({
    loop:true,
     autoPlay:true,
    responsiveClass:true,
    items : 3,
      autoplayTimeout:1000,
      
    responsive:{
        0:{
            items:1,
            nav:true,
             autoPlay:true,
             
        },
        600:{
            items:1,
            nav:false,
             autoPlay:true,
            
        },
        1200:{
            items:3,
            nav:true,
            loop:false,
             autoPlay:true,
            
        }
    }
})

});

$(document).on('ready', function() {
    "use strict";
   $('.partnershome').owlCarousel({
    loop:true,
     autoPlay:true,
    responsiveClass:true,
    items : 6,
     autoplayTimeout:1000,
    responsive:{
        0:{
            items:1,
            nav:true,
             autoPlay:true,
             margin:30
        },
        600:{
            items:2,
            nav:false,
             autoPlay:true,
             margin:10
        },
        1200:{
            items:4,
            nav:true,
            loop:false,
             autoPlay:true,
             margin:30
        }
    }
})

});





$(document).on('ready', function() {
    "use strict";
   $('.bank-list').owlCarousel({
    loop:true,
     autoPlay:true,
    responsiveClass:true,
    items : 6,
     dot:true,
     autoplayTimeout:1000,
    responsive:{
        0:{
            items:1,
            nav:true,
             autoPlay:true,
              dot:true
        },
        600:{
            items:3,
            nav:true,
             autoPlay:true,
              dot:true
         },
        1200:{
            items:4,
            nav:true,
            loop:false,
             autoPlay:true,
              dot:true
         }
    }
})

});
 