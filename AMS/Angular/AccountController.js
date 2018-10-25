App.controller("AccountCtrl", function ($scope, $http) {

    $scope.list = [];

    $scope.BlockSubmit = true;
    $scope.PasswordMatched = true;
    $scope.OldPassword = "";
    $scope.NewPassword = "";
    $scope.ConfirmPassword = "";

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
    };

    $scope.ChangePassword = function () {
        var pram = {
            "OldPassword": JSON.stringify($scope.OldPassword),
            "NewPassword": JSON.stringify($scope.NewPassword),
        };

        if ($scope.PasswordMatched == false) {
            $scope.BlockSubmit == true;
            alert("Password does not matched...");
        }

        if ($scope.BlockSubmit == false) {
            JsonCallParam("Account", "ChangePassword", pram)
        }

        $scope.Clear();
    };

    $scope.Clear = function () {
        $scope.BlockSubmit = false;
        $scope.OldPassword = "";
        $scope.NewPassword = "";
        $scope.ConfirmPassword = "";
    };

    $scope.ValidatePassword = function () {
        if ($scope.OldPassword == ""
            || $scope.NewPassword == ""
            || $scope.ConfirmPassword == "") {
            $scope.BlockSubmit = true;
        }
        else {
            $scope.BlockSubmit = false;
        }

        $scope.MatchPassword();
    };

    $scope.MatchPassword = function () {
        if ($scope.NewPassword == $scope.ConfirmPassword) {
            $scope.PasswordMatched = true;
        }
        else {
            $scope.PasswordMatched = false;
        }
    };
});