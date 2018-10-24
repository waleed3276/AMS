App.controller("NotifyCtrl", function ($scope, $http) {

    var list = [];
    $scope.MessageList = [];
    $scope.NotificationCount = 0;

    function JsonCall(Controller, Action) {
        $.ajax({
            type: "POST",
            traditional: true,
            async: false,
            cache: false,
            url: '/' + Controller + '/' + Action,
            context: document.body,
            success: function (json) {
                list = null; list = json;
            },
            error: function (xhr) {
                list = null;
                //debugger;
            }
        });
    }
    function JsonCallParam(Controller, Action, Parameters) {
        $.ajax({
            type: "POST",
            traditional: true,
            async: false,
            cache: false,
            url: '/' + Controller + '/' + Action,
            context: document.body,
            data: Parameters,
            success: function (json) {
                list = null; list = json;
            }
       ,
            error: function (xhr) {
                //debugger;
                list = null;
            }
        });
    }
    function WebApiCall(Type, Action, Parameters) {

        $.ajax({
            type: Type,
            traditional: true,
            async: false,
            cache: false,
            url: '/api/' + Action,
            context: document.body,
            data: Parameters,
            ContentType: 'application/json;utf-8',
            success: function (json) {
                list = json;
            }
       ,
            error: function (xhr) {
                //debugger;
            }
        });
    }

    var notifyList = [];
    function CheckAndNotify() {
        $.ajax({
            type: "POST",
            traditional: true,
            async: false,
            cache: false,
            url: '/Notification/CheckAndNotify',
            context: document.body,
            success: function (json) {
                notifyList = json;
                $scope.MessageList = json;
                $scope.NotificationCount = notifyList.length;
                for (var i = 0; i < notifyList.length; i++) {

                    if (notifyList != null && notifyList != [] && notifyList != "") {
                        var nNo = document.getElementById("notificationMsg");
                    }
                }
                if (notifyList.length == 0) {

                    var nNo = document.getElementById("notificationNo");
                }
            },
            error: function (ex) {
            }
        });

    }

    $scope.notifySeen = function () {
        $scope.NotificationCount = 0;
        JsonCall("Home", "AllNotifySeen");
    }

    CheckAndNotify();
    setInterval(function () {
        CheckAndNotify();
    }, 60000)
});