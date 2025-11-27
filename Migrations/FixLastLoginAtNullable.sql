UPDATE "Students" 
SET "LastLoginAt" = NULL 
WHERE "LastLoginAt" IS NOT NULL AND "LastLoginAt" < '2000-01-01';
  
UPDATE "Teachers" 
SET "LastLoginAt" = NULL 
WHERE "LastLoginAt" IS NOT NULL AND "LastLoginAt" < '2000-01-01';

ALTER TABLE "Students" ALTER COLUMN "LastLoginAt" DROP NOT NULL;
ALTER TABLE "Teachers" ALTER COLUMN "LastLoginAt" DROP NOT NULL;

SELECT 'LastLoginAt migration completed successfully' as status;