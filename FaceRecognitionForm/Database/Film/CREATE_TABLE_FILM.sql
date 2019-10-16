USE [FaceRecognition]
GO

/****** Object:  Table [dbo].[Film]    Script Date: 16/10/2019 11:44:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Film](
	[Title] [varchar](max) NULL,
	[Duration] [varchar](max) NULL,
	[DirectorName] [varchar](max) NULL,
	[Director_facebook_likes] [varchar](max) NULL,
	[Actor_1_Name] [varchar](max) NULL,
	[Actor_1_facebook_likes] [varchar](max) NULL,
	[Actor_2_Name] [varchar](max) NULL,
	[Actor_2_facebook_likes] [varchar](max) NULL,
	[Actor_3_Name] [varchar](max) NULL,
	[Actor_3_facebook_likes] [varchar](max) NULL,
	[Num_user_for_reviews] [varchar](max) NULL,
	[Num_critic_for_reviews] [varchar](max) NULL,
	[Num_voted_users] [varchar](max) NULL,
	[Cast_total_facebook_likes] [varchar](max) NULL,
	[Movie_facebook_likes] [varchar](max) NULL,
	[Plot_keywords] [varchar](max) NULL,
	[Facenumber_in_poster] [varchar](max) NULL,
	[Color] [varchar](max) NULL,
	[Genres] [varchar](max) NULL,
	[Title_year] [varchar](max) NULL,
	[Language] [varchar](max) NULL,
	[Country] [varchar](max) NULL,
	[Content_rating] [varchar](max) NULL,
	[Aspect_ratio] [varchar](max) NULL,
	[Movie_imbd_link] [varchar](max) NULL,
	[Gross] [varchar](max) NULL,
	[Budget] [varchar](max) NULL,
	[Imdb_score] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


