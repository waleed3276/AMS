App.controller("CategoryCtrl", function ($scope, $http) {
    
    $scope.list = [];
    $scope.CategoryData = [];
    $scope.AllowSubmit = false;

    $scope.CategoryObj = { Category_Id: 0, Category_Title: "", Category_Code: "", Category_Description: "" }


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

    $scope.SaveCategory = function () {

            var pram = { "CategoryObj": JSON.stringify($scope.CategoryObj) };

            if ($scope.CategoryObj.Category_Id == 0) {
                JsonCallParam("Categories", "CreateCategory", pram)
            }
        
        else {
            JsonCallParam("Categories", "UpdateCategory", pram)
        }
        
        $scope.GetCategory();
        $scope.Clear();

    };

    $scope.GetCategory = function () {
        JsonCall("Categories", "GetCategory");
        $scope.CategoryData = list;

    };
    $scope.GetCategory();


    $scope.Clear = function () {
        $scope.CategoryObj = { Category_Id: 0, Category_Title: "", Category_Code: "", Category_Description: "" }

    };

    $scope.ValidateCategory = function () {
        if ($scope.CategoryObj.Category_Title == "",
            $scope.CategoryObj.Category_Code == "",
            $scope.CategoryObj.Category_Description == ""
            ) {
            $scope.AllowSubmit = false;
        }
        else {

            $scope.AllowSubmit = true;
        }

    };

    $scope.LoadCategory = function (object) {
        $scope.CategoryObj = object;
        $scope.ValidateCategory();

    };

    $scope.DeleteCategory = function (object) {

        if (confirm("Are you sure you want to delete the selected Category?")) {
            JsonCallParam("Categories", "DeleteCategory", { "id": object.Category_Id })
            $scope.GetCategory();
        }
    };
});