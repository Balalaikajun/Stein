-- 0) Delete
TRUNCATE TABLE
    "AcademicPerformances",
    "Students",
    "Groups",
    "Teachers",
    "Specializations",
    "Departments"
    RESTART IDENTITY CASCADE;


-- 1) Departments
WITH dept_data ("Title","IsActive") AS (
    VALUES
        ('Department of Mathematics',              true),
        ('Department of Computer Science',         true),
        ('Department of Physics',                  true),
        ('Department of Chemistry',                true),
        ('Department of Biology',                  true),
        ('Department of History',                  true),
        ('Department of Literature',               true),
        ('Department of Economics',                true),
        ('Department of Management',               true),
        ('Department of Psychology',               true),
        ('Department of Sociology',                true),
        ('Department of Law',                      true),
        ('Department of Medicine',                 true),
        ('Department of Philosophy',               true),
        ('Department of Arts',                     true),
        ('Department of Engineering',              true),
        ('Department of Architecture',             true),
        ('Department of Linguistics',              true),
        ('Department of Geography',                true),
        ('Department of Environmental Science',    true),
        ('Department of Astronomy',                true),
        ('Department of Geology',                  true),
        ('Department of Anthropology',             true),
        ('Department of Political Science',        true),
        ('Department of Education',                true),
        ('Department of Music',                    true),
        ('Department of Theatre',                  true),
        ('Department of Journalism',               true),
        ('Department of Theology',                 true),
        ('Department of Computer Engineering',     false)
)
INSERT INTO "Departments"("Title","IsActive")
SELECT * FROM dept_data;


-- 2) Specializations
WITH spec_data ("Code","Title","Acronym","DepartmentId","IsActive") AS (
    VALUES
        ('APMATH',   'Applied Mathematics',       'AM',   1,  true),
        ('CS',       'Computer Science',          'CS',   2,  true),
        ('PHY',      'Physics',                   'PH',   3,  true),
        ('CHEM',     'Chemistry',                 'CH',   4,  true),
        ('BIOL',     'Biology',                   'BI',   5,  true),
        ('HIST',     'History',                   'HI',   6,  true),
        ('LIT',      'Literature',                'LT',   7,  true),
        ('ECON',     'Economics',                 'EC',   8,  true),
        ('MGMT',     'Management',                'MG',   9,  true),
        ('PSY',      'Psychology',                'PS',  10,  true),
        ('SOC',      'Sociology',                 'SO',  11,  true),
        ('LAW',      'Law',                       'LA',  12,  true),
        ('MED',      'Medicine',                  'MD',  13,  true),
        ('PHIL',     'Philosophy',                'PI',  14,  true),
        ('ART',      'Arts',                      'AR',  15,  true),
        ('ENG',      'Engineering',               'EN',  16,  true),
        ('ARCH',     'Architecture',              'AH',  17,  true),
        ('LING',     'Linguistics',               'LI',  18,  true),
        ('GEO',      'Geography',                 'GR',  19,  true),
        ('ENV',      'Environmental Science',     'EV',  20,  true),
        ('ASTR',     'Astronomy',                 'AS',  21,  true),
        ('GEOLOGY',  'Geology',                   'GL',  22,  true),
        ('ANTH',     'Anthropology',              'AN',  23,  true),
        ('POLSCI',   'Political Science',         'PSCI',24,  true),
        ('EDU',      'Education',                 'ED',  25,  true),
        ('MUS',      'Music',                     'MU',  26,  true),
        ('THEA',     'Theatre',                   'TH',  27,  true),
        ('JOUR',     'Journalism',                'JO',  28,  true),
        ('THEO',     'Theology',                  'TO',  29,  true),
        ('CENG',     'Computer Engineering',      'CE',  30,  false)
)
INSERT INTO "Specializations"("Code","Title","Acronym","DepartmentId","IsActive")
SELECT * FROM spec_data;


