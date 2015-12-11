'use strict'
function DrinkController($scope, DrinkService) {
  $scope.getDrinks = function() {
      $scope.currentDrinks = [];
      DrinkService.getDrinks()
      then(function(response){
          $scope.currentDrinks = new Drinks(response.data);
      }, function (error){
          $scope.ErrorMessage = error.data.ExceptionMessage;
      });
  };
  $scope.addDrink = function() {
      DrinkService
          .addDrink($scope.DrinkToAdd)
          .then(function (response) {
              $scope.currentDrinks.push(new Drink(response.data);
              $scope.DrinkToAdd = {}
          }, function (error) {
              $scope.ErrorMessage = 'Error adding the new Drink';
          });
  };
  $scope.addDrink = function() {
      DrinkService
          .addDrink($scope.DrinkToAdd)
          .then(function (response) {
              $scope.currentDrinks.push(new Drink(response.data);
              $scope.DrinkToAdd = {}
          }, function (error) {
              $scope.ErrorMessage = 'Error adding the new Drink';
          });
  };
};
angular.module('Drink').controller('DrinkController', ['$scope', 'DrinkService', DrinkController]);
