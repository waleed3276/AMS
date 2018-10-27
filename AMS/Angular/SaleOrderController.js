App.directive('jqdatepicker', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            element.datepicker({
                dateFormat: 'DD, d  MM, yy',
                onSelect: function (date) {
                    scope.date = date;
                    scope.$apply();
                }
            });
        }
    };
});

App.controller("SaleOrderCtrl", function ($scope, $http) {

    $scope.list = [];
    $scope.Grid = [];
    
    $scope.CustomerData = [];
    $scope.ProductData = [];
    $scope.SaleOrder_PtData = [];
    $scope.SaleOrder_ChList = [];

    $scope.selectedAll = false;
    $scope.AllowAddNewRow = true;
    $scope.AllowSubmit = false;
    $scope.IsSOExist = false;
    $scope.isCash = true;
    $scope.isBankAccount = false;
    $scope.isCheckbook = false;
    $scope.NetTotal = 0;
    $scope.AmountRemaining = 0;
    $scope.AmountRemainingOld = 0;
    $scope.InvoiceNo = 0;
    $scope.Invoice_DocumentNo = 0;
    $scope.SaleOrder_PtObj = { SOP_Id: 0, CustomerId: 0, Customer: {}, SOP_TotalQuantity: 0, SOP_TotalAmount: 0, SOP_TotalReceived: 0, SOP_Charges: 0, SOP_GST: 0, SOP_TaxPercent: 0, SOP_TaxAmount: 0, SOP_SO: "" };
    $scope.OldSaleOrder_PtObj = {};
    $scope.Transaction = { Transaction_Description: "", Transaction_Debit: 0, Transaction_Credit: 0 };

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
        angular.forEach($scope.SaleOrder_ChList, function (obj) {
            if (obj.Product_Id == 0 || obj.SOC_Quantity == 0)
            {
                $scope.AllowAddNewRow = false;
                return;
            }
        });

        if ($scope.AllowAddNewRow)
        {
            $scope.SaleOrder_ChList.push({
                'SOC_Id': 0,
                'SOP_Id': 0,
                'SaleOrder_Pt': {},
                'Product_Id': 0,
                'Product': {},
                'SOC_Quantity': 0,
                'SOC_Rate': 0,
                'SOC_Description': "",
                'SOC_Amount': 0,
                'SOC_ItemCode': "",
                'SOC_Unit': "",
            });
        }
    };

    $scope.SaveSaleOrder = function () {
        angular.forEach($scope.SaleOrder_ChList, function (obj) {
            obj.SaleOrder_Pt = null;
            obj.Product = null;
        });
        $scope.SaleOrder_PtObj.CustomerId = Customer_Id;
        $scope.SaleOrder_PtObj.SOP_TotalAmount = $scope.SaleOrder_PtObj.SOP_TotalReceived;//$scope.NetTotal;
        var pram = {
            "SaleOrder_PtObj": JSON.stringify($scope.SaleOrder_PtObj),
            "SaleOrder_ChList": JSON.stringify($scope.SaleOrder_ChList),
            "InvoiceNo": JSON.stringify($scope.InvoiceNo),
            "Invoice_DocumentNo": JSON.stringify($scope.Invoice_DocumentNo),
            "DeliveryDate": JSON.stringify($scope.date),
        };

        if ($scope.SaleOrder_PtObj.SOP_Id == 0) {
            JsonCallParam("SaleOrder", "CreateSaleOrder", pram)
            if (list == "Save")
            {
                $scope.AddTransaction();
                $scope.AddCustomerRemaining();
            }
        }
        else {
            var pram2 = {
                "SaleOrder_PtObj": JSON.stringify($scope.SaleOrder_PtObj),
                "SaleOrder_ChList": JSON.stringify($scope.SaleOrder_ChList),
                "OldSaleOrder_PtObj": JSON.stringify($scope.OldSaleOrder_PtObj),
                "Deleted_SaleOrder_ChList": JSON.stringify($scope.Deleted_SaleOrder_ChList),
            };
            JsonCallParam("SaleOrder", "UpdateSaleOrder", pram2)
            if (list == "Update")
            {
                $scope.UpdateTransaction();
                $scope.UpdateCustomerRemaining();
            }
        }

        $scope.GetSaleOrder();
        $scope.Clear();
    };

    $scope.AddTransaction = function () {
        $scope.Transaction.Transaction_Debit = $scope.SaleOrder_PtObj.SOP_TotalReceived;
        $scope.Transaction.Transaction_Credit = 0;
        $scope.Transaction.Transaction_Description = "Sale Order: " + $scope.Transaction.Transaction_Description;

        if ($scope.Transaction.Transaction_Debit > 0) {
            var pram = {
                'Transaction': JSON.stringify($scope.Transaction),
                'isCash': $scope.isCash,
                'isBankAccount': $scope.isBankAccount,
                'isCheckbook': $scope.isCheckbook,
                'CheckNo': $scope.CheckNo,
                'BankAccountNo': $scope.BankAccountNo,
            };
            JsonCallParam("SaleOrder", "AddTransaction", pram);
        }
    };

    $scope.UpdateTransaction = function () {
        $scope.Transaction.Transaction_Debit = $scope.SaleOrder_PtObj.SOP_TotalReceived;
        $scope.Transaction.Transaction_Credit = 0;
        $scope.Transaction.Transaction_Description = "Sale Order: " + $scope.Transaction.Transaction_Description;

        if ($scope.Transaction.Transaction_Debit > 0) {
            var pram = {
                'Transaction': JSON.stringify($scope.Transaction),
                'SOP_Id': JSON.stringify($scope.SaleOrder_PtObj.SOP_Id),
                'isCash': $scope.isCash,
                'isBankAccount': $scope.isBankAccount,
                'isCheckbook': $scope.isCheckbook,
                'CheckNo': $scope.CheckNo,
                'BankAccountNo': $scope.BankAccountNo,
            };
            JsonCallParam("SaleOrder", "UpdateTransaction", pram);
        }
    };

    $scope.AddCustomerRemaining = function () {
        JsonCallParam("SaleOrder", "AddCustomerRemaining", { "RemainingAmount": JSON.stringify($scope.AmountRemaining) });
    };

    $scope.UpdateCustomerRemaining = function () {
        JsonCallParam("SaleOrder", "UpdateCustomerRemaining", { "RemainingAmount": JSON.stringify($scope.AmountRemaining), "RemainingAmountOld": JSON.stringify($scope.AmountRemainingOld) });
    };

    $scope.ShowDescription = function () {
        $scope.Transaction.Transaction_Description = "";
        var count = 0;

        angular.forEach($scope.SaleOrder_ChList, function (obj) {

            if (obj.Product.Product_Title == null) {
                obj.Product.Product_Title = "";
            }
            if (obj.Product.Product_Title != "") {
                if (count == 0) {
                    $scope.Transaction.Transaction_Description += "" + obj.Product.Product_Title;
                }
                else {
                    $scope.Transaction.Transaction_Description += ", " + obj.Product.Product_Title;
                }

            }
            count++;
        });
    };

    $scope.GetCustomers = function () {
        JsonCall("SaleOrder", "GetCustomers");
        $scope.CustomerData = list;
    };
    $scope.GetCustomers();

    $scope.GetSaleOrder = function () {
        JsonCall("SaleOrder", "GetSaleOrder");
        $scope.Grid = list;
        //$scope.SaleOrder_PtData = list;
    };
    $scope.GetSaleOrder();

    $scope.GetProducts = function () {
        JsonCall("SaleOrder", "GetProducts");
        $scope.ProductData = list;
    };
    $scope.GetProducts();

    $scope.GetGST = function () {
        JsonCall("Home", "GetGST");
        $scope.SaleOrder_PtObj.SOP_GST = list;
    };
    $scope.GetGST();

    $scope.GetProductDetail = function (i) {
        if ($scope.SaleOrder_ChList[i].Product_Id > 0)
        {
            JsonCallParam("SaleOrder", "GetProductDetail", { "id": $scope.SaleOrder_ChList[i].Product_Id });
            if (list != null) {
                $scope.SaleOrder_ChList[i].Product = list;
                $scope.SaleOrder_ChList[i].SOC_ItemCode = list.Product_Code;
                $scope.SaleOrder_ChList[i].SOC_Description = list.Product_Description;
                $scope.SaleOrder_ChList[i].SOC_Rate = list.Product_Rate;
                $scope.SaleOrder_ChList[i].SOC_Unit = list.Product_Unit;
            }
        }
        else
        {
            $scope.SaleOrder_ChList[i].Product = {};
            $scope.SaleOrder_ChList[i].SOC_ItemCode = "";
            $scope.SaleOrder_ChList[i].SOC_Description = "";
            $scope.SaleOrder_ChList[i].SOC_Rate = 0;
            $scope.SaleOrder_ChList[i].SOC_Unit = "";
        }
        $scope.QuantityChange(i);
        $scope.ShowDescription();
        $scope.ValidateSaleOrder();
    };

    $scope.QuantityChange = function (i) {
        if ($scope.SaleOrder_ChList[i].SOC_Quantity < 0)
            $scope.SaleOrder_ChList[i].SOC_Quantity = 0;

        $scope.SaleOrder_ChList[i].SOC_Amount = $scope.SaleOrder_ChList[i].SOC_Quantity * $scope.SaleOrder_ChList[i].SOC_Rate;
        $scope.CalculateTotal();
    };

    $scope.CalculateTotal = function () {
        $scope.SaleOrder_PtObj.SOP_TotalQuantity = 0;
        $scope.SaleOrder_PtObj.SOP_TotalAmount = 0;

        angular.forEach($scope.SaleOrder_ChList, function (obj) {
            $scope.SaleOrder_PtObj.SOP_TotalQuantity += obj.SOC_Quantity;
            $scope.SaleOrder_PtObj.SOP_TotalAmount += obj.SOC_Amount;
        });

        $scope.CalculateTax();
        $scope.NetTotal = $scope.SaleOrder_PtObj.SOP_TotalAmount + $scope.SaleOrder_PtObj.SOP_Charges + $scope.SaleOrder_PtObj.SOP_TaxAmount;
    };

    $scope.CalculateTax = function () {
        /*if ($scope.SaleOrder_PtObj.SOP_TaxPercent > 0 && $scope.SaleOrder_PtObj.SOP_TaxPercent <= 100 && $scope.SaleOrder_PtObj.SOP_TotalAmount > 0)
        {
            $scope.SaleOrder_PtObj.SOP_TaxAmount = ($scope.SaleOrder_PtObj.SOP_TotalAmount / 100) * $scope.SaleOrder_PtObj.SOP_TaxPercent;
        }
        else
        {
            $scope.SaleOrder_PtObj.SOP_TaxPercent = 0;
            $scope.SaleOrder_PtObj.SOP_TaxAmount = 0;
        }*/
        if ($scope.SaleOrder_PtObj.SOP_GST > 0 && $scope.SaleOrder_PtObj.SOP_TotalAmount > 0) {
            //$scope.SaleOrder_PtObj.SOP_TaxAmount = ($scope.SaleOrder_PtObj.SOP_TotalAmount / 100) * $scope.SaleOrder_PtObj.SOP_GST;
            $scope.SaleOrder_PtObj.SOP_TaxAmount = 0;
        }
        else {
            $scope.SaleOrder_PtObj.SOP_TaxAmount = 0;
        }
    };

    $scope.Clear = function () {
        $scope.Transaction = { Transaction_Description: "", Transaction_Debit: 0, Transaction_Credit: 0 };
        $scope.SaleOrder_PtObj = { SOP_Id: 0, CustomerId: 0, Customer: {}, SOP_TotalQuantity: 0, SOP_TotalAmount: 0, SOP_TotalReceived: 0, SOP_Charges: 0, SOP_GST: 0, SOP_TaxPercent: 0, SOP_TaxAmount: 0, SOP_SO: "" };
        $scope.SaleOrder_ChList = [];
        $scope.AllowSubmit = false;
        $scope.isCash = true;
        $scope.isBankAccount = false;
        $scope.isCheckbook = false;
        $scope.NetTotal = 0;
        $scope.AmountRemaining = 0;
        $scope.InvoiceNo = 0;
        $scope.Invoice_DocumentNo = 0;
        $scope.date = "";
        $scope.ShowDescription();
        $scope.GetGST();
    };

    $scope.CashClick = function () {
        $scope.isCash = "true";
        $scope.isBankAccount = "false";
        $scope.isCheckbook = "false";
    };
    $scope.BankClick = function () {
        $scope.isCash = "false";
        $scope.isBankAccount = "true";
        $scope.isCheckbook = "false";
    };
    $scope.CheckClick = function () {
        $scope.isCash = "false";
        $scope.isBankAccount = "false";
        $scope.isCheckbook = "true";
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
        $scope.CalculateTotal();
        $scope.ShowDescription();
        $scope.ValidateSaleOrder();
    };

    $scope.CheckSaleOrderNoExist = function () {
        $scope.ValidateSaleOrder();
        if ($scope.SaleOrder_PtObj.SOP_SO != "")
        {
            JsonCallParam("SaleOrder", "CheckSaleOrderNoExist", { "soNumber": $scope.SaleOrder_PtObj.SOP_SO });
            if (list == true)
            {
                $scope.IsSOExist = true;
                $scope.AllowSubmit = true;
            }
            else
            {
                $scope.IsSOExist = false;
            }
        }
    };

    $scope.ValidateSaleOrder = function () {
        if ($scope.SaleOrder_PtObj.SOP_TotalReceived >= $scope.NetTotal) {
            $scope.SaleOrder_PtObj.SOP_TotalReceived = $scope.NetTotal;
        }
        if ($scope.SaleOrder_PtObj.SOP_TotalReceived <= 0) {
            $scope.SaleOrder_PtObj.SOP_TotalReceived = 0;
        }
        $scope.AmountRemaining = $scope.NetTotal - $scope.SaleOrder_PtObj.SOP_TotalReceived;

        if ($scope.SaleOrder_PtObj.SOP_TotalReceived >= 0 && $scope.SaleOrder_PtObj.SOP_SO != "") {
            $scope.AllowSubmit = false;
        }
        else {
            $scope.AllowSubmit = true;
        }
    };

    $scope.LoadSaleOrder = function (obj) {
        $scope.SaleOrder_PtObj = obj.Item1;
        $scope.NetTotal = obj.Item1.SOP_TotalAmount;
        $scope.InvoiceNo = obj.Item4;
        $scope.Invoice_DocumentNo = obj.Item7;
        JsonCallParam("SaleOrder", "LoadSaleOrder", { "SoPtId": obj.Item1.SOP_Id })
        $scope.SaleOrder_ChList = list;
        $scope.CalculateTotal();
        $scope.ShowDescription();
        $scope.ValidateSaleOrder();
        $scope.OldSaleOrder_PtObj = obj.Item1;
        $scope.AmountRemainingOld = obj.Item1.SOP_TotalAmount - obj.Item1.SOP_TotalReceived;
    };

    $scope.DeleteSaleOrder = function (obj) {
        if (confirm("Are you sure you want to delete sale order?")) {
            JsonCallParam("SaleOrder", "DeleteSaleOrder", { "SoPtId": obj.SaleOrder_PtObj.SOP_Id })
            $scope.GetSaleOrder();
        }
    };
});