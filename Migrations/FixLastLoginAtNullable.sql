-- Migration: Fix LastLoginAt nullable issue
-- This script updates existing records with NULL LastLoginAt values

-- Update Students table
UPDATE "Students" 
SET "LastLoginAt" = NULL 
WHERE "LastLoginAt" IS NOT NULL AND "LastLoginAt" < '2000-01-01';

-- Update Teachers table  
UPDATE "Teachers" 
SET "LastLoginAt" = NULL 
WHERE "LastLoginAt" IS NOT NULL AND "LastLoginAt" < '2000-01-01';

-- Ensure column allows NULL
ALTER TABLE "Students" ALTER COLUMN "LastLoginAt" DROP NOT NULL;
ALTER TABLE "Teachers" ALTER COLUMN "LastLoginAt" DROP NOT NULL;

SELECT 'LastLoginAt migration completed successfully' as status;