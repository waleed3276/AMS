App.controller("SaleOrderCtrl", function ($scope, $http) {

    $scope.list = [];
    
    $scope.ProductData = [];
    $scope.SaleOrder_PtData = [];
    $scope.SaleOrder_ChList = [];

    $scope.selectedAll = false;
    $scope.AllowSubmit = false;
    $scope.SaleOrder_PtObj = { SOP_Id: 0, CustomerId: 0, SOP_TotalQuantity: 0, SOP_TotalAmount: 0, SOP_TotalReceived: 0, SOP_Charges: 0, SOP_TaxPercent: 0, SOP_TaxAmount: 0 };

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

    $scope.AddNew = function () {
        $scope.SaleOrder_ChList.push({
            'SOC_Id': 0,
            'SOP_Id': 0,
            'SaleOrder_Pt': {},
            'Product_Id': 0,
            'Product': {},
            'SOC_Quantity': 0,
            'SOC_Rate': 0,
            'SOC_Description': "",
            'SOC_GST': 0,
            'SOC_Amount': 0,
            'SOC_ItemCode': 0,
            'SOC_Unit': "",
        });
    };

    $scope.SaveSaleOrder = function () {
        $scope.CategorySubObj.Category_Id = Category_Id;
        var pram = { "CategorySubObj": JSON.stringify($scope.CategorySubObj) };

        if ($scope.CategorySubObj.CategorySub_Id == 0) {
            JsonCallParam("CategoriesSub", "CreateCategorySub", pram)
        }
        else {
            JsonCallParam("CategoriesSub", "UpdateCategorySub", pram)
        }

        $scope.GetSaleOrder();
        $scope.Clear();
    };

    $scope.GetSaleOrder = function () {
        JsonCall("SaleOrder", "GetSaleOrder");
        $scope.SaleOrder_PtData = list;
    };
    $scope.GetSaleOrder();

    $scope.GetProducts = function () {
        JsonCall("SaleOrder", "GetProducts");
        $scope.ProductData = list;
    };
    $scope.GetProducts();

    $scope.GetProductDetail = function (i) {
        JsonCallParam("SaleOrder", "GetProductDetail", { "id": $scope.SaleOrder_ChList[i].Product_Id });
        if (list != null)
        {
            $scope.SaleOrder_ChList[i].Product = list;
            $scope.SaleOrder_ChList[i].Product.SOC_ItemCode = list.Product_Code;
            $scope.SaleOrder_ChList[i].Product.SOC_Description = list.Product_Description;
            $scope.SaleOrder_ChList[i].Product.SOC_Rate = list.Product_Rate;
            $scope.SaleOrder_ChList[i].Product.SOC_Unit = list.Product_Unit;
        }
    };

    $scope.Clear = function () {
        $scope.SaleOrder_PtObj = { SOP_Id: 0, CustomerId: 0, SOP_TotalQuantity: 0, SOP_TotalAmount: 0, SOP_TotalReceived: 0, SOP_Charges: 0, SOP_TaxPercent: 0, SOP_TaxAmount: 0 };
        $scope.SaleOrder_ChList = [];
    };

    $scope.CheckAll = function () {
        if (!$scope.selectedAll) {
            $scope.selectedAll = false;
        } else {
            $scope.selectedAll = true;
        }
        angular.forEach($scope.SaleOrder_ChList, function (obj) {
            obj.selected = $scope.selectedAll;
        });
    };

    $scope.Deleted_SaleOrder_ChList = [];
    $scope.Remove = function () {
        var newDataList = [];
        $scope.selectedAll = false;
        angular.forEach($scope.SaleOrder_ChList, function (obj) {
            if (!obj.selected) {
                newDataList.push(obj);
            }
            else {
                if (obj.SOC_Id > 0) {
                    $scope.Deleted_SaleOrder_ChList.push(obj);
                }
            }
        });
        $scope.SaleOrder_ChList = newDataList;

    };

    $scope.ValidateSaleOrder = function () {
        $scope.ProductData[i].Product_Id = Product_Id;
        if ($scope.ProductData[i].Category_Id > 0) {
            $scope.AllowSubmit = false;
        }
        else {
            $scope.AllowSubmit = true;
        }
    };

    $scope.LoadCategorySub = function (obj) {
        $scope.CategorySubObj = obj;
        $scope.ValidateSaleOrder();
    };

    $scope.DeleteCategorySub = function (obj) {
        if (confirm("Are you sure you want to delete sub-category?")) {
            JsonCallParam("CategoriesSub", "DeleteCategorySub", { "id": obj.CategorySub_Id })
            $scope.GetCategorySub();
        }
    };
});