-- 3) Teachers
WITH teacher_data ("Surname","Name","Patronymic","IsActive") AS (
    VALUES
        ('Ivanov',    'Ivan',      'Ivanovich',    true),
        ('Petrov',    'Petr',      'Ivanovich',    true),
        ('Sidorov',   'Dmitry',    'Alexandrovich',true),
        ('Smirnov',   'Alexey',    'Alexeevich',   true),
        ('Kuznetsov', 'Dmitry',    'Dmitrievich',  true),
        ('Popov',     'Mikhail',   'Mikhailovich', true),
        ('Sokolov',   'Andrey',    'Andreevich',   true),
        ('Lebedev',   'Alexander', 'Sergeevich',   true),
        ('Kozlov',    'Maxim',     'Dmitrievich',  true),
        ('Novikov',   'Vladimir',  'Vladimirovich',true),
        ('Morozov',   'Nikolay',   'Nikolayevich', true),
        ('Volkov',    'Igor',      'Igorevich',    true),
        ('Semenov',   'Konstantin','Konstantinovich',true),
        ('Egorov',    'Roman',     'Romanovich',   true),
        ('Pavlov',    'Ilya',      'Ilyich',       true),
        ('Krylova',   'Olga',      'Olegovna',     true),
        ('Nikolaev',  'Natalia',   'Nikolaevna',   true),
        ('Antonov',   'Marina',    'Antonovna',    true),
        ('Fedorov',   'Elena',     'Fedorovna',    true),
        ('Mikhailov', 'Tatiana',   'Mikhailovna',  true),
        ('Davydov',   'Svetlana',  'Davydovna',    true),
        ('Borisov',   'Victoria',  'Borisovna',    true),
        ('Filippov',  'Anastasia', 'Filippovna',   true),
        ('Vasiliev',  'Ekaterina','Vasilievna',   true),
        ('Markov',    'Kristina',  'Markovna',     true),
        ('Yakovlev',  'Irina',     'Yakovlevna',   false),
        ('Grigorev',  'Larisa',    'Grigorevna',   true),
        ('Nikifanov', 'Olga',      'Nikifanovna',  true),
        ('Zakharov',  'Andrey',    'Vladimirovich',true)
)
INSERT INTO "Teachers"("Surname","Name","Patronymic","IsActive")
SELECT * FROM teacher_data;


-- 4) Groups
WITH group_data (
                 "SpecializationId","Year","Id","Acronym","TeacherId","IsActive"
    ) AS (
    VALUES
        ( 1, 2023,'001','G001',  1, true),
        ( 2, 2023,'002','G002',  2, true),
        ( 3, 2023,'003','G003',  3, true),
        ( 4, 2023,'004','G004',  4, true),
        ( 5, 2023,'005','G005',  5, true),
        ( 6, 2022,'006','G006',  6, true),
        ( 7, 2022,'007','G007',  7, true),
        ( 8, 2022,'008','G008',  8, true),
        ( 9, 2022,'009','G009',  9, true),
        (10, 2022,'010','G010', 10, true),
        (11, 2021,'011','G011', 11, true),
        (12, 2021,'012','G012', 12, true),
        (13, 2021,'013','G013', 13, true),
        (14, 2021,'014','G014', 14, true),
        (15, 2021,'015','G015', 15, true),
        (16, 2023,'016','G016', 16, true),
        (17, 2023,'017','G017', 17, true),
        (18, 2022,'018','G018', 18, true),
        (19, 2022,'019','G019', 19, true),
        (20, 2021,'020','G020', 20, true),
        (21, 2023,'021','G021', 21, true),
        (22, 2023,'022','G022', 22, true),
        (23, 2022,'023','G023', 23, true),
        (24, 2022,'024','G024', 24, true),
        (25, 2021,'025','G025', 25, true),
        (26, 2021,'026','G026', 26, true),
        (27, 2023,'027','G027', 27, true),
        (28, 2022,'028','G028', 28, false),
        (29, 2021,'029','G029', 29, true),
        (30, 2023,'030','G030', 29, true)
)
INSERT INTO "Groups"(
    "SpecializationId","Year","Id","Acronym","TeacherId","IsActive"
)
SELECT * FROM group_data;


