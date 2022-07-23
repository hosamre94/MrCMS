const MrCMSFeatherlightSettings = {
    type: 'iframe',
    iframeWidth: 800,
    afterOpen: function () {
        setCloseButtonPosition(this.$instance);
    },
    beforeOpen: function () {
        const size = this.$currentTarget.data('fb-size');
        this.iframeWidth = size === 'lg' ? 1140 : size === 'sm' ? 500 : size === 'xs' ? 350 : 800
    },
    onResize: function () {
        if (this.autoHeight) {
            // Shrink:
            this.$content.css('height', '10px');
            // Then set to the full height:
            this.$content.css('height', this.$content.contents().find('body')[0].scrollHeight);
        }
        setCloseButtonPosition(this.$instance);
    }
};

export function setCloseButtonPosition(contents) {
    const offset = contents.find(".featherlight-content").offset();
    const scrollTop = $(document).scrollTop();
    contents.find(".featherlight-close-icon").css('top', offset.top - scrollTop);
    contents.find(".featherlight-close-icon").css('right', offset.left - 20);
}

export function getRemoteModel(href, size) {
    const link = $("<a>");
    link.attr('href', href);
    link.attr('data-toggle', 'fb-modal');
    link.attr('data-fb-size', size)
    link.featherlight(MrCMSFeatherlightSettings).click();
}

export function setupFeatherlight() {
    const featherlightSettings = $.extend({}, MrCMSFeatherlightSettings, {
        filter: '[data-toggle="fb-modal"]'
    });
    $(document).featherlight(featherlightSettings);
}

export function setupImageFeatherlight() {
    $(document).featherlight({
        filter: '[data-toggle="fb-image-modal"]',
        type: 'image',
    });
}
