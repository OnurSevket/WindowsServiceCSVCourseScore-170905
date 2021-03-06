USE [master]
GO
/****** Object:  Database [DBLutegSchool]    Script Date: 6.09.2017 01:30:02 ******/
CREATE DATABASE [DBLutegSchool]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DBLutegSchool', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\DBLutegSchool.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'DBLutegSchool_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\DBLutegSchool_log.ldf' , SIZE = 24384KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [DBLutegSchool] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DBLutegSchool].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DBLutegSchool] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DBLutegSchool] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DBLutegSchool] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DBLutegSchool] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DBLutegSchool] SET ARITHABORT OFF 
GO
ALTER DATABASE [DBLutegSchool] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DBLutegSchool] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DBLutegSchool] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DBLutegSchool] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DBLutegSchool] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DBLutegSchool] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DBLutegSchool] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DBLutegSchool] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DBLutegSchool] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DBLutegSchool] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DBLutegSchool] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DBLutegSchool] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DBLutegSchool] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DBLutegSchool] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DBLutegSchool] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DBLutegSchool] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DBLutegSchool] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DBLutegSchool] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [DBLutegSchool] SET  MULTI_USER 
GO
ALTER DATABASE [DBLutegSchool] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DBLutegSchool] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DBLutegSchool] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DBLutegSchool] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [DBLutegSchool] SET DELAYED_DURABILITY = DISABLED 
GO
USE [DBLutegSchool]
GO
/****** Object:  User [NT AUTHORITY\SYSTEM]    Script Date: 6.09.2017 01:30:02 ******/
CREATE USER [NT AUTHORITY\SYSTEM] FOR LOGIN [NT AUTHORITY\SYSTEM] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [NT AUTHORITY\SYSTEM]
GO
/****** Object:  Table [dbo].[StudentCourse]    Script Date: 6.09.2017 01:30:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentCourse](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Course] [nvarchar](20) NOT NULL,
	[StudentNumber] [nvarchar](10) NOT NULL,
	[Midterm1] [nvarchar](10) NOT NULL,
	[Midterm2] [nvarchar](10) NOT NULL,
	[Midterm3] [nvarchar](10) NOT NULL,
	[Final] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_StudentCourse] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllStudentScore]    Script Date: 6.09.2017 01:30:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[sp_GetAllStudentScore]
                as
				select ID,Course,StudentNumber,Midterm1,Midterm2,Midterm3,Final from StudentCourse
GO
/****** Object:  StoredProcedure [dbo].[sp_GetStudentScore]    Script Date: 6.09.2017 01:30:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 create procedure [dbo].[sp_GetStudentScore]
         @id int
         as
         select ID,Course,StudentNumber,Midterm1,Midterm2,Midterm3,Final
          from StudentCourse where ID=@id

GO
/****** Object:  StoredProcedure [dbo].[sp_InsertStudentScore]    Script Date: 6.09.2017 01:30:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[sp_InsertStudentScore]
  @course nvarchar(50), 
  @studentNumber nvarchar(50),
  @midterm1 nvarchar(50),
  @midterm2 nvarchar(50),
  @midterm3 nvarchar(50),
  @final nvarchar(50)
  as
  begin
      insert into StudentCourse(Course, StudentNumber, Midterm1, Midterm2, Midterm3, Final) 
	  values (@course, @studentNumber, @midterm1, @midterm2, @midterm3, @final)
  end

GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateStudentScore]    Script Date: 6.09.2017 01:30:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 create proc [dbo].[sp_UpdateStudentScore]
                @id int,
				@course nvarchar(50), 
				@studentNumber nvarchar(50),
				@midterm1 nvarchar(50),
				@midterm2 nvarchar(50),
				@midterm3 nvarchar(50),
				@final nvarchar(50)
                as
                begin
                Update StudentCourse set Course=@course,StudentNumber=@studentNumber,Midterm1=@midterm1,Midterm2=@midterm2,Midterm3=@midterm3,Final=@final where ID=@id
                end

GO
USE [master]
GO
ALTER DATABASE [DBLutegSchool] SET  READ_WRITE 
GO
