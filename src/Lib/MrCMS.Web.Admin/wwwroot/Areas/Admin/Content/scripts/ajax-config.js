import NProgress from "NProgress";

export function ajaxSetup() {
    let requestVerificationToken = $('body').data('antiforgery-token');
    $.ajaxSetup({
        cache: false,
        headers: {
            'RequestVerificationToken': requestVerificationToken
        }
    });
}
export function showHideLoading(){
    $(document).ajaxStart(function () {
        if (!NProgress.isRendered())
            NProgress.start()
    });
    $(document).ajaxStop(function () {
        NProgress.done()
    });
}
