CREATE TABLE "Languages" (
                             "LanguageId" SERIAL PRIMARY KEY,
                             "Name" VARCHAR(100) NOT NULL,
                             "Code" VARCHAR(10) NOT NULL
);

CREATE TABLE "Levels" (
                          "LevelId" SERIAL PRIMARY KEY,
                          "Name" VARCHAR(100) NOT NULL,
                          "Description" VARCHAR(500),
                          "LanguageId" INTEGER REFERENCES "Languages"("LanguageId"),
                          "BaseCost" NUMERIC(18, 2),
                          "DurationMonths" INTEGER DEFAULT 3
);

CREATE TABLE "Classrooms" (
                              "ClassroomId" SERIAL PRIMARY KEY,
                              "RoomNumber" VARCHAR(20) NOT NULL,
                              "Capacity" INTEGER DEFAULT 20,
                              "Equipment" VARCHAR(500)
);

CREATE TABLE "Teachers" (
                            "TeacherId" SERIAL PRIMARY KEY,
                            "FirstName" VARCHAR(100) NOT NULL,
                            "LastName" VARCHAR(100) NOT NULL,
                            "Email" VARCHAR(255),
                            "Phone" VARCHAR(20),
                            "HireDate" DATE,
                            "PasswordHash" VARCHAR(255),
                            "LastLoginAt" TIMESTAMP DEFAULT NOW(),
                            "IsActive" BOOLEAN DEFAULT TRUE
);
CREATE TABLE "TeacherLanguages" (
                                    "TeacherLanguageId" SERIAL PRIMARY KEY,
                                    "TeacherId" INTEGER NOT NULL REFERENCES "Teachers"("TeacherId") ON DELETE CASCADE,
                                    "LanguageId" INTEGER NOT NULL REFERENCES "Languages"("LanguageId") ON DELETE CASCADE
);
CREATE TABLE "Groups" (
                          "GroupId" SERIAL PRIMARY KEY,
                          "GroupName" VARCHAR(100) NOT NULL,
                          "LevelId" INTEGER REFERENCES "Levels"("LevelId"),
                          "TeacherId" INTEGER REFERENCES "Teachers"("TeacherId"),
                          "StartDate" DATE,
                          "EndDate" DATE,
                          "MaxStudents" INTEGER DEFAULT 20,
                          "LanguageId" INTEGER DEFAULT 1 REFERENCES "Languages"("LanguageId"),
                          "LevelName" VARCHAR(100) DEFAULT 'Beginner'
);
CREATE TABLE "Students" (
                            "StudentId" SERIAL PRIMARY KEY,
                            "FirstName" VARCHAR(100) NOT NULL,
                            "LastName" VARCHAR(100) NOT NULL,
                            "DateOfBirth" DATE,
                            "Phone" VARCHAR(20),
                            "Email" VARCHAR(255),
                            "RegistrationDate" TIMESTAMP DEFAULT NOW(),
                            "HasDiscount" BOOLEAN DEFAULT FALSE,
                            "DiscountPercentage" NUMERIC(5, 2) DEFAULT 0,
                            "CreatedAt" TIMESTAMP DEFAULT NOW(),
                            "GroupId" INTEGER REFERENCES "Groups"("GroupId"),
                            "PaymentStatus" VARCHAR(30),
                            "PasswordHash" VARCHAR(255),
                            "LastLoginAt" TIMESTAMP DEFAULT NOW(),
                            "IsActive" BOOLEAN DEFAULT TRUE
);
CREATE TABLE "Exams" (
                         "ExamId" SERIAL PRIMARY KEY,
                         "ExamDate" DATE,
                         "Description" VARCHAR(255),
                         "Level" VARCHAR(50) DEFAULT 'Beginner (A1)'
);

CREATE TABLE "ExamResults" (
                               "ExamResultId" SERIAL PRIMARY KEY,
                               "StudentId" INTEGER REFERENCES "Students"("StudentId") ON DELETE CASCADE,
                               "ExamId" INTEGER REFERENCES "Exams"("ExamId") ON DELETE CASCADE,
                               "Grade" INTEGER,
                               "ExamDate" DATE
);

CREATE TABLE "Schedules" (
                             "ScheduleId" SERIAL PRIMARY KEY,
                             "GroupId" INTEGER REFERENCES "Groups"("GroupId") ON DELETE CASCADE,
                             "ClassroomId" INTEGER REFERENCES "Classrooms"("ClassroomId"),
                             "DayOfWeek" VARCHAR(20),
                             "StartTime" TIME,
                             "EndTime" TIME,
                             "TeacherId" INTEGER REFERENCES "Teachers"("TeacherId"),
                             "Date" DATE DEFAULT CURRENT_DATE,
                             "Room" VARCHAR(50)
);

CREATE TABLE "Enrollments" (
                               "EnrollmentId" SERIAL PRIMARY KEY,
                               "StudentId" INTEGER REFERENCES "Students"("StudentId") ON DELETE CASCADE,
                               "GroupId" INTEGER REFERENCES "Groups"("GroupId") ON DELETE CASCADE,
                               "EnrollmentDate" TIMESTAMP DEFAULT NOW(),
                               "CompletionDate" TIMESTAMP,
                               "Cost" NUMERIC(18, 2),
                               "IsCompleted" BOOLEAN DEFAULT FALSE
);

CREATE TABLE "Payments" (
                            "PaymentId" SERIAL PRIMARY KEY,
                            "StudentId" INTEGER REFERENCES "Students"("StudentId"),
                            "Amount" NUMERIC(18, 2),
                            "PaymentDate" TIMESTAMP DEFAULT NOW(),
                            "Description" VARCHAR(500)
);

CREATE TABLE "PaymentDeferrals" (
                                    "PaymentDeferralId" SERIAL PRIMARY KEY,
                                    "StudentId" INTEGER REFERENCES "Students"("StudentId") ON DELETE CASCADE,
                                    "DeferredAmount" NUMERIC(18, 2),
                                    "DeferralDate" TIMESTAMP DEFAULT NOW(),
                                    "DueDate" DATE,
                                    "Reason" VARCHAR(500),
                                    "IsPaid" BOOLEAN DEFAULT FALSE
);

CREATE INDEX "IX_Groups_TeacherId" ON "Groups" ("TeacherId");
CREATE INDEX "IX_Groups_LevelId" ON "Groups" ("LevelId");
CREATE INDEX "IX_Students_GroupId" ON "Students" ("GroupId");
CREATE INDEX "IX_Schedules_GroupId" ON "Schedules" ("GroupId");
CREATE INDEX "IX_Schedules_TeacherId" ON "Schedules" ("TeacherId");
CREATE INDEX "IX_ExamResults_StudentId" ON "ExamResults" ("StudentId");
CREATE INDEX "IX_ExamResults_ExamId" ON "ExamResults" ("ExamId");
