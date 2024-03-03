DROP DATABASE IF EXISTS InmanDB;
CREATE DATABASE InmanDB;
USE InmanDB;

CREATE TABLE Cliente
(
    id        INT                  AUTO_INCREMENT,
    nombre    NVARCHAR(100)        NOT NULL,
    apellido  NVARCHAR(100)        NOT NULL,
    sexo      ENUM ('M', 'F')      NOT NULL,
    telefono  NVARCHAR(20) UNIQUE  NOT NULL,
    correo    NVARCHAR(255) UNIQUE NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE Factura
(
    codigoFactura      NVARCHAR(30),
    idCliente          INT            NOT NULL,
    porcentajeImpuesto INT            NOT NULL DEFAULT 18,
    subtotal           DECIMAL(10, 2) NOT NULL DEFAULT 0,
    total              DECIMAL(10, 2) NOT NULL DEFAULT 0,
    descuentoTotal     DECIMAL(10, 2) NOT NULL DEFAULT 0,
    impuestoTotal      DECIMAL(10, 2) NOT NULL DEFAULT 0,
    PRIMARY KEY (codigoFactura),
    FOREIGN KEY (idCliente) REFERENCES Cliente (id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE TipoProducto
(
    id         INT AUTO_INCREMENT,
    nombreTipo NVARCHAR(100) UNIQUE NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE Producto
(
    codigoProducto      NVARCHAR(30),
    idTipoProducto      INT            NOT NULL,
    nombre              NVARCHAR(100)  NOT NULL,
    precio              DECIMAL(10, 2) NOT NULL DEFAULT 0,
    porcentajeDescuento INT            NOT NULL DEFAULT 0,
    PRIMARY KEY (codigoProducto),
    FOREIGN KEY (idTipoProducto) REFERENCES TipoProducto (id) ON DELETE CASCADE
);

CREATE TABLE FacturaProducto
(
    id             INT AUTO_INCREMENT,
    codigoFactura  NVARCHAR(30),
    codigoProducto NVARCHAR(30),
    precio         DECIMAL(10, 2) NOT NULL DEFAULT 0,
    descuento      DECIMAL(10, 2) NOT NULL DEFAULT 0,
    PRIMARY KEY (id, codigoFactura, codigoProducto),
    FOREIGN KEY (codigoFactura) REFERENCES Factura (codigoFactura) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (codigoProducto) REFERENCES Producto (codigoProducto) ON DELETE CASCADE ON UPDATE CASCADE
);

DELIMITER //
CREATE PROCEDURE InsertarCliente(
    IN p_nombre NVARCHAR(100),
    IN p_apellido NVARCHAR(100),
    IN p_sexo ENUM ('M', 'F'),
    IN p_telefono NVARCHAR(20),
    IN p_correo NVARCHAR(255)
)
BEGIN
    INSERT INTO Cliente (nombre, apellido, sexo, telefono, correo)
    VALUES (p_nombre, p_apellido, p_sexo, p_telefono, p_correo);
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE ActualizarCliente (
    IN p_id INT,
    IN p_nombre NVARCHAR(100),
    IN p_apellido NVARCHAR(100),
    IN p_sexo ENUM('M', 'F'),
    IN p_telefono NVARCHAR(20),
    IN p_correo NVARCHAR(255)
)
BEGIN
    UPDATE Cliente
    SET
        nombre = p_nombre,
        apellido = p_apellido,
        sexo = p_sexo,
        telefono = p_telefono,
        correo = p_correo
    WHERE
        id = p_id;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE InsertarFactura(
    IN p_codigoFactura NVARCHAR(30),
    IN p_idCliente INT,
    IN p_porcentajeImpuesto INT
)
BEGIN
    DECLARE isUpdate INT DEFAULT 0;
    DECLARE p_subtotal DECIMAL(10, 2) DEFAULT 0;

    -- Verificar si se está actualizando un registro existente
    SELECT COUNT(*) INTO isUpdate FROM Factura WHERE codigoFactura = p_codigoFactura;

    -- Si se está actualizando un registro existente
    IF isUpdate > 0 THEN
        -- Calcular el subtotal de la factura
        SELECT COALESCE(SUM(precio - descuento), 0)
        INTO p_subtotal
        FROM FacturaProducto
        WHERE codigoFactura = p_codigoFactura;

        -- Actualizar los campos en la factura
        UPDATE Factura
        SET porcentajeImpuesto = p_porcentajeImpuesto,
            subtotal           = p_subtotal,
            impuestoTotal      = (p_subtotal * (p_porcentajeImpuesto / 100.0)),
            total              = p_subtotal + (p_subtotal * (p_porcentajeImpuesto / 100.0))
        WHERE codigoFactura = p_codigoFactura;
    ELSE
        -- Si no se está actualizando un registro existente, simplemente insertar el nuevo registro
        INSERT INTO Factura (codigoFactura, idCliente, porcentajeImpuesto)
        VALUES (p_codigoFactura, p_idCliente, p_porcentajeImpuesto);
    END IF;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE InsertarTipoProducto(
    IN p_nombreTipoProducto NVARCHAR(100)
)
BEGIN
    INSERT INTO TipoProducto (nombreTipo)
    VALUES (p_nombreTipoProducto)
    ON DUPLICATE KEY UPDATE nombreTipo = p_nombreTipoProducto;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE InsertarProducto(
    IN p_codigoProducto NVARCHAR(30),
    IN p_idTipoProducto INT,
    IN p_nombreProducto NVARCHAR(100),
    IN p_precio DECIMAL(10, 2),
    IN p_porcentajeDescuento INT
)
BEGIN
    INSERT INTO Producto (codigoProducto, idTipoProducto, nombre, precio, porcentajeDescuento)
    VALUES (p_codigoProducto, p_idTipoProducto, p_nombreProducto, p_precio, p_porcentajeDescuento)
    ON DUPLICATE KEY UPDATE idTipoProducto      = p_idTipoProducto,
                            nombre              = p_nombreProducto,
                            precio              = p_precio,
                            porcentajeDescuento = p_porcentajeDescuento;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE InsertarFacturaProducto(
    IN p_codigoFactura NVARCHAR(30),
    IN p_codigoProducto NVARCHAR(30),
    IN p_precio DECIMAL(10, 2)
)
BEGIN
    DECLARE p_porcentajeDescuento INT;
    DECLARE p_descuento DECIMAL(10, 2);

    -- Obtener el porcentaje de descuento del producto
    SELECT porcentajeDescuento
    INTO p_porcentajeDescuento
    FROM Producto
    WHERE codigoProducto = p_codigoProducto;

    -- Calcular el descuento para el producto
    SET p_descuento = p_precio * (p_porcentajeDescuento / 100.0);

    -- Insertar el registro en FacturaProducto
    INSERT INTO FacturaProducto (codigoFactura, codigoProducto, precio, descuento)
    VALUES (p_codigoFactura, p_codigoProducto, p_precio, p_descuento);
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE ObtenerClientes()
BEGIN
    SELECT * FROM Cliente;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE ObtenerFacturas()
BEGIN
    SELECT * FROM Factura;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE ObtenerTiposProducto()
BEGIN
    SELECT * FROM TipoProducto;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE ObtenerProductos()
BEGIN
    SELECT * FROM Producto;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE ObtenerFacturasProductos()
BEGIN
    SELECT * FROM FacturaProducto;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE BorrarCliente(IN p_id INT)
BEGIN
    DELETE FROM Cliente WHERE id = p_id;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE BorrarFactura(IN p_codigoFactura VARCHAR(30))
BEGIN
    DELETE FROM Factura WHERE codigoFactura = p_codigoFactura;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE BorrarTipoProducto(IN p_id INT)
BEGIN
    DELETE FROM TipoProducto WHERE id = p_id;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE BorrarProducto(IN p_codigoProducto VARCHAR(30))
BEGIN
    DELETE FROM Producto WHERE codigoProducto = p_codigoProducto;
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE BorrarFacturaProducto(IN p_id INT)
BEGIN
    DELETE FROM FacturaProducto WHERE id = p_id;
END //
DELIMITER ;

DELIMITER //
CREATE TRIGGER actualizarPrecioDescuentoFacturaProducto
    AFTER INSERT
    ON FacturaProducto
    FOR EACH ROW
BEGIN
    DECLARE tmpSubtotalPrecio DECIMAL(10, 2);
    DECLARE tmpTotalDescuento DECIMAL(10, 2);
    DECLARE tmpImpuesto DECIMAL(10, 2) default 0;
    DECLARE tmpTotal DECIMAL(10, 2);
    DECLARE tmpPorcentajeImpuesto INT;

    -- Calcular el subtotal de la factura
    SELECT SUM(precio)
    INTO tmpSubtotalPrecio
    FROM FacturaProducto
    WHERE codigoFactura = NEW.codigoFactura;

    -- Calcular el total de descuento para la factura
    SELECT SUM(descuento)
    INTO tmpTotalDescuento
    FROM FacturaProducto
    WHERE codigoFactura = NEW.codigoFactura;

    -- Obtener el porcentaje de impuesto de la factura
    SELECT porcentajeImpuesto
    INTO tmpPorcentajeImpuesto
    FROM Factura
    WHERE codigoFactura = NEW.codigoFactura;

    -- Calcular el impuesto de la factura
    SET tmpImpuesto = (tmpSubtotalPrecio - tmpTotalDescuento) * (tmpPorcentajeImpuesto / 100);

    -- Calcular el total de la factura
    SET tmpTotal = (tmpSubtotalPrecio - tmpTotalDescuento) + tmpImpuesto;

    -- Actualizar los campos en la factura
    UPDATE Factura
    SET subtotal       = tmpSubtotalPrecio,
        descuentoTotal = tmpTotalDescuento,
        impuestoTotal  = tmpImpuesto,
        total          = tmpTotal
    WHERE codigoFactura = NEW.codigoFactura;
END //
DELIMITER ;