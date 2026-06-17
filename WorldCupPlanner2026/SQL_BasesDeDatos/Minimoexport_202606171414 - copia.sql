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




INSERT INTO WorldCupPlanner.dbo.Groups (Label) VALUES
	 (N'A'),
	 (N'B'),
	 (N'C'),
	 (N'D'),
	 (N'E'),
	 (N'F'),
	 (N'G'),
	 (N'H'),
	 (N'I'),
	 (N'J');
INSERT INTO WorldCupPlanner.dbo.Groups (Label) VALUES
	 (N'K'),
	 (N'L');
INSERT INTO WorldCupPlanner.dbo.Pots (Number) VALUES
	 (1),
	 (2),
	 (3),
	 (4);
INSERT INTO WorldCupPlanner.dbo.Stadiums (Name,City,Description,Capacity) VALUES
	 (N'CENTENARIO',N'MONTEVIDEO',N'Estadio histórico',60000),
	 (N'CAMPEÓN DEL SIGLO',N'MONTEVIDEO',N'Estadio de Peñarol',40000),
	 (N'GALLINERO CENTRAL',N'MONTEVIDEO',N'Estadio de Naciomal',38000),
	 (N'LUIS FRANZINI',N'MONTEVIDEO',N'Estadio de los TussiWarriors',20000),
	 (N'AZTECA',N'CIUDAD DE MEXICO',N'Donde Maradona gano un mundial',20014);

INSERT INTO WorldCupPlanner.dbo.Users (Name,Surname,Email,DateOfBirth,PasswordHash,Roles) VALUES
	 (N'Solo',N'Admin',N'solo.admin@test.com','2000-01-01 00:00:00.0000000',N'Sdvvzrug4$',1),
	 (N'Admin',N'2',N'admin2@test.com','2008-06-17 00:00:00.0000000',N'Sdvvzrug4$',1),
	 (N'Editor',N'1',N'editor@test.com','2008-06-17 00:00:00.0000000',N'Sdvvzrug4$',2),
	 (N'Editor',N'2',N'editor2@test.com','2008-06-17 00:00:00.0000000',N'Sdvvzrug4$',2),
	 (N'Periodista',N'1',N'periodista@test.com','2008-06-17 00:00:00.0000000',N'Sdvvzrug4$',4),
	 (N'Periodista',N'2',N'periodista2@test.com','2008-06-17 00:00:00.0000000',N'Sdvvzrug4$',4);

INSERT INTO WorldCupPlanner.dbo.Teams (Name,ConfederationName,RankingFifa,Points,GoalDifference,GoalsInFavor,GroupLabel,FlagBase64,ConfederationId,GroupId,PotId) VALUES
	 (N'CABO VERDE',N'AFC',300,1,-4,6,N'',N'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',NULL,12,4),
	 (N'COLOMBIA',N'CONMEBOL',1800,5,2,6,N'',N'iVBORw0KGgoAAAANSUhEUgAAAVwAAAFcBAMAAAB2OBsfAAAALVBMVEX90RUAN5POECXu7u////8AMp7XBA/PESbw7+v9ywBnPXSoo1rheITx6s3/+dxHJivEAAAD30lEQVR42u2azZWbMBRG1UIWboCNCsAL7ceLbL1wCylAGxfANJEW0kJaSAtJCakh+kFGEogwNtjozL0nHvwhGS7yQ9jE4uR5az07jwJddNFFF1100UUX3Vfr9iuO/ZqdR3TRRRdddNFFF110X67Lx3N00UUXXXTRRRdddLkDiS666KKL7ifVVZ7QsPNY2+j+9vz569l5FN+rQgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAwBb8rApxqQrxpSrQRRdddNFFF1100UUXXXTRRRdddNFFF1100UUXXXTRRRdddNFFF1100UUXXXQr0J34sdbhcjm7J2fz9HCex3Zx/e1LDiYeDm5t4Oy3dY63a5azG8x0Bi7iR1WIb1UhrlUh5PWq69K1wrIS3cYtZC2j23jq8G2CbtP0a3RUylo2utHSLK7mj60Y+1xr89xgjjEc6/DUYJql9Ct0/4jQfbv2D+mea58a36Ltq93mb1382sbqZtuLkcki7C9keyhui25FWDSNvLVEi6tfPIgICrPau0G8dvdyeLtGixz9gK78lKP7KXQ51SrTLZ/lt0laz88M6Sz+2MxAMexUN66PRcWwwmWCYlioKxlddNFFd0pXVqRb1USmqd2NdR/4/r/gM8rEHYq798ZEdv8bktz5mexX6zfh/vCG70WSYlhdNy2fHere+abrrc63cRlGLZUWg7tFzrw7WUWTt2nyWWlyUe8dyGde1ZbeD9Hp+8GXn80/nssq7kQ/PLqlk3vuY9Xwv2yy/KpkIXW0Stz7uXn+QtQsvGDxXQ1ddNFd4zJx709xpLz2P81wP9iwP0EyjyH5Dm6lNqv8XGsb5PCC4sajDr2aT7X9juxXVYjT6XQ8vYRjsuO3zOOY5qPtcDqJ1hOadh7RRTfS7ZT5p0KD8jlEG6LWFWO2o4WxttHtvHnhsGyKWleM+Y5mR7cNkmF034uHpdaNhR2p2dFVQbKv3XZ0HLejtLtZLRZ39J/Y9pIiXFT649l5fLluNxuLup0njWr72M7GvHOtxaD8mRtFO+9sHzu7164Us8433a9mve0Yxch+u/jm9brpOOo8FIMt8uGwXIy2slnsWreiEPPXRrXb2bYoqth+y9jGb/coJp1Fclhq4fyyYlT+YpdGVZrXRFw0aUnZ49o89tWZxKQmXe+pmUHFVaJGY7BR9NWZxG7ZVS0tqfSwnhW97XAiZu+MiBqSCnNRPT3mNVke3biGxoP9pJhfF8q1m52hqn1F9NNueXT5Jowuuuiiiy666KKL7od0uQOJLrrooosuuuiii+6HdPl4jm6I/wDESgTt3Q2CPQAAAABJRU5ErkJggg==',NULL,7,2),
	 (N'ALEMANIA',N'UEFA',1900,5,4,6,N'',N'iVBORw0KGgoAAAANSUhEUgAAAQMAAADCCAMAAAB6zFdcAAAAV1BMVEX///8AAAD/zADMAAGOjo7HAAA4BQjTAQTzuBP+zwD84Ij+//3IHBgkJCTIGhQdHR38zA4WFhYLCwshIyYWHR9DISE2AAA/GhvOHBvDABjDABPztw/60BBUXodhAAABS0lEQVR4nO3QyVECQAAAQQ4PFA+8EIX847QmAPbhAx7bE8FULxaSJEmSJEmSpP91M0sDg7flHL0PDJ6vPXehXhkwWDIoBgyKAYNiwKAYMCgGDIoBg2LAoBgwKAYMigGDYsCgGDAoBgyKAYNiwKAYMCgGDIoBg2LAoBgwKAYMisHY4OPacxfqc2Dwtb+bof33wOBwP0eHgcHTeo5eGDBYMygGDIoBg2LAoBgwKAYMigGDYsCgGDAoBgyKAYNiwKAYMCgGDIoBg2LAoBgwKAYMigGDYsCgGDAoBgyKAYMaGfzcztHveYLt5riZoeNme97g9DBHp4HBbjVHOwYMVgyKAYNiwKAYMCgGDIoBg2LAoBgwKAYMigGDYsCgGDAoBgyKAYNiwKAYMCgGDIoBg2LAoBgwKAYMigGDYsCgRgaPs3TeQJIkSZIkSZI07A9V7kb3VAV9AgAAAABJRU5ErkJggg==',NULL,3,2),
	 (N'BOSNIA',N'UEFA',500,2,-3,5,N'',N'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',NULL,5,4),
	 (N'UEFA_01',N'UEFA',1898,4,1,6,N'',N'',NULL,4,2),
	 (N'UEFA_02',N'UEFA',2098,2,-2,4,N'',N'',NULL,11,1),
	 (N'UEFA_03',N'UEFA',1990,4,-2,3,N'',N'',NULL,1,2),
	 (N'UEFA_04',N'UEFA',1528,4,-1,4,N'',N'',NULL,2,3),
	 (N'UEFA_05',N'UEFA',753,0,-9,0,N'',N'',NULL,4,4),
	 (N'UEFA_06',N'UEFA',1530,4,-1,5,N'',N'',NULL,12,2);
