-- WorldCupPlanner.dbo.AuditLogs definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.AuditLogs;

CREATE TABLE WorldCupPlanner.dbo.AuditLogs (
	AuditLogId int IDENTITY(1,1) NOT NULL,
	[Timestamp] datetime2 NOT NULL,
	ActorEmail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Action] nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Details nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK_AuditLogs PRIMARY KEY (AuditLogId)
);


-- WorldCupPlanner.dbo.Confederations definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Confederations;

CREATE TABLE WorldCupPlanner.dbo.Confederations (
	ConfederationId int IDENTITY(1,1) NOT NULL,
	Name nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Slots int NOT NULL,
	CONSTRAINT PK_Confederations PRIMARY KEY (ConfederationId)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_Confederations_Name ON WorldCupPlanner.dbo.Confederations (  Name ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- WorldCupPlanner.dbo.Groups definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Groups;

CREATE TABLE WorldCupPlanner.dbo.Groups (
	GroupId int IDENTITY(1,1) NOT NULL,
	Label nvarchar(1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK_Groups PRIMARY KEY (GroupId)
);


-- WorldCupPlanner.dbo.Pots definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Pots;

CREATE TABLE WorldCupPlanner.dbo.Pots (
	PotId int IDENTITY(1,1) NOT NULL,
	Number int NOT NULL,
	CONSTRAINT PK_Pots PRIMARY KEY (PotId)
);


-- WorldCupPlanner.dbo.Stadiums definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Stadiums;

CREATE TABLE WorldCupPlanner.dbo.Stadiums (
	StadiumId int IDENTITY(1,1) NOT NULL,
	Name nvarchar(80) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	City nvarchar(60) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Description nvarchar(400) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Capacity int NOT NULL,
	CONSTRAINT PK_Stadiums PRIMARY KEY (StadiumId)
);


-- WorldCupPlanner.dbo.Users definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Users;

CREATE TABLE WorldCupPlanner.dbo.Users (
	UserId int IDENTITY(1,1) NOT NULL,
	Name nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Surname nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Email nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	DateOfBirth datetime2 NOT NULL,
	PasswordHash nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Roles int NOT NULL,
	CONSTRAINT PK_Users PRIMARY KEY (UserId)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email ON WorldCupPlanner.dbo.Users (  Email ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- WorldCupPlanner.dbo.[__EFMigrationsHistory] definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.[__EFMigrationsHistory];

CREATE TABLE WorldCupPlanner.dbo.[__EFMigrationsHistory] (
	MigrationId nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ProductVersion nvarchar(32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
);


-- WorldCupPlanner.dbo.Notifications definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Notifications;

CREATE TABLE WorldCupPlanner.dbo.Notifications (
	NotificationId int IDENTITY(1,1) NOT NULL,
	UserId int NOT NULL,
	Message nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CreatedAt datetime2 NOT NULL,
	IsRead bit NOT NULL,
	MatchId int NOT NULL,
	CONSTRAINT PK_Notifications PRIMARY KEY (NotificationId),
	CONSTRAINT FK_Notifications_Users_UserId FOREIGN KEY (UserId) REFERENCES WorldCupPlanner.dbo.Users(UserId) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_Notifications_UserId ON WorldCupPlanner.dbo.Notifications (  UserId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- WorldCupPlanner.dbo.Teams definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Teams;

CREATE TABLE WorldCupPlanner.dbo.Teams (
	TeamId int IDENTITY(1,1) NOT NULL,
	Name nvarchar(60) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ConfederationName nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	RankingFifa int NOT NULL,
	Points int NOT NULL,
	GoalDifference int NOT NULL,
	GoalsInFavor int NOT NULL,
	GroupLabel nvarchar(1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	FlagBase64 nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ConfederationId int NULL,
	GroupId int NULL,
	PotId int NULL,
	CONSTRAINT PK_Teams PRIMARY KEY (TeamId),
	CONSTRAINT FK_Teams_Confederations_ConfederationId FOREIGN KEY (ConfederationId) REFERENCES WorldCupPlanner.dbo.Confederations(ConfederationId),
	CONSTRAINT FK_Teams_Groups_GroupId FOREIGN KEY (GroupId) REFERENCES WorldCupPlanner.dbo.Groups(GroupId),
	CONSTRAINT FK_Teams_Pots_PotId FOREIGN KEY (PotId) REFERENCES WorldCupPlanner.dbo.Pots(PotId)
);
 CREATE NONCLUSTERED INDEX IX_Teams_ConfederationId ON WorldCupPlanner.dbo.Teams (  ConfederationId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Teams_GroupId ON WorldCupPlanner.dbo.Teams (  GroupId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE UNIQUE NONCLUSTERED INDEX IX_Teams_Name ON WorldCupPlanner.dbo.Teams (  Name ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Teams_PotId ON WorldCupPlanner.dbo.Teams (  PotId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- WorldCupPlanner.dbo.Matches definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Matches;

CREATE TABLE WorldCupPlanner.dbo.Matches (
	MatchId int IDENTITY(1,1) NOT NULL,
	StadiumId int NOT NULL,
	HomeTeamId int NOT NULL,
	AwayTeamId int NOT NULL,
	WinnerId int NULL,
	GroupId int NULL,
	[Date] datetime2 NOT NULL,
	HomeGoals int NULL,
	AwayGoals int NULL,
	Status int NOT NULL,
	[Group] nvarchar(1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Phase int NOT NULL,
	MatchCode nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	RankingApplied bit NOT NULL,
	CONSTRAINT PK_Matches PRIMARY KEY (MatchId),
	CONSTRAINT FK_Matches_Groups_GroupId FOREIGN KEY (GroupId) REFERENCES WorldCupPlanner.dbo.Groups(GroupId),
	CONSTRAINT FK_Matches_Stadiums_StadiumId FOREIGN KEY (StadiumId) REFERENCES WorldCupPlanner.dbo.Stadiums(StadiumId),
	CONSTRAINT FK_Matches_Teams_AwayTeamId FOREIGN KEY (AwayTeamId) REFERENCES WorldCupPlanner.dbo.Teams(TeamId),
	CONSTRAINT FK_Matches_Teams_HomeTeamId FOREIGN KEY (HomeTeamId) REFERENCES WorldCupPlanner.dbo.Teams(TeamId),
	CONSTRAINT FK_Matches_Teams_WinnerId FOREIGN KEY (WinnerId) REFERENCES WorldCupPlanner.dbo.Teams(TeamId)
);
 CREATE NONCLUSTERED INDEX IX_Matches_AwayTeamId ON WorldCupPlanner.dbo.Matches (  AwayTeamId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Matches_GroupId ON WorldCupPlanner.dbo.Matches (  GroupId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Matches_HomeTeamId ON WorldCupPlanner.dbo.Matches (  HomeTeamId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Matches_StadiumId ON WorldCupPlanner.dbo.Matches (  StadiumId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Matches_WinnerId ON WorldCupPlanner.dbo.Matches (  WinnerId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- WorldCupPlanner.dbo.MatchIncidences definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.MatchIncidences;

CREATE TABLE WorldCupPlanner.dbo.MatchIncidences (
	IncidenceId int IDENTITY(1,1) NOT NULL,
	TeamId int NOT NULL,
	[Minute] int NOT NULL,
	IncidenceType nvarchar(21) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	MatchId int NULL,
	CONSTRAINT PK_MatchIncidences PRIMARY KEY (IncidenceId),
	CONSTRAINT FK_MatchIncidences_Matches_MatchId FOREIGN KEY (MatchId) REFERENCES WorldCupPlanner.dbo.Matches(MatchId) ON DELETE CASCADE,
	CONSTRAINT FK_MatchIncidences_Teams_TeamId FOREIGN KEY (TeamId) REFERENCES WorldCupPlanner.dbo.Teams(TeamId)
);
 CREATE NONCLUSTERED INDEX IX_MatchIncidences_MatchId ON WorldCupPlanner.dbo.MatchIncidences (  MatchId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_MatchIncidences_TeamId ON WorldCupPlanner.dbo.MatchIncidences (  TeamId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


INSERT INTO WorldCupPlanner.dbo.Users (Name,Surname,Email,DateOfBirth,PasswordHash,Roles) VALUES
	 (N'Solo',N'Admin',N'solo.admin@test.com','2000-01-01 00:00:00.0000000',N'Sdvvzrug4$',1);


