using Giftango.Domain.Models;
using Giftango.Domain.Reader;
using Giftango.Domain.Writer;

namespace Giftango.Domain.Actions
{
     public interface IDrinkPostAction
     {
         readonly IDrinkReader _reader;
         readonly IDrinkWriter _writer;

         int Write(Drink data);
     }

     public class DrinkPostAction : IDrinkPostAction
     {
         private readonly IDrinkReader _reader;
         private readonly IDrinkWriter _writer;

         public DrinkPostAction() : this(new DrinkReader(), new DrinkWriter())
         { }

         public DrinkPostAction (IDrinkReader reader, IDrinkWriter writer)
         {
           _reader = reader;
           _writer = writer;
         }

         public int Write(Drink data)
         {
             return _writer.Write(data);
         }
     }

}