INSERT INTO WorldCupPlanner.dbo.Teams (Name,ConfederationName,RankingFifa,Points,GoalDifference,GoalsInFavor,GroupLabel,FlagBase64,ConfederationId,GroupId,PotId) VALUES
	 (N'UEFA_07',N'UEFA',2294,7,4,7,N'',N'',NULL,8,1),
	 (N'UEFA_08',N'UEFA',1273,6,4,8,N'',N'',NULL,9,3),
	 (N'UEFA_09',N'UEFA',2451,2,-3,4,N'',N'',NULL,5,1),
	 (N'UEFA_10',N'UEFA',902,3,0,4,N'',N'',NULL,1,4),
	 (N'UEFA_11',N'UEFA',942,7,7,9,N'',N'',NULL,12,4),
	 (N'UEFA_12',N'UEFA',1328,4,-2,6,N'',N'',NULL,7,3),
	 (N'UEFA_13',N'UEFA',1692,3,-2,5,N'',N'',NULL,10,2),
	 (N'UEFA_14',N'UEFA',1333,2,-2,5,N'',N'',NULL,6,3),
	 (N'CAF_01',N'CAF',2461,4,2,6,N'',N'',NULL,4,1),
	 (N'CAF_02',N'CAF',366,0,-6,3,N'',N'',NULL,8,4);
INSERT INTO WorldCupPlanner.dbo.Teams (Name,ConfederationName,RankingFifa,Points,GoalDifference,GoalsInFavor,GroupLabel,FlagBase64,ConfederationId,GroupId,PotId) VALUES
	 (N'CAF_03',N'CAF',2198,5,3,5,N'',N'',NULL,9,1),
	 (N'CAF_04',N'CAF',2490,5,3,5,N'',N'',NULL,1,1),
	 (N'CAF_05',N'CAF',1790,9,7,9,N'',N'',NULL,11,2),
	 (N'CAF_06',N'CAF',992,2,-4,0,N'',N'',NULL,3,3),
	 (N'CAF_07',N'CAF',2098,4,-2,5,N'',N'',NULL,12,1),
	 (N'CAF_08',N'CAF',2166,6,1,5,N'',N'',NULL,10,1),
	 (N'CAF_09',N'CAF',2483,9,6,8,N'',N'',NULL,2,1),
	 (N'AFC_01',N'AFC',371,4,-1,3,N'',N'',NULL,11,4),
	 (N'AFC_02',N'AFC',1840,4,1,8,N'',N'',NULL,6,2),
	 (N'AFC_03',N'AFC',1458,5,2,6,N'',N'',NULL,3,3);
INSERT INTO WorldCupPlanner.dbo.Teams (Name,ConfederationName,RankingFifa,Points,GoalDifference,GoalsInFavor,GroupLabel,FlagBase64,ConfederationId,GroupId,PotId) VALUES
	 (N'AFC_04',N'AFC',2355,5,4,4,N'',N'',NULL,7,1),
	 (N'AFC_05',N'AFC',1813,4,0,5,N'',N'',NULL,8,2),
	 (N'AFC_06',N'AFC',1503,2,-1,4,N'',N'',NULL,1,3),
	 (N'AFC_07',N'AFC',478,3,-1,6,N'',N'',NULL,10,4),
	 (N'CONCACAF_01',N'CONCACAF',711,2,-1,5,N'',N'',NULL,6,4),
	 (N'CONCACAF_02',N'CONCACAF',1297,2,-4,4,N'',N'',NULL,5,3),
	 (N'CONCACAF_03',N'CONCACAF',954,1,-4,3,N'',N'',NULL,11,3),
	 (N'CONCACAF_04',N'CONCACAF',2475,2,-2,2,N'',N'',NULL,3,1),
	 (N'CONCACAF_05',N'CONCACAF',1714,4,0,4,N'',N'',NULL,9,2),
	 (N'CONCACAF_06',N'CONCACAF',1979,4,1,5,N'',N'',NULL,2,2);
INSERT INTO WorldCupPlanner.dbo.Teams (Name,ConfederationName,RankingFifa,Points,GoalDifference,GoalsInFavor,GroupLabel,FlagBase64,ConfederationId,GroupId,PotId) VALUES
	 (N'CONCACAF_07',N'CONCACAF',366,1,-4,4,N'',N'',NULL,7,4),
	 (N'CONMEBOL_01',N'CONMEBOL',1138,6,2,6,N'',N'',NULL,8,3),
	 (N'CONMEBOL_02',N'CONMEBOL',1055,6,2,8,N'',N'',NULL,10,3),
	 (N'CONMEBOL_03',N'CONMEBOL',2407,7,2,6,N'',N'',NULL,6,1),
	 (N'CONMEBOL_04',N'CONMEBOL',1411,9,6,9,N'',N'',NULL,4,3),
	 (N'CONMEBOL_05',N'CONMEBOL',1875,9,10,11,N'',N'',NULL,5,2),
	 (N'CONMEBOL_06',N'CONMEBOL',561,1,-7,3,N'',N'',NULL,9,4),
	 (N'OFC_01',N'OFC',901,0,-6,3,N'',N'',NULL,2,4);

