-- ===========================================================
-- 1) PERSONA
-- ===========================================================
CREATE TABLE `persona` (
  `id`                 BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `dni`                VARCHAR(20)     NOT NULL,
  `sexo`               ENUM('M','F','X') NOT NULL,
  `nombre`             VARCHAR(120)    NOT NULL,
  `apellido`           VARCHAR(120)    NOT NULL,
  `fecha_nacimiento`   DATE            NULL,
  `email`              VARCHAR(255)    NULL,
  `telefono`           VARCHAR(30)     NULL,
  `fecha_creacion`     TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_persona_dni` (`dni`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===========================================================
-- 2) USUARIO
-- ===========================================================

CREATE TABLE `usuario` (
  `id`                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `persona_id`        BIGINT UNSIGNED NOT NULL,   -- FK al final
  `username`          VARCHAR(80)     NOT NULL,
  `password`          VARCHAR(255)    NOT NULL,
  `estado`            VARCHAR(20)     NOT NULL DEFAULT 'ACTIVO',
  `fecha_creacion`    TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ultimo_login`      DATETIME        NULL,
  `fecha_modificacion` TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `creado_por`         BIGINT UNSIGNED NULL,
  `modificado_por`     BIGINT UNSIGNED NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_usuario_username` (`username`),
  UNIQUE KEY `uk_usuario_persona`  (`persona_id`),
  CONSTRAINT `fk_usuario_creado_por`
    FOREIGN KEY (`creado_por`)  REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_usuario_modificado_por`
    FOREIGN KEY (`modificado_por`) REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_persona_id_por`
    FOREIGN KEY (`persona_id`) REFERENCES `persona`(`id`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===========================================================
-- 3) DIRECCION
-- ===========================================================
CREATE TABLE `direccion` (
  `id`              BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `persona_id`      BIGINT UNSIGNED NOT NULL,
  `calle`           VARCHAR(200)    NOT NULL,
  `numero`          VARCHAR(20),
  `piso`            VARCHAR(20),
  `dpto`            VARCHAR(20),
  `barrio`          VARCHAR(120),
  `ciudad`          VARCHAR(120)    NOT NULL,
  `provincia`       VARCHAR(120)    NOT NULL,
  `pais`            VARCHAR(120)    NOT NULL DEFAULT 'Argentina',
  `cp`              VARCHAR(20),
  `fecha_creacion`     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `creado_por`         BIGINT UNSIGNED NULL,
  `modificado_por`     BIGINT UNSIGNED NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_direccion_persona`
    FOREIGN KEY (`persona_id`) REFERENCES `persona`(`id`)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_direccion_creado_por`
    FOREIGN KEY (`creado_por`)  REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_direccion_modificado_por`
    FOREIGN KEY (`modificado_por`) REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===========================================================
-- 4) PROPIETARIO
-- ===========================================================
CREATE TABLE `propietario` (
  `id`              BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `persona_id`      BIGINT UNSIGNED NOT NULL,
  `estado`          VARCHAR(20) NOT NULL DEFAULT 'ACTIVO',
  `fecha_creacion`     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `creado_por`         BIGINT UNSIGNED NULL,
  `modificado_por`     BIGINT UNSIGNED NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_propietario_persona` (`persona_id`),
  CONSTRAINT `fk_propietario_persona`
    FOREIGN KEY (`persona_id`) REFERENCES `persona`(`id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_propietario_creado_por`
    FOREIGN KEY (`creado_por`)  REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_propietario_modificado_por`
    FOREIGN KEY (`modificado_por`) REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===========================================================
-- 5) INQUILINO
-- ===========================================================
CREATE TABLE `inquilino` (
  `id`              BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `persona_id`      BIGINT UNSIGNED NOT NULL,
  `estado`          VARCHAR(20) NOT NULL DEFAULT 'ACTIVO',
  `fecha_creacion`     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `creado_por`         BIGINT UNSIGNED NULL,
  `modificado_por`     BIGINT UNSIGNED NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_inquilino_persona` (`persona_id`),
  CONSTRAINT `fk_inquilino_persona`
    FOREIGN KEY (`persona_id`) REFERENCES `persona`(`id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_inquilino_creado_por`
    FOREIGN KEY (`creado_por`)  REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_inquilino_modificado_por`
    FOREIGN KEY (`modificado_por`) REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===========================================================
-- 6) INMUEBLE
-- ===========================================================
CREATE TABLE `inmueble` (
  `id`              BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `propietario_id`  BIGINT UNSIGNED NOT NULL,
  `estado`          VARCHAR(20) NOT NULL DEFAULT 'ACTIVO',
  `tipo`            VARCHAR(50),
  `superficie_m2`   DECIMAL(10,2),
  `ambientes`       TINYINT,
  `banos`           TINYINT,
  `cochera`         TINYINT(1) NOT NULL DEFAULT 0,
  `direccion`       VARCHAR(500),
  `descripcion`     TEXT,
  `fecha_creacion`     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `creado_por`         BIGINT UNSIGNED NULL,
  `modificado_por`     BIGINT UNSIGNED NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_inmueble_propietario`
    FOREIGN KEY (`propietario_id`) REFERENCES `propietario`(`id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_inmueble_creado_por`
    FOREIGN KEY (`creado_por`)  REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_inmueble_modificado_por`
    FOREIGN KEY (`modificado_por`) REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===========================================================
-- 7) CONTRATO
-- ===========================================================
CREATE TABLE `contrato` (
  `id`              BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `inmueble_id`     BIGINT UNSIGNED NOT NULL,
  `inquilino_id`    BIGINT UNSIGNED NOT NULL,
  `fecha_inicio`    DATE NOT NULL,
  `fecha_fin`       DATE NOT NULL,
  `estado`          VARCHAR(20) NOT NULL DEFAULT 'VIGENTE',
  `monto_mensual`   DECIMAL(12,2),
  `moneda`          VARCHAR(10),
  `deposito`        DECIMAL(12,2),
  `observaciones`   TEXT,
  `fecha_creacion`     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `creado_por`         BIGINT UNSIGNED NULL,
  `modificado_por`     BIGINT UNSIGNED NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_contrato_inmueble`
    FOREIGN KEY (`inmueble_id`) REFERENCES `inmueble`(`id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_contrato_inquilino`
    FOREIGN KEY (`inquilino_id`) REFERENCES `inquilino`(`id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_contrato_creado_por`
    FOREIGN KEY (`creado_por`)  REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_contrato_modificado_por`
    FOREIGN KEY (`modificado_por`) REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `ck_contrato_fechas` CHECK (`fecha_fin` > `fecha_inicio`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===========================================================
-- 8) PAGO
-- ===========================================================
CREATE TABLE `pago` (
  `id`                 BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `contrato_id`        BIGINT UNSIGNED NOT NULL,
  `estado`             VARCHAR(20) NOT NULL DEFAULT 'PENDIENTE',
  `periodo_anio`       SMALLINT NOT NULL,
  `periodo_mes`        TINYINT  NOT NULL,
  `fecha_vencimiento`  DATE NOT NULL,
  `importe`            DECIMAL(12,2) NOT NULL DEFAULT 0.00,
  `recargo`            DECIMAL(12,2) NOT NULL DEFAULT 0.00,
  `descuento`          DECIMAL(12,2) NOT NULL DEFAULT 0.00,
  `importe_pagado`     DECIMAL(12,2),
  `fecha_pago`         DATETIME,
  `medio_pago`         VARCHAR(50),
  `observaciones`      TEXT,
  `fecha_creacion`     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `creado_por`         BIGINT UNSIGNED NULL,
  `modificado_por`     BIGINT UNSIGNED NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_pago_periodo` (`contrato_id`,`periodo_anio`,`periodo_mes`),
  CONSTRAINT `fk_pago_contrato`
    FOREIGN KEY (`contrato_id`) REFERENCES `contrato`(`id`)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_pago_creado_por`
    FOREIGN KEY (`creado_por`)  REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_pago_modificado_por`
    FOREIGN KEY (`modificado_por`) REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `ck_pago_mes` CHECK (`periodo_mes` BETWEEN 1 AND 12)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===========================================================
-- 9) ROL
-- ===========================================================
CREATE TABLE `rol` (
  `id`              BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `nombre`          VARCHAR(120) NOT NULL,
  `codigo`          VARCHAR(60)  NOT NULL,
  `estado`          VARCHAR(20)  NOT NULL DEFAULT 'ACTIVO',
  `fecha_creacion`     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `creado_por`         BIGINT UNSIGNED NULL,
  `modificado_por`     BIGINT UNSIGNED NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_rol_codigo` (`codigo`),
  UNIQUE KEY `uk_rol_nombre` (`nombre`),
  CONSTRAINT `fk_rol_creado_por`
    FOREIGN KEY (`creado_por`)  REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_rol_modificado_por`
    FOREIGN KEY (`modificado_por`) REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ===========================================================
-- 10) USUARIO_ROL
-- ===========================================================
CREATE TABLE `usuario_rol` (
  `id`              BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `usuario_id`      BIGINT UNSIGNED NOT NULL,
  `rol_id`          BIGINT UNSIGNED NOT NULL,
  `estado`          VARCHAR(20) NOT NULL DEFAULT 'ACTIVO',
  `fecha_creacion`     TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `fecha_modificacion` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `creado_por`         BIGINT UNSIGNED NULL,
  `modificado_por`     BIGINT UNSIGNED NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_usuario_rol` (`usuario_id`,`rol_id`),

  CONSTRAINT `fk_usuario_rol_usuario`
    FOREIGN KEY (`usuario_id`) REFERENCES `usuario`(`id`)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_usuario_rol_rol`
    FOREIGN KEY (`rol_id`) REFERENCES `rol`(`id`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_usuario_rol_creado_por`
    FOREIGN KEY (`creado_por`)  REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `fk_usuario_rol_modificado_por`
    FOREIGN KEY (`modificado_por`) REFERENCES `usuario`(`id`)
    ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

/* TODO 
Agregar tabla imagen referenciando a usuario(foto perfil)
Agregar tabla inmueble_img*/


