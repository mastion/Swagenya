using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Giftango.Domain.Models;
using Giftango.Component.Utility;
namespace Giftango.Domain.Reader
{
   public class ExtraReader
   {
      public List<Extra> GetAll()
      {
         using (var connection = ConnectionHelper.GetConnection())
         {
            return connection.Query<Extra>("[dbo].[GetAllExtra]", commandType: CommandType.StoredProcedure);
         }
      }
      public Extra GetById(int id)
      {
         using (var connection = ConnectionHelper.GetConnection())
         {
            return connection.Query<Extra>("[dbo].[GetExtra]", new {id}, commandType: CommandType.StoredProcedure);
         }
      }
   }
}
