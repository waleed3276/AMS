App.controller("ProductSizeCtrl", function ($scope, $http) {

    $scope.list = [];
    $scope.ProductSizeData = [];

    $scope.AllowSubmit = false;
    $scope.ProductSizeObj = { ProductSize_Id: 0, ProductSize_Length: 0, ProductSize_Width: 0, ProductSize_Height: 0, ProductSize_Unit: "" };

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

    $scope.SaveProductSize = function () {
        var pram = { "ProductSizeObj": JSON.stringify($scope.ProductSizeObj) };

        if ($scope.ProductSizeObj.ProductSize_Id == 0) {
            JsonCallParam("ProductSizes", "CreateProductSize", pram)
        }
        else {
            JsonCallParam("ProductSizes", "UpdateProductSize", pram)
        }

        $scope.GetProductSize();
        $scope.Clear();
    };

    $scope.GetProductSize = function () {
        JsonCall("ProductSizes", "GetProductSize");
        $scope.ProductSizeData = list;
    };
    $scope.GetProductSize();

    $scope.Clear = function () {
        $scope.ProductSizeObj = { ProductSize_Id: 0, ProductSize_Length: 0, ProductSize_Width: 0, ProductSize_Height: 0, ProductSize_Unit: "" };
    }

    $scope.ValidateProductSize = function () {
        if ($scope.ProductSizeObj.ProductSize_Length < 1
            || $scope.ProductSizeObj.ProductSize_Unit == "") {
            $scope.AllowSubmit = false;
        }
        else {
            $scope.AllowSubmit = true;
        }
    }

    $scope.LoadProductSize = function (obj) {
        $scope.ProductSizeObj = obj;
        $scope.ValidateProductSize();
    }

    $scope.DeleteProductSize = function (obj) {
        if (confirm('Are you sure you want to delete product size?')) {
            JsonCallParam("ProductSizes", "DeleteProductSize", { "id": obj.ProductSize_Id })
            $scope.GetProductSize();
        } else {
            // Do nothing!
        }
    }
});