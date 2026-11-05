/* ============================================================================
   PROYECTO FINAL - AVANCE 2: SISTEMA DE MATRÍCULA UNIVERSITARIA (UniMatrícula)
   Curso: SC-601 Programación Avanzada - Universidad Fidélitas
   Estudiante: Mauricio Vásquez Loría (Cédula: 305220648)
   Motor: SQL Server / LocalDB
   ============================================================================ */

IF DB_ID(N'UniMatriculaDB') IS NULL
BEGIN
    CREATE DATABASE [UniMatriculaDB];
END
GO

USE [UniMatriculaDB];
GO

-- 1. Tabla de Carreras
IF OBJECT_ID(N'[dbo].[Carreras]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Carreras] (
        [Id]          INT            IDENTITY (1, 1) NOT NULL,
        [Nombre]      NVARCHAR (100) NOT NULL,
        [Descripcion] NVARCHAR (300) NOT NULL,
        [Activa]      BIT            NOT NULL DEFAULT ((1)),
        CONSTRAINT [PK_Carreras] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- 2. Tabla de Docentes
IF OBJECT_ID(N'[dbo].[Docentes]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Docentes] (
        [Id]           INT            IDENTITY (1, 1) NOT NULL,
        [Nombre]       NVARCHAR (100) NOT NULL,
        [Correo]       NVARCHAR (150) NOT NULL,
        [Especialidad] NVARCHAR (100) NOT NULL,
        CONSTRAINT [PK_Docentes] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Docentes_Correo] ON [dbo].[Docentes]([Correo] ASC);
END
GO

-- 3. Tabla de Cursos
IF OBJECT_ID(N'[dbo].[Cursos]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Cursos] (
        [Id]        INT            IDENTITY (1, 1) NOT NULL,
        [Codigo]    NVARCHAR (20)  NOT NULL,
        [Nombre]    NVARCHAR (120) NOT NULL,
        [Creditos]  INT            NOT NULL,
        [Tipo]      NVARCHAR (MAX) NOT NULL,
        [Cupos]     INT            NOT NULL,
        [DocenteId] INT            NULL,
        CONSTRAINT [PK_Cursos] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Cursos_Docentes_DocenteId] FOREIGN KEY ([DocenteId])
            REFERENCES [dbo].[Docentes] ([Id]) ON DELETE SET NULL
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Cursos_Codigo] ON [dbo].[Cursos]([Codigo] ASC);
    CREATE NONCLUSTERED INDEX [IX_Cursos_DocenteId] ON [dbo].[Cursos]([DocenteId] ASC);
END
GO

-- 4. Tabla intermedia CarreraCursos (Relación Muchos a Muchos)
IF OBJECT_ID(N'[dbo].[CarreraCursos]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[CarreraCursos] (
        [CarreraId] INT NOT NULL,
        [CursoId]   INT NOT NULL,
        CONSTRAINT [PK_CarreraCursos] PRIMARY KEY CLUSTERED ([CarreraId] ASC, [CursoId] ASC),
        CONSTRAINT [FK_CarreraCursos_Carreras_CarreraId] FOREIGN KEY ([CarreraId])
            REFERENCES [dbo].[Carreras] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CarreraCursos_Cursos_CursoId] FOREIGN KEY ([CursoId])
            REFERENCES [dbo].[Cursos] ([Id]) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX [IX_CarreraCursos_CursoId] ON [dbo].[CarreraCursos]([CursoId] ASC);
END
GO

-- 5. Tablas de ASP.NET Core Identity y Tabla Estudiantes (ApplicationUser)
IF OBJECT_ID(N'[dbo].[AspNetRoles]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetRoles] (
        [Id]               NVARCHAR (450) NOT NULL,
        [Name]             NVARCHAR (256) NULL,
        [NormalizedName]   NVARCHAR (256) NULL,
        [ConcurrencyStamp] NVARCHAR (MAX) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]([NormalizedName] ASC) WHERE ([NormalizedName] IS NOT NULL);
END
GO

