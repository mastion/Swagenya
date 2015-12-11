CREATE PROCEDURE [dbo].[GetDrink] (
   @Id INT
   )
AS
SET NOCOUNT ON

SELECT name,size,price
FROM dbo.Drink
WHERE Id = @Id
GO

GRANT EXECUTE
   ON OBJECT::[dbo].[GetDrink]
   TO [GenericRole] AS [dbo];
