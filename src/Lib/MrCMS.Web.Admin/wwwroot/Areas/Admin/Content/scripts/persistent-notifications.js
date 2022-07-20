export function setupNotificationBar() {
    var PersistentNotifications = function () {
        var initialized = false;
        var navContainer = $('#persistentNotificationLink');
        var updateNotificationCount = function () {
            $.get("/Admin/PersistentNotification/GetCount", function (data) {
                $('[data-notification-count]').html(data);
            });
        };
        var listContainer = $('[data-notification-list]');
        var initializeNotifications = function () {
            $.get('/Admin/PersistentNotification/Get', function (notifications) {
                listContainer.empty();
                if (notifications.length) {
                    notifications.reverse();
                    $.each(notifications, function (idx, notification) {
                        prependNotification(notification);
                    });
                } else {
                    resetNotifications();
                }
                initialized = true;
            });
        };
        var displayNotification = function (notification) {
            return $(`<div class="list-group-item" data-notification="true">
                            <div class="row align-items-center">
                                <div class="col-auto">
                                    <span class="status-dot d-block"></span>
                                </div>
                                <div class="col text-truncate">
                                    <a href="#" class="text-body d-block">${notification.date}</a>
                                    <div class="d-block text-muted text-truncate mt-n1">
                                        ${notification.message}
                                    </div>
                                </div>
                            </div>
                        </div>`)
        };
        var displayNoNotifications = function () {
            return $('<div class="list-group-item">').html(`
            <div class="empty" style="min-width: 300px">
              <div class="empty-img">
                <img src="/Areas/Admin/Content/img/empty_result.svg" height="128" alt="">
              </div>
              <p class="h3">No notifications found</p>
            </div>`);
        };
        var prependNotification = function (notification) {
            listContainer.prepend(displayNotification(notification));   
            if (initialized) {
                updateNotificationCount();
            }
        };
        var resetNotifications = function () {
            listContainer.empty();
            listContainer.append(displayNoNotifications());
            updateNotificationCount();
        };
        var markAllAsRead = function () {
            $.post(navContainer.data('mark-as-read-url'), function () {
                initialized = false;
                initializeNotifications();
            });
        };  
        
        return {
            init: function () {
                updateNotificationCount();
                navContainer.on('hidden.bs.dropdown', function (e) {
                    markAllAsRead();
                });
                navContainer.on('show.bs.dropdown', function (e) {
                    initializeNotifications();
                });
                var connection = new signalR.HubConnectionBuilder()
                    .withUrl('/notificationsHub')
                    .configureLogging(signalR.LogLevel.Warning)
                    .build();
                connection.on('sendPersistentNotification', prependNotification);
                connection.start().catch(err => console.error(err.toString()));
            }
        };
        
       
    };
    $(function () {
        let disableNotifications = $('body').data('disable-notifications');
        if (disableNotifications === "False") {
            new PersistentNotifications().init();
        }
    })
}