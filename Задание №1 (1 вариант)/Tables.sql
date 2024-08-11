-- PostgreSQL syntax

create table Courses (
    ID int,
    Name varchar(50),
    constraint courses_pk primary key (ID)
);

create table Students (
    ID int,
    Name varchar(20),
    constraint students_pk primary key (ID)
);

create table Enrolments (
    IDCourse int,
    IDStudent int,
    constraint enrolments_pk primary key (IDCourse, IDStudent),
    constraint enrolments_fk_courses foreign key (IDCourse) references Courses (ID),
    constraint enrolments_fk_students foreign key (IDStudent) references Students (ID)
);

create table Schedule (
    ID int,
    IDCourse int,
    StartDate timestamp,
    EndDate timestamp,
    Capacity int,
    constraint schedule_pk primary key (ID),
    constraint schedule_fk_courses foreign key (IDCourse) references Courses (ID),
    constraint schedule_date check (EndDate > StartDate),
    constraint schedule_capacity check (Capacity > 0)
);
