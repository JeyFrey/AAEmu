-- -------------------------------------------------
-- Add placing and demolish timers to housing table
-- -------------------------------------------------
ALTER TABLE `housings` 
ADD COLUMN `place_date` DATETIME NOT NULL DEFAULT NOW() AFTER `permission`,
ADD COLUMN `protected_until` DATETIME NOT NULL DEFAULT NOW() AFTER `place_date`;
ADD COLUMN `faction_id` INT UNSIGNED NOT NULL DEFAULT 1 AFTER `protected_until`;

ALTER TABLE `mails`
CHANGE COLUMN `extra` `extra` BIGINT(20) NOT NULL ;
