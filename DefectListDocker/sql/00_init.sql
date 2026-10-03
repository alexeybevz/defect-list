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
	CodeLsf82 int,
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
	Defect nvarchar(2000) COLLATE Cyrillic_General_CI_AS NULL,
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
	Defect nvarchar(2000) COLLATE Cyrillic_General_CI_AS NULL,
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
	CreateDate datetime NOT NULL DEFAULT(GETDATE()),
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
INSERT INTO LogActionType (LogActionTypeId, LogActionTypeName, CreateDate, CreatedBy, RecordDate, UpdatedBy) VALUES(6, N'BomItemReplaceQty', getdate(), N'admin', getdate(), N'admin');
INSERT INTO LogActionType (LogActionTypeId, LogActionTypeName, CreateDate, CreatedBy, RecordDate, UpdatedBy) VALUES(7, N'BomItemSplit', getdate(), N'admin', getdate(), N'admin');

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

INSERT INTO BomHeader (Orders, SerialNumber, IzdelQty, StateDetalsId, Comment, DateOfSpecif, DateOfTehproc, DateOfMtrl, CreateDate, CreatedBy, RecordDate, UpdatedBy, Dic_Ordering_ID, Code_LSF82, RootItemId, Contract, ContractDateOpen, SerialNumberAfterRepair, State, DateOfPreparation, HeaderType)
VALUES(N'тест', N'12345', 1, 1, NULL, '2026-07-02 00:00:00.000', '2026-07-02 00:00:00.000', '2026-07-02 00:00:00.000', getdate(), N'admin', getdate(), N'admin', NULL, NULL, 1, N'тест', '2026-07-02', N'12345', NULL, NULL, N'Ремонт');

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
CREATE UNIQUE NONCLUSTERED INDEX IX_BomHeader_SerialNumber ON BomHeader (Contract, SerialNumber);
CREATE UNIQUE NONCLUSTERED INDEX IX_RootItem_Izdel ON RootItem (Izdel);
CREATE NONCLUSTERED INDEX IX_BomItem_SerialNumber ON BomItem (SerialNumber);

ALTER TABLE dbo.BomHeader ADD CONSTRAINT FK_BomHeader_RootItem FOREIGN KEY (RootItemId) REFERENCES dbo.RootItem(Id);

--

CREATE TABLE dbo.NotificationEventType (
	Id int IDENTITY(1,1) NOT NULL,
	Name nvarchar(150) COLLATE Cyrillic_General_CI_AS NOT NULL,
	PRIMARY KEY (Id)
);

SET IDENTITY_INSERT Users ON

INSERT INTO dbo.NotificationEventType (Name) VALUES ('UserBomHeaderSubscriptionDailyDigest');
INSERT INTO dbo.NotificationEventType (Name) VALUES ('BomItemsBzrChangesDailyDigest');

SET IDENTITY_INSERT Users OFF

CREATE TABLE dbo.NotificationDispatches (
	Id int IDENTITY(1,1) NOT NULL,
	UserId int NOT NULL,
	NotificationEventTypeId int NOT NULL,
	PayloadHash varchar(64) NOT NULL,
	Status nvarchar(10) COLLATE Cyrillic_General_CI_AS NOT NULL, -- Pending, Sent, Failed
	Attempts int NOT NULL DEFAULT 0,
	ScheduleAt datetime NOT NULL, -- Когда система поставила уведомление в очередь на отправку
	SentAt datetime NULL, -- Когда реально ушло письмо
	LastAttempAt datetime NULL, -- Когда была последняя попытка
	Error nvarchar(max) COLLATE Cyrillic_General_CI_AS NULL,
	PRIMARY KEY (Id)
)

ALTER TABLE dbo.NotificationDispatches ADD CONSTRAINT FK_NotificationDispatches_User FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);
ALTER TABLE dbo.NotificationDispatches ADD CONSTRAINT FK_NotificationDispatches_NType FOREIGN KEY (NotificationEventTypeId) REFERENCES dbo.NotificationEventType(Id);

