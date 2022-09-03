import NProgress from 'NProgress'

export function setupProgressbar() {
    NProgress.configure({showSpinner: false})
    NProgress.start();
    window.onLoad = () => setTimeout(NProgress.done(), 300)
}
