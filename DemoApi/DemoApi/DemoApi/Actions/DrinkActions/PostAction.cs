using System.Collections.Generic;
using Giftango.Domain.Models;
using Giftango.Domain.Writer;
using Giftango.Domain.Reader;

namespace Giftango.Domain.Actions
{
     public interface IDrinkPostAction
     {

         int Write(Drink data);
     }

     public class DrinkPostAction : IDrinkPostAction
     {
         private readonly DrinkWriter _writer;


         public DrinkPostAction() : this(new DrinkWriter())
         { }

         public DrinkPostAction (DrinkWriter writer)
         {
           _writer = writer;
         }

         public int Write(Drink data)
         {
             return _writer.Write(data);
         }
     }

}
