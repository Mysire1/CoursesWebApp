-- Migration: Remove Users table and add authentication fields to Students and Teachers
-- Run this script AFTER deploying the new code

-- 1. Add new fields to Students table
ALTER TABLE "Students"
    ADD COLUMN IF NOT EXISTS "PasswordHash" VARCHAR(255) DEFAULT '',
    ADD COLUMN IF NOT EXISTS "LastLoginAt" TIMESTAMP,
    ADD COLUMN IF NOT EXISTS "IsActive" BOOLEAN DEFAULT true;

-- 2. Make Email required in Students table
ALTER TABLE "Students"
    ALTER COLUMN "Email" SET NOT NULL,
    ALTER COLUMN "Email" SET DEFAULT '';

-- 3. Add unique index on Students Email
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Students_Email" ON "Students"("Email");

-- 4. Add new fields to Teachers table
ALTER TABLE "Teachers"
    ADD COLUMN IF NOT EXISTS "PasswordHash" VARCHAR(255) DEFAULT '',
    ADD COLUMN IF NOT EXISTS "LastLoginAt" TIMESTAMP,
    ADD COLUMN IF NOT EXISTS "IsActive" BOOLEAN DEFAULT true;

-- 5. Make Email required in Teachers table
ALTER TABLE "Teachers"
    ALTER COLUMN "Email" SET NOT NULL,
    ALTER COLUMN "Email" SET DEFAULT '';

-- 6. Add unique index on Teachers Email
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Teachers_Email" ON "Teachers"("Email");

-- 7. Migrate data from Users to Students (if Users table exists)
DO $$
BEGIN
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'Users') THEN
        -- Update existing students with User credentials
        UPDATE "Students" s
        SET 
            "Email" = COALESCE(u."Email", s."Email", ''),
            "PasswordHash" = u."PasswordHash",
            "LastLoginAt" = u."LastLoginAt",
            "IsActive" = u."IsActive"
        FROM "Users" u
        WHERE u."StudentId" = s."StudentId" AND u."Role" = 'Student';
        
        -- Update existing teachers with User credentials
        UPDATE "Teachers" t
        SET 
            "Email" = COALESCE(u."Email", t."Email", ''),
            "PasswordHash" = u."PasswordHash",
            "LastLoginAt" = u."LastLoginAt",
            "IsActive" = u."IsActive"
        FROM "Users" u
        WHERE u."TeacherId" = t."TeacherId" AND u."Role" = 'Teacher';
    END IF;
END $$;

-- 8. Drop Users table (if exists)
DROP TABLE IF EXISTS "Users";

-- 9. Verification queries (optional - comment out if not needed)
-- SELECT 'Students with authentication' as info, COUNT(*) as count FROM "Students" WHERE "PasswordHash" != '';
-- SELECT 'Teachers with authentication' as info, COUNT(*) as count FROM "Teachers" WHERE "PasswordHash" != '';
-- SELECT 'Students without email' as info, COUNT(*) as count FROM "Students" WHERE "Email" IS NULL OR "Email" = '';
-- SELECT 'Teachers without email' as info, COUNT(*) as count FROM "Teachers" WHERE "Email" IS NULL OR "Email" = '';

SELECT 'Migration completed successfully' as status;