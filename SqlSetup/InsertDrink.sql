CREATE PROCEDURE [dbo].[InsertDrink] (
@name VARCHAR(MAX),
@size VARCHAR(MAX),
@price MONEY
   )
AS
SET NOCOUNT ON

INSERT INTO dbo.Drink (
name,size,price
)
SELECT @name,@size,@price
GO

GRANT EXECUTE
   ON OBJECT::[dbo].[InsertDrink]
   TO [GenericRole] AS [dbo];
