var dashboard = angular.module('dashboard', ['ngRoute']);

dashboard.directive('fileModel', ['$parse', function ($parse) {
    return {
        link: function (scope, element, attrs) {
            var model = $parse(attrs.fileModel);
            var modelSetter = model.assign;

            element.bind('change', function () {
                scope.$apply(function () {
                    modelSetter(scope, element[0].files[0]);
                });
            });
        }
    };
}]);

dashboard.controller('job-list-controller', function ($scope, $http) {

    $scope.jobs = [];

    $scope.onDataReceived = function (data) {
        $scope.jobs = data;
    }

    $http.get('/api/jobs').success($scope.onDataReceived);
});

dashboard.controller("job-details-controller", function ($scope, $http, $routeParams, $location) {

    $scope.id = $routeParams.jobId;

    $scope.settings = {
        isReadOnly: true
    };

    $scope.onDataReceived = function (data) {
        $scope.version = data.version;
        $scope.name = data.name;
        $scope.description = data.description;
        $scope.executable = data.executable;
        $scope.arguments = data.arguments;
    }

    $scope.edit = function () {
        $location.path('/jobs/' + $scope.id + '/edit');
    }

    $scope.cancel = function () {
        $location.path('/jobs');
    }

    $http.get('/api/jobs/' + $scope.id).success($scope.onDataReceived);
});

dashboard.controller("job-new-controller", function ($scope, $http, $location) {

    $scope.settings = {
        isCreatable: true
    };

    $scope.onJobCreated = function (data, status) {
        $scope.cancel();
    };

    $scope.onBlobCreated = function (data, status) {
        $http.post('/api/jobs', {
            name: $scope.name,
            description: $scope.description,
            executable: $scope.executable,
            arguments: $scope.arguments,
            blob: data.id
        }).success($scope.onJobCreated);
    };

    $scope.post = function () {
        $http.post('/api/blobs', $scope.blob, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': 'application/octet-stream' }
        }).success($scope.onBlobCreated);
    }

    $scope.cancel = function () {
        $location.path('/jobs');
    }

});

dashboard.controller("job-edit-controller", function ($scope, $http, $routeParams, $location) {

    $scope.id = $routeParams.jobId;

    $scope.settings = {
        isDeletable: true,
        isUpdateable: true
    };

    $scope.onDataReceived = function (data) {
        $scope.version = data.version;
        $scope.name = data.name;
        $scope.description = data.description;
        $scope.executable = data.executable;
        $scope.arguments = data.arguments;
    }

    $scope.onJobUpdated = function (data) {
    }

    $scope.onJobDeleted = function () {
        $scope.home();
    }

    $scope.post = function () {
        $http.patch('/api/jobs/' + $scope.id, {
            name: $scope.name,
            description: $scope.description,
            executable: $scope.executable,
            arguments: $scope.arguments
        }).success($scope.onJobUpdated);
    }

    $scope.delete = function () {
        $http.delete('/api/jobs/' + $scope.id).success($scope.onJobDeleted);
    }

    $scope.cancel = function () {
        $location.path('/jobs/' + $scope.id);
    }

    $scope.home = function () {
        $location.path('/jobs');
    }
    
    $http.get('/api/jobs/' + $scope.id).success($scope.onDataReceived);

});

dashboard.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
         when('/', {
             templateUrl: 'job-list',
             controller: 'job-list-controller'
         }).
         when('/jobs', {
             templateUrl: 'job-list',
             controller: 'job-list-controller'
         }).
         when('/jobs/new', {
             templateUrl: 'job-view',
             controller: 'job-new-controller'
         }).
         when('/jobs/:jobId', {
             templateUrl: 'job-view',
             controller: 'job-details-controller'
         }).
         when('/jobs/:jobId/edit', {
             templateUrl: 'job-view',
             controller: 'job-edit-controller'
         })
  }]);