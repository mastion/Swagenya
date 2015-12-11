

CREATE TABLE [dbo].[Drink] (
    [Id]          INT			 IDENTITY (0, 1) NOT NULL,
    [Name]        VARCHAR (MAX)  NOT NULL,
    [Size]        VARCHAR (MAX)  NOT NULL,
    [Price]		  MONEY			 NOT NULL,
    CONSTRAINT [PK_Drink] PRIMARY KEY CLUSTERED ([Id] ASC)
);


--exec InsertDrink 'AlexDrink', 'ExtraLarge', 99.00
--exec GetDrink 1
--exec GetAllDrink

--select * from dbo.Drink