INSERT INTO WorldCupPlanner.dbo.Matches (StadiumId,HomeTeamId,AwayTeamId,WinnerId,GroupId,Date,HomeGoals,AwayGoals,Status,[Group],Phase,MatchCode,RankingApplied) VALUES
	 (5,22,14,NULL,1,'2026-06-01 14:00:00.0000000',1,1,0,N'A',0,NULL,0),
	 (2,7,33,7,1,'2026-06-01 18:00:00.0000000',2,1,0,N'A',0,NULL,0),
	 (1,22,33,NULL,1,'2026-06-04 14:00:00.0000000',1,1,0,N'A',0,NULL,0),
	 (3,7,14,NULL,1,'2026-06-04 18:00:00.0000000',1,1,0,N'A',0,NULL,0),
	 (4,22,7,22,1,'2026-06-07 14:00:00.0000000',3,0,0,N'A',0,NULL,0),
	 (5,33,14,NULL,1,'2026-06-07 14:00:00.0000000',2,2,0,N'A',0,NULL,0),
	 (2,27,48,27,2,'2026-06-04 14:00:00.0000000',4,2,0,N'B',0,NULL,0),
	 (1,40,8,NULL,2,'2026-06-04 18:00:00.0000000',2,2,0,N'B',0,NULL,0),
	 (3,27,8,27,2,'2026-06-07 14:00:00.0000000',2,0,0,N'B',0,NULL,0),
	 (4,40,48,40,2,'2026-06-07 18:00:00.0000000',3,0,0,N'B',0,NULL,0);
INSERT INTO WorldCupPlanner.dbo.Matches (StadiumId,HomeTeamId,AwayTeamId,WinnerId,GroupId,Date,HomeGoals,AwayGoals,Status,[Group],Phase,MatchCode,RankingApplied) VALUES
	 (5,27,40,27,2,'2026-06-10 14:00:00.0000000',2,0,0,N'B',0,NULL,0),
	 (2,8,48,8,2,'2026-06-10 14:00:00.0000000',2,1,0,N'B',0,NULL,0),
	 (1,38,24,NULL,3,'2026-06-07 14:00:00.0000000',0,0,0,N'C',0,NULL,0),
	 (3,3,30,NULL,3,'2026-06-07 18:00:00.0000000',2,2,0,N'C',0,NULL,0),
	 (4,38,30,30,3,'2026-06-10 14:00:00.0000000',2,4,0,N'C',0,NULL,0),
	 (5,3,24,3,3,'2026-06-10 18:00:00.0000000',4,0,0,N'C',0,NULL,0),
	 (2,38,3,NULL,3,'2026-06-13 14:00:00.0000000',0,0,0,N'C',0,NULL,0),
	 (1,30,24,NULL,3,'2026-06-13 14:00:00.0000000',0,0,0,N'C',0,NULL,0),
	 (3,19,9,19,4,'2026-06-10 14:00:00.0000000',3,0,0,N'D',0,NULL,0),
	 (4,5,45,45,4,'2026-06-10 18:00:00.0000000',2,3,0,N'D',0,NULL,0);
INSERT INTO WorldCupPlanner.dbo.Matches (StadiumId,HomeTeamId,AwayTeamId,WinnerId,GroupId,Date,HomeGoals,AwayGoals,Status,[Group],Phase,MatchCode,RankingApplied) VALUES
	 (5,19,45,45,4,'2026-06-13 14:00:00.0000000',1,2,0,N'D',0,NULL,0),
	 (2,5,9,5,4,'2026-06-13 18:00:00.0000000',2,0,0,N'D',0,NULL,0),
	 (1,19,5,NULL,4,'2026-06-16 14:00:00.0000000',2,2,0,N'D',0,NULL,0),
	 (3,45,9,45,4,'2026-06-16 14:00:00.0000000',4,0,0,N'D',0,NULL,0),
	 (4,13,4,NULL,5,'2026-06-13 14:00:00.0000000',2,2,0,N'E',0,NULL,0),
	 (5,46,36,46,5,'2026-06-13 18:00:00.0000000',4,0,0,N'E',0,NULL,0),
	 (2,13,36,NULL,5,'2026-06-16 14:00:00.0000000',2,2,0,N'E',0,NULL,0),
	 (1,46,4,46,5,'2026-06-16 18:00:00.0000000',4,1,0,N'E',0,NULL,0),
	 (3,13,46,46,5,'2026-06-19 14:00:00.0000000',0,3,0,N'E',0,NULL,0),
	 (4,36,4,NULL,5,'2026-06-19 14:00:00.0000000',2,2,0,N'E',0,NULL,0);
INSERT INTO WorldCupPlanner.dbo.Matches (StadiumId,HomeTeamId,AwayTeamId,WinnerId,GroupId,Date,HomeGoals,AwayGoals,Status,[Group],Phase,MatchCode,RankingApplied) VALUES
	 (5,44,35,44,6,'2026-06-16 14:00:00.0000000',2,1,0,N'F',0,NULL,0),
	 (2,29,18,29,6,'2026-06-16 18:00:00.0000000',4,2,0,N'F',0,NULL,0),
	 (1,44,18,NULL,6,'2026-06-19 14:00:00.0000000',1,1,0,N'F',0,NULL,0),
	 (3,29,35,NULL,6,'2026-06-19 18:00:00.0000000',2,2,0,N'F',0,NULL,0),
	 (4,44,29,44,6,'2026-06-22 14:00:00.0000000',3,2,0,N'F',0,NULL,0),
	 (5,18,35,NULL,6,'2026-06-22 14:00:00.0000000',2,2,0,N'F',0,NULL,0),
	 (2,31,41,NULL,7,'2026-06-19 14:00:00.0000000',0,0,0,N'G',0,NULL,0),
	 (1,2,16,NULL,7,'2026-06-19 18:00:00.0000000',2,2,0,N'G',0,NULL,0),
	 (3,31,16,31,7,'2026-06-22 14:00:00.0000000',4,0,0,N'G',0,NULL,0),
	 (4,2,41,2,7,'2026-06-22 18:00:00.0000000',4,2,0,N'G',0,NULL,0);
