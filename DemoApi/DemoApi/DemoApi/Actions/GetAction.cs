using Giftango.Domain.Models;
using Giftango.Domain.Reader;
using Giftango.Domain.Writer;

namespace Giftango.Domain.Actions
{
     public interface IDrinkGetAction
     {
         readonly IDrinkReader _reader;
         readonly IDrinkWriter _writer;

         List<Drink> GetAll();
         Drink Get(int Id);
     }

     public class DrinkGetAction : IDrinkGetAction
     {
         private readonly IDrinkReader _reader;
         private readonly IDrinkWriter _writer;

         public DrinkGetAction() : this(new DrinkReader(), new DrinkWriter())
         { }

         public DrinkGetAction (IDrinkReader reader, IDrinkWriter writer)
         {
           _reader = reader;
           _writer = writer;
         }

         public List<Drink> GetAll()
         {
             return _reder.GetAll();
         }
         public Drink Get(int Id)
         {
             return _reder.Get(Id);
         }
     }

}
