use DBBTA

INSERT INTO CTEmployee (FName, LName, Pass, Email, EmployeeType) VALUES ('Pera', 'Detljić', 'bujanovac', 'fragg01@yandex.com', 1)

INSERT INTO  [Admin SMTP parameteres] (UserName, password, host) VALUES ("draff557@gmail.com"  , "KnyFXPIUr7H6ZIzZrHt7QQ==" ,"smtp.gmail.com");

ALTER TABLE CTEmployee ALTER column FName nvarchar(30) not null;
ALTER TABLE CTEmployee ALTER column LName nvarchar(30) not null;
ALTER TABLE CTEmployee ALTER column Pass nvarchar(30) not null;

ALTER TABLE Feedback DROP COLUMN Comment;
ALTER TABLE Feedback add Comment NTEXT NOT null;

INSERT INTO Country (Name) VALUES ("Serbia");
INSERT INTO Feedback (IDLifeInCity, IDUser, FeedbackDate, Rating, Heading, Comment) VALUES (2, 5, "2019-06-12", 4, "Mlogo dobra rana", "I dobro se jede.");

INSERT INTO destination (IdCountry, CityName) VALUES ( 1, "Belgrade"), (1, "Novi Sad")

UPDATE [Admin SMTP parameteres] SET password="KnyFXPIUr7EvCoE0lLRheQ==" WHERE UserName="draff557@gmail.com"


DELETE FROM CTEmployee WHERE ID= 14

SELECT * FROM [CTEmployee]
SELECT * FROM [destination]
SELECT * FROM [Lifeincity]
SELECT * FROM [Feedback]