CREATE UNIQUE INDEX UX_NotificationDispatches_PayloadHash ON NotificationDispatches(PayloadHash);

CREATE TABLE dbo.NotificationEventSubscribers (
	Id int IDENTITY(1,1) NOT NULL,
	UserId int NOT NULL,
	NotificationEventTypeId int NOT NULL,
	IsEnabled tinyint NOT NULL DEFAULT 1,
	CreatedAt datetime NOT NULL DEFAULT getdate(),
	PRIMARY KEY (Id)
);

ALTER TABLE dbo.NotificationEventSubscribers ADD CONSTRAINT FK_NotificationEventSubscribers_User FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);
ALTER TABLE dbo.NotificationEventSubscribers ADD CONSTRAINT FK_NotificationEventSubscribers_NType FOREIGN KEY (NotificationEventTypeId) REFERENCES dbo.NotificationEventType(Id);

-- === Начало блока Карт Измерения (КИ)

CREATE TABLE PossibleDefect (
    Id   int IDENTITY(1,1) NOT NULL,
    Name nvarchar(300) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
    IsActive tinyint NOT NULL DEFAULT 1,
    CONSTRAINT PK_PossibleDefect PRIMARY KEY (Id),
    CONSTRAINT UQ_PossibleDefect_Name UNIQUE(Name)
);

CREATE TABLE NominalValue (
    Id   int IDENTITY(1,1) NOT NULL,
    Name nvarchar(100) COLLATE Cyrillic_General_CI_AS NOT NULL,
    IsActive tinyint NOT NULL DEFAULT 1,
    CONSTRAINT PK_NominalValue PRIMARY KEY (Id),
    CONSTRAINT UQ_NominalValue_Name UNIQUE(Name)
);

CREATE TABLE RecommendedRepairMethod (
    Id   int IDENTITY(1,1) NOT NULL,
    Name nvarchar(300) COLLATE Cyrillic_General_CI_AS NOT NULL,
    IsActive tinyint NOT NULL DEFAULT 1,
    CONSTRAINT PK_RecommendedRepairMethod PRIMARY KEY (Id),
    CONSTRAINT UQ_RecommendedRepairMethod_Name UNIQUE(Name)
);

-- Типы форм для печати (Форма 2, Форма 3а, ...)
CREATE TABLE MeasurementsMapTypeForm (
    Id   int IDENTITY(1,1) NOT NULL,
    Name nvarchar(50) COLLATE Cyrillic_General_CI_AS NOT NULL,
    CONSTRAINT PK_MeasurementsMapTypeForm PRIMARY KEY (Id),
    CONSTRAINT UQ_MeasurementsMapTypeForm_Name UNIQUE(Name)
);

CREATE TABLE RequirementPostRepair (
    Id   int IDENTITY(1,1) NOT NULL,
    Name nvarchar(300) COLLATE Cyrillic_General_CI_AS NOT NULL,
    IsActive tinyint NOT NULL DEFAULT 1,
    CONSTRAINT PK_RequirementPostRepair PRIMARY KEY (Id),
    CONSTRAINT UQ_RequirementPostRepair_Name UNIQUE(Name)
);

-- ============================================================
-- СПРАВОЧНИК КАРТЫ ИЗМЕРЕНИЙ
-- Один справочник на тип номенклатуры (Code_LSF82)
-- ============================================================

