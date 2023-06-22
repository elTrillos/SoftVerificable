Create Database InscripcionesBrDb
GO

USE [InscripcionesBrDb]
GO

CREATE TABLE [dbo].[Persona](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Rut] [nvarchar](10) NULL,
	[Nombre] [nvarchar](50) NOT NULL,
	[FechaNacimiento] [date] NOT NULL,
	[Email] [nchar](50) NOT NULL,
	[Dirección] [nchar](50) NULL,
 CONSTRAINT [PK_Persona] PRIMARY KEY CLUSTERED(
	[Id] ASC
))
GO
CREATE TABLE [dbo].[Escritura] (
    [NumeroAtencion] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [CNE] [nvarchar](50) NOT NULL,
    [Comuna] [nvarchar](50) NOT NULL,
    [Manzana] [nvarchar](50) NOT NULL,
    [Predio] [nvarchar](50) NOT NULL,
    [Fojas] [int] NOT NULL,
    [FechaInscripcion] [date] NOT NULL,
    [NumeroInscripcion] [nvarchar](50) NOT NULL,
	[Estado] [nvarchar](50) NOT NULL
);
GO
CREATE TABLE [dbo].[Enajenante] (
    [id] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [NumeroAtencion] [int] NOT NULL,
    [RunRut] [nvarchar](20) NOT NULL,
    [PorcentajeDerecho] DECIMAL(5,2) NOT NULL,
    [DerechoNoAcreditado] BIT NOT NULL,
    FOREIGN KEY (NumeroAtencion) REFERENCES Escritura(NumeroAtencion)
);
GO
CREATE TABLE [dbo].[Adquiriente] (
    [Id] [int] PRIMARY KEY IDENTITY(1,1),
    [NumeroAtencion] [int] NOT NULL,
    [RunRut] [nvarchar](20) NOT NULL,
    [PorcentajeDerecho] DECIMAL(5,2) NOT NULL,
    [DerechoNoAcreditado] BIT NOT NULL,
    FOREIGN KEY (NumeroAtencion) REFERENCES Escritura(NumeroAtencion)
);
GO
CREATE TABLE [dbo].[Multipropietario] (
    [Id] [int] PRIMARY KEY IDENTITY(1,1),
    [Comuna] [nvarchar](50) NOT NULL,
    [Manzana] [nvarchar](50) NOT NULL,
    [Predio] [nvarchar](50) NOT NULL,
    [RunRut] [nvarchar](20) NOT NULL,
    [PorcentajeDerecho] DECIMAL(5,2) NOT NULL,
    [Fojas] [int] NOT NULL,
    [AñoInscripcion] [int] NOT NULL,
    [NumeroInscripcion] [int] NOT NULL,  
    [FechaInscripcion] [date] NOT NULL,
    [AñoVigenciaInicial] [int] NOT NULL,
    [AñoVigenciaFinal] [int] NOT NULL,
);
GO
USE [InscripcionesBrDb]
GO
SET IDENTITY_INSERT [dbo].[Persona] ON 
GO
INSERT [dbo].[Persona] ([Id], [Rut], [Nombre], [FechaNacimiento], [Email], [Dirección]) VALUES (1, N'10915348-6', N'Mario Abellan', CAST(N'1982-01-15' AS Date), N'marioabellan@gmail.com                            ', N'Galvarino Gallardo 1534                           ')
GO
SET IDENTITY_INSERT [dbo].[Persona] OFF
GO