INSERT INTO WorldCupPlanner.dbo.Matches (StadiumId,HomeTeamId,AwayTeamId,WinnerId,GroupId,Date,HomeGoals,AwayGoals,Status,[Group],Phase,MatchCode,RankingApplied) VALUES
	 (5,31,2,NULL,7,'2026-06-25 14:00:00.0000000',0,0,0,N'G',0,NULL,0),
	 (2,16,41,16,7,'2026-06-25 14:00:00.0000000',4,2,0,N'G',0,NULL,0),
	 (1,11,20,11,8,'2026-06-22 14:00:00.0000000',3,2,0,N'H',0,NULL,0),
	 (3,32,42,42,8,'2026-06-22 18:00:00.0000000',0,4,0,N'H',0,NULL,0),
	 (4,11,42,11,8,'2026-06-25 14:00:00.0000000',3,0,0,N'H',0,NULL,0),
	 (5,32,20,32,8,'2026-06-25 18:00:00.0000000',4,0,0,N'H',0,NULL,0),
	 (2,11,32,NULL,8,'2026-06-28 14:00:00.0000000',1,1,0,N'H',0,NULL,0),
	 (1,42,20,42,8,'2026-06-28 14:00:00.0000000',2,1,0,N'H',0,NULL,0),
	 (3,21,47,NULL,9,'2026-06-25 14:00:00.0000000',2,2,0,N'I',0,NULL,0),
	 (4,39,12,12,9,'2026-06-25 18:00:00.0000000',0,4,0,N'I',0,NULL,0);
INSERT INTO WorldCupPlanner.dbo.Matches (StadiumId,HomeTeamId,AwayTeamId,WinnerId,GroupId,Date,HomeGoals,AwayGoals,Status,[Group],Phase,MatchCode,RankingApplied) VALUES
	 (5,21,12,21,9,'2026-06-28 14:00:00.0000000',3,0,0,N'I',0,NULL,0),
	 (2,39,47,39,9,'2026-06-28 18:00:00.0000000',4,0,0,N'I',0,NULL,0),
	 (1,21,39,NULL,9,'2026-07-01 14:00:00.0000000',0,0,0,N'I',0,NULL,0),
	 (3,12,47,12,9,'2026-07-01 14:00:00.0000000',4,1,0,N'I',0,NULL,0),
	 (4,26,34,26,10,'2026-06-28 14:00:00.0000000',2,0,0,N'J',0,NULL,0),
	 (5,17,43,17,10,'2026-06-28 18:00:00.0000000',3,1,0,N'J',0,NULL,0),
	 (2,26,43,43,10,'2026-07-01 14:00:00.0000000',1,4,0,N'J',0,NULL,0),
	 (1,17,34,34,10,'2026-07-01 18:00:00.0000000',2,4,0,N'J',0,NULL,0),
	 (3,26,17,26,10,'2026-07-04 14:00:00.0000000',2,0,0,N'J',0,NULL,0),
	 (4,43,34,43,10,'2026-07-04 14:00:00.0000000',3,2,0,N'J',0,NULL,0);
INSERT INTO WorldCupPlanner.dbo.Matches (StadiumId,HomeTeamId,AwayTeamId,WinnerId,GroupId,Date,HomeGoals,AwayGoals,Status,[Group],Phase,MatchCode,RankingApplied) VALUES
	 (5,6,28,NULL,11,'2026-07-01 14:00:00.0000000',1,1,0,N'K',0,NULL,0),
	 (2,23,37,23,11,'2026-07-01 18:00:00.0000000',3,1,0,N'K',0,NULL,0),
	 (1,6,37,NULL,11,'2026-07-04 14:00:00.0000000',2,2,0,N'K',0,NULL,0),
	 (3,23,28,23,11,'2026-07-04 18:00:00.0000000',3,0,0,N'K',0,NULL,0),
	 (4,6,23,23,11,'2026-07-07 14:00:00.0000000',1,3,0,N'K',0,NULL,0),
	 (5,37,28,28,11,'2026-07-07 14:00:00.0000000',0,2,0,N'K',0,NULL,0),
	 (2,25,1,25,12,'2026-07-04 14:00:00.0000000',4,2,0,N'L',0,NULL,0),
	 (1,10,15,15,12,'2026-07-04 18:00:00.0000000',0,3,0,N'L',0,NULL,0),
	 (3,25,15,15,12,'2026-07-07 14:00:00.0000000',0,4,0,N'L',0,NULL,0),
	 (4,10,1,10,12,'2026-07-07 18:00:00.0000000',4,2,0,N'L',0,NULL,0);
INSERT INTO WorldCupPlanner.dbo.Matches (StadiumId,HomeTeamId,AwayTeamId,WinnerId,GroupId,Date,HomeGoals,AwayGoals,Status,[Group],Phase,MatchCode,RankingApplied) VALUES
	 (5,25,10,NULL,12,'2026-07-10 14:00:00.0000000',1,1,0,N'L',0,NULL,0),
	 (2,15,1,NULL,12,'2026-07-10 14:00:00.0000000',2,2,0,N'L',0,NULL,0);

INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (23,118,N'Red',64),
	 (28,94,N'Red',64),
	 (9,73,N'Red',22),
	 (43,72,N'Red',60),
	 (34,118,N'Red',60),
	 (46,55,N'Red',28),
	 (4,73,N'Red',28),
	 (48,87,N'Red',10),
	 (22,70,N'Red',1),
	 (14,68,N'Red',1);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (18,89,N'Red',36),
	 (19,60,N'Red',21),
	 (16,28,N'Red',39),
	 (13,68,N'Red',29),
	 (46,36,N'Red',29),
	 (31,108,N'Red',41),
	 (11,22,N'Red',45),
	 (42,14,N'Red',45),
	 (38,56,N'Red',15),
	 (30,74,N'Red',15);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (6,86,N'Red',63),
	 (39,2,N'Red',52),
	 (47,110,N'Red',52),
	 (16,6,N'Red',42),
	 (41,57,N'Red',42),
	 (19,114,N'Red',23),
	 (36,82,N'Red',27),
	 (2,116,N'Red',40),
	 (37,102,N'Red',66),
	 (12,79,N'Red',54);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (47,85,N'Red',54),
	 (12,28,N'Red',51),
	 (3,104,N'Red',16),
	 (24,121,N'Red',16),
	 (42,52,N'Red',48),
	 (17,105,N'Red',59),
	 (24,88,N'Red',18),
	 (44,97,N'Red',33),
	 (44,34,N'Red',35),
	 (45,79,N'Red',24);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (25,59,N'Red',69),
	 (7,19,N'Red',2),
	 (27,31,N'Red',11),
	 (10,47,N'Red',71),
	 (7,65,N'Red',4),
	 (14,5,N'Red',4),
	 (7,75,N'Red',5),
	 (40,96,N'Red',8),
	 (27,86,N'Red',7),
	 (3,83,N'Red',14);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (38,72,N'Red',13),
	 (24,89,N'Red',13),
	 (45,25,N'Red',20),
	 (19,7,N'Red',19),
	 (46,3,N'Red',26),
	 (36,130,N'Red',26),
	 (13,29,N'Red',25),
	 (18,94,N'Red',32),
	 (16,98,N'Red',38),
	 (31,88,N'Red',37);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (39,74,N'Red',50),
	 (47,100,N'Red',49),
	 (43,77,N'Red',56),
	 (26,11,N'Red',55),
	 (37,2,N'Red',62),
	 (28,43,N'Red',61),
	 (10,5,N'Red',68),
	 (15,32,N'Red',68),
	 (25,14,N'Red',67),
	 (1,122,N'Red',67);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (23,84,N'Yellow',64),
	 (23,89,N'Yellow',64),
	 (28,2,N'Yellow',64),
	 (28,129,N'Yellow',64),
	 (5,34,N'Yellow',22),
	 (5,74,N'Yellow',22),
	 (9,43,N'Yellow',22),
	 (9,8,N'Yellow',22),
	 (9,12,N'Yellow',22),
	 (43,77,N'Yellow',60);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (43,5,N'Yellow',60),
	 (46,79,N'Yellow',28),
	 (46,76,N'Yellow',28),
	 (46,56,N'Yellow',28),
	 (46,25,N'Yellow',28),
	 (46,27,N'Yellow',28),
	 (4,1,N'Yellow',28),
	 (4,60,N'Yellow',28),
	 (4,25,N'Yellow',28),
	 (22,120,N'Yellow',3);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (22,29,N'Yellow',3),
	 (33,49,N'Yellow',3),
	 (33,39,N'Yellow',3),
	 (14,6,N'Yellow',1),
	 (14,87,N'Yellow',1),
	 (14,40,N'Yellow',1),
	 (14,124,N'Yellow',1),
	 (35,125,N'Yellow',36),
	 (35,48,N'Yellow',36),
	 (35,98,N'Yellow',36);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (19,1,N'Yellow',21),
	 (19,76,N'Yellow',21),
	 (45,48,N'Yellow',21),
	 (45,102,N'Yellow',21),
	 (45,80,N'Yellow',21),
	 (45,112,N'Yellow',21),
	 (45,50,N'Yellow',21),
	 (11,15,N'Yellow',47),
	 (11,43,N'Yellow',47),
	 (11,118,N'Yellow',47);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (32,4,N'Yellow',47),
	 (32,77,N'Yellow',47),
	 (32,48,N'Yellow',47),
	 (32,30,N'Yellow',47),
	 (31,25,N'Yellow',39),
	 (16,87,N'Yellow',39),
	 (8,103,N'Yellow',12),
	 (8,68,N'Yellow',12),
	 (48,97,N'Yellow',12),
	 (48,75,N'Yellow',12);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (48,72,N'Yellow',12),
	 (48,116,N'Yellow',12),
	 (48,98,N'Yellow',12),
	 (13,25,N'Yellow',29),
	 (13,3,N'Yellow',29),
	 (13,27,N'Yellow',29),
	 (46,86,N'Yellow',29),
	 (46,95,N'Yellow',29),
	 (46,12,N'Yellow',29),
	 (10,22,N'Yellow',70);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (10,71,N'Yellow',70),
	 (10,81,N'Yellow',70),
	 (10,108,N'Yellow',70),
	 (31,58,N'Yellow',41),
	 (2,119,N'Yellow',41),
	 (2,119,N'Yellow',41),
	 (2,78,N'Yellow',41),
	 (11,3,N'Yellow',45),
	 (11,65,N'Yellow',45),
	 (11,8,N'Yellow',45);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (11,25,N'Yellow',45),
	 (11,62,N'Yellow',45),
	 (42,23,N'Yellow',45),
	 (42,103,N'Yellow',45),
	 (1,48,N'Yellow',72),
	 (1,130,N'Yellow',72),
	 (1,92,N'Yellow',72),
	 (1,90,N'Yellow',72),
	 (30,86,N'Yellow',15),
	 (30,98,N'Yellow',15);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (30,23,N'Yellow',15),
	 (30,45,N'Yellow',15),
	 (6,71,N'Yellow',63),
	 (6,29,N'Yellow',63),
	 (37,120,N'Yellow',63),
	 (37,85,N'Yellow',63),
	 (37,20,N'Yellow',63),
	 (39,88,N'Yellow',52),
	 (39,106,N'Yellow',52),
	 (39,47,N'Yellow',52);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (39,1,N'Yellow',52),
	 (39,29,N'Yellow',52),
	 (47,87,N'Yellow',52),
	 (47,31,N'Yellow',52),
	 (47,89,N'Yellow',52),
	 (6,60,N'Yellow',65),
	 (23,82,N'Yellow',65),
	 (23,112,N'Yellow',65),
	 (27,102,N'Yellow',9),
	 (8,42,N'Yellow',9);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (26,2,N'Yellow',57),
	 (26,98,N'Yellow',57),
	 (26,116,N'Yellow',57),
	 (26,103,N'Yellow',57),
	 (26,30,N'Yellow',57),
	 (43,32,N'Yellow',57),
	 (43,126,N'Yellow',57),
	 (43,40,N'Yellow',57),
	 (43,87,N'Yellow',57),
	 (16,1,N'Yellow',42);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (16,108,N'Yellow',42),
	 (16,3,N'Yellow',42),
	 (16,20,N'Yellow',42),
	 (41,54,N'Yellow',42),
	 (32,26,N'Yellow',46),
	 (32,82,N'Yellow',46),
	 (20,83,N'Yellow',46),
	 (20,95,N'Yellow',46),
	 (20,77,N'Yellow',46),
	 (19,104,N'Yellow',23);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (5,69,N'Yellow',23),
	 (5,7,N'Yellow',23),
	 (5,43,N'Yellow',23),
	 (13,52,N'Yellow',27),
	 (13,17,N'Yellow',27),
	 (13,86,N'Yellow',27),
	 (36,89,N'Yellow',27),
	 (36,8,N'Yellow',27),
	 (2,56,N'Yellow',40),
	 (2,127,N'Yellow',40);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (41,41,N'Yellow',40),
	 (41,58,N'Yellow',40),
	 (41,7,N'Yellow',40),
	 (41,9,N'Yellow',40),
	 (38,103,N'Yellow',17),
	 (38,3,N'Yellow',17),
	 (38,1,N'Yellow',17),
	 (38,65,N'Yellow',17),
	 (38,76,N'Yellow',17),
	 (3,42,N'Yellow',17);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (3,129,N'Yellow',17),
	 (3,67,N'Yellow',17),
	 (3,30,N'Yellow',17),
	 (3,14,N'Yellow',17),
	 (28,60,N'Yellow',66),
	 (28,7,N'Yellow',66),
	 (28,52,N'Yellow',66),
	 (28,49,N'Yellow',66),
	 (28,122,N'Yellow',66),
	 (47,70,N'Yellow',54);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (47,28,N'Yellow',54),
	 (47,44,N'Yellow',54),
	 (21,22,N'Yellow',53),
	 (21,49,N'Yellow',53),
	 (39,101,N'Yellow',53),
	 (39,19,N'Yellow',53),
	 (21,65,N'Yellow',51),
	 (21,69,N'Yellow',51),
	 (21,108,N'Yellow',51),
	 (21,130,N'Yellow',51);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (12,103,N'Yellow',51),
	 (12,41,N'Yellow',51),
	 (12,58,N'Yellow',51),
	 (12,128,N'Yellow',51),
	 (12,80,N'Yellow',51),
	 (17,124,N'Yellow',58),
	 (24,57,N'Yellow',16),
	 (24,72,N'Yellow',16),
	 (42,87,N'Yellow',48),
	 (42,23,N'Yellow',48);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (42,3,N'Yellow',48),
	 (42,73,N'Yellow',48),
	 (20,120,N'Yellow',48),
	 (20,124,N'Yellow',48),
	 (20,39,N'Yellow',48),
	 (20,62,N'Yellow',48),
	 (26,102,N'Yellow',59),
	 (26,3,N'Yellow',59),
	 (26,75,N'Yellow',59),
	 (17,72,N'Yellow',59);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (17,5,N'Yellow',59),
	 (17,124,N'Yellow',59),
	 (30,27,N'Yellow',18),
	 (24,89,N'Yellow',18),
	 (24,64,N'Yellow',18),
	 (24,102,N'Yellow',18),
	 (24,34,N'Yellow',18),
	 (24,107,N'Yellow',18),
	 (44,55,N'Yellow',33),
	 (44,62,N'Yellow',33);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (18,91,N'Yellow',33),
	 (18,67,N'Yellow',33),
	 (18,97,N'Yellow',33),
	 (18,105,N'Yellow',33),
	 (44,73,N'Yellow',35),
	 (44,93,N'Yellow',35),
	 (44,102,N'Yellow',35),
	 (44,66,N'Yellow',35),
	 (44,3,N'Yellow',35),
	 (29,89,N'Yellow',35);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (29,4,N'Yellow',35),
	 (29,51,N'Yellow',35),
	 (29,124,N'Yellow',34),
	 (29,79,N'Yellow',34),
	 (29,69,N'Yellow',34),
	 (35,43,N'Yellow',34),
	 (35,42,N'Yellow',34),
	 (35,60,N'Yellow',34),
	 (35,24,N'Yellow',34),
	 (4,122,N'Yellow',30);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (4,61,N'Yellow',30),
	 (9,29,N'Yellow',24),
	 (9,30,N'Yellow',24),
	 (9,94,N'Yellow',24),
	 (9,58,N'Yellow',24),
	 (9,69,N'Yellow',24),
	 (25,109,N'Yellow',69),
	 (25,109,N'Yellow',69),
	 (25,20,N'Yellow',69),
	 (15,37,N'Yellow',69);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (15,58,N'Yellow',69),
	 (15,122,N'Yellow',69),
	 (7,55,N'Yellow',2),
	 (7,46,N'Yellow',2),
	 (33,30,N'Yellow',2),
	 (33,39,N'Yellow',2),
	 (27,104,N'Yellow',11),
	 (27,112,N'Yellow',11),
	 (27,86,N'Yellow',11),
	 (27,36,N'Yellow',11);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (27,76,N'Yellow',11),
	 (40,108,N'Yellow',11),
	 (40,95,N'Yellow',11),
	 (40,48,N'Yellow',11),
	 (25,89,N'Yellow',71),
	 (25,18,N'Yellow',71),
	 (25,55,N'Yellow',71),
	 (25,3,N'Yellow',71),
	 (10,68,N'Yellow',71),
	 (10,40,N'Yellow',71);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (7,110,N'Yellow',4),
	 (7,50,N'Yellow',4),
	 (14,84,N'Yellow',4),
	 (14,27,N'Yellow',4),
	 (22,49,N'Yellow',5),
	 (22,81,N'Yellow',5),
	 (22,59,N'Yellow',5),
	 (22,32,N'Yellow',5),
	 (7,116,N'Yellow',5),
	 (7,12,N'Yellow',5);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (7,49,N'Yellow',5),
	 (7,64,N'Yellow',5),
	 (14,53,N'Yellow',6),
	 (40,62,N'Yellow',8),
	 (40,61,N'Yellow',8),
	 (8,64,N'Yellow',8),
	 (27,21,N'Yellow',7),
	 (27,56,N'Yellow',7),
	 (27,41,N'Yellow',7),
	 (27,36,N'Yellow',7);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (27,39,N'Yellow',7),
	 (48,8,N'Yellow',7),
	 (3,9,N'Yellow',14),
	 (30,39,N'Yellow',14),
	 (30,26,N'Yellow',14),
	 (30,81,N'Yellow',14),
	 (30,94,N'Yellow',14),
	 (38,123,N'Yellow',13),
	 (38,87,N'Yellow',13),
	 (5,91,N'Yellow',20);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (5,43,N'Yellow',20),
	 (5,76,N'Yellow',20),
	 (5,98,N'Yellow',20),
	 (45,126,N'Yellow',20),
	 (45,105,N'Yellow',20),
	 (19,22,N'Yellow',19),
	 (19,88,N'Yellow',19),
	 (46,28,N'Yellow',26),
	 (36,127,N'Yellow',26),
	 (36,71,N'Yellow',26);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (13,110,N'Yellow',25),
	 (13,87,N'Yellow',25),
	 (13,105,N'Yellow',25),
	 (13,55,N'Yellow',25),
	 (4,63,N'Yellow',25),
	 (4,84,N'Yellow',25),
	 (4,16,N'Yellow',25),
	 (29,50,N'Yellow',32),
	 (29,47,N'Yellow',32),
	 (29,2,N'Yellow',32);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (29,60,N'Yellow',32),
	 (29,19,N'Yellow',32),
	 (18,116,N'Yellow',32),
	 (18,16,N'Yellow',32),
	 (18,84,N'Yellow',32),
	 (44,22,N'Yellow',31),
	 (44,121,N'Yellow',31),
	 (44,103,N'Yellow',31),
	 (44,29,N'Yellow',31),
	 (2,28,N'Yellow',38);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (16,114,N'Yellow',38),
	 (16,72,N'Yellow',38),
	 (16,54,N'Yellow',38),
	 (31,96,N'Yellow',37),
	 (32,37,N'Yellow',44),
	 (42,124,N'Yellow',44),
	 (11,23,N'Yellow',43),
	 (20,22,N'Yellow',43),
	 (20,69,N'Yellow',43),
	 (20,91,N'Yellow',43);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (20,129,N'Yellow',43),
	 (20,24,N'Yellow',43),
	 (39,68,N'Yellow',50),
	 (39,54,N'Yellow',50),
	 (12,25,N'Yellow',50),
	 (12,85,N'Yellow',50),
	 (12,123,N'Yellow',50),
	 (12,55,N'Yellow',50),
	 (21,83,N'Yellow',49),
	 (21,88,N'Yellow',49);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (21,39,N'Yellow',49),
	 (47,117,N'Yellow',49),
	 (47,56,N'Yellow',49),
	 (17,38,N'Yellow',56),
	 (17,67,N'Yellow',56),
	 (17,28,N'Yellow',56),
	 (17,21,N'Yellow',56),
	 (17,6,N'Yellow',56),
	 (43,64,N'Yellow',56),
	 (43,83,N'Yellow',56);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (43,31,N'Yellow',56),
	 (26,23,N'Yellow',55),
	 (26,55,N'Yellow',55),
	 (26,35,N'Yellow',55),
	 (34,22,N'Yellow',55),
	 (34,71,N'Yellow',55),
	 (34,73,N'Yellow',55),
	 (34,108,N'Yellow',55),
	 (34,88,N'Yellow',55),
	 (23,38,N'Yellow',62);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (23,89,N'Yellow',62),
	 (23,89,N'Yellow',62),
	 (23,129,N'Yellow',62),
	 (37,83,N'Yellow',62),
	 (37,80,N'Yellow',62),
	 (37,112,N'Yellow',62),
	 (28,69,N'Yellow',61),
	 (28,89,N'Yellow',61),
	 (28,71,N'Yellow',61),
	 (28,83,N'Yellow',61);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (28,118,N'Yellow',61),
	 (10,107,N'Yellow',68),
	 (10,121,N'Yellow',68),
	 (10,20,N'Yellow',68),
	 (10,129,N'Yellow',68),
	 (10,93,N'Yellow',68),
	 (15,65,N'Yellow',68),
	 (15,115,N'Yellow',68),
	 (15,38,N'Yellow',68),
	 (1,23,N'Yellow',67);