CREATE TABLE MeasurementMapDictionary (
    Id                        int IDENTITY(1,1) NOT NULL,
	Name 					  nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,

    -- Привязка к типу номенклатуры.
    -- Code_LSF82 здесь не FK, потому что это внешний код из другой системы
    -- (он живёт в BomItem.Code_LSF82, не в отдельной таблице номенклатуры).
    Code_LSF82                int NOT NULL,

    -- Шаблон формы для печати карты (Форма 2, Форма 3а, ...)
    MeasurementsMapTypeFormId int NOT NULL,

    SketchFilePath			  nvarchar(150) COLLATE Cyrillic_General_CI_AS NULL,

    -- Версионирование
    Version                   int NOT NULL DEFAULT 1,

    -- IsActive = 0 означает, что справочник архивирован (не удалён физически).
    IsActive                  tinyint NOT NULL DEFAULT 1,

    Comment    nvarchar(200) COLLATE Cyrillic_General_CI_AS NULL,
    CreateDate datetime NOT NULL DEFAULT getdate(),
    CreatedBy  nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
    RecordDate datetime NOT NULL DEFAULT getdate(),
    UpdatedBy  nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,

    CONSTRAINT PK_MeasurementMapDictionary PRIMARY KEY (Id),
    CONSTRAINT FK_MeasurementMapDictionary_Form
        FOREIGN KEY (MeasurementsMapTypeFormId)
        REFERENCES MeasurementsMapTypeForm (Id)
);

-- Уникальность: один активный справочник на номенклатуру.
-- Если нужно допустить несколько версий активными одновременно — убрать.
--CREATE UNIQUE INDEX UX_MeasurementMapDictionary_Code
--    ON MeasurementMapDictionary (Code_LSF82)
--    WHERE IsActive = 1;

CREATE TABLE MeasurementMapDictionaryItem (
    Id                         int IDENTITY(1,1) NOT NULL,
    MeasurementMapDictionaryId int NOT NULL,

    -- Смысловая нумерация из документа: '1', '1а', '1б', '4в' и т.п.
    -- Хранится как строка, отдельно от SortOrder.
    CustomNumeration           nvarchar(10) COLLATE Cyrillic_General_CI_AS NULL,

    -- Числовой порядок для сортировки (чтобы не зависеть от алфавитной сортировки строки).
    SortOrder                  int NOT NULL,

    PossibleDefectId           int NOT NULL,

    NominalValueId             int NULL,
    AlternateNominalValueId    int NULL,

    RecommendedRepairMethodId  int NULL,

    RequirementPostRepairId    int NULL,

    ItemType                   tinyint NOT NULL DEFAULT 1,

    CreateDate datetime NOT NULL DEFAULT getdate(),
    CreatedBy  nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
    RecordDate datetime NOT NULL DEFAULT getdate(),
    UpdatedBy  nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,

    CONSTRAINT PK_MeasurementMapDictionaryItem PRIMARY KEY (Id),
    CONSTRAINT FK_MMDictionaryItem_Dictionary
        FOREIGN KEY (MeasurementMapDictionaryId)
        REFERENCES MeasurementMapDictionary (Id),
    CONSTRAINT FK_MMDictionaryItem_Defect
        FOREIGN KEY (PossibleDefectId)
        REFERENCES PossibleDefect (Id),
    CONSTRAINT FK_MMDictionaryItem_NominalValue
        FOREIGN KEY (NominalValueId)
        REFERENCES NominalValue (Id),
    CONSTRAINT FK_MMDictionaryItem_AlternateNominalValue
        FOREIGN KEY (AlternateNominalValueId)
        REFERENCES NominalValue (Id),
    CONSTRAINT FK_MMDictionaryItem_RepairMethod
        FOREIGN KEY (RecommendedRepairMethodId)
        REFERENCES RecommendedRepairMethod (Id),
    CONSTRAINT FK_MMDictionaryItem_RequirementPostRepair
        FOREIGN KEY (RequirementPostRepairId)
        REFERENCES RequirementPostRepair (Id),
    CONSTRAINT UQ_MeasurementMapDictionaryItem_PossibleDefect UNIQUE(MeasurementMapDictionaryId, PossibleDefectId)
);

CREATE INDEX IX_MMDictionaryItem_DictionaryId
    ON MeasurementMapDictionaryItem (MeasurementMapDictionaryId);


-- ============================================================
-- ЛОГ СПРАВОЧНИКА (версионирование)
-- Снимок делается при каждом UPDATE справочника или его строк.
-- Action: 1 = Create, 2 = Update, 3 = Delete
-- ============================================================