-- 5) Students
WITH student_data (
                   "Surname","Name","Patronymic",
                   "GroupSpecializationId","GroupYear","GroupId",
                   "IsCitizen","Gender","DateOfBirth"
    ) AS (
    VALUES
        ('Ivanov',    'Ivan',     'Ivanovich',     1,2023,'001', true, 0, TO_DATE('2001-01-15', 'YYYY-MM-DD')),
        ('Petrova',   'Anna',     'Sergeevna',     2,2023,'002', false,1, TO_DATE('2002-03-22', 'YYYY-MM-DD')),
        ('Sidorov',   'Dmitry',   'Alexandrovich', 3,2023,'003', true, 0, TO_DATE('2000-07-08', 'YYYY-MM-DD')),
        ('Smirnova',  'Elena',    'Viktorovna',    4,2023,'004', true, 1, TO_DATE('2001-11-30', 'YYYY-MM-DD')),
        ('Kuznetsov', 'Alexey',   'Petrovich',     5,2023,'005', true, 0, TO_DATE('2002-02-14', 'YYYY-MM-DD')),
        ('Popova',    'Maria',    'Ivanovna',      6,2022,'006', false,1, TO_DATE('2000-12-01', 'YYYY-MM-DD')),
        ('Sokolov',   'Sergey',   'Nikolaevich',   7,2022,'007', true, 0, TO_DATE('2001-06-19', 'YYYY-MM-DD')),
        ('Lebedeva',  'Olga',     'Sergeevna',     8,2022,'008', true, 1, TO_DATE('2002-09-05', 'YYYY-MM-DD')),
        ('Kozlov',    'Maxim',    'Dmitrievich',   9,2022,'009', true, 0, TO_DATE('2001-04-27', 'YYYY-MM-DD')),
        ('Novikova',  'Natalia',  'Yurievna',     10,2022,'010', true, 1, TO_DATE('2000-08-16', 'YYYY-MM-DD')),
        ('Morozova',  'Tatiana',  'Vladimirovna', 11,2021,'011', true, 1, TO_DATE('2001-02-02', 'YYYY-MM-DD')),
        ('Vasiliev',  'Ivan',     'Petrovich',    12,2021,'012', true, 0, TO_DATE('2000-05-21', 'YYYY-MM-DD')),
        ('Egorova',   'Svetlana','Mikhailovna',  13,2021,'013', true, 1, TO_DATE('2001-10-29', 'YYYY-MM-DD')),
        ('Pavlov',    'Igor',     'Konstantinovich',14,2021,'014', true, 0, TO_DATE('2002-01-11', 'YYYY-MM-DD')),
        ('Fedorova',  'Anastasia','Alexandrovna', 15,2021,'015', true, 1, TO_DATE('2000-03-03', 'YYYY-MM-DD')),
        ('Mikhailov', 'Oleg',     'Dmitrievich',  16,2023,'016', true, 0, TO_DATE('2001-12-24', 'YYYY-MM-DD')),
        ('Borisova',  'Yulia',    'Sergeevna',    17,2023,'017', true, 1, TO_DATE('2000-07-07', 'YYYY-MM-DD')),
        ('Antonov',   'Konstantin','Ivanovich',   18,2022,'018', true, 0, TO_DATE('2002-10-10', 'YYYY-MM-DD')),
        ('Nikiforova','Ekaterina','Vladimirovna', 19,2022,'019', true, 1, TO_DATE('2001-09-09', 'YYYY-MM-DD')),
        ('Yakovlev',  'Alexander','Petrovich',    20,2021,'020', false,0, TO_DATE('2000-11-11', 'YYYY-MM-DD')),
        ('Grigoreva', 'Marina',   'Sergeevna',    21,2023,'021', true, 1, TO_DATE('2002-06-06', 'YYYY-MM-DD')),
        ('Markov',    'Denis',    'Nikolaevich',  22,2023,'022', true, 0, TO_DATE('2001-08-08', 'YYYY-MM-DD')),
        ('Filippova', 'Olga',     'Ivanovna',     23,2022,'023', true, 1, TO_DATE('2000-04-04', 'YYYY-MM-DD')),
        ('Davydov',   'Roman',    'Sergeevich',   24,2022,'024', true, 0, TO_DATE('2002-05-05', 'YYYY-MM-DD')),
        ('Nikolaeva', 'Irina',    'Alexeevna',    25,2021,'025', true, 1, TO_DATE('2001-03-03', 'YYYY-MM-DD')),
        ('Morozov',   'Viktor',   'Petrovich',    27,2023,'027', true, 0, TO_DATE('2001-12-30', 'YYYY-MM-DD')),
        ('Vinogradova','Anna',    'Nikolayevna',  28,2022,'028', false,1, TO_DATE('2002-07-07', 'YYYY-MM-DD')),
        ('Nikitin',   'Kirill',   'Olegovich',    29,2021,'029', true, 0, TO_DATE('2000-10-10', 'YYYY-MM-DD'))
)
INSERT INTO "Students"(
    "Surname","Name","Patronymic",
    "GroupSpecializationId","GroupYear","GroupId",
    "IsCitizen","Gender","DateOfBirth","Status"
)
SELECT *,
       CASE
           WHEN random() < 0.8 THEN 1  -- 80% Active
           WHEN random() < 0.9 THEN 2  -- 10% Expelled
           WHEN random() < 0.95 THEN 3 -- 5% Vacation
           ELSE 4                      -- 5% Released
           END
