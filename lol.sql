-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server Version:               10.1.28-MariaDB - mariadb.org binary distribution
-- Server Betriebssystem:        Win32
-- HeidiSQL Version:             9.4.0.5125
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Exportiere Struktur von Tabelle lol.accounts
CREATE TABLE IF NOT EXISTS `accounts` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `password` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `summonerId` int(11) DEFAULT NULL,
  `RP` int(11) DEFAULT '55555',
  `IP` int(11) DEFAULT '55555',
  `isBanned` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Exportiere Daten aus Tabelle lol.accounts: ~0 rows (ungefähr)
DELETE FROM `accounts`;
/*!40000 ALTER TABLE `accounts` DISABLE KEYS */;
INSERT INTO `accounts` (`id`, `username`, `password`, `summonerId`, `RP`, `IP`, `isBanned`) VALUES
	(1, 'test', 'test', 1, 99999, 99999, 0);
/*!40000 ALTER TABLE `accounts` ENABLE KEYS */;

-- Exportiere Struktur von Tabelle lol.summoner
CREATE TABLE IF NOT EXISTS `summoner` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `summonerName` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '0',
  `icon` int(55) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Exportiere Daten aus Tabelle lol.summoner: ~0 rows (ungefähr)
DELETE FROM `summoner`;
/*!40000 ALTER TABLE `summoner` DISABLE KEYS */;
INSERT INTO `summoner` (`id`, `summonerName`, `icon`) VALUES
	(1, 'Maufeat', 55);
/*!40000 ALTER TABLE `summoner` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
