'use strict';

angular.module('MetaQuotesApp', [])
  .controller('MainController', function($scope, $http){
    $scope.searchByCityVisible = true;
    $scope.searchByIpVisible = false;

    $scope.searchByCity = function(){
        $http({
            url: "http://127.0.0.1:5961/city/locations", 
            method: "GET",
            params: {city: $scope.searchText}
        }).then(function(response){ $scope.searchResult = response.data; });

    };

    $scope.search = "Sherlock Holmes";

    function fetch(){
      $http.get("http://www.omdbapi.com/?t=" + $scope.search + "&tomatoes=true&plot=full")
      .then(function(response){ $scope.details = response.data; });

      $http.get("http://www.omdbapi.com/?s=" + $scope.search)
      .then(function(response){ $scope.related = response.data; });
    }

    $scope.update = function(movie){
      $scope.search = movie.Title;
    };

    $scope.select = function(){
      this.setSelectionRange(0, this.value.length);
    }
  });
