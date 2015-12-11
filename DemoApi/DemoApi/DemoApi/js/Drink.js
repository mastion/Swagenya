function Drink(data){
  if (!data){
      this.name = null;
      this.size = null;
      this.price = null;
  }
  this.name = data.name || null;
  this.size = data.size || null;
  this.price = data.price || null;
}
function Drinks(data){
  var Drinks = [];
  if (data){
      Drinks = data.map(function(singleDrink){
          return new Drink(singleDrink);
      });
  }
  return Drinks;
}
angular.module('Drink').value('Drink', Drink).value('Drinks', Drinks);
