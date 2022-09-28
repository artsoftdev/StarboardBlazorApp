function AcceptBidProcess(val) {
    //
    $(val).html(`
    <img class="w_h_30" src="./img/loader_primary.svg">
    `)
    $('[data-step-action="acceptBitProcess"]').html(`
    
    <span class="loading_custom_spinner"> </span>
    `)
}