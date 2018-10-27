App.controller("SalesTaxCtrl", function ($scope, $http) {

    $scope.list = [];
    $scope.Grid = [];

    $scope.CustomerSO_List = [];
    $scope.CustomerData = [];
    $scope.SalesTax_PtData = [];
    $scope.SalesTax_ChList = [];

    $scope.selectedAll = false;
    $scope.AllowAddNewRow = true;
    $scope.AllowSubmit = false;
    $scope.NetTotal = 0;
    $scope.AmountRemaining = 0;
    $scope.InvoiceNo = 0;
    $scope.Invoice_DocumentNo = 0;
    $scope.SalesTax_PtObj = { STP_Id: 0, CustomerId: 0, Customer: {}, STP_DeliveryChallanNo: "", STP_TotalAmount: 0, STP_GST: 0, STP_TaxAmount: 0, STP_TotalReceived: 0 };

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
        $scope.AllowAddNewRow = true;
        angular.forEach($scope.SalesTax_ChList, function (obj) {
            if (obj.SOC_Quantity == 0) {
                $scope.AllowAddNewRow = false;
                return;
            }
        });

        if ($scope.AllowAddNewRow) {
            $scope.SalesTax_ChList.push({
                'SOC_Id': 0,
                'SOP_Id': 0,
                'SOC_Quantity': 0,
                'SOC_Rate': 0,
                'SOC_Description': "",
                'SOC_Amount': 0,
                'SOC_ItemCode': "",
                'SOC_Unit': "",
                'SOP_SO': "",
                'CustomerProduct_List': [],
            });
        }
    };

    $scope.GenerateSalesTaxInvoice = function () {
        $scope.SalesTax_PtObj.STP_TotalAmount = $scope.SalesTax_PtObj.STP_TotalReceived;//$scope.NetTotal;
        var pram = {
            "SalesTax_PtObj": JSON.stringify($scope.SalesTax_PtObj),
            "SalesTax_ChList": JSON.stringify($scope.SalesTax_ChList),
            "InvoiceNo": JSON.stringify($scope.InvoiceNo),
            "Invoice_DocumentNo": JSON.stringify($scope.Invoice_DocumentNo),
        };

        JsonCallParam("SalesTax", "GenerateSalesTaxInvoice", pram)
        if (list == "Save") {
            $scope.GetSalesTax();
            $scope.Clear();
        }
    };

    $scope.GetCustomers = function () {
        JsonCall("SalesTax", "GetCustomers");
        $scope.CustomerData = list;
    };
    $scope.GetCustomers();

    $scope.GetSalesTax = function () {
        JsonCall("SalesTax", "GetSalesTax");
        $scope.Grid = list;
    };
    $scope.GetSalesTax();

    $scope.GetCustomerSOList = function () {
        JsonCallParam("SalesTax", "GetCustomerSOList", { 'id': $scope.SalesTax_PtObj.CustomerId });
        $scope.CustomerSO_List = list;
    };
    $scope.GetCustomerSOList();

    $scope.GetGST = function () {
        JsonCall("Home", "GetGST");
        $scope.SalesTax_PtObj.STP_GST = list;
    };
    $scope.GetGST();

    $scope.SOChange = function (i) {
        angular.forEach($scope.SalesTax_ChList, function (obj, index) {
            if (index == i)
            {
                JsonCallParam("SalesTax", "GetCustomerProductList", { 'id': $scope.SalesTax_PtObj.CustomerId, 'soPtId': obj.SOP_Id });
                obj.CustomerProduct_List = list;
            }
        });
    };

    $scope.ProductChange = function (i) {
        angular.forEach($scope.SalesTax_ChList, function (obj, index) {
            if (index == i) {
                angular.forEach(obj.CustomerProduct_List, function (obj2) {
                    if (obj.SOC_Id == obj2.SOC_Id) {
                        obj.SOC_Quantity = obj2.SOC_Quantity;
                        obj.SOC_Rate = obj2.SOC_Rate;
                        obj.SOC_Unit = obj2.SOC_Unit;
                        obj.SOC_Amount = obj2.SOC_Amount;
                    }
                });
            }
        });

        $scope.CalculateTotal();
    };

    $scope.CalculateTotal = function () {
        $scope.SalesTax_PtObj.STP_TotalAmount = 0;

        angular.forEach($scope.SalesTax_ChList, function (obj) {
            $scope.SalesTax_PtObj.STP_TotalAmount += obj.SOC_Amount;
        });

        $scope.CalculateTax();
        $scope.NetTotal = $scope.SalesTax_PtObj.STP_TotalAmount + $scope.SalesTax_PtObj.STP_TaxAmount;
    };

    $scope.CalculateTax = function () {
        if ($scope.SalesTax_PtObj.STP_GST > 0 && $scope.SalesTax_PtObj.STP_TotalAmount > 0) {
            $scope.SalesTax_PtObj.STP_TaxAmount = ($scope.SalesTax_PtObj.STP_TotalAmount / 100) * $scope.SalesTax_PtObj.STP_GST;
        }
        else {
            $scope.SalesTax_PtObj.STP_TaxAmount = 0;
        }
    };

    $scope.Clear = function () {
        $scope.SalesTax_PtObj = { STP_Id: 0, CustomerId: 0, Customer: {}, STP_DeliveryChallanNo: "", STP_TotalAmount: 0, STP_GST: 0, SOP_TaxAmount: 0, SOP_TotalReceived: 0 };
        $scope.SalesTax_ChList = [];
        $scope.CustomerSO_List = [];

        $scope.selectedAll = false;
        $scope.AllowAddNewRow = true;
        $scope.AllowSubmit = false;
        $scope.NetTotal = 0;
        $scope.InvoiceNo = 0;
        $scope.Invoice_DocumentNo = 0;
        $scope.GetGST();
    };

    $scope.CheckAll = function () {
        if (!$scope.selectedAll) {
            $scope.selectedAll = false;
        } else {
            $scope.selectedAll = true;
        }
        angular.forEach($scope.SalesTax_ChList, function (obj) {
            obj.selected = $scope.selectedAll;
        });
    };

    $scope.Deleted_SalesTax_ChList = [];
    $scope.Remove = function () {
        var newDataList = [];
        $scope.selectedAll = false;
        angular.forEach($scope.SalesTax_ChList, function (obj) {
            if (!obj.selected) {
                newDataList.push(obj);
            }
            else {
                if (obj.STC_Id > 0) {
                    $scope.Deleted_SalesTax_ChList.push(obj);
                }
            }
        });
        $scope.SalesTax_ChList = newDataList;
        $scope.CalculateTotal();
        $scope.ValidateSalesTax();
    };

    $scope.ValidateSalesTax = function () {
        if ($scope.SalesTax_PtObj.STP_TotalReceived >= $scope.NetTotal) {
            $scope.SalesTax_PtObj.STP_TotalReceived = $scope.NetTotal;
        }
        if ($scope.SalesTax_PtObj.STP_TotalReceived <= 0) {
            $scope.SalesTax_PtObj.STP_TotalReceived = 0;
        }
        $scope.AmountRemaining = $scope.NetTotal - $scope.SalesTax_PtObj.STP_TotalReceived;

        if ($scope.SalesTax_PtObj.STP_TotalReceived > 0 && $scope.SalesTax_PtObj.STP_TotalReceived > 0 && $scope.SalesTax_PtObj.STP_TotalReceived > 0) {
            $scope.AllowSubmit = false;
        }
        else {
            $scope.AllowSubmit = true;
        }
    };
});