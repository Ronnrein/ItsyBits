/*
    Run this as root on your dev server before migrating
    (This will remove existing databases, beware)
*/
DROP DATABASE IF EXISTS `itsybits`;
DROP DATABASE IF EXISTS `hangfire`;
CREATE DATABASE `itsybits`;
CREATE DATABASE `hangfire`;
CREATE USER IF NOT EXISTS `itsybits`@`localhost` IDENTIFIED BY 'itsybits';
GRANT ALL ON itsybits.* TO `itsybits`@`localhost`;
GRANT ALL ON hangfire.* TO `itsybits`@`localhost`;
FLUSH PRIVILEGES;
