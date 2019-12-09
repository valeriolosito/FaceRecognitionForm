USE [FaceRecognition]
GO

/****** Object:  Table [dbo].[Feedback]    Script Date: 09/12/2019 15:11:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Feedback](
	[Feedback] [varchar](5) NULL,
	[CF] [varchar](16) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FK_FeedbackCF_PersonCF] FOREIGN KEY([CF])
REFERENCES [dbo].[Person] ([CF])
GO

ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FK_FeedbackCF_PersonCF]
GO


