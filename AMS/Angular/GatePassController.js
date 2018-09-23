App.controller("GatePassCtrl", function ($scope, $http) {

    $scope.list = [];
    $scope.Grid = [];

    $scope.PurchaseOrders = [];
    $scope.PurchaseOrderData = [];

    $scope.AllowSubmit = false;
    $scope.PurchaseOrderPt_Id = 0;
    $scope.NetTotal = 0;
    $scope.AmountRemaining = 0;
    $scope.CustomerObj = {};
    $scope.GatePassObj = { GatePass_Id: 0, Customer_Id: 0, Customer: {}, POP_Id: 0, PurchaseOrder_Pt: {}, GatePass_No: "" };
    $scope.PurchaseOrder_PtObj = { POP_Id: 0, Vendor_Id: 0, Vendor: {}, POP_TotalQuantity: 0, POP_TotalAmount: 0, POP_TotalPaid: 0, POP_Charges: 0, POP_GST: 0, POP_TaxPercent: 0, POP_TaxAmount: 0, POP_PO: "" };

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

    $scope.SaveGatePass = function () {
        if ($scope.GatePassObj.GatePass_Id == 0) {
            var pram = {
                "GatePassObj": JSON.stringify($scope.GatePassObj),
                "poPtId": JSON.stringify($scope.PurchaseOrderPt_Id),
            };

            $scope.PurchaseOrderApprove($scope.PurchaseOrderPt_Id);
            JsonCallParam("GatePass", "GenerateGatePass", pram)
            $scope.GetGatePass();
            $scope.Clear();
        }
    };

    $scope.GetPurchaseOrder = function () {
        JsonCall("GatePass", "GetPurchaseOrder");
        $scope.PurchaseOrders = list;
    };
    $scope.GetPurchaseOrder();

    $scope.GetGatePass = function () {
        JsonCall("GatePass", "GetGatePass");
        $scope.Grid = list;
    };
    $scope.GetGatePass();

    $scope.GetGST = function () {
        JsonCall("Home", "GetGST");
        $scope.PurchaseOrder_PtObj.POP_GST = list;
    };
    $scope.GetGST();

    $scope.PurchaseOrderApprove = function (popId) {
        JsonCallParam("GatePass", "PurchaseOrderApprove", { "poId": popId });
        $scope.GetPurchaseOrder();
    };

    $scope.StatusChangePO_FillDC = function (obj) {
        JsonCallParam("GatePass", "GetCustomerToShip", { "poId": obj.POP_Id });
        $scope.CustomerObj = list;

        JsonCallParam("GatePass", "StatusChangePO_FillDC", { "poId": obj.POP_Id });
        document.getElementById("collapsePendingPO").click();
        $scope.PurchaseOrderPt_Id = obj.POP_Id;
        $scope.GatePassObj.POP_Id = obj.POP_Id;
        $scope.PurchaseOrderData = [];
        angular.forEach(list, function (obj, index) {
            $scope.PurchaseOrderData.push({
                'POC_Id': 0,
                'POP_Id': 0,
                'PurchaseOrder_Pt': {},
                'Product_Id': obj.Product_Id,
                'Product': obj.Product,
                'POC_Quantity': obj.POC_Quantity,
                'POC_Rate': obj.Product.Product_UnitPrice,
                'POC_Description': obj.POC_Description,
                'POC_Amount': 0,
                'POC_ItemCode': obj.POC_ItemCode,
                'POC_Unit': obj.POC_Unit,
            });
            $scope.QuantityChange(index);
        });
        $scope.ValidateGatePass();
    };

    $scope.QuantityChange = function (i) {
        if ($scope.PurchaseOrderData[i].POC_Quantity < 0)
            $scope.PurchaseOrderData[i].POC_Quantity = 0;

        $scope.PurchaseOrderData[i].POC_Amount = $scope.PurchaseOrderData[i].POC_Quantity * $scope.PurchaseOrderData[i].POC_Rate;
        $scope.CalculateTotal();
    };

    $scope.CalculateTotal = function () {
        $scope.PurchaseOrder_PtObj.POP_TotalQuantity = 0;
        $scope.PurchaseOrder_PtObj.POP_TotalAmount = 0;

        angular.forEach($scope.PurchaseOrderData, function (obj) {
            $scope.PurchaseOrder_PtObj.POP_TotalQuantity += obj.POC_Quantity;
            $scope.PurchaseOrder_PtObj.POP_TotalAmount += obj.POC_Amount;
        });

        $scope.CalculateTax();
        $scope.NetTotal = $scope.PurchaseOrder_PtObj.POP_TotalAmount + $scope.PurchaseOrder_PtObj.POP_Charges + $scope.PurchaseOrder_PtObj.POP_TaxAmount;
    };

    $scope.CalculateTax = function () {
        if ($scope.PurchaseOrder_PtObj.POP_GST > 0 && $scope.PurchaseOrder_PtObj.POP_TotalAmount > 0) {
            $scope.PurchaseOrder_PtObj.POP_TaxAmount = ($scope.PurchaseOrder_PtObj.POP_TotalAmount / 100) * $scope.PurchaseOrder_PtObj.POP_GST;
        }
        else {
            $scope.PurchaseOrder_PtObj.POP_TaxAmount = 0;
        }
    };

    $scope.Clear = function () {
        $scope.GatePassObj = { GatePass_Id: 0, Customer_Id: 0, Customer: {}, POP_Id: 0, PurchaseOrder_Pt: {}, GatePass_No: "" };
        $scope.PurchaseOrder_PtObj = { POP_Id: 0, Vendor_Id: 0, Vendor: {}, POP_TotalQuantity: 0, POP_TotalAmount: 0, POP_TotalPaid: 0, POP_Charges: 0, POP_GST: 0, POP_TaxPercent: 0, POP_TaxAmount: 0, POP_PO: "" };
        $scope.PurchaseOrderData = [];
        $scope.AllowSubmit = false;
        $scope.NetTotal = 0;
        $scope.AmountRemaining = 0;
        $scope.GetGST();
    };

    $scope.ValidateGatePass = function () {
        $scope.AmountRemaining = $scope.NetTotal - $scope.PurchaseOrder_PtObj.POP_TotalPaid;

        if ($scope.GatePassObj.POP_Id > 0) {
            $scope.AllowSubmit = true;
        }
        else {
            $scope.AllowSubmit = false;
        }
    };
});