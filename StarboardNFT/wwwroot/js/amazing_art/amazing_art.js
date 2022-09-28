function CheckoutModal(val) {
    //     $('[data-purchasing-ul]').html(`
    //     <!-- processing list -->
    //                                     <li class="px-1 list-group-item border-0 bg_transparent">
    //                                         <div class="d-flex align-items-center">
    //                                             <div class="spinner-border mr-3 text-primary"></div>
    //                                             <div class="content">
    //                                                 <strong class="text_23">Purchasing</strong>
    //                                                 <br>
    //                                                 <span class="text_7 font_size_14">
    //                                                     Sending transaction with your wallet
    //                                                 </span>
    //                                             </div>
    //                                         </div>
    //                                     </li>
    //                                     <!-- processing list end-->

    //     `);
    //    // $('[data-purchasing-title]').html('Follow steps')

    //     //$('#checkout_modal').css('transition-delay', '3s')
    //    // $('#purchasing_process_modal').css('transition-delay', '3s')
    //     setTimeout(() => {
    //         $('#checkout_modal').css('transition-delay', '0s')
    //        // $('#purchasing_process_modal').css('transition-delay', '0s')
    //     }, 3000);

}

function ConnectWallet(val) {
    //
}

function ActionStepStart(val) {
    //
    $(val).html(` <img class="w_h_30" src="./img/loader_primary.svg">`)
    $('[data-step-action="start"]').html(`<span class="loading_custom_spinner"> </span>`);

    setTimeout(() => {
        $(val).html('Done').addClass('process_succeed').attr('disabled', true);

        $('[data-step-action="start"]').html(`<span class="d-inline-block  rounded-circle bg_teal  d-flex justify-content-center align-items-center w_h_48">
        <svg xmlns="http://www.w3.org/2000/svg" width="14" height="10"
            viewBox="0 0 14 10">
            <path id="check_Stroke_" data-name="check (Stroke)"
                d="M13.707.293a1,1,0,0,1,0,1.414l-8,8a1,1,0,0,1-1.414,0l-4-4A1,1,0,1,1,1.707,4.293L5,7.586,12.293.293A1,1,0,0,1,13.707.293Z"
                fill="#fcfcfd" />
        </svg>
    </span>`)

        $('[data-action-step="approve"]').attr('disabled', false)


    }, 3000)
}


function ActionStepApprove(val) {
    // 

    // processing action
    $(val).html(`Cancelled`).removeClass('btn-primary').addClass('btn-muted border text_deeppink').parent().append(`
    <p class="text_7 font_size_12 py-3 px-2">
                                                Something went wrong, please <a href="#">try again</a>
                                            </p>
    `)
    $('[data-step-action="approve"]').html(`
    <span
                                                class="d-inline-block  rounded-circle border_deeppink d-flex justify-content-center align-items-center"
                                                style="width: 48px; height: 48px">
                                                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="10"
                                                    viewBox="0 0 14 10">
                                                    <path id="check_Stroke_" data-name="check (Stroke)"
                                                        d="M13.707.293a1,1,0,0,1,0,1.414l-8,8a1,1,0,0,1-1.414,0l-4-4A1,1,0,1,1,1.707,4.293L5,7.586,12.293.293A1,1,0,0,1,13.707.293Z"
                                                        fill="#EF466F" />
                                                </svg>
    `)

    // success action
    setTimeout(()=> {
    $(val).html(`Done`).removeClass('btn-muted border text_deeppink').addClass('process_succeed btn-primary').attr('disabled', true).next().remove();



    $('[data-step-action="approve"]').html(`<span class="d-inline-block  rounded-circle bg_teal  d-flex justify-content-center align-items-center w_h_48">
        <svg xmlns="http://www.w3.org/2000/svg" width="14" height="10"
            viewBox="0 0 14 10">
            <path id="check_Stroke_" data-name="check (Stroke)"
                d="M13.707.293a1,1,0,0,1,0,1.414l-8,8a1,1,0,0,1-1.414,0l-4-4A1,1,0,1,1,1.707,4.293L5,7.586,12.293.293A1,1,0,0,1,13.707.293Z"
                fill="#fcfcfd" />
        </svg>
    </span>`)


    $('[data-action-step="signature"]').attr('disabled', false)
    }, 5000)

}