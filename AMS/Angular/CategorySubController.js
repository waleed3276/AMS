﻿App.controller("CategorySubCtrl", function ($scope, $http) {

    $scope.list = [];
    $scope.CategorySubData = [];
    $scope.CategoryData = [];

    $scope.AllowSubmit = false;
    $scope.CategorySubObj = { CategorySub_Id: 0, Category_Id: 0, CategorySub_Title: "", CategorySub_Code: "", CategorySub_Description: "" };

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

    $scope.SaveCategorySub = function () {
        $scope.CategorySubObj.Category_Id = Category_Id;
        var pram = { "CategorySubObj": JSON.stringify($scope.CategorySubObj) };

        if ($scope.CategorySubObj.CategorySub_Id == 0) {
            JsonCallParam("CategoriesSub", "CreateCategorySub", pram)
        }
        else {
            JsonCallParam("CategoriesSub", "UpdateCategorySub", pram)
        }

        $scope.GetCategorySub();
        $scope.Clear();
    };

    $scope.GetCategorySub = function () {
        JsonCall("CategoriesSub", "GetCategorySub");
        $scope.CategorySubData = list;
    };
    $scope.GetCategorySub();

    $scope.GetCategories = function () {
        JsonCall("CategoriesSub", "GetCategory");
        $scope.CategoryData = list;
    }
    $scope.GetCategories();

    $scope.Clear = function () {
        $scope.CategorySubObj = { CategorySub_Id: 0, Category_Id: 0, CategorySub_Title: "", CategorySub_Code: "", CategorySub_Description: "" }
    };

    $scope.ValidateCategorySub = function () {
        $scope.CategorySubObj.Category_Id = Category_Id;
        if ($scope.CategorySubObj.CategorySub_Title == ""
            || $scope.CategorySubObj.CategorySub_Code == ""
            || $scope.CategorySubObj.CategorySub_Description == ""
            || !$scope.CategorySubObj.Category_Id > 0) {
            $scope.AllowSubmit = false;
        }
        else {
            $scope.AllowSubmit = true;
        }
    };

    $scope.LoadCategorySub = function (obj) {
        $scope.CategorySubObj = obj;
        $scope.ValidateCategorySub();
    };

    $scope.DeleteCategorySub = function (obj) {
        if (confirm("Are you sure you want to delete sub-category?")) {
            JsonCallParam("CategoriesSub", "DeleteCategorySub", { "id": obj.CategorySub_Id })
            $scope.GetCategorySub();
        }
    };
});