FROM student_data;


-- 6) AcademicPerformances
INSERT INTO "AcademicPerformances"("Year","Month","StudentId","Performance")
SELECT
    2023,
    ((g - 1) % 6) + 1, -- месяц от 1 до 6
    g,                 -- StudentId 1–30
    CASE
        WHEN score >= 90 THEN 1  -- Excellent
        WHEN score >= 80 THEN 2  -- Good
        WHEN score >= 70 THEN 3  -- Normal
        ELSE 4                   -- Falling
        END
FROM generate_series(1,28) AS g,
     LATERAL (SELECT (65 + floor(random() * 36))::int AS score) AS s;

-- 7) Orders (TPH)
WITH orders_data (
                  "OrderType","OrderNumber","StudentID","Date","Discriminator",
                  "ToSpecializationId","ToYear","ToGroupId",
                  "FromSpecializationId","FromYear","FromGroupId"
    ) AS (
    VALUES
        -- EnrollmentOrder
        (1, 'ENR-1001',  1, DATE '2023-09-01', 'EnrollmentOrder',  1, 2023, '001', NULL,  NULL,  NULL),
        (1, 'ENR-1002',  2, DATE '2023-09-02', 'EnrollmentOrder',  2, 2023, '002', NULL,  NULL,  NULL),
        (1, 'ENR-1003',  3, DATE '2023-09-03', 'EnrollmentOrder',  3, 2023, '003', NULL,  NULL,  NULL),

        -- TransferOrder
        (2, 'TRF-2001',  4, DATE '2024-02-15', 'TransferOrder',     5, 2023, '005', 4, 2023, '004'),
        (2, 'TRF-2002',  5, DATE '2024-03-20', 'TransferOrder',     1, 2023, '001', 5, 2023, '005'),
        (2, 'TRF-2003',  6, DATE '2024-04-10', 'TransferOrder',     7, 2022, '007', 6, 2022, '006'),

        -- ExpulsionOrder
        (3, 'EXP-3001',  7, DATE '2024-06-30', 'ExpulsionOrder',    NULL, NULL, NULL, 8, 2022, '008'),
        (3, 'EXP-3002',  8, DATE '2024-07-01', 'ExpulsionOrder',    NULL, NULL, NULL, 9, 2022, '009'),
        (3, 'EXP-3003',  9, DATE '2024-08-15', 'ExpulsionOrder',    NULL, NULL, NULL,10, 2022, '010')
)
INSERT INTO "Orders"(
    "OrderType","OrderNumber","StudentID","Date","Discriminator",
    "ToSpecializationId","ToYear","ToGroupId",
    "FromSpecializationId","FromYear","FromGroupId"
)
SELECT * FROM orders_data;