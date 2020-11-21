-- -------------------------------------------------
-- Add placing and demolish timers to housing table
-- -------------------------------------------------
ALTER TABLE `housings` 
ADD COLUMN `place_date` DATETIME NOT NULL DEFAULT NOW() AFTER `permission`,
ADD COLUMN `protected_until` DATETIME NOT NULL DEFAULT NOW() AFTER `place_date`;