CREATE TABLE MeasurementMapDictionaryLog (
    Id                         int IDENTITY(1,1) NOT NULL,
    MeasurementMapDictionaryId int NOT NULL,
    Action                     tinyint NOT NULL,

    -- Снимок заголовка на момент изменения
    Name 					   nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
    Code_LSF82                 int NOT NULL,
    MeasurementsMapTypeFormId  int NOT NULL,
    Version                    int NOT NULL,
    IsActive                   tinyint NOT NULL,
    Comment                    nvarchar(200) COLLATE Cyrillic_General_CI_AS NULL,

    BoundRootItems 			   nvarchar(1000) COLLATE Cyrillic_General_CI_AS NULL,

    CreateDate datetime NOT NULL DEFAULT getdate(),
    CreatedBy  nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,

    CONSTRAINT PK_MeasurementMapDictionaryLog PRIMARY KEY (Id)
);

CREATE TABLE MeasurementMapDictionaryItemLog (
    Id                             int IDENTITY(1,1) NOT NULL,
    MeasurementMapDictionaryItemId int NOT NULL,
    MeasurementMapDictionaryId     int NOT NULL,
    Action                         tinyint NOT NULL,

    -- Снимок строки на момент изменения
    CustomNumeration               nvarchar(10) COLLATE Cyrillic_General_CI_AS NULL,
    SortOrder                      int NOT NULL,
    PossibleDefectId               int NULL,
    PossibleDefectName             nvarchar(300) COLLATE SQL_Latin1_General_CP1_CS_AS NULL,
    NominalValueId                 int NULL,
    NominalValueName               nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
    AlternateNominalValueId        int NULL,
    AlternateNominalValueName      nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
    RecommendedRepairMethodId      int NULL,
    RecommendedRepairMethodName    nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,
    RecommendedRepairMethodAlternativeNames nvarchar(3000) COLLATE Cyrillic_General_CI_AS NULL,
    RequirementPostRepairId        int NULL,
    RequirementPostRepairName      nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,

    ItemType                       tinyint NOT NULL,

    CreateDate datetime NOT NULL DEFAULT getdate(),
    CreatedBy  nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,

    CONSTRAINT PK_MeasurementMapDictionaryItemLog PRIMARY KEY (Id)
);


-- ============================================================
-- ЭКЗЕМПЛЯР КАРТЫ ИЗМЕРЕНИЙ
-- Один экземпляр на строку ДВ (BomItemId).
-- Строки копируются из справочника по Code_LSF82 при создании,
-- после чего живут независимо (можно корректировать без изменения справочника).
-- ============================================================

CREATE TABLE MeasurementMap (
    Id                         int IDENTITY(1,1) NOT NULL,

    -- Привязка к строке ДВ (1:1)
    BomItemId                  int NOT NULL,

    -- Из какого справочника скопировано (для трассировки).
    -- NULL не допускается: карта всегда создаётся на основе справочника.
    MeasurementMapDictionaryId int NOT NULL,

    -- Форма для печати: копируется из справочника, но может быть скорректирована.
    MeasurementsMapTypeFormId  int NOT NULL,

    -- Status: 1 = Draft (в работе), 2 = Completed (завершена)
    Status                     tinyint NOT NULL DEFAULT 1,

    Comment    nvarchar(200) COLLATE Cyrillic_General_CI_AS NULL,
    CreateDate datetime NOT NULL DEFAULT getdate(),
    CreatedBy  nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
    RecordDate datetime NOT NULL DEFAULT getdate(),
    UpdatedBy  nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,

    CONSTRAINT PK_MeasurementMap PRIMARY KEY (Id),
    CONSTRAINT FK_MeasurementMap_BomItem
        FOREIGN KEY (BomItemId)
        REFERENCES BomItem (Id),
    CONSTRAINT FK_MeasurementMap_Dictionary
        FOREIGN KEY (MeasurementMapDictionaryId)
        REFERENCES MeasurementMapDictionary (Id),
    CONSTRAINT FK_MeasurementMap_Form
        FOREIGN KEY (MeasurementsMapTypeFormId)
        REFERENCES MeasurementsMapTypeForm (Id)
);

