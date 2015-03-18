USE [Test]
GO

/****** Object:  StoredProcedure [dbo].[GetEntities]    Script Date: 03/17/2015 21:50:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetEntities] 
	-- Add the parameters for the stored procedure here
	@birthData DateTime = GETDATE
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM mapper_ex_person;
	
	SELECT * FROM mapper_ex_person WHERE dateofbirth > @birthData;
END

GO