INSERT INTO WorldCupPlanner.dbo.MatchIncidences (TeamId,[Minute],IncidenceType,MatchId) VALUES
	 (1,87,N'Yellow',67),
	 (1,0,N'Yellow',67),
	 (1,84,N'Yellow',67);

INSERT INTO WorldCupPlanner.dbo.Notifications (UserId,Message,CreatedAt,IsRead,MatchId) VALUES
	 (5,N'Simulación completada: Fase de grupos (72 partidos)','2026-06-17 17:09:53.8449980',0,0),
	 (6,N'Simulación completada: Fase de grupos (72 partidos)','2026-06-17 17:09:53.9100462',0,0);


INSERT INTO WorldCupPlanner.dbo.AuditLogs (Timestamp,ActorEmail,[Action],Details) VALUES
	 ('2026-06-17 16:54:35.4604934',N'admin2@test.com',N'Usuario creado',N'Nuevo usuario registrado: Admin 2 (admin2@test.com)'),
	 ('2026-06-17 16:54:42.7939923',N'solo.admin@test.com',N'Rol agregado',N'Rol Admin otorgado a admin2@test.com por solo.admin@test.com'),
	 ('2026-06-17 16:55:34.0429793',N'editor@test.com',N'Usuario creado',N'Nuevo usuario registrado: Editor 1 (editor@test.com)'),
	 ('2026-06-17 16:55:38.4000130',N'solo.admin@test.com',N'Rol agregado',N'Rol Editor otorgado a editor@test.com por solo.admin@test.com'),
	 ('2026-06-17 16:56:04.3935779',N'editor2@test.com',N'Usuario creado',N'Nuevo usuario registrado: Editor 2 (editor2@test.com)'),
	 ('2026-06-17 16:56:07.3550869',N'solo.admin@test.com',N'Rol agregado',N'Rol Editor otorgado a editor2@test.com por solo.admin@test.com'),
	 ('2026-06-17 16:56:25.1409330',N'periodista@test.com',N'Usuario creado',N'Nuevo usuario registrado: Periodista 1 (periodista@test.com)'),
	 ('2026-06-17 16:56:28.7178547',N'solo.admin@test.com',N'Rol agregado',N'Rol Journalist otorgado a periodista@test.com por solo.admin@test.com'),
	 ('2026-06-17 16:56:52.4511728',N'periodista2@test.com',N'Usuario creado',N'Nuevo usuario registrado: Periodista 2 (periodista2@test.com)'),
	 ('2026-06-17 16:56:54.8012660',N'solo.admin@test.com',N'Rol agregado',N'Rol Journalist otorgado a periodista2@test.com por solo.admin@test.com');
