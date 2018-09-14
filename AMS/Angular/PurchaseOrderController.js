App.controller("PurchaseOrderCtrl", function ($scope, $http)
{

    $scope.list = [];
    $scope.ProductData = [];
    $scope.PurchaseOrder_PtData = [];
    $scope.PurchaseOrder_ChList = [];

    $scope.selectedAll = false;
    $scope.AllowSubmit = false;
    $scope.PurchaseOrder_PtObj = { POP_Id: 0, Vendor_Id: 0, POP_TotalQuantity: 0, POP_TotalAmount: 0, POP_TotalPaid: 0, POP_Charges: 0, POP_TaxPercent: 0, POP_TaxAmount: 0 };

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


    $scope.AddNew = function () {
        $scope.PurchaseOrder_ChList.push({
            'POC_Id': 0,
            'POP_Id': 0,
            'PurchaseOrder_Pt': {},
            'Product_Id': 0,
            'Product': {},
            'POC_Quantity': 0,
            'POC_Rate': 0,
            'POC_Description': "",
            'POC_GST': 0,
            'POC_Amount': 0,
            'POC_ItemCode': 0,
            'POC_Unit': "",

        });
    };


    $scope.GetPurchaseOrder = function () {
        JsonCall("AdminPurchaseOrder", "GetPurchaseOrder");
        $scope.PurchaseOrder_PtData = list;
    };
    $scope.GetPurchaseOrder();


    $scope.GetProducts = function () {

        JsonCall("AdminPurchaseOrder", "GetProducts")
        $scope.ProductData = list;
    };
    $scope.GetProducts();

    $scope.GetProductDetails = function (i) {
        JsonCallParam("AdminPurchaseOrder", "GetProductDetails", {"id" : $scope.PurchaseOrder_ChList[i].Product_Id} )
        if (list != null)
        {
            $scope.PurchaseOrder_ChList[i].Product = list;
            $scope.PurchaseOrder_ChList[i].Product.POC_ItemCode = list.Product_Code;
            $scope.PurchaseOrder_ChList[i].Product.POC_Description = list.Product_Description;
            $scope.PurchaseOrder_ChList[i].Product.POC_Rate = list.Product_Rate;
            $scope.PurchaseOrder_ChList[i].Product.POC_Unit = list.Product_Unit;

        }
    };
    $scope.Clear = function () {
        $scope.PurchaseOrder_PtObj = { POP_Id: 0, Vendor_Id: 0, POP_TotalQuantity: 0, POP_TotalAmount: 0, POP_TotalPaid: 0, POP_Charges: 0, POP_TaxPercent: 0, POP_TaxAmount: 0 };
        $scope.PurchaseOrder_ChList = [];
    };



});