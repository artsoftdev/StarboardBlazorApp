function CheckInstantPrice(val) {
    //console.log(val.checked )
    if (val.checked === true) {
        $('[data-instant-price-open=""]').removeClass('d-none');
    } else {
        $('[data-instant-price-open=""]').addClass('d-none');
    }
}

function ActionSaleStart(val) {
    $(val).html(`<img src="./img/loader_primary.svg" class="w_h_30">`)
    $('[data-step-sale-action="start"]').html(`
    
    <span class="loading_custom_spinner"> </span>
    `)


    // if failed
    setTimeout(() => {
        $(val).html(`Failed`).removeClass('btn-primary').addClass('bg_transparent border text_deeppink').attr('disabled', true);

        $('[data-step-sale-action="start"]').html(`
        
        <span class="d-inline-block  rounded-circle border_deeppink  d-flex justify-content-center align-items-center w_h_48">
        <svg xmlns="http://www.w3.org/2000/svg" width="18" height="22" viewBox="0 0 18 22">
        <g id="icons_File_Upload_Line" data-name="icons/File Upload/Line" transform="translate(-3 -1)">
            <path id="Shape" d="M0,4A4,4,0,0,1,4,0h8.343a4,4,0,0,1,2.829,1.172l1.657,1.657A4,4,0,0,1,18,5.657V18a4,4,0,0,1-4,4H4a4,4,0,0,1-4-4ZM16,7V18a2,2,0,0,1-2,2H4a2,2,0,0,1-2-2V4A2,2,0,0,1,4,2h7V4a3,3,0,0,0,3,3Zm-.111-2a2,2,0,0,0-.475-.757L13.757,2.586A2,2,0,0,0,13,2.111V4a1,1,0,0,0,1,1Z" transform="translate(3 1)" fill="#ef466f" fill-rule="evenodd"></path>
            <path id="Shape-2" data-name="Shape" d="M3.617.076a1,1,0,0,0-.324.217l-3,3A1,1,0,0,0,1.707,4.707L3,3.414V8A1,1,0,1,0,5,8V3.414L6.293,4.707A1,1,0,0,0,7.707,3.293l-3-3A1,1,0,0,0,3.617.076Z" transform="translate(8 9)" fill="#ef466f"></path>
        </g>
    </svg>
        </span>`)


        $(val).parent().append(`
        <p class="text_7 font_size_12 py-3 px-2">
                                                Something went wrong, please <a href="#">try again</a>
                                            </p>
        `)

        $('[data-action-step="signature"]').attr('disabled', false)
    }, 3000)

}