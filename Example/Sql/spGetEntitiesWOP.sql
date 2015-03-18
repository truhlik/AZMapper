USE [Test]
GO

/****** Object:  StoredProcedure [dbo].[GetEntitiesWOP]    Script Date: 03/17/2015 21:50:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetEntitiesWOP]
	-- Add the parameters for the stored procedure here
	@maxid int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT @maxid = MAX(id) 
	FROM mapper_ex_person;
END

GO


