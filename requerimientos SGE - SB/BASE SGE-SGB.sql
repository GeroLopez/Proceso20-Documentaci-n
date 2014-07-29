CREATE TABLE Departamento (
  idDepartamento INTEGER UNSIGNED NOT NULL,
  nombre VARCHAR(20) NULL,
  PRIMARY KEY(idDepartamento)
);

CREATE TABLE Empleado (
  cedula INTEGER UNSIGNED NOT NULL,
  Departamento_idDepartamento INTEGER UNSIGNED NOT NULL,
  nombre1 VARCHAR(20) NULL,
  nombre2 VARCHAR(20) NULL,
  apellido1 VARCHAR(20) NULL,
  apellido2 VARCHAR(20) NULL,
  estado BOOL NULL,
  usuario VARCHAR(20) NOT NULL,
  pass VARCHAR(20) NULL,
  tipo VARCHAR(20) NOT NULL,
  PRIMARY KEY(cedula),
  INDEX Empleado_FKIndex1(Departamento_idDepartamento),
  FOREIGN KEY(Departamento_idDepartamento)
    REFERENCES Departamento(idDepartamento)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION
);

CREATE TABLE Estudiante (
  Empleado_cedula INTEGER UNSIGNED NOT NULL,
  fechaIngreso DATE NOT NULL AUTO_INCREMENT,
  fechaSalida DATE NULL,
  enActividad BOOL NULL,
  horasQueDebe INTEGER UNSIGNED NULL,
  horasExtra INTEGER UNSIGNED NULL,
  investigacion BOOL NULL,
  monitoreo BOOL NULL,
  PRIMARY KEY(Empleado_cedula),
  INDEX Estudiante_FKIndex1(Empleado_cedula),
  FOREIGN KEY(Empleado_cedula)
    REFERENCES Empleado(cedula)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION
);

CREATE TABLE Entrada (
  idEntrada INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
  Estudiante_Empleado_cedula INTEGER UNSIGNED NOT NULL,
  fecha DATE NULL,
  horaEntrada NUMERIC NULL,
  horaSalida NUMERIC NULL,
  duracion NUMERIC NULL,
  tipo VARCHAR(20) NOT NULL,
  PRIMARY KEY(idEntrada),
  INDEX Entrada_FKIndex1(Estudiante_Empleado_cedula),
  FOREIGN KEY(Estudiante_Empleado_cedula)
    REFERENCES Estudiante(Empleado_cedula)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION
);

CREATE TABLE EntradaMonitoreo (
  Entrada_idEntrada INTEGER UNSIGNED NOT NULL,
  turno VARCHAR(10) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY(Entrada_idEntrada),
  INDEX Monitoreo_FKIndex1(Entrada_idEntrada),
  FOREIGN KEY(Entrada_idEntrada)
    REFERENCES Entrada(idEntrada)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION
);

CREATE TABLE NoticiasMonitoreo (
  idNoticiasMonitoreo NUMERIC NOT NULL AUTO_INCREMENT,
  EntradaMonitoreo_Entrada_idEntrada INTEGER UNSIGNED NOT NULL,
  contenido VARCHAR(500) NOT NULL,
  PRIMARY KEY(idNoticiasMonitoreo),
  INDEX NoticiasMonitoreo_FKIndex1(EntradaMonitoreo_Entrada_idEntrada),
  FOREIGN KEY(EntradaMonitoreo_Entrada_idEntrada)
    REFERENCES EntradaMonitoreo(Entrada_idEntrada)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION
);

CREATE TABLE BitacoraMonitoreo (
  idBitacoraMonitoreo NUMERIC NOT NULL AUTO_INCREMENT,
  EntradaMonitoreo_Entrada_idEntrada INTEGER UNSIGNED NOT NULL,
  cotenido VARCHAR(800) NULL,
  resaltado NUMERIC NULL,
  imagen BLOB NULL,
  PRIMARY KEY(idBitacoraMonitoreo),
  INDEX BitacoraMonitoreo_FKIndex1(EntradaMonitoreo_Entrada_idEntrada),
  FOREIGN KEY(EntradaMonitoreo_Entrada_idEntrada)
    REFERENCES EntradaMonitoreo(Entrada_idEntrada)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION
);

CREATE TABLE EntradaInvestigacion (
  Entrada_idEntrada INTEGER UNSIGNED NOT NULL,
  descripcion VARCHAR(150) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY(Entrada_idEntrada),
  INDEX Investigacion_FKIndex1(Entrada_idEntrada),
  FOREIGN KEY(Entrada_idEntrada)
    REFERENCES Entrada(idEntrada)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION
);

CREATE TABLE ErroresMonitoreo (
  idErroresMonitoreo NUMERIC NOT NULL AUTO_INCREMENT,
  EntradaMonitoreo_Entrada_idEntrada INTEGER UNSIGNED NOT NULL,
  descripcion VARCHAR(800) NULL,
  fechaError TIMESTAMP NULL,
  fechaSolucion TIMESTAMP NULL,
  informadoA INTEGER UNSIGNED NOT NULL,
  PRIMARY KEY(idErroresMonitoreo),
  INDEX ErroresMonitoreo_FKIndex1(EntradaMonitoreo_Entrada_idEntrada),
  UNIQUE INDEX ErroresMonitoreo_FKIndex2(),
  FOREIGN KEY(EntradaMonitoreo_Entrada_idEntrada)
    REFERENCES EntradaMonitoreo(Entrada_idEntrada)
      ON DELETE NO ACTION
      ON UPDATE NO ACTION
);