-- Один экземпляр на строку ДВ
CREATE UNIQUE INDEX UX_MeasurementMap_BomItemId
    ON MeasurementMap (BomItemId);


CREATE TABLE MeasurementMapItem (
    Id                             int IDENTITY(1,1) NOT NULL,
    MeasurementMapId               int NOT NULL,

    -- Трассировка к строке справочника, из которой скопировано.
    -- NULL допускается: пользователь может добавить строку вручную сверх справочника.
    MeasurementMapDictionaryItemId int NULL,

    -- Поля скопированы из справочника при создании и живут независимо.
    -- Пользователь может скорректировать их для конкретного экземпляра.
    CustomNumeration               nvarchar(10) COLLATE Cyrillic_General_CI_AS NULL,
    SortOrder                      int NOT NULL,
    PossibleDefectId               int NOT NULL,
    NominalValueId                 int NULL,
    AlternateNominalValueId        int NULL,
    RecommendedRepairMethodId      int NULL,
    RequirementPostRepairId        int NULL,

    -- Фактические данные — вносит пользователь
    ActualValue                    nvarchar(200) COLLATE Cyrillic_General_CI_AS NULL,   -- фактическое значение
    ActualValueRecordDate 		   datetime NULL, 		 -- дата изменения фактического значения
    ActualRecommendedRepairMethodId int NULL,

    -- Отметка о выполнении работ по устранению дефекта (Выполнено / Устранено)
    MarkOfWorkCompletion           nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
    MarkOfWorkCompletionBy         nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,

    ItemType                       tinyint NOT NULL,

    CreateDate datetime NOT NULL DEFAULT getdate(),
    CreatedBy  nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
    RecordDate datetime NOT NULL DEFAULT getdate(),
    UpdatedBy  nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,
    RowVersion int NOT NULL DEFAULT 1,

    CONSTRAINT PK_MeasurementMapItem PRIMARY KEY (Id),
    CONSTRAINT FK_MeasurementMapItem_Map
        FOREIGN KEY (MeasurementMapId)
        REFERENCES MeasurementMap (Id),
    CONSTRAINT FK_MeasurementMapItem_DictionaryItem
        FOREIGN KEY (MeasurementMapDictionaryItemId)
        REFERENCES MeasurementMapDictionaryItem (Id),
    CONSTRAINT FK_MeasurementMapItem_Defect
        FOREIGN KEY (PossibleDefectId)
        REFERENCES PossibleDefect (Id),
    CONSTRAINT FK_MeasurementMapItem_NominalValue
        FOREIGN KEY (NominalValueId)
        REFERENCES NominalValue (Id),
    CONSTRAINT FK_MeasurementMapItem_AlternateNominalValue
        FOREIGN KEY (AlternateNominalValueId)
        REFERENCES NominalValue (Id),        
    CONSTRAINT FK_MeasurementMapItem_RepairMethod
        FOREIGN KEY (RecommendedRepairMethodId)
        REFERENCES RecommendedRepairMethod (Id),
    CONSTRAINT FK_MeasurementMapItem_ActualRepairMethod
        FOREIGN KEY (ActualRecommendedRepairMethodId)
        REFERENCES RecommendedRepairMethod (Id),        
    CONSTRAINT FK_MeasurementMapItem_RequirementPostRepair
        FOREIGN KEY (RequirementPostRepairId)
        REFERENCES RequirementPostRepair (Id)        
);

CREATE INDEX IX_MeasurementMapItem_MapId
    ON MeasurementMapItem (MeasurementMapId);


-- ============================================================
-- ЛОГ ЭКЗЕМПЛЯРА (версионирование)
-- Снимок делается при каждом изменении карты или её строк.
-- ============================================================

