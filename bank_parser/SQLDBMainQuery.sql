﻿create table Bank
(
IndexB int identity(1,1) not null,
NameB nvarchar(50) not null,
NameIdB nvarchar(50) not null,
constraint PK_IndexB primary key (IndexB)
)

create table Contribution
(
IndexCont int identity(1,1) not null,
constraint PK_IndexCont primary key (IndexCont),
NameC nvarchar(max) not null,
Valuta nvarchar(50) not null,
Summa nvarchar(50) not null,
Srok nvarchar(50) not null,
Protsent nvarchar(50) not null,
IndexB int not null,
constraint IndexBCont_FK foreign key(IndexB) references Bank(IndexB)
)

create table Credit
(
IndexСr int identity(1,1) not null,
constraint PK_IndexCr primary key (IndexСr),
NameCr nvarchar(max) not null,
Valuta nvarchar(50) not null,
Summa nvarchar(50) not null,
Srok nvarchar(50) not null,
Protsent nvarchar(50) not null,
IndexB int not null,
constraint IndexBCr_FK foreign key(IndexB) references Bank(IndexB)
)

create table Departament
(
IndexDep int identity(1,1) not null,
constraint PK_IndexDep primary key (IndexDep),
NameD nvarchar(max) not null,
AddressD nvarchar(max) not null,
PhonesD nvarchar(max) not null,
WorkTimeD nvarchar(max) not null,
CloseTimeD nvarchar(max) not null,
CityD nvarchar(30) not null,
IndexB int not null,
constraint IndexBDep_FK foreign key(IndexB) references Bank(IndexB)
)

create table Сurrency
(
IndexСur int identity(1,1) not null,
constraint PK_IndexCur primary key (IndexСur),
NameCur nvarchar(50) not null,
BuyCur nvarchar(20) not null,
SellCur nvarchar(20) not null,
NB_RB nvarchar(20) not null,
UpdateTime nvarchar(30) not null,
IndexB int not null,
constraint IndexBCur_FK foreign key(IndexB) references Bank(IndexB)
)

select * from Bank
select * from Departament
select * from Сurrency
drop table Bank
drop table Сurrency
drop table Departament
drop table Contribution
drop table Credit