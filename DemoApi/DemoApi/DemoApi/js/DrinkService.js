'use strict'
function DrinkService($http) {
  this.getDrinks = function () {
      return $http.get('drink');
  };
  this.addDrink = function (Drink) {
      return $http({
          url: 'drink',
          method: 'post',
          data: Drink
      });
  };
  this.addDrink = function (Drink) {
      return $http({
          url: 'extra',
          method: 'post',
          data: Drink
      });
  };
}
angular.module('Drink').service('DrinkService', ['$http', DrinkService]);