IF OBJECT_ID(N'[dbo].[Estudiantes]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Estudiantes] (
        [Id]                   NVARCHAR (450)     NOT NULL,
        [Nombre]               NVARCHAR (80)      NOT NULL,
        [Apellidos]            NVARCHAR (80)      NOT NULL,
        [Cedula]               NVARCHAR (20)      NOT NULL,
        [CarreraId]            INT                NULL,
        [UserName]             NVARCHAR (256)     NULL,
        [NormalizedUserName]   NVARCHAR (256)     NULL,
        [Email]                NVARCHAR (256)     NULL,
        [NormalizedEmail]      NVARCHAR (256)     NULL,
        [EmailConfirmed]       BIT                NOT NULL,
        [PasswordHash]         NVARCHAR (MAX)     NULL,
        [SecurityStamp]        NVARCHAR (MAX)     NULL,
        [ConcurrencyStamp]     NVARCHAR (MAX)     NULL,
        [PhoneNumber]          NVARCHAR (MAX)     NULL,
        [PhoneNumberConfirmed] BIT                NOT NULL,
        [TwoFactorEnabled]     BIT                NOT NULL,
        [LockoutEnd]           DATETIMEOFFSET (7) NULL,
        [LockoutEnabled]       BIT                NOT NULL,
        [AccessFailedCount]    INT                NOT NULL,
        CONSTRAINT [PK_Estudiantes] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Estudiantes_Carreras_CarreraId] FOREIGN KEY ([CarreraId])
            REFERENCES [dbo].[Carreras] ([Id]) ON DELETE NO ACTION
    );
    CREATE NONCLUSTERED INDEX [IX_Estudiantes_Cedula] ON [dbo].[Estudiantes]([Cedula] ASC);
    CREATE NONCLUSTERED INDEX [IX_Estudiantes_CarreraId] ON [dbo].[Estudiantes]([CarreraId] ASC);
    CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[Estudiantes]([NormalizedEmail] ASC);
    CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[Estudiantes]([NormalizedUserName] ASC) WHERE ([NormalizedUserName] IS NOT NULL);
END
GO

IF OBJECT_ID(N'[dbo].[AspNetUserRoles]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetUserRoles] (
        [UserId] NVARCHAR (450) NOT NULL,
        [RoleId] NVARCHAR (450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId])
            REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_Estudiantes_UserId] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Estudiantes] ([Id]) ON DELETE CASCADE
    );
    CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]([RoleId] ASC);
END
GO

IF OBJECT_ID(N'[dbo].[AspNetRoleClaims]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetRoleClaims] (
        [Id]         INT            IDENTITY (1, 1) NOT NULL,
        [RoleId]     NVARCHAR (450) NOT NULL,
        [ClaimType]  NVARCHAR (MAX) NULL,
        [ClaimValue] NVARCHAR (MAX) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId])
            REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END
GO

IF OBJECT_ID(N'[dbo].[AspNetUserClaims]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetUserClaims] (
        [Id]         INT            IDENTITY (1, 1) NOT NULL,
        [UserId]     NVARCHAR (450) NOT NULL,
        [ClaimType]  NVARCHAR (MAX) NULL,
        [ClaimValue] NVARCHAR (MAX) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_AspNetUserClaims_Estudiantes_UserId] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Estudiantes] ([Id]) ON DELETE CASCADE
    );
END
GO

IF OBJECT_ID(N'[dbo].[AspNetUserLogins]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetUserLogins] (
        [LoginProvider]       NVARCHAR (450) NOT NULL,
        [ProviderKey]         NVARCHAR (450) NOT NULL,
        [ProviderDisplayName] NVARCHAR (MAX) NULL,
        [UserId]              NVARCHAR (450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC),
        CONSTRAINT [FK_AspNetUserLogins_Estudiantes_UserId] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Estudiantes] ([Id]) ON DELETE CASCADE
    );
END
GO

IF OBJECT_ID(N'[dbo].[AspNetUserTokens]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AspNetUserTokens] (
        [UserId]        NVARCHAR (450) NOT NULL,
        [LoginProvider] NVARCHAR (450) NOT NULL,
        [Name]          NVARCHAR (450) NOT NULL,
        [Value]         NVARCHAR (MAX) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [Name] ASC),
        CONSTRAINT [FK_AspNetUserTokens_Estudiantes_UserId] FOREIGN KEY ([UserId])
            REFERENCES [dbo].[Estudiantes] ([Id]) ON DELETE CASCADE
    );
END
GO

