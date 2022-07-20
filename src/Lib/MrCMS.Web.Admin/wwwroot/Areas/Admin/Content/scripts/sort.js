import Sortable from 'sortablejs';

export function initSortable() {
    [].forEach.call(document.querySelectorAll("[data-sortable]"), function (element) {
        new Sortable(el, {
            onUpdate: function (event) {
                $(el).find('li').each(function (index, domElement) {
                    $(domElement).find('[name*="Order"]').val(index);
                });
            }
        })
    });
}