using Giftango.Domain.Models;
using Giftango.Domain.Reader;
using Giftango.Domain.Writer;

namespace Giftango.Domain.Actions
{
     public interface IDrinkUpdateAction
     {
         readonly IDrinkReader _reader;
         readonly IDrinkWriter _writer;

         int WriteById(int Id Drink data);
     }

     public class DrinkUpdateAction : IDrinkUpdateAction
     {
         private readonly IDrinkReader _reader;
         private readonly IDrinkWriter _writer;

         public DrinkUpdateAction() : this(new DrinkReader(), new DrinkWriter())
         { }

         public DrinkUpdateAction (IDrinkReader reader, IDrinkWriter writer)
         {
           _reader = reader;
           _writer = writer;
         }

         public int WriteById(int Id Drink data)
         {
             return _writer.Write(Id, data);
         }
     }

}
