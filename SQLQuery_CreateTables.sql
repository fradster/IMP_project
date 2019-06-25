use master
create database DBBTA

DROP DATABASE DBBTA

use DBBTA

create table CTEmployee(
	ID int identity(1, 1) not null,
	FName varchar(30) not null,
	LName varchar(30) not null,
	Pass varchar(30) not null,
	Email varchar(30) not null,
	EmployeeType int not null, /*0 - regular user, 1 - admin*/
);

create table Country(
	ID int identity(1, 1) not null,
	Name nvarchar(50) not null
);

create table Destination(
	ID int identity(1, 1) not null,
	IDCountry int not null,
	CityName nvarchar(50) not null,
	State nvarchar(50)
);

create table LifeInCity(
	ID int identity(1, 1) not null,
	IDDestination int not null,
	IDAccTraDesCategory int not null,
	Description text,
	ChangeDate datetime not null,
	IDAdmin int not null
);
/*drop table LifeInCity*/

create table Accommodation(
	ID int identity(1, 1) not null,
	IDDestination int not null,
	IDAccTraDesCategory int not null,
	Description text
);
/*drop table Accommodation*/

create table Transportation(
	ID int identity(1, 1) not null,
	IDDestinationFrom int not null, /*CityFrom*/
	IDAccommodation int not null, /*CityTo*/
	IDAccTraDesCategory int not null,
	Description text
);
/*drop table Transportation*/

create table AccTraDesCategory(
	ID int identity(1, 1) not null,
	AccTraDesType varchar(30) not null,
);
/*drop table AccTraDesCategory*/

create table Provider(
	ID int identity(1, 1) not null,
	IDAccTraDesCategory int not null,
	Name nvarchar(50) not null
);

create table Feedback(
	ID int identity(1, 1) not null,
	IDLifeInCity int,
	IDTransportation int,
	IDAccommodation int,
	IDUser int not null,
	FeedbackDate datetime not null,
	Comment text not null,
	Rating int,
	Heading nvarchar(50) not null
);

create table Info(
	ID int identity(1, 1) not null,
	IDProvider int,
	IDEmployee int,
	IDDestinationProvider int, /*provider city*/
	IDAccommodation int,
	AccommodationTypeName nvarchar(50), /*hotel/Hostel etc name*/
	WebAddress varchar(50), /*web address for hotel/hostel etc*/
	Category int, /*number of stars for hotel/hostel etc*/
	Address1 varchar(50) not null,
	Address2 varchar(50),
	Tel1 varchar(20) not null,
	Tel2 varchar(20)
);
/*drop table Info*/

create table Picture(
	ID int identity(1, 1) not null,
	IDLifeInCity int,
	IDAccommodation int,
	IDEmployee int,
	IDProvider int
);