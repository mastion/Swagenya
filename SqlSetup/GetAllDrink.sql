CREATE PROCEDURE [dbo].[GetAllDrink]
AS
SET NOCOUNT ON

SELECT name,size,price
FROM dbo.Drink
GO

GRANT EXECUTE
   ON OBJECT::[dbo].[GetAllDrink]
   TO [GenericRole] AS [dbo];
