-- Script para crear la tabla usuario en MySQL
-- Este script es compatible con la estructura existente de la base de datos

-- Verificar si la tabla existe y eliminarla si es necesario (¡CUIDADO EN PRODUCCIÓN!)
-- DROP TABLE IF EXISTS `usuario`;

-- Crear la tabla usuario
CREATE TABLE IF NOT EXISTS `usuario` (
  `IDUsuario` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(200) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `Pass` varchar(50) DEFAULT NULL,
  `EmailConfirmado` tinyint DEFAULT '0',
  `EmailConfirmadoToken` varchar(50) DEFAULT NULL,
  `EmailConfirmadoTokenExpira` datetime(3) DEFAULT NULL,
  `Telefono` varchar(20) DEFAULT NULL,
  `FechaCambioPass` datetime(3) DEFAULT NULL,
  `IntentosFallidos` int DEFAULT NULL,
  `Activo` tinyint DEFAULT NULL,
  `IDPerfil` int DEFAULT NULL,
  PRIMARY KEY (`IDUsuario`),
  UNIQUE KEY `idx_email` (`Email`),
  KEY `idx_activo` (`Activo`),
  KEY `idx_email_confirmado` (`EmailConfirmado`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Insertar un usuario de ejemplo para pruebas (opcional)
-- NOTA: La contraseña debería estar hasheada en un sistema real
INSERT INTO `usuario` (`Nombre`, `Email`, `Pass`, `EmailConfirmado`, `Telefono`, `IntentosFallidos`, `Activo`, `IDPerfil`)
VALUES 
  ('Administrador', 'admin@lusegestion.com', 'Admin123', 1, '555-0100', 0, 1, 1),
  ('Usuario Demo', 'demo@lusegestion.com', 'Demo123', 1, '555-0101', 0, 1, 2)
ON DUPLICATE KEY UPDATE 
  `Nombre` = VALUES(`Nombre`);
