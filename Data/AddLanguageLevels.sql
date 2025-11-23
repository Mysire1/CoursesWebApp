-- SQL script to add language levels for all languages
-- This script assumes the following languages exist:
-- English (LanguageId = 1)
-- German (LanguageId = 2)
-- French (LanguageId = 3)
-- Spanish (LanguageId = 4)
-- Italian (LanguageId = 5)

-- Check if any of these levels already exist and delete them to avoid duplicates
DELETE FROM Levels WHERE Name IN (
    'Beginner (A1)', 'Elementary (A2)', 'Intermediate (B1)', 'Upper-Intermediate (B2)', 'Advanced (C1)',
    'Anfänger (A1)', 'Grundstufe (A2)', 'Mittelstufe (B1)',
    'Débutant (A1)', 'Élémentaire (A2)',
    'Principiante (A1)', 'Elemental (A2)'
);

-- English Levels
INSERT INTO Levels (Name, Description, LanguageId, BaseCost, DurationMonths) VALUES 
('Beginner (A1)', 'Basic English level', 1, 1000.00, 3),
('Elementary (A2)', 'Elementary English level', 1, 1200.00, 3),
('Intermediate (B1)', 'Intermediate English level', 1, 1400.00, 3),
('Upper-Intermediate (B2)', 'Upper-Intermediate English level', 1, 1600.00, 3),
('Advanced (C1)', 'Advanced English level', 1, 1800.00, 3);

-- German Levels
INSERT INTO Levels (Name, Description, LanguageId, BaseCost, DurationMonths) VALUES 
('Anfänger (A1)', 'Basic German level', 2, 1100.00, 3),
('Grundstufe (A2)', 'Elementary German level', 2, 1300.00, 3),
('Mittelstufe (B1)', 'Intermediate German level', 2, 1500.00, 3);

-- French Levels
INSERT INTO Levels (Name, Description, LanguageId, BaseCost, DurationMonths) VALUES 
('Débutant (A1)', 'Basic French level', 3, 1050.00, 3),
('Élémentaire (A2)', 'Elementary French level', 3, 1250.00, 3);

-- Spanish Levels
INSERT INTO Levels (Name, Description, LanguageId, BaseCost, DurationMonths) VALUES 
('Principiante (A1)', 'Basic Spanish level', 4, 1000.00, 3),
('Elemental (A2)', 'Elementary Spanish level', 4, 1200.00, 3);

-- Italian Levels
INSERT INTO Levels (Name, Description, LanguageId, BaseCost, DurationMonths) VALUES 
('Principiante (A1)', 'Basic Italian level', 5, 1000.00, 3);

SELECT 'Language levels added successfully!' AS Result;