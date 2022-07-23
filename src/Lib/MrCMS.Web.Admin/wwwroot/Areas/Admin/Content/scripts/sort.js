import Sortable from 'sortablejs';

export function initSortable() {
    [].forEach.call(document.querySelectorAll("[data-sortable]"), function (el) {
        new Sortable(el, {
            ghostClass: 'bg-warning-lt',
            animation: 100,
            onUpdate: function (event) {
                $(el).find('li').each(function (index, domElement) {
                    $(domElement).find('[name*="Order"]').val(index);
                });
            }
        })
    });
}