INSERT INTO WorldCupPlanner.dbo.AuditLogs (Timestamp,ActorEmail,[Action],Details) VALUES
	 ('2026-06-17 16:58:20.1592078',N'solo.admin@test.com',N'Estadio creado',N'Estadio ''CENTENARIO'' creado en MONTEVIDEO con capacidad 60000'),
	 ('2026-06-17 16:58:20.1939417',N'solo.admin@test.com',N'Estadio creado',N'Estadio ''CAMPEÓN DEL SIGLO'' creado en MONTEVIDEO con capacidad 40000'),
	 ('2026-06-17 16:58:20.2285176',N'solo.admin@test.com',N'Estadio creado',N'Estadio ''GALLINERO CENTRAL'' creado en MONTEVIDEO con capacidad 38000'),
	 ('2026-06-17 16:58:20.2627523',N'solo.admin@test.com',N'Estadio creado',N'Estadio ''LUIS FRANZINI'' creado en MONTEVIDEO con capacidad 20000'),
	 ('2026-06-17 17:05:05.0881836',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''Cabo Verde'' agregado a AFC con ranking FIFA 300'),
	 ('2026-06-17 17:05:50.8342824',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''Colombia'' agregado a CONMEBOL con ranking FIFA 1800'),
	 ('2026-06-17 17:06:24.3015228',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''Alemania'' agregado a UEFA con ranking FIFA 1900'),
	 ('2026-06-17 17:06:51.6208206',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''Bosnia'' agregado a UEFA con ranking FIFA 500'),
	 ('2026-06-17 17:07:09.0297362',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''UEFA_01'' agregado a UEFA con ranking FIFA 1898'),
	 ('2026-06-17 17:07:09.0614961',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''UEFA_02'' agregado a UEFA con ranking FIFA 2098');
INSERT INTO WorldCupPlanner.dbo.AuditLogs (Timestamp,ActorEmail,[Action],Details) VALUES
	 ('2026-06-17 17:07:09.0953321',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''UEFA_03'' agregado a UEFA con ranking FIFA 1990'),
	 ('2026-06-17 17:07:09.1260892',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''UEFA_04'' agregado a UEFA con ranking FIFA 1528'),
	 ('2026-06-17 17:07:09.1596491',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''UEFA_05'' agregado a UEFA con ranking FIFA 753'),
	 ('2026-06-17 17:07:09.1904646',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''UEFA_06'' agregado a UEFA con ranking FIFA 1530'),
	 ('2026-06-17 17:07:09.2291030',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''UEFA_07'' agregado a UEFA con ranking FIFA 2294'),
	 ('2026-06-17 17:07:09.2705568',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''UEFA_08'' agregado a UEFA con ranking FIFA 1273'),
	 ('2026-06-17 17:07:09.3078037',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''UEFA_09'' agregado a UEFA con ranking FIFA 2451'),
	 ('2026-06-17 17:07:09.3620826',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''UEFA_10'' agregado a UEFA con ranking FIFA 902'),
	 ('2026-06-17 17:07:09.4026411',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''UEFA_11'' agregado a UEFA con ranking FIFA 942'),
	 ('2026-06-17 17:07:09.4487082',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''UEFA_12'' agregado a UEFA con ranking FIFA 1328');
INSERT INTO WorldCupPlanner.dbo.AuditLogs (Timestamp,ActorEmail,[Action],Details) VALUES
	 ('2026-06-17 17:07:09.4919330',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''UEFA_13'' agregado a UEFA con ranking FIFA 1692'),
	 ('2026-06-17 17:07:09.5334347',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''UEFA_14'' agregado a UEFA con ranking FIFA 1333'),
	 ('2026-06-17 17:07:09.5932158',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CAF_01'' agregado a CAF con ranking FIFA 2461'),
	 ('2026-06-17 17:07:09.6332575',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CAF_02'' agregado a CAF con ranking FIFA 366'),
	 ('2026-06-17 17:07:09.6702535',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CAF_03'' agregado a CAF con ranking FIFA 2198'),
	 ('2026-06-17 17:07:09.7082267',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CAF_04'' agregado a CAF con ranking FIFA 2490'),
	 ('2026-06-17 17:07:09.7474102',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CAF_05'' agregado a CAF con ranking FIFA 1790'),
	 ('2026-06-17 17:07:09.7924640',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CAF_06'' agregado a CAF con ranking FIFA 992'),
	 ('2026-06-17 17:07:09.8441744',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CAF_07'' agregado a CAF con ranking FIFA 2098'),
	 ('2026-06-17 17:07:09.8810898',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CAF_08'' agregado a CAF con ranking FIFA 2166');
INSERT INTO WorldCupPlanner.dbo.AuditLogs (Timestamp,ActorEmail,[Action],Details) VALUES
	 ('2026-06-17 17:07:09.9171541',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CAF_09'' agregado a CAF con ranking FIFA 2483'),
	 ('2026-06-17 17:07:09.9712795',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''AFC_01'' agregado a AFC con ranking FIFA 371'),
	 ('2026-06-17 17:07:10.0102719',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''AFC_02'' agregado a AFC con ranking FIFA 1840'),
	 ('2026-06-17 17:07:10.0468001',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''AFC_03'' agregado a AFC con ranking FIFA 1458'),
	 ('2026-06-17 17:07:10.0869178',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''AFC_04'' agregado a AFC con ranking FIFA 2355'),
	 ('2026-06-17 17:07:10.1194516',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''AFC_05'' agregado a AFC con ranking FIFA 1813'),
	 ('2026-06-17 17:07:10.1591678',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''AFC_06'' agregado a AFC con ranking FIFA 1503'),
	 ('2026-06-17 17:07:10.1964937',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''AFC_07'' agregado a AFC con ranking FIFA 478'),
	 ('2026-06-17 17:07:10.2456198',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CONCACAF_01'' agregado a CONCACAF con ranking FIFA 711'),
	 ('2026-06-17 17:07:10.2806690',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CONCACAF_02'' agregado a CONCACAF con ranking FIFA 1297');
INSERT INTO WorldCupPlanner.dbo.AuditLogs (Timestamp,ActorEmail,[Action],Details) VALUES
	 ('2026-06-17 17:07:10.3215628',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CONCACAF_03'' agregado a CONCACAF con ranking FIFA 954'),
	 ('2026-06-17 17:07:10.3633191',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CONCACAF_04'' agregado a CONCACAF con ranking FIFA 2475'),
	 ('2026-06-17 17:07:10.4024365',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CONCACAF_05'' agregado a CONCACAF con ranking FIFA 1714'),
	 ('2026-06-17 17:07:10.4385232',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CONCACAF_06'' agregado a CONCACAF con ranking FIFA 1979'),
	 ('2026-06-17 17:07:10.4731691',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CONCACAF_07'' agregado a CONCACAF con ranking FIFA 366'),
	 ('2026-06-17 17:07:10.5262440',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CONMEBOL_01'' agregado a CONMEBOL con ranking FIFA 1138'),
	 ('2026-06-17 17:07:10.5626938',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CONMEBOL_02'' agregado a CONMEBOL con ranking FIFA 1055'),
	 ('2026-06-17 17:07:10.5986068',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CONMEBOL_03'' agregado a CONMEBOL con ranking FIFA 2407'),
	 ('2026-06-17 17:07:10.6343648',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CONMEBOL_04'' agregado a CONMEBOL con ranking FIFA 1411'),
	 ('2026-06-17 17:07:10.6690501',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CONMEBOL_05'' agregado a CONMEBOL con ranking FIFA 1875');
INSERT INTO WorldCupPlanner.dbo.AuditLogs (Timestamp,ActorEmail,[Action],Details) VALUES
	 ('2026-06-17 17:07:10.7038042',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''CONMEBOL_06'' agregado a CONMEBOL con ranking FIFA 561'),
	 ('2026-06-17 17:07:10.7534420',N'solo.admin@test.com',N'Equipo creado',N'Equipo ''OFC_01'' agregado a OFC con ranking FIFA 901'),
	 ('2026-06-17 17:08:07.6645237',N'solo.admin@test.com',N'Estadio creado',N'Estadio ''AZTECA'' creado en CIUDAD DE MEXICO con capacidad 20014'),
	 ('2026-06-17 17:09:53.8183081',N'editor@test.com',N'Simulación de fase de grupos ejecutada',N'Simulación de fase de grupos ejecutada con semilla 67 usando Probabilístico');

