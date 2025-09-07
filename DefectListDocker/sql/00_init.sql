IF DB_ID(N'$(DB_NAME)') IS NULL
BEGIN
    CREATE DATABASE [$(DB_NAME)];
END
GO

USE [$(DB_NAME)];
GO

-- dbo.BomItemDoc определение

-- Drop table

-- DROP TABLE dbo.BomItemDoc;

CREATE TABLE BomItemDoc (
	DocId int IDENTITY(1,1) NOT NULL,
	BomItemId int NOT NULL,
	Status tinyint NOT NULL,
	Comment nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
	CreateDate datetime NOT NULL,
	CreatedBy nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
	RecordDate datetime NOT NULL,
	UpdatedBy nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
	CONSTRAINT PK__BomItemD__3EF188AD3F67FA83 PRIMARY KEY (DocId)
);


-- dbo.GroupDefect определение

-- Drop table

-- DROP TABLE dbo.GroupDefect;

CREATE TABLE GroupDefect (
	GroupDefectId int IDENTITY(1,1) NOT NULL,
	GroupDefectName nvarchar(50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	CONSTRAINT PK__GroupDef__D7A4790C284F8B01 PRIMARY KEY (GroupDefectId)
);


-- dbo.LogActionType определение

-- Drop table

-- DROP TABLE dbo.LogActionType;

CREATE TABLE LogActionType (
	LogActionTypeId int IDENTITY(1,1) NOT NULL,
	LogActionTypeName nvarchar(255) COLLATE Cyrillic_General_CI_AS NOT NULL,
	CreateDate datetime NOT NULL,
	CreatedBy nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
	RecordDate datetime NOT NULL,
	UpdatedBy nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
	CONSTRAINT PK__LogActio__FE12575E5151A094 PRIMARY KEY (LogActionTypeId)
);


-- dbo.MapBomItemToRouteChart определение

-- Drop table

-- DROP TABLE dbo.MapBomItemToRouteChart;

CREATE TABLE MapBomItemToRouteChart (
	BomItemId int NOT NULL,
	MkartaId int NOT NULL,
	RouteChart_Number nvarchar(30) COLLATE Cyrillic_General_CI_AS NOT NULL,
	QtyLaunched decimal(18,8) NOT NULL,
	CreateDate datetime NOT NULL,
	CreatedBy nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
	ProductId int,
	Detal nvarchar(50),
	CONSTRAINT PK__MapBomIt__430C1DA37CFB18A8 PRIMARY KEY (BomItemId,RouteChart_Number)
);


-- dbo.RepairMethod определение

-- Drop table

-- DROP TABLE dbo.RepairMethod;

CREATE TABLE RepairMethod (
	Id int IDENTITY(1,1) NOT NULL,
	Name nvarchar(100) COLLATE Cyrillic_General_CI_AS NOT NULL,
	CONSTRAINT PK__RepairMe__3214EC07564B5FDB PRIMARY KEY (Id)
);


-- dbo.Roles определение

-- Drop table

-- DROP TABLE dbo.Roles;

CREATE TABLE Roles (
	RoleId int IDENTITY(1,1) NOT NULL,
	RoleName nvarchar(100) COLLATE Cyrillic_General_CI_AS NOT NULL,
	CreateDate datetime NOT NULL,
	RecordDate datetime NOT NULL,
	CONSTRAINT PK__Roles__8AFACE1A2F3192BA PRIMARY KEY (RoleId)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_Roles_RoleName ON dbo.Roles (  RoleName ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- dbo.RootItem определение

-- Drop table

-- DROP TABLE dbo.RootItem;

CREATE TABLE RootItem (
	Id int IDENTITY(1,1) NOT NULL,
	Izdels nvarchar(50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	Izdel nvarchar(50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	IzdelIma nvarchar(100) COLLATE Cyrillic_General_CI_AS NOT NULL,
	IzdelTyp nvarchar(5) COLLATE Cyrillic_General_CI_AS NOT NULL,
	IzdelInitial nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
	CONSTRAINT PK__RootItem__3214EC07527ACEF7 PRIMARY KEY (Id)
);
 CREATE NONCLUSTERED INDEX IX_BomHeader_RootItemId ON dbo.RootItem (  Id ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- dbo.StateDetals определение

-- Drop table

-- DROP TABLE dbo.StateDetals;

CREATE TABLE StateDetals (
	StateDetalsId int IDENTITY(1,1) NOT NULL,
	StateName nvarchar(50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	CONSTRAINT PK__StateDet__782FC25A54982193 PRIMARY KEY (StateDetalsId)
);

-- dbo.Users определение

-- Drop table

-- DROP TABLE dbo.Users;

CREATE TABLE Users (
	UserId int IDENTITY(1,1) NOT NULL,
	[Login] nvarchar(50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	ActiveDirectoryCN nvarchar(100) COLLATE Cyrillic_General_CI_AS NOT NULL,
	Email nvarchar(100) COLLATE Cyrillic_General_CI_AS NOT NULL,
	IsActive tinyint NOT NULL,
	CreateDate datetime NOT NULL,
	RecordDate datetime NOT NULL,
	CONSTRAINT PK__Users__1788CC4C2978B964 PRIMARY KEY (UserId)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Login ON dbo.Users (  Login ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- dbo.BomHeader определение

-- Drop table

-- DROP TABLE dbo.BomHeader;

CREATE TABLE BomHeader (
	BomId int IDENTITY(1,1) NOT NULL,
	Orders nvarchar(100) COLLATE Cyrillic_General_CI_AS NOT NULL,
	SerialNumber nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
	IzdelQty int NOT NULL,
	StateDetalsId int NOT NULL,
	Comment nvarchar(200) COLLATE Cyrillic_General_CI_AS NULL,
	DateOfSpecif datetime NULL,
	DateOfTehproc datetime NULL,
	DateOfMtrl datetime NULL,
	CreateDate datetime NOT NULL,
	CreatedBy nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
	RecordDate datetime NOT NULL,
	UpdatedBy nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
	Dic_Ordering_ID int NULL,
	Code_LSF82 int NULL,
	RootItemId int NOT NULL,
	Contract nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
	ContractDateOpen date NULL,
	SerialNumberAfterRepair nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
	State tinyint NULL,
	DateOfPreparation date NULL,
	HeaderType nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
	CONSTRAINT PK__BomHeade__7D5F6A375E218BCD PRIMARY KEY (BomId),
	CONSTRAINT FK__BomHeader__State__61F21CB1 FOREIGN KEY (StateDetalsId) REFERENCES StateDetals(StateDetalsId)
);


-- dbo.BomHeaderSubscribers определение

-- Drop table

-- DROP TABLE dbo.BomHeaderSubscribers;

CREATE TABLE BomHeaderSubscribers (
	UserId int NOT NULL,
	BomId int NOT NULL,
	CreateDate datetime NOT NULL DEFAULT(getdate()),
	CONSTRAINT PK__BomHeade__705D3AEF5C2434C2 PRIMARY KEY (UserId,BomId),
	CONSTRAINT FK__BomHeader__BomId__5F00A16D FOREIGN KEY (BomId) REFERENCES BomHeader(BomId),
	CONSTRAINT FK__BomHeader__UserI__5E0C7D34 FOREIGN KEY (UserId) REFERENCES Users(UserId)
);


-- dbo.BomItem определение

-- Drop table

-- DROP TABLE dbo.BomItem;

CREATE TABLE BomItem (
	BomId int NOT NULL,
	Id int IDENTITY(1,1) NOT NULL,
	ParentId int NULL,
	Detals nvarchar(50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	Detal nvarchar(50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	DetalIma nvarchar(100) COLLATE Cyrillic_General_CI_AS NOT NULL,
	DetalTyp nvarchar(5) COLLATE Cyrillic_General_CI_AS NOT NULL,
	DetalUm nvarchar(5) COLLATE Cyrillic_General_CI_AS NOT NULL,
	QtyMnf float NOT NULL,
	QtyConstr float NOT NULL,
	QtyRestore float NOT NULL,
	QtyReplace float NOT NULL,
	Comment nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
	Defect nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,
	Decision nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,
	CreateDate datetime NOT NULL,
	CreatedBy nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
	RecordDate datetime NOT NULL,
	UpdatedBy nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
	CommentDef nvarchar(200) COLLATE Cyrillic_General_CI_AS NULL,
	SerialNumber nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
	TechnologicalProcessUsed nvarchar(200) COLLATE Cyrillic_General_CI_AS NULL,
	FinalDecision nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,
	IsRequiredSubmit tinyint NULL,
	IsSubmitted tinyint NULL,
	IsExpanded tinyint NULL,
	IsShowItem tinyint NULL,
	ClassifierID nvarchar(30) COLLATE Cyrillic_General_CI_AS NULL,
	ProductID int NULL,
	Code_LSF82 int NULL,
	ResearchAction nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,
	ResearchResult nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,
	CONSTRAINT PK__BomItem__3214EC0766B6D1CE PRIMARY KEY (Id),
	CONSTRAINT FK__BomItem__BomId__689F1A40 FOREIGN KEY (BomId) REFERENCES BomHeader(BomId),
	CONSTRAINT FK__BomItem__ParentI__69933E79 FOREIGN KEY (ParentId) REFERENCES BomItem(Id)
);
 CREATE NONCLUSTERED INDEX IX_BomItem_BomId ON dbo.BomItem (  BomId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_BomItem_Detal ON dbo.BomItem (  BomId ASC  , Detal ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_BomItem_ParentId ON dbo.BomItem (  ParentId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- dbo.BomItemLog определение

-- Drop table

-- DROP TABLE dbo.BomItemLog;

CREATE TABLE BomItemLog (
	Id int IDENTITY(1,1) NOT NULL,
	BomItemDocId int NOT NULL,
	BomItemId int NOT NULL,
	BomItemParentId int NULL,
	Detals nvarchar(50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	Detal nvarchar(50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	DetalIma nvarchar(100) COLLATE Cyrillic_General_CI_AS NOT NULL,
	DetalTyp nvarchar(5) COLLATE Cyrillic_General_CI_AS NOT NULL,
	DetalUm nvarchar(5) COLLATE Cyrillic_General_CI_AS NOT NULL,
	QtyMnf float NOT NULL,
	QtyConstr float NOT NULL,
	QtyRestore float NOT NULL,
	QtyReplace float NOT NULL,
	Comment nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
	Defect nvarchar(1000) COLLATE Cyrillic_General_CI_AS NULL,
	Decision nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,
	CommentDef nvarchar(200) COLLATE Cyrillic_General_CI_AS NULL,
	[Action] tinyint NOT NULL,
	CreateDate datetime NOT NULL,
	CreatedBy nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
	SerialNumber nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
	TechnologicalProcessUsed nvarchar(200) COLLATE Cyrillic_General_CI_AS NULL,
	FinalDecision nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,
	IsRequiredSubmit tinyint NULL,
	IsSubmitted tinyint NULL,
	BomId int NULL,
	ResearchAction nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,
	ResearchResult nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,
	CONSTRAINT PK__BomItemL__3214EC074520D3D9 PRIMARY KEY (Id),
	CONSTRAINT FK__BomItemLo__BomIt__47091C4B FOREIGN KEY (BomItemDocId) REFERENCES BomItemDoc(DocId)
);


-- dbo.FilterItemsToReport определение

-- Drop table

-- DROP TABLE dbo.FilterItemsToReport;

CREATE TABLE FilterItemsToReport (
	RootItemId int NOT NULL,
	ReportName nvarchar(500) COLLATE Cyrillic_General_CI_AS NOT NULL,
	Detal nvarchar(50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	CONSTRAINT FK__FilterIte__RootI__64997F32 FOREIGN KEY (RootItemId) REFERENCES RootItem(Id)
);
 CREATE NONCLUSTERED INDEX IX_FilterItemsToReport ON dbo.FilterItemsToReport (  RootItemId ASC  , ReportName ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- dbo.LogAction определение

-- Drop table

-- DROP TABLE dbo.LogAction;

CREATE TABLE LogAction (
	LogActionId int IDENTITY(1,1) NOT NULL,
	LogActionTypeId int NULL,
	LogActionContext nvarchar(255) COLLATE Cyrillic_General_CI_AS NOT NULL,
	LogActionText nvarchar(255) COLLATE Cyrillic_General_CI_AS NOT NULL,
	CreateDate datetime NOT NULL,
	CreatedBy nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
	BomId int NULL,
	UserActionText nvarchar(500) COLLATE Cyrillic_General_CI_AS NULL,
	CONSTRAINT PK__LogActio__1EBD4AC6570A79EA PRIMARY KEY (LogActionId),
	CONSTRAINT FK__LogAction__LogAc__58F2C25C FOREIGN KEY (LogActionTypeId) REFERENCES LogActionType(LogActionTypeId)
);


-- dbo.MapDefectToDecision определение

-- Drop table

-- DROP TABLE dbo.MapDefectToDecision;

CREATE TABLE MapDefectToDecision (
	Id int IDENTITY(1,1) NOT NULL,
	Defect nvarchar(1000) COLLATE Cyrillic_General_CI_AS NULL,
	Decision nvarchar(300) COLLATE Cyrillic_General_CI_AS NOT NULL,
	IsAllowCombine tinyint NOT NULL,
	StateDetalsId int NOT NULL,
	GroupDefectId int NOT NULL,
	CONSTRAINT PK__MapDefec__3214EC072C201BE5 PRIMARY KEY (Id),
	CONSTRAINT FK__MapDefect__Group__2EFC8890 FOREIGN KEY (GroupDefectId) REFERENCES GroupDefect(GroupDefectId),
	CONSTRAINT FK__MapDefect__State__2E086457 FOREIGN KEY (StateDetalsId) REFERENCES StateDetals(StateDetalsId)
);
 CREATE NONCLUSTERED INDEX IX_MapDefectToDecision_StateDetalsId ON dbo.MapDefectToDecision (  StateDetalsId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- dbo.RepairMethodToItem определение

-- Drop table

-- DROP TABLE dbo.RepairMethodToItem;

CREATE TABLE RepairMethodToItem (
	Id int IDENTITY(1,1) NOT NULL,
	RootItemId int NOT NULL,
	ParentItem nvarchar(50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	ChildItem nvarchar(50) COLLATE Cyrillic_General_CI_AS NOT NULL,
	RepairMethodId int NOT NULL,
	CONSTRAINT PK__RepairMe__3214EC075A1BF0BF PRIMARY KEY (Id),
	CONSTRAINT FK__RepairMet__Repai__5CF85D6A FOREIGN KEY (RepairMethodId) REFERENCES RepairMethod(Id),
	CONSTRAINT FK__RepairMet__RootI__5C043931 FOREIGN KEY (RootItemId) REFERENCES RootItem(Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_RepairMethodToItem ON dbo.RepairMethodToItem (  RootItemId ASC  , ParentItem ASC  , ChildItem ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- dbo.UserRoles определение

-- Drop table

-- DROP TABLE dbo.UserRoles;

CREATE TABLE UserRoles (
	UserId int NOT NULL,
	RoleId int NOT NULL,
	CreateDate datetime NOT NULL,
	RecordDate datetime NOT NULL,
	CONSTRAINT PK__UserRole__AF2760AD34EA6C10 PRIMARY KEY (UserId,RoleId),
	CONSTRAINT FK__UserRoles__RoleI__39AF212D FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
	CONSTRAINT FK__UserRoles__UserI__38BAFCF4 FOREIGN KEY (UserId) REFERENCES Users(UserId)
);


-- dbo.UserSignLog определение

-- Drop table

-- DROP TABLE dbo.UserSignLog;

CREATE TABLE UserSignLog (
	UserId int NOT NULL,
	ProcessId int NULL,
	SignInDate datetime NOT NULL,
	CONSTRAINT FK__UserSignL__UserI__3B97699F FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
 CREATE NONCLUSTERED INDEX IX_UserSignLog_UserId ON dbo.UserSignLog (  UserId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- Тестовые	данные
SET IDENTITY_INSERT GroupDefect ON

INSERT INTO GroupDefect (GroupDefectId, GroupDefectName) VALUES(1, N'Общие дефекты');
INSERT INTO GroupDefect (GroupDefectId, GroupDefectName) VALUES(2, N'Механические дефекты');
INSERT INTO GroupDefect (GroupDefectId, GroupDefectName) VALUES(3, N'Электрические дефекты');

SET IDENTITY_INSERT GroupDefect OFF

SET IDENTITY_INSERT LogActionType ON

INSERT INTO LogActionType (LogActionTypeId, LogActionTypeName, CreateDate, CreatedBy, RecordDate, UpdatedBy) VALUES(1, N'BomItemReplace', getdate(), N'admin', getdate(), N'admin');
INSERT INTO LogActionType (LogActionTypeId, LogActionTypeName, CreateDate, CreatedBy, RecordDate, UpdatedBy) VALUES(2, N'BomItemAdd', getdate(), N'admin', getdate(), N'admin');
INSERT INTO LogActionType (LogActionTypeId, LogActionTypeName, CreateDate, CreatedBy, RecordDate, UpdatedBy) VALUES(3, N'BomItemDelete', getdate(), N'admin', getdate(), N'admin');
INSERT INTO LogActionType (LogActionTypeId, LogActionTypeName, CreateDate, CreatedBy, RecordDate, UpdatedBy) VALUES(4, N'BomHeaderChanged', getdate(), N'admin', getdate(), N'admin');
INSERT INTO LogActionType (LogActionTypeId, LogActionTypeName, CreateDate, CreatedBy, RecordDate, UpdatedBy) VALUES(5, N'BomItemReplaceName', getdate(), N'admin', getdate(), N'admin');

SET IDENTITY_INSERT LogActionType OFF

SET IDENTITY_INSERT StateDetals ON

INSERT INTO StateDetals (StateDetalsId, StateName) VALUES(1, N'Ремонт');
INSERT INTO StateDetals (StateDetalsId, StateName) VALUES(2, N'Замена');
INSERT INTO StateDetals (StateDetalsId, StateName) VALUES(3, N'Годная');

SET IDENTITY_INSERT StateDetals OFF

SET IDENTITY_INSERT MapDefectToDecision ON

INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(1, N'разового применения', N'заменить', 0, 2, 1);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(2, N'истечение гарантийного срока', N'заменить', 0, 2, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(3, N'разового применения (при замене сб.ед.)', N'заменить', 0, 2, 1);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(4, N'отсутствует', N'скомплектовать', 0, 2, 1);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(5, N'соответствует КД', N'использовать', 0, 3, 1);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(6, N'оценка технического состояния', N'ремонт', 1, 1, 1);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(7, N'забоины', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(8, N'царапины', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(11, N'сколы', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(12, N'загрязнение', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(13, N'разрушение', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(14, N'смятие граней', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(15, N'негерметичность', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(16, N'срыв резьбы', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(17, N'вмятины', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(18, N'коррозия', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(19, N'разрывы', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(20, N'потеря эластичности', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(21, N'растрескивание резины', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(22, N'нарушение геометрии', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(23, N'повреждение резьбы', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(24, N'задиры', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(25, N'деформация', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(26, N'отклонение размеров', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(27, N'не соответствует эталону', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(28, N'не соответствует действующей КД', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(29, N'нагар', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(30, N'трещины', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(31, N'обрыв проводов', N'ремонт', 1, 1, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(34, N'пробой изоляции', N'ремонт', 1, 1, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(36, N'не используется', N'ремонтное воздействие не требуется', 0, 3, 1);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(37, N'разового применения (при замене дет.)', N'заменить', 0, 2, 1);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(39, N'сажа', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(40, N'выкрашивание', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(41, N'трещины в сварных швах', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(42, N'повреждение покрытия', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(43, N'повреждение ЛКП', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(44, N'механические повреждения', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(45, N'шероховатость поверхности не соответствует КД', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(46, N'перегибы', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(47, N'дефект резьбы', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(48, N'повреждение резьбы не более 2-х витков', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(49, N'царапины на шаровой поверхности', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(50, N'царапины (волосовины) на конусной поверхности', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(51, N'деформация фланца', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(52, N'повреждение резьбы более 2-х витков', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(53, N'прижог', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(54, N'закоксованность', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(55, N'повреждение керамики', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(56, N'отклонение биения', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(57, N'отклонение частот собственных колебаний лопаток ротора от КД', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(58, N'разрывы перемычки над отверстием для контровочной проволоки', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(59, N'пропускная способность не соответствует КД', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(60, N'износ гребешков лабиринтного уплотнения', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(61, N'нарушение четкости маркировки (читаемости надписей)', N'заменить', 0, 2, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(62, N'нарушение пайки', N'ремонт', 1, 1, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(63, N'разрушение герметика', N'ремонт', 1, 1, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(64, N'соравана резьба гайки соединителя', N'ремонт', 1, 1, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(65, N'деформация разъемов и контактов разъемов', N'заменить', 0, 2, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(66, N'деформация наконечников', N'ремонт', 1, 1, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(67, N'трещины и сколы на изоляторе', N'ремонт', 1, 1, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(68, N'обрыв пряди плетенки на длине более 5-и плетений', N'заменить', 0, 2, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(69, N'обрыв менее 4-х проволок плетенки', N'ремонт', 1, 1, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(70, N'нарушение изоляции (разрывы, обгар, оплавление)', N'ремонт', 1, 1, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(71, N'сопротивление изоляции не соответствует КД', N'ремонт', 1, 1, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(72, N'сопротивление не соответствует КД', N'ремонт', 1, 1, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(73, N'разборка, оценка технического состояния, сборка', N'ремонт', 1, 1, 1);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(74, N'испытания электрические', N'ремонт', 1, 1, 1);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(75, N'испытания гидравлические', N'ремонт', 1, 1, 1);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(77, N'попадание масла и топлива в теплоизоляцию', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(78, N'отсутствует γ’ фаза в структуре сплава ВЖЛ-12У (колесо турбины)', N'заменить', 0, 2, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(80, N'подвижность штуцера', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(81, N'кольцевой зазор не соответствует кд', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(82, N'обгорание кромок более 1 мм.', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(83, N'магнитный контроль', N'ремонт', 1, 1, 1);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(84, N'нарушение обмотки наконечников лентой', N'ремонт', 1, 1, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(85, N'нарушение четкости (несоответствие КД) маркировки на бирках', N'ремонт', 1, 1, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(86, N'отсутсвие фрагментов', N'заменить', 0, 2, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(87, N'проверка искрообразования', N'использовать', 0, 3, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(88, N'обрыв более 4-х проволок или обрыв пряди плетенки на длине не более 5-и плетений', N'ремонт', 0, 1, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(89, N'испытания пневматические', N'ремонт', 1, 1, 1);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(90, N'истечение срока службы', N'заменить', 0, 2, 3);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(91, N'контроль герметичности', N'ремонт', 1, 1, 1);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(92, N'нарушение (отсутствие) маркировки', N'ремонт', 1, 1, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(93, N'врезание гребешков лабиринта более 0,5 мм.', N'заменить', 0, 2, 2);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(94, N'люминесцентный контроль', N'ремонт', 1, 1, 1);
INSERT INTO MapDefectToDecision (Id, Defect, Decision, IsAllowCombine, StateDetalsId, GroupDefectId) VALUES(95, N'Отклонение диаметра Г', N'ремонт', 1, 1, 2);

SET IDENTITY_INSERT MapDefectToDecision OFF

SET IDENTITY_INSERT RepairMethod ON

INSERT INTO RepairMethod (Id, Name) VALUES(1, N'карта дефектации и ремонта');
INSERT INTO RepairMethod (Id, Name) VALUES(2, N'обязательная замена');
INSERT INTO RepairMethod (Id, Name) VALUES(3, N'повторное использование');
INSERT INTO RepairMethod (Id, Name) VALUES(4, N'установить технологический');

SET IDENTITY_INSERT RepairMethod OFF

SET IDENTITY_INSERT Roles ON

INSERT INTO Roles (RoleId, RoleName, CreateDate, RecordDate) VALUES(1, N'Администратор', getdate(), getdate());
INSERT INTO Roles (RoleId, RoleName, CreateDate, RecordDate) VALUES(2, N'ОИТ', getdate(), getdate());
INSERT INTO Roles (RoleId, RoleName, CreateDate, RecordDate) VALUES(3, N'ОТК Деф.вед. чтение', getdate(), getdate());
INSERT INTO Roles (RoleId, RoleName, CreateDate, RecordDate) VALUES(4, N'ОТК Деф.вед. запись', getdate(), getdate());
INSERT INTO Roles (RoleId, RoleName, CreateDate, RecordDate) VALUES(5, N'ОТК Деф.вед. руководство', getdate(), getdate());
INSERT INTO Roles (RoleId, RoleName, CreateDate, RecordDate) VALUES(6, N'ОТК Деф.вед. создание МК', getdate(), getdate());
INSERT INTO Roles (RoleId, RoleName, CreateDate, RecordDate) VALUES(7, N'ОТК Деф.вед. ВП', getdate(), getdate());
INSERT INTO Roles (RoleId, RoleName, CreateDate, RecordDate) VALUES(8, N'ОТК Деф.вед. заполнение окончательного решения из Контроля', getdate(), getdate());
INSERT INTO Roles (RoleId, RoleName, CreateDate, RecordDate) VALUES(9, N'ОТК Деф.вед. создание МК без ограничений', getdate(), getdate());

SET IDENTITY_INSERT Roles OFF

SET IDENTITY_INSERT RootItem ON

INSERT INTO dbo.RootItem (Id, Izdels, Izdel, IzdelIma, IzdelTyp, IzdelInitial) VALUES(1, N'ИЗДЕЛИЕРЕМОНТ', N'ИЗДЕЛИЕ_РЕМОНТ', N'ИЗДЕЛИЕ_РЕМОНТ', N'издел', N'ИЗДЕЛИЕ');

SET IDENTITY_INSERT RootItem OFF

INSERT INTO FilterItemsToReport (RootItemId, ReportName, Detal) VALUES(1, N'PurchaseItemsReport', N'ДСЕ5');

SET IDENTITY_INSERT RepairMethodToItem ON

INSERT INTO RepairMethodToItem (Id, RootItemId, ParentItem, ChildItem, RepairMethodId) VALUES(1, 1, N'ДСЕ2', N'ДСЕ4', 2);

SET IDENTITY_INSERT RepairMethodToItem OFF

SET IDENTITY_INSERT Users ON

INSERT INTO Users (UserId, [Login], ActiveDirectoryCN, Email, IsActive, CreateDate, RecordDate) VALUES(1, N'admin', N'Админ А. Админов', N'admin@testmail.com', 1, getdate(), getdate());
INSERT INTO Users (UserId, [Login], ActiveDirectoryCN, Email, IsActive, CreateDate, RecordDate) VALUES(2, N'guest', N'Гость Г. Гостевой', N'guest@testmail.com', 1, getdate(), getdate());

SET IDENTITY_INSERT Users OFF

INSERT INTO UserRoles (UserId, RoleId, CreateDate, RecordDate) VALUES(1, 1, getdate(), getdate());

--

ALTER TABLE dbo.BomHeader ADD  DEFAULT getdate() FOR CreateDate;
ALTER TABLE dbo.BomHeader ADD  DEFAULT getdate() FOR RecordDate;

ALTER TABLE dbo.BomItem ADD  DEFAULT 0 FOR QtyMnf;
ALTER TABLE dbo.BomItem ADD  DEFAULT 0 FOR QtyConstr;
ALTER TABLE dbo.BomItem ADD  DEFAULT 0 FOR QtyRestore;
ALTER TABLE dbo.BomItem ADD  DEFAULT 0 FOR QtyReplace;

ALTER TABLE dbo.BomItem ADD  DEFAULT getdate() FOR CreateDate;
ALTER TABLE dbo.BomItem ADD  DEFAULT getdate() FOR RecordDate;

ALTER TABLE dbo.BomItemDoc ADD  DEFAULT getdate() FOR CreateDate;
ALTER TABLE dbo.BomItemDoc ADD  DEFAULT getdate() FOR RecordDate;

ALTER TABLE dbo.BomItemLog ADD  DEFAULT getdate() FOR CreateDate;

ALTER TABLE dbo.LogActionType ADD  DEFAULT getdate() FOR CreateDate;
ALTER TABLE dbo.LogActionType ADD  DEFAULT getdate() FOR RecordDate;

ALTER TABLE dbo.LogAction ADD  DEFAULT getdate() FOR CreateDate;

--

CREATE UNIQUE NONCLUSTERED INDEX IX_BomHeader_Orders ON BomHeader (Orders);
CREATE UNIQUE NONCLUSTERED INDEX IX_BomHeader_SerialNumber ON BomHeader (SerialNumber);
CREATE UNIQUE NONCLUSTERED INDEX IX_RootItem_Izdel ON RootItem (Izdel);
CREATE NONCLUSTERED INDEX IX_BomItem_SerialNumber ON BomItem (SerialNumber);

ALTER TABLE dbo.BomHeader ADD CONSTRAINT FK_BomHeader_RootItem FOREIGN KEY (RootItemId) REFERENCES dbo.RootItem(Id);

--

