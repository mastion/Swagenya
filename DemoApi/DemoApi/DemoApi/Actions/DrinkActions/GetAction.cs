using System.Collections.Generic;
using Giftango.Domain.Models;
using Giftango.Domain.Writer;
using Giftango.Domain.Reader;

namespace Giftango.Domain.Actions
{
     public interface IDrinkGetAction
     {

         List<Drink> GetAll();
         Drink Get(int Id);
     }

     public class DrinkGetAction : IDrinkGetAction
     {
         private readonly DrinkReader _reader;


         public DrinkGetAction() : this(new DrinkReader())
         { }

         public DrinkGetAction (DrinkReader reader)
         {
           _reader = reader;
         }

         public List<Drink> GetAll()
         {
             return _reader.GetAll();
         }
         public Drink Get(int Id)
         {
             return _reader.GetById(Id);
         }
     }

}
