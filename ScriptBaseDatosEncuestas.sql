
Create table Usuario
(
    IDUser int IDENTITY PRIMARY KEY,
    Usuario varchar(100),
    Pass varchar(100)
);

Create Table Encuesta(
    IDEncuesta int IDENTITY PRIMARY KEY,
    Nombre Varchar(100),
    Descripcion Varchar(100),
    Link Varchar (255),
    IDUser int,
	eliminado bit DEFAULT 0 NULL,
    FOREIGN KEY (IDUser) REFERENCES Usuario(IDUser)
);

Create table TipoDato(
    IDTipo int IDENTITY PRIMARY KEY,
    Tipo varchar(100)
);
insert into TipoDato(Tipo) values ('Texto');
insert into TipoDato(Tipo) values ('Número');
insert into TipoDato(Tipo) values ('Fecha');

Create table Campo(
    IDCampo int IDENTITY PRIMARY KEY,
    Nombre varchar(100),
    Titulo varchar(100),
    EsRequerido bit,
    IDTipo int,
    IDEncuesta int,
    FOREIGN key (IDTipo) REFERENCES TipoDato(IDTipo),
    FOREIGN key (IDEncuesta) REFERENCES Encuesta(IDEncuesta)
);

Create table Respuesta(
    IDRespuesta int IDENTITY PRIMARY key,
    IDEncuesta int,
    IDCampo int,
    Respuesta varchar(100)
    FOREIGN key (IDEncuesta) REFERENCES Encuesta(IDEncuesta),
    FOREIGN key (IDCampo) REFERENCES Campo(IDCampo)
);