-- 6. Tabla de Matrículas
IF OBJECT_ID(N'[dbo].[Matriculas]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Matriculas] (
        [Id]             INT            IDENTITY (1, 1) NOT NULL,
        [EstudianteId]   NVARCHAR (450) NOT NULL,
        [CursoId]        INT            NOT NULL,
        [FechaMatricula] DATETIME2 (7)  NOT NULL,
        CONSTRAINT [PK_Matriculas] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Matriculas_Estudiantes_EstudianteId] FOREIGN KEY ([EstudianteId])
            REFERENCES [dbo].[Estudiantes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Matriculas_Cursos_CursoId] FOREIGN KEY ([CursoId])
            REFERENCES [dbo].[Cursos] ([Id]) ON DELETE NO ACTION
    );
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Matriculas_EstudianteId_CursoId] ON [dbo].[Matriculas]([EstudianteId] ASC, [CursoId] ASC);
    CREATE NONCLUSTERED INDEX [IX_Matriculas_CursoId] ON [dbo].[Matriculas]([CursoId] ASC);
END
GO

-- ============================================================================
-- DATOS SEMILLA INICIALES (Carreras, Docentes, Cursos y Asociaciones)
-- Nota: Al iniciar la aplicación, DbInitializer.cs también verifica e inserta
-- automáticamente los roles, el administrador, estudiantes de prueba y cursos.
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM [dbo].[Carreras])
BEGIN
    SET IDENTITY_INSERT [dbo].[Carreras] ON;
    INSERT INTO [dbo].[Carreras] ([Id], [Nombre], [Descripcion], [Activa]) VALUES
        (1, N'Ingeniería en Sistemas', N'Desarrollo de software', 1),
        (2, N'Administración de Empresas', N'Gestión empresarial', 1),
        (3, N'Contaduría Pública', N'Contabilidad y finanzas', 1),
        (4, N'Mercadeo', N'Marca y mercados', 1),
        (5, N'Derecho', N'Ciencias jurídicas', 1),
        (6, N'Psicología', N'Comportamiento humano y salud mental', 1);
    SET IDENTITY_INSERT [dbo].[Carreras] OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Docentes])
BEGIN
    SET IDENTITY_INSERT [dbo].[Docentes] ON;
    INSERT INTO [dbo].[Docentes] ([Id], [Nombre], [Correo], [Especialidad]) VALUES
        (1, N'M. Vargas', N'mvargas@uni.edu', N'Programación'),
        (2, N'L. Solano', N'lsolano@uni.edu', N'Matemática'),
        (3, N'C. Mena', N'cmena@uni.edu', N'Bases de datos'),
        (4, N'P. Rojas', N'projas@uni.edu', N'Idiomas'),
        (5, N'J. Araya', N'jaraya@uni.edu', N'Humanidades');
    SET IDENTITY_INSERT [dbo].[Docentes] OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Cursos])
BEGIN
    SET IDENTITY_INSERT [dbo].[Cursos] ON;
    INSERT INTO [dbo].[Cursos] ([Id], [Codigo], [Nombre], [Creditos], [Tipo], [Cupos], [DocenteId]) VALUES
        (1, N'SIS-101', N'Programación I', 4, N'Obligatorio', 12, 1),
        (2, N'SIS-110', N'Matemática discreta', 3, N'Obligatorio', 8, 2),
        (3, N'SIS-120', N'Bases de datos', 4, N'Obligatorio', 15, 3),
        (4, N'SIS-150', N'Inglés técnico', 2, N'Electivo', 20, 4),
        (5, N'SIS-160', N'Ética profesional', 2, N'Electivo', 0, 5),
        (6, N'ADM-210', N'Contabilidad básica', 3, N'Obligatorio', 22, 2),
        (7, N'MER-105', N'Investigación de mercados', 3, N'Obligatorio', 18, 3),
        (8, N'DER-101', N'Introducción al Derecho', 4, N'Obligatorio', 26, 5),
        (9, N'PSI-101', N'Psicología general', 4, N'Obligatorio', 21, 4);
    SET IDENTITY_INSERT [dbo].[Cursos] OFF;

    INSERT INTO [dbo].[CarreraCursos] ([CarreraId], [CursoId]) VALUES
        (1, 1), (1, 2), (1, 3), (1, 4), (1, 5),
        (2, 4), (2, 5), (2, 6), (2, 7),
        (3, 4), (3, 5), (3, 6),
        (4, 4), (4, 5), (4, 6), (4, 7),
        (5, 4), (5, 5), (5, 8),
        (6, 4), (6, 5), (6, 9);
END
GO
