-- Bookstore schema + 2000 rows per table (plain SQL, MariaDB/MySQL, no extra tools).
-- Run in phpMyAdmin (Import / SQL tab) or: mysql -uroot -proot < db-bookstore.sql

CREATE DATABASE IF NOT EXISTS `t120b197` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `t120b197`;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS `OrderItem`, `Orders`, `Customer`, `Book`, `Author`, `Example`;
SET FOREIGN_KEY_CHECKS = 1;

-- ---------------------------------------------------------------- schema
CREATE TABLE `Author` (
  `id`         INT NOT NULL AUTO_INCREMENT,
  `name`       VARCHAR(100) NOT NULL,
  `country`    VARCHAR(50)  NOT NULL,
  `birth_year` INT NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Book` (
  `id`           INT NOT NULL AUTO_INCREMENT,
  `title`        VARCHAR(150) NOT NULL,
  `isbn`         CHAR(13) NOT NULL,
  `genre`        VARCHAR(30) NOT NULL,
  `price`        DECIMAL(6,2) NOT NULL,
  `pages`        INT NOT NULL,
  `publish_year` INT NOT NULL,
  `author_id`    INT NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_book_isbn` (`isbn`),
  KEY `ix_book_author` (`author_id`),
  CONSTRAINT `fk_book_author` FOREIGN KEY (`author_id`) REFERENCES `Author` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `Customer` (
  `id`            INT NOT NULL AUTO_INCREMENT,
  `name`          VARCHAR(100) NOT NULL,
  `email`         VARCHAR(120) NOT NULL,
  `city`          VARCHAR(50)  NOT NULL,
  `registered_at` DATE NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_customer_email` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- "Order" is a reserved word, so the table is called Orders
CREATE TABLE `Orders` (
  `id`          INT NOT NULL AUTO_INCREMENT,
  `customer_id` INT NOT NULL,
  `order_date`  DATE NOT NULL,
  `status`      VARCHAR(20) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_orders_customer` (`customer_id`),
  CONSTRAINT `fk_orders_customer` FOREIGN KEY (`customer_id`) REFERENCES `Customer` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `OrderItem` (
  `id`         INT NOT NULL AUTO_INCREMENT,
  `order_id`   INT NOT NULL,
  `book_id`    INT NOT NULL,
  `quantity`   INT NOT NULL,
  `unit_price` DECIMAL(6,2) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_item_order` (`order_id`),
  KEY `ix_item_book` (`book_id`),
  CONSTRAINT `fk_item_order` FOREIGN KEY (`order_id`) REFERENCES `Orders` (`id`),
  CONSTRAINT `fk_item_book`  FOREIGN KEY (`book_id`)  REFERENCES `Book` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------- data
-- helper: numbers 1..2000 (portable, no recursive CTE needed)
CREATE TEMPORARY TABLE `nums` (`n` INT PRIMARY KEY);
INSERT INTO `nums`
SELECT n FROM (
  SELECT a.d + 10*b.d + 100*c.d + 1000*e.d + 1 AS n
  FROM (SELECT 0 d UNION SELECT 1 UNION SELECT 2 UNION SELECT 3 UNION SELECT 4
        UNION SELECT 5 UNION SELECT 6 UNION SELECT 7 UNION SELECT 8 UNION SELECT 9) a,
       (SELECT 0 d UNION SELECT 1 UNION SELECT 2 UNION SELECT 3 UNION SELECT 4
        UNION SELECT 5 UNION SELECT 6 UNION SELECT 7 UNION SELECT 8 UNION SELECT 9) b,
       (SELECT 0 d UNION SELECT 1 UNION SELECT 2 UNION SELECT 3 UNION SELECT 4
        UNION SELECT 5 UNION SELECT 6 UNION SELECT 7 UNION SELECT 8 UNION SELECT 9) c,
       (SELECT 0 d UNION SELECT 1) e
) t WHERE n <= 2000;

INSERT INTO `Author` (`name`, `country`, `birth_year`)
SELECT
  CONCAT(ELT(1 + n % 20, 'Jonas','Petras','Antanas','Mindaugas','Tomas','Anna','Maria','John','Emma','Oliver',
                         'Sophie','Liam','Noah','Ava','Lukas','Ieva','Greta','Karolis','Rasa','Vytautas'),
         ' ',
         ELT(1 + FLOOR(n / 20) % 25, 'Kazlauskas','Petrauskas','Jankauskas','Butkus','Smith','Johnson','Brown','Miller',
                         'Garcia','Muller','Novak','Kowalski','Ivanov','Berg','Larsen','Rossi','Dubois','Silva',
                         'Tamm','Ozols','Virtanen','Nowak','Horvat','Popescu','Kovacs')),
  ELT(1 + n % 8, 'Lithuania','Latvia','Estonia','Poland','Germany','France','UK','USA'),
  1920 + FLOOR(RAND() * 80)
FROM `nums` ORDER BY n;

INSERT INTO `Book` (`title`, `isbn`, `genre`, `price`, `pages`, `publish_year`, `author_id`)
SELECT
  CONCAT(ELT(1 + n % 10, 'The Last','A Quiet','Return of the','Secrets of the','Beyond the',
                         'The Hidden','Songs of the','Echoes of the','The Long','Under the'),
         ' ',
         ELT(1 + FLOOR(n / 10) % 12, 'River','Forest','Empire','Garden','Storm','Mirror','Island','Harbor','Winter','Machine','Kingdom','Night'),
         ' #', n),
  LPAD(9780000000000 + n, 13, '0'),
  ELT(1 + n % 8, 'Fantasy','Sci-Fi','Mystery','Romance','History','Biography','Science','Poetry'),
  ROUND(5 + RAND() * 45, 2),
  80 + FLOOR(RAND() * 820),
  1950 + FLOOR(RAND() * 75),
  1 + FLOOR(RAND() * 2000)
FROM `nums` ORDER BY n;

INSERT INTO `Customer` (`name`, `email`, `city`, `registered_at`)
SELECT
  CONCAT(ELT(1 + n % 16, 'Jonas','Ieva','Tomas','Greta','Lukas','Rasa','Mantas','Egle','Darius','Lina',
                         'Paulius','Monika','Andrius','Jurga','Simas','Aiste'),
         ' ',
         ELT(1 + FLOOR(n / 16) % 15, 'Kazlauskas','Petrauskas','Jankauskas','Butkus','Navickas','Paulauskas','Stankevicius',
                         'Vasiliauskas','Zukauskas','Balciunas','Rimkus','Urbonas','Sadauskas','Mockus','Grigas')),
  CONCAT('customer', n, '@example.com'),
  ELT(1 + n % 8, 'Vilnius','Kaunas','Klaipeda','Siauliai','Panevezys','Alytus','Marijampole','Utena'),
  DATE_ADD('2020-01-01', INTERVAL FLOOR(RAND() * 1800) DAY)
FROM `nums` ORDER BY n;

INSERT INTO `Orders` (`customer_id`, `order_date`, `status`)
SELECT
  1 + FLOOR(RAND() * 2000),
  DATE_ADD('2022-01-01', INTERVAL FLOOR(RAND() * 1400) DAY),
  ELT(1 + FLOOR(RAND() * 4), 'NEW','PAID','SHIPPED','CANCELLED')
FROM `nums` ORDER BY n;

-- first 2000 rows cover every order exactly once -> no empty orders;
-- books are random, so some books are never ordered (good for NOT EXISTS queries)
INSERT INTO `OrderItem` (`order_id`, `book_id`, `quantity`, `unit_price`)
SELECT n, 1 + FLOOR(RAND() * 2000), 1 + FLOOR(RAND() * 5), 0
FROM `nums` ORDER BY n;

-- unit_price = current book price (keeps data consistent)
UPDATE `OrderItem` oi JOIN `Book` b ON b.id = oi.book_id SET oi.unit_price = b.price;

DROP TEMPORARY TABLE `nums`;
