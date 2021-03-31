"use strict";
function formatState (state) {
  if (!state.id) {
    return state.text;
  }
  var baseUrl = "flag-icons/fonts";
  var $state = $(
    '<span><img src="' + baseUrl + '/' + state.element.value.toLowerCase() + '.svg" class="img-flag" /> ' + state.text + '</span>'
  
  );
$state.find("div").text(state.text);
  $state.find("img").attr("src", baseUrl + "/" + state.element.value.toLowerCase() + ".svg");

  return $state;
};




$(".js-example-templating").select2({
   templateResult: formatState,
  templateSelection: formatState,
  minimumResultsForSearch: -1,
  

}); 
  
 

