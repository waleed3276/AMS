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

App.controller("PurchaseOrderCtrl", function ($scope, $http) {

    $scope.list = [];
    $scope.Grid = [];
    
    $scope.SaleOrders = [];
    $scope.VendorData = [];
    $scope.PurchaseOrder_PtData = [];
    $scope.PurchaseOrder_ChList = [];

    $scope.selectedAll = false;
    $scope.AllowAddNewRow = true;
    $scope.AllowSubmit = false;
    $scope.isCash = true;
    $scope.isBankAccount = false;
    $scope.isCheckbook = false;
    $scope.SaleOrderPt_Id = 0;
    $scope.NetTotal = 0;
    $scope.AmountRemaining = 0;
    $scope.AmountRemainingOld = 0;
    $scope.InvoiceNo = 0;
    $scope.Invoice_DocumentNo = 0;
    $scope.PurchaseOrder_PtObj = { POP_Id: 0, Vendor_Id: 0, Vendor: {}, POP_TotalQuantity: 0, POP_TotalAmount: 0, POP_TotalPaid: 0, POP_Charges: 0, POP_GST: 0, POP_TaxPercent: 0, POP_TaxAmount: 0, POP_PO: "" };
    $scope.OldPurchaseOrder_PtObj = {};
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
        angular.forEach($scope.PurchaseOrder_ChList, function (obj) {
            if (obj.Product_Id == 0 || obj.POC_Quantity == 0) {
                $scope.AllowAddNewRow = false;
                return;
            }
        });

        if ($scope.AllowAddNewRow) {
            $scope.PurchaseOrder_ChList.push({
                'POC_Id': 0,
                'POP_Id': 0,
                'PurchaseOrder_Pt': {},
                'Product_Id': 0,
                'Product': {},
                'POC_Quantity': 0,
                'POC_Rate': 0,
                'POC_Description': "",
                'POC_Amount': 0,
                'POC_ItemCode': "",
                'POC_Unit': "",
            });
        }
    };

    $scope.SavePurchaseOrder = function () {
        angular.forEach($scope.PurchaseOrder_ChList, function (obj) {
            obj.PurchaseOrder_Pt = null;
            obj.Product = null;
        });
        $scope.PurchaseOrder_PtObj.POP_TotalAmount = $scope.NetTotal;
        $scope.PurchaseOrder_PtObj.Vendor_Id = Vendor_Id;
        var pram = {
            "PurchaseOrder_PtObj": JSON.stringify($scope.PurchaseOrder_PtObj),
            "PurchaseOrder_ChList": JSON.stringify($scope.PurchaseOrder_ChList),
            "InvoiceNo": JSON.stringify($scope.InvoiceNo),
            "Invoice_DocumentNo": JSON.stringify($scope.Invoice_DocumentNo),
            "DeliveryDate": JSON.stringify($scope.date),
        };

        if ($scope.PurchaseOrder_PtObj.POP_Id == 0) {
            $scope.SaleOrderApprove($scope.SaleOrderPt_Id);
            JsonCallParam("PurchaseOrder", "CreatePurchaseOrder", pram)
            if (list == "Save") {
                $scope.AddTransaction();
                $scope.AddVendorRemaining();
            }
        }
        else {
            var pram2 = {
                "PurchaseOrder_PtObj": JSON.stringify($scope.PurchaseOrder_PtObj),
                "PurchaseOrder_ChList": JSON.stringify($scope.PurchaseOrder_ChList),
                "OldPurchaseOrder_PtObj": JSON.stringify($scope.OldPurchaseOrder_PtObj),
                "Deleted_PurchaseOrder_ChList": JSON.stringify($scope.Deleted_PurchaseOrder_ChList),
            };
            JsonCallParam("PurchaseOrder", "UpdatePurchaseOrder", pram2)
            if (list == "Update") {
                $scope.UpdateTransaction();
                $scope.UpdateVendorRemaining();
            }
        }

        $scope.GetPurchaseOrder();
        $scope.Clear();
    };

    $scope.AddTransaction = function () {
        $scope.Transaction.Transaction_Debit = 0;
        $scope.Transaction.Transaction_Credit = $scope.PurchaseOrder_PtObj.POP_TotalPaid;
        $scope.Transaction.Transaction_Description = "Purchase Order: " + $scope.Transaction.Transaction_Description;

        if ($scope.Transaction.Transaction_Credit > 0) {
            var pram = {
                'Transaction': JSON.stringify($scope.Transaction),
                'isCash': $scope.isCash,
                'isBankAccount': $scope.isBankAccount,
                'isCheckbook': $scope.isCheckbook,
                'CheckNo': $scope.CheckNo,
                'BankAccountNo': $scope.BankAccountNo,
            };
            JsonCallParam("PurchaseOrder", "AddTransaction", pram);
        }
    };

    $scope.UpdateTransaction = function () {
        $scope.Transaction.Transaction_Debit = 0;
        $scope.Transaction.Transaction_Credit = $scope.PurchaseOrder_PtObj.POP_TotalPaid;
        $scope.Transaction.Transaction_Description = "Purchase Order: " + $scope.Transaction.Transaction_Description;

        if ($scope.Transaction.Transaction_Credit > 0) {
            var pram = {
                'Transaction': JSON.stringify($scope.Transaction),
                'POP_Id': JSON.stringify($scope.PurchaseOrder_PtObj.POP_Id),
                'isCash': $scope.isCash,
                'isBankAccount': $scope.isBankAccount,
                'isCheckbook': $scope.isCheckbook,
                'CheckNo': $scope.CheckNo,
                'BankAccountNo': $scope.BankAccountNo,
            };
            JsonCallParam("PurchaseOrder", "UpdateTransaction", pram);
        }
    };

    $scope.AddVendorRemaining = function () {
        var pram = {
            "Vendor_Id": JSON.stringify($scope.PurchaseOrder_PtObj.Vendor_Id),
            "RemainingAmount": JSON.stringify($scope.AmountRemaining),
        };
        JsonCallParam("PurchaseOrder", "AddVendorRemaining", pram);
    };

    $scope.UpdateVendorRemaining = function () {
        var pram = {
            "Vendor_Id": JSON.stringify($scope.PurchaseOrder_PtObj.Vendor_Id),
            "RemainingAmount": JSON.stringify($scope.AmountRemaining),
            "RemainingAmountOld": JSON.stringify($scope.AmountRemainingOld),
        };
        JsonCallParam("PurchaseOrder", "UpdateVendorRemaining", pram);
    };

    $scope.ShowDescription = function () {
        $scope.Transaction.Transaction_Description = "";
        var count = 0;

        angular.forEach($scope.PurchaseOrder_ChList, function (obj) {

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

    $scope.GetVendors = function () {
        JsonCall("PurchaseOrder", "GetVendors");
        $scope.VendorData = list;
    };
    $scope.GetVendors();

    $scope.GetSaleOrder = function () {
        JsonCall("PurchaseOrder", "GetSaleOrder");
        $scope.SaleOrders = list;
    };
    $scope.GetSaleOrder();

    $scope.GetPurchaseOrder = function () {
        JsonCall("PurchaseOrder", "GetPurchaseOrder");
        $scope.Grid = list;
    };
    $scope.GetPurchaseOrder();

    $scope.GetGST = function () {
        JsonCall("Home", "GetGST");
        $scope.PurchaseOrder_PtObj.POP_GST = list;
    };
    $scope.GetGST();

    $scope.SaleOrderApprove = function (sopId) {
        JsonCallParam("PurchaseOrder", "SaleOrderApprove", { "soId": sopId });
        $scope.GetSaleOrder();
    };

    $scope.StatusChangeSO_FillPOC = function (obj) {
        JsonCallParam("PurchaseOrder", "StatusChangeSO_FillPOC", { "soId": obj.SOP_Id });
        document.getElementById("collapsePendingSO").click();
        $scope.SaleOrderPt_Id = obj.SOP_Id;
        $scope.PurchaseOrder_PtObj.POP_PO = obj.SOP_SO;
        $scope.PurchaseOrder_ChList = [];
        angular.forEach(list, function (obj, index) {
            $scope.PurchaseOrder_ChList.push({
                'POC_Id': 0,
                'POP_Id': 0,
                'PurchaseOrder_Pt': {},
                'Product_Id': obj.Product_Id,
                'Product': obj.Product,
                'POC_Quantity': obj.SOC_Quantity,
                'POC_Rate': obj.Product.Product_UnitPrice,
                'POC_Description': obj.SOC_Description,
                'POC_Amount': 0,
                'POC_ItemCode': obj.SOC_ItemCode,
                'POC_Unit': obj.SOC_Unit,
            });
            $scope.QuantityChange(index);
        });
        $scope.ShowDescription();
        $scope.ValidatePurchaseOrder();
    };

    $scope.QuantityChange = function (i) {
        if ($scope.PurchaseOrder_ChList[i].POC_Quantity < 0)
            $scope.PurchaseOrder_ChList[i].POC_Quantity = 0;

        $scope.PurchaseOrder_ChList[i].POC_Amount = $scope.PurchaseOrder_ChList[i].POC_Quantity * $scope.PurchaseOrder_ChList[i].POC_Rate;
        $scope.CalculateTotal();
    };

    $scope.CalculateTotal = function () {
        $scope.PurchaseOrder_PtObj.POP_TotalQuantity = 0;
        $scope.PurchaseOrder_PtObj.POP_TotalAmount = 0;

        angular.forEach($scope.PurchaseOrder_ChList, function (obj) {
            $scope.PurchaseOrder_PtObj.POP_TotalQuantity += obj.POC_Quantity;
            $scope.PurchaseOrder_PtObj.POP_TotalAmount += obj.POC_Amount;
        });

        $scope.CalculateTax();
        $scope.NetTotal = $scope.PurchaseOrder_PtObj.POP_TotalAmount + $scope.PurchaseOrder_PtObj.POP_Charges + $scope.PurchaseOrder_PtObj.POP_TaxAmount;
    };

    $scope.CalculateTax = function () {
        if ($scope.PurchaseOrder_PtObj.POP_GST > 0 && $scope.PurchaseOrder_PtObj.POP_TotalAmount > 0) {
            //$scope.PurchaseOrder_PtObj.POP_TaxAmount = ($scope.PurchaseOrder_PtObj.POP_TotalAmount / 100) * $scope.PurchaseOrder_PtObj.POP_GST;
            $scope.PurchaseOrder_PtObj.POP_TaxAmount = 0;
        }
        else {
            $scope.PurchaseOrder_PtObj.POP_TaxAmount = 0;
        }
    };

    $scope.Clear = function () {
        $scope.Transaction = { Transaction_Description: "", Transaction_Debit: 0, Transaction_Credit: 0 };
        $scope.PurchaseOrder_PtObj = { POP_Id: 0, Vendor_Id: 0, Vendor: {}, POP_TotalQuantity: 0, POP_TotalAmount: 0, POP_TotalPaid: 0, POP_Charges: 0, POP_GST: 0, POP_TaxPercent: 0, POP_TaxAmount: 0, POP_PO: "" };
        $scope.PurchaseOrder_ChList = [];
        $scope.AllowSubmit = false;
        $scope.isCash = true;
        $scope.isBankAccount = false;
        $scope.isCheckbook = false;
        $scope.NetTotal = 0;
        $scope.AmountRemaining = 0;
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
        angular.forEach($scope.PurchaseOrder_ChList, function (obj) {
            obj.selected = $scope.selectedAll;
        });
    };

    $scope.Deleted_PurchaseOrder_ChList = [];
    $scope.Remove = function () {
        var newDataList = [];
        $scope.selectedAll = false;
        angular.forEach($scope.PurchaseOrder_ChList, function (obj) {
            if (!obj.selected) {
                newDataList.push(obj);
            }
            else {
                if (obj.POC_Id > 0) {
                    $scope.Deleted_PurchaseOrder_ChList.push(obj);
                }
            }
        });
        $scope.PurchaseOrder_ChList = newDataList;
        $scope.CalculateTotal();
        $scope.ShowDescription();
        $scope.ValidatePurchaseOrder();
    };

    $scope.ValidatePurchaseOrder = function () {
        if ($scope.PurchaseOrder_PtObj.POP_TotalPaid >= $scope.NetTotal) {
            $scope.PurchaseOrder_PtObj.POP_TotalPaid = $scope.NetTotal;
        }
        if ($scope.PurchaseOrder_PtObj.POP_TotalPaid <= 0) {
            $scope.PurchaseOrder_PtObj.POP_TotalPaid = 0;
        }
        $scope.AmountRemaining = $scope.NetTotal - $scope.PurchaseOrder_PtObj.POP_TotalPaid;
        $scope.PurchaseOrder_PtObj.Vendor_Id = Vendor_Id;

        if ($scope.PurchaseOrder_PtObj.POP_TotalPaid >= 0 && $scope.PurchaseOrder_PtObj.POP_PO != "") {
            $scope.AllowSubmit = false;
        }
        else {
            $scope.AllowSubmit = true;
        }
    };

    $scope.LoadPurchaseOrder = function (obj) {
        $scope.PurchaseOrder_PtObj = obj.Item1;
        $scope.NetTotal = obj.Item1.POP_TotalAmount;
        JsonCallParam("PurchaseOrder", "LoadPurchaseOrder", { "PoPtId": obj.Item1.POP_Id })
        $scope.PurchaseOrder_ChList = list;
        $scope.CalculateTotal();
        $scope.ShowDescription();
        $scope.ValidatePurchaseOrder();
        $scope.OldPurchaseOrder_PtObj = obj.Item1;
        $scope.AmountRemainingOld = obj.Item1.POP_TotalAmount - obj.Item1.POP_TotalPaid;
    };

    $scope.DeletePurchaseOrder = function (obj) {
        if (confirm("Are you sure you want to delete purchase order?")) {
            JsonCallParam("PurchaseOrder", "DeletePurchaseOrder", { "PoPtId": obj.PurchaseOrder_PtObj.POP_Id })
            $scope.GetPurchaseOrder();
        }
    };
});