CREATE TABLE MeasurementMapLog (
    Id                         int IDENTITY(1,1) NOT NULL,
    MeasurementMapId           int NOT NULL,
    Action                     tinyint NOT NULL,

    -- Снимок заголовка
    BomItemId                  int NOT NULL,
    MeasurementMapDictionaryId int NOT NULL,
    MeasurementsMapTypeFormId  int NOT NULL,
    Status                     tinyint NOT NULL,
    Comment                    nvarchar(200) COLLATE Cyrillic_General_CI_AS NULL,

    CreateDate datetime NOT NULL DEFAULT getdate(),
    CreatedBy  nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,

    CONSTRAINT PK_MeasurementMapLog PRIMARY KEY (Id)
);

CREATE TABLE MeasurementMapItemLog (
    Id                             int IDENTITY(1,1) NOT NULL,
    MeasurementMapItemId           int NOT NULL,
    MeasurementMapId               int NOT NULL,
    Action                         tinyint NOT NULL,

    -- Снимок строки
    CustomNumeration               nvarchar(10) COLLATE Cyrillic_General_CI_AS NULL,
    SortOrder                      int NOT NULL,
    PossibleDefectId               int NULL,
    PossibleDefectName             nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,
    NominalValueId                 int NULL,
    NominalValueName               nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
    AlternateNominalValueId        int NULL,
    AlternateNominalValueName      nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
    RecommendedRepairMethodId      int NULL,
    RecommendedRepairMethodName    nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,
    RequirementPostRepairId        int NULL,
    RequirementPostRepairName      nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,
    ActualValue                    nvarchar(200) COLLATE Cyrillic_General_CI_AS NULL,
    ActualRecommendedRepairMethodId int NULL,
    ActualRecommendedRepairMethodName nvarchar(300) COLLATE Cyrillic_General_CI_AS NULL,
    MarkOfWorkCompletion           nvarchar(100) COLLATE Cyrillic_General_CI_AS NULL,
    MarkOfWorkCompletionBy         nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,

    ItemType                       tinyint NOT NULL,

    CreateDate datetime NOT NULL DEFAULT getdate(),
    CreatedBy  nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,

    CONSTRAINT PK_MeasurementMapItemLog PRIMARY KEY (Id)
);

CREATE TABLE MeasurementMapDictionarySketchFile (
    Id int IDENTITY(1,1) NOT NULL,
    MeasurementMapDictionaryId int NOT NULL,
    SketchFilePath nvarchar(500) COLLATE Cyrillic_General_CI_AS NULL,
    PRIMARY KEY (Id),
    CONSTRAINT FK_MeasurementMapDictionarySketchFile_DictId
        FOREIGN KEY (MeasurementMapDictionaryId)
        REFERENCES MeasurementMapDictionary (Id)
);

INSERT INTO PossibleDefect (Name) VALUES (N'Обрыв провода');
INSERT INTO PossibleDefect (Name) VALUES (N'Деформация контактов КУ-50В, КУ19');
INSERT INTO PossibleDefect (Name) VALUES (N'Трещины и сколы на изоляторе контакта КУ-19');
INSERT INTO PossibleDefect (Name) VALUES (N'Незначительные механические повреждения, точечная коррозия на гайках и втулках');
INSERT INTO PossibleDefect (Name) VALUES (N'Нарушение изоляции провода, сопротивление изоляции');
INSERT INTO PossibleDefect (Name) VALUES (N'Проверка эл. прочности изоляции');
INSERT INTO PossibleDefect (Name) VALUES (N'Обрыв прядей или проволок оплетки рукава, наличие механических повреждений, следов коррозий');
INSERT INTO PossibleDefect (Name) VALUES (N'Нарушение четкости маркировки');
INSERT INTO PossibleDefect (Name) VALUES (N'Истечение сроков службы контактов');
INSERT INTO PossibleDefect (Name) VALUES (N'Отклонение допуска радиального биения на Ø 〖175〗_(−0,04)');

