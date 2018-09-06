App.controller("VendorCtrl", function ($scope, $http) {

    $scope.list = [];
    $scope.VendorData = [];

    $scope.AllowSubmit = false;
    $scope.VendorObj = { Vendor_Id: 0, Vendor_Name: "", Vendor_MobileNo: "", Vendor_Address: "", Vendor_NTN: "", Vendor_Company: "" };

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

    $scope.SaveVendor = function () {
        var pram = { "VendorObj": JSON.stringify($scope.VendorObj) };

        if ($scope.VendorObj.Vendor_Id == 0) {
            JsonCallParam("Account", "Register2", { "UserRole": JSON.stringify("Vendor"), "Username": JSON.stringify($scope.VendorObj.Vendor_Name) })
            if (list != null)
                JsonCallParam("Vendors", "CreateVendor", pram)
        }
        else {
            JsonCallParam("Vendors", "UpdateVendor", pram)
        }

        $scope.GetVendor();
        $scope.Clear();
    };

    $scope.GetVendor = function () {
        JsonCall("Vendors", "GetVendor");
        $scope.VendorData = list;
    };

    $scope.GetVendor();

    $scope.Clear = function () {
        $scope.VendorObj = { Vendor_Id: 0, Vendor_Name: "", Vendor_MobileNo: "", Vendor_Address: "", Vendor_NTN: "", Vendor_Company: "" };
    };

    $scope.ValidateVendor = function () {
        if ($scope.VendorObj.Vendor_Name == ""
            || $scope.VendorObj.Vendor_MobileNo == ""
            || $scope.VendorObj.Vendor_Address == ""
            || $scope.VendorObj.Vendor_NTN == ""
            || $scope.VendorObj.Vendor_Company == "") {
            $scope.AllowSubmit = false;
        }
        else {
            $scope.AllowSubmit = true;
        }
    }

    $scope.LoadVendor = function (obj) {
        $scope.VendorObj = obj;
        $scope.ValidateVendor();
    }

    $scope.DeleteVendor = function (obj) {
        if (confirm('Are you sure you want to delete vendor?')) {
            JsonCallParam("Vendors", "DeleteVendor", { "id": obj.Vendor_Id })
            $scope.GetVendor();
        } 
    }

});