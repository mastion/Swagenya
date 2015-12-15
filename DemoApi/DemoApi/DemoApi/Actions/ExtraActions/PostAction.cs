using System.Collections.Generic;
using Giftango.Domain.Models;
using Giftango.Domain.Writer;
using Giftango.Domain.Reader;

namespace Giftango.Domain.Actions
{
     public interface IExtraPostAction
     {

         int Write(Extra data);
     }

     public class ExtraPostAction : IExtraPostAction
     {
         private readonly ExtraWriter _writer;


         public ExtraPostAction() : this(new ExtraWriter())
         { }

         public ExtraPostAction (ExtraWriter writer)
         {
           _writer = writer;
         }

         public int Write(Extra data)
         {
             return _writer.Write(data);
         }
     }

}