INSERT INTO NominalValue (Name) VALUES (N'прозвонка');
INSERT INTO NominalValue (Name) VALUES (N'нет');
INSERT INTO NominalValue (Name) VALUES (N'не менее 1000 Мом');
INSERT INTO NominalValue (Name) VALUES (N'пробоя нет');
INSERT INTO NominalValue (Name) VALUES (N'да');
INSERT INTO NominalValue (Name) VALUES (N'пробой есть');
INSERT INTO NominalValue (Name) VALUES(N'〖32〗_(−0,16)
↗0,04
↗0,025');


INSERT INTO RecommendedRepairMethod (Name) VALUES (N'Ремонт');
INSERT INTO RecommendedRepairMethod (Name) VALUES (N'Замена провода');
INSERT INTO RecommendedRepairMethod (Name) VALUES (N'Замена контактов');
INSERT INTO RecommendedRepairMethod (Name) VALUES (N'Замена бирок');

INSERT INTO MeasurementsMapTypeForm (Name) VALUES (N'Форма 2');
INSERT INTO MeasurementsMapTypeForm (Name) VALUES (N'Форма 2в');
INSERT INTO MeasurementsMapTypeForm (Name) VALUES (N'Форма 3');

INSERT INTO MeasurementMapDictionary (Code_LSF82, MeasurementsMapTypeFormId, Version, IsActive, Comment) VALUES(3, 1, 1, 1, NULL);

INSERT INTO MeasurementMapDictionaryItem (MeasurementMapDictionaryId, CustomNumeration, SortOrder, PossibleDefectId, NominalValueId, AlternateNominalValueId, ItemType, RecommendedRepairMethodId, RequirementPostRepairId)
SELECT 1, N'1'	, 1	, 1	, 1, null	, 1, 2, NULL UNION ALL
SELECT 1, N'2'	, 2	, 2	, 2, 5		, 2, 3, NULL UNION ALL
SELECT 1, N'3'	, 3	, 3	, 2, 5		, 2, 3, NULL UNION ALL
SELECT 1, N'4,5', 4	, 4	, 2, 5		, 2, 1, NULL UNION ALL
SELECT 1, N'6'	, 5	, 5	, 3, null	, 3, 2, NULL UNION ALL
SELECT 1, N'6'	, 6	, 6	, 4, 6		, 2, 2, NULL UNION ALL
SELECT 1, N'7'	, 7	, 7	, 2, 5		, 2, 2, NULL UNION ALL
SELECT 1, N'8'	, 8	, 8	, 2, 5		, 2, 4, NULL UNION ALL
SELECT 1, N'9'	, 9	, 9	, 2, 5		, 2, 3, NULL UNION ALL
SELECT 1, N'10'	, 10, 10, 7, null	, 3, 3, NULL;

INSERT INTO RepairMethodToItem (RootItemId, ParentItem, ChildItem, RepairMethodId) VALUES (1, N'ДСЕ2', N'ДСЕ3', 1);

-- ============================================================
-- АЛЬТЕРНАТИВЫ РЕКОМЕНДУЕМОГО МЕТОДА РЕМОНТА
--
-- Для строки справочника карты (MeasurementMapDictionaryItem)
-- задаётся набор допустимых значений RecommendedRepairMethod,
-- из которых пользователь может выбирать при редактировании
-- конкретного экземпляра карты измерения.
--
-- Дефолтное значение (MeasurementMapDictionaryItem.RecommendedRepairMethodId)
-- НЕ удаляется и продолжает копироваться в MeasurementMapItem при создании карты,
-- как и раньше. Эта таблица только РАСШИРЯЕТ список вариантов на выбор.
--
-- Важный инвариант: дефолтное значение должно входить в набор альтернатив,
-- иначе пользователь, выбрав другое значение, не сможет вернуться к дефолту
-- через выпадающий список. Это обеспечивается на уровне приложения
-- (см. GetMeasurementMapDictionaryItemRepairMethodAlternativesQuery),
-- а не CHECK-constraint-ом, чтобы не блокировать ручное заполнение админом
-- в произвольном порядке.
-- ============================================================

CREATE TABLE MeasurementMapDictionaryItemRepairMethodAlternative (
    Id                              int IDENTITY(1,1) NOT NULL,
    MeasurementMapDictionaryItemId  int NOT NULL,
    RecommendedRepairMethodId       int NOT NULL,

    CreateDate datetime NOT NULL DEFAULT getdate(),
    CreatedBy  nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,

    CONSTRAINT PK_MMDictItemRepairMethodAlternative PRIMARY KEY (Id),
    CONSTRAINT FK_MMDictItemRepairMethodAlt_DictItem
        FOREIGN KEY (MeasurementMapDictionaryItemId)
        REFERENCES MeasurementMapDictionaryItem (Id),
    CONSTRAINT FK_MMDictItemRepairMethodAlt_Method
        FOREIGN KEY (RecommendedRepairMethodId)
        REFERENCES RecommendedRepairMethod (Id)
);

-- Одна и та же пара (строка справочника, метод) не должна повторяться
CREATE UNIQUE INDEX UX_MMDictItemRepairMethodAlt_DictItem_Method
    ON MeasurementMapDictionaryItemRepairMethodAlternative (MeasurementMapDictionaryItemId, RecommendedRepairMethodId);

CREATE INDEX IX_MMDictItemRepairMethodAlt_DictItemId
    ON MeasurementMapDictionaryItemRepairMethodAlternative (MeasurementMapDictionaryItemId);

INSERT INTO MeasurementMapDictionaryItemRepairMethodAlternative (
    MeasurementMapDictionaryItemId,
    RecommendedRepairMethodId,
    CreatedBy
)
VALUES
    (1, 3, 'admin'),
    (1, 2, 'admin'),
    (1, 4, 'admin');

-- ============================================================
--    Таблица привязок справочника к изделиям
--
--    Code_LSF82 намеренно денормализован из MeasurementMapDictionary —
--    это позволяет поставить уникальный индекс (RootItemId, Code_LSF82)
--    прямо на таблице Binding без JOIN, что гарантирует инвариант:
--    одна пара (изделие + номенклатура) → максимум один справочник.
--
--    Без денормализации индекс поставить невозможно (Code_LSF82
--    находится в другой таблице), а проверка только на уровне
--    приложения ненадёжна при конкурентном доступе.
-- ============================================================
CREATE TABLE MeasurementMapDictionaryBinding (
    Id                         int IDENTITY(1,1) NOT NULL,
    MeasurementMapDictionaryId int NOT NULL,
    RootItemId                 int NOT NULL,

    -- Денормализованный Code_LSF82 из MeasurementMapDictionary —
    -- нужен для уникального индекса (RootItemId, Code_LSF82)
    Code_LSF82                 int NOT NULL,

    CreateDate datetime NOT NULL DEFAULT getdate(),
    CreatedBy  nvarchar(50) COLLATE Cyrillic_General_CI_AS NULL,

    CONSTRAINT PK_MMDictionaryBinding PRIMARY KEY (Id),
    CONSTRAINT FK_MMDictionaryBinding_Dictionary
        FOREIGN KEY (MeasurementMapDictionaryId)
        REFERENCES MeasurementMapDictionary (Id),
    CONSTRAINT FK_MMDictionaryBinding_RootItem
        FOREIGN KEY (RootItemId)
        REFERENCES RootItem (Id)
);

-- Уникальность: одна пара (изделие + номенклатура) → один справочник.
-- Именно этот индекс является главным инвариантом системы привязок.
CREATE UNIQUE INDEX UX_MMDictionaryBinding_Root_Code
    ON MeasurementMapDictionaryBinding (RootItemId, Code_LSF82);

-- Вспомогательный индекс для быстрой выборки привязок по справочнику
-- (используется в AttachBindingsAsync при загрузке списка справочников)
CREATE INDEX IX_MMDictionaryBinding_DictionaryId
    ON MeasurementMapDictionaryBinding (MeasurementMapDictionaryId);

INSERT INTO MeasurementMapDictionaryBinding
    (MeasurementMapDictionaryId, RootItemId, Code_LSF82, CreatedBy)
SELECT
    d.Id,
    1,              -- RootItemId = 1 (единственное изделие в тестовой БД)
    d.Code_LSF82,
    'admin'
FROM MeasurementMapDictionary d;