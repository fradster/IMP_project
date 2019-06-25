use DBBTA

CREATE TABLE [Employee Activation] (
	Id					INT										PRIMARY KEY CLUSTERED,
	ActivationCode	uniqueidentifier	NOT NULL,
);

CREATE TABLE [Admin SMTP parameteres] (
	Ind					int						PRIMARY KEY IDENTITY(1,1),
	UserName		NVARCHAR(50)	NOT NULL,
	password		NVARCHAR(30)	NOT NULL,
	host				NVARCHAR(30)	NOT NULL
);

SELECT * FROM [Admin SMTP parameteres]
SELECT * FROM [CTEmployee]