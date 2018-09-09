App.controller("ProductCtrl", function ($scope, $http) {

    $scope.list = [];
    $scope.ProductData = [];
    $scope.CategorySubData = [];
    $scope.ProductSizeData = [];

    $scope.AllowSubmit = false;
    $scope.ProductObj = { Product_Id: 0, CategorySub_Id: 0, ProductSize_Id: 0, Product_Title: "", Product_Code: "", Product_Description: "", Product_Color: "Default", Product_Unit: "", Product_UnitPrice: 0, Product_Rate: 0 };

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

    $scope.SaveProduct = function () {
        $scope.ProductObj.CategorySub_Id = CategorySub_Id;
        $scope.ProductObj.ProductSize_Id = ProductSize_Id;
        var pram = { "ProductObj": JSON.stringify($scope.ProductObj) };

        if ($scope.ProductObj.Product_Id == 0) {
            JsonCallParam("Products", "CreateProduct", pram)
        }
        else {
            JsonCallParam("Products", "UpdateProduct", pram)
        }

        $scope.GetProduct();
        $scope.Clear();
    };

    $scope.GetProduct = function () {
        JsonCall("Products", "GetProduct");
        $scope.ProductData = list;
    }
    $scope.GetProduct();

    $scope.GetCategorySub = function () {
        JsonCall("Products", "GetCategorySub");
        $scope.CategorySubData = list;
    };
    $scope.GetCategorySub();

    $scope.GetProductSize = function () {
        JsonCall("Products", "GetProductSize");
        $scope.ProductSizeData = list;
    };
    $scope.GetProductSize();

    $scope.Clear = function () {
        $scope.ProductObj = { Product_Id: 0, CategorySub_Id: 0, ProductSize_Id: 0, Product_Title: "", Product_Code: "", Product_Description: "", Product_Color: "Default", Product_Unit: "", Product_UnitPrice: 0, Product_Rate: 0 };
    }

    $scope.ValidateProduct = function () {
        $scope.ProductObj.CategorySub_Id = CategorySub_Id;
        $scope.ProductObj.ProductSize_Id = ProductSize_Id;
        if ($scope.ProductObj.Product_Title == ""
            || $scope.ProductObj.Product_Code == ""
            || $scope.ProductObj.Product_Description == ""
            || $scope.ProductObj.Product_Color == ""
            || $scope.ProductObj.Product_Unit == ""
            || $scope.ProductObj.Product_UnitPrice == 0
            || $scope.ProductObj.Product_Rate == 0
            || !$scope.ProductObj.CategorySub_Id > 0
            || !$scope.ProductObj.ProductSize_Id > 0) {
            $scope.AllowSubmit = false;
        }
        else {
            $scope.AllowSubmit = true;
        }
    }

    $scope.LoadProduct = function (obj) {
        $scope.ProductObj = obj;
        $scope.ValidateProduct();
    }

    $scope.DeleteProduct = function (obj) {
        if (confirm('Are you sure you want to delete product?')) {
            JsonCallParam("Products", "DeleteProduct", { "id": obj.Product_Id })
            $scope.GetProduct();
        } else {
            // Do nothing!
        }
    }
});