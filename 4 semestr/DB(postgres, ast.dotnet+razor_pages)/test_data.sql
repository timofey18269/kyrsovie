-- =========================
-- ТЕСТОВЫЕ ДАННЫЕ
-- =========================

-- Очистка таблиц
TRUNCATE TABLE results RESTART IDENTITY CASCADE;
TRUNCATE TABLE event_schedule RESTART IDENTITY CASCADE;
TRUNCATE TABLE participants RESTART IDENTITY CASCADE;
TRUNCATE TABLE venues RESTART IDENTITY CASCADE;
TRUNCATE TABLE sports RESTART IDENTITY CASCADE;
TRUNCATE TABLE countries RESTART IDENTITY CASCADE;


-- =========================
-- COUNTRIES
-- =========================

INSERT INTO countries (country_code, name)
VALUES
('USA', 'United States'),
('CHN', 'China'),
('GER', 'Germany'),
('FRA', 'France'),
('BRA', 'Brazil');


-- =========================
-- SPORTS
-- =========================

INSERT INTO sports (sport_name, sport_type)
VALUES
('Swimming', 'individual'),
('Athletics', 'individual'),
('Football', 'team'),
('Basketball', 'team'),
('Tennis', 'individual');


-- =========================
-- VENUES
-- =========================

INSERT INTO venues (name, location, possible_sports)
VALUES
(
    'Olympic Swimming Pool',
    'North Olympic District',
    ARRAY[1]
),
(
    'Central Stadium',
    'Main Olympic Park',
    ARRAY[2,3]
),
(
    'Basketball Arena',
    'West Olympic Zone',
    ARRAY[4]
),
(
    'Tennis Center',
    'South Olympic Complex',
    ARRAY[5]
);


-- =========================
-- PARTICIPANTS
-- =========================

INSERT INTO participants (
    country_code,
    sport_id,
    full_name,
    birth_date
)
VALUES
('USA', 1, 'Michael Johnson', '1995-03-12'),
('CHN', 1, 'Li Wei', '1998-07-21'),
('GER', 2, 'Hans Muller', '1993-01-15'),
('FRA', 3, 'Pierre Dubois', '1990-11-03'),
('BRA', 4, 'Carlos Silva', '1994-05-10'),
('USA', 5, 'John Smith', '1997-09-18');

-- =========================
-- EVENT SCHEDULE
-- =========================

INSERT INTO event_schedule (
    sport_id,
    start_date,
    start_time,
    end_time,
    venue_id
)
VALUES
(1, '2026-07-10', '10:00', '12:00', 1),
(2, '2026-07-10', '13:00', '15:00', 2),
(3, '2026-07-11', '16:00', '18:00', 2),
(4, '2026-07-11', '19:00', '21:00', 3),
(5, '2026-07-12', '11:00', '13:00', 4),
(1, '2026-07-12', '14:00', '16:00', 1),
(2, '2026-07-13', '09:00', '11:00', 2),
(3, '2026-07-13', '12:00', '14:00', 2),
(4, '2026-07-13', '15:00', '17:00', 3),
(5, '2026-07-14', '10:00', '12:00', 4),
(1, '2026-07-14', '13:00', '15:00', 1),
(2, '2026-07-15', '11:00', '13:00', 2),
(3, '2026-07-15', '16:00', '18:00', 2),
(4, '2026-07-16', '18:00', '20:00', 3),
(5, '2026-07-16', '09:00', '11:00', 4),
(1, '2026-07-17', '08:00', '10:00', 1);

-- =========================
-- RESULTS
-- =========================

INSERT INTO results (
    sport_id,
    participant_id,
    start_id,
    place,
    result
)
VALUES
(1, 1, 1, 1, '52.13 sec'),
(1, 2, 1, 2, '53.02 sec'),
(2, 3, 2, 1, '9.95 sec'),
(3, 4, 3, 1, '2:1'),
(4, 5, 4, 2, '89:92'),
(5, 6, 5, 3, '6-4 4-6 5-7'),
(1, 1, 6, 2, '52.88 sec'),
(1, 2, 6, 1, '52.40 sec'),
(2, 3, 7, 1, '10.01 sec'),
(3, 4, 8, 2, '1:2'),
(4, 5, 9, 1, '95:90'),
(5, 6, 10, 1, '6-2 6-3'),
(1, 1, 11, 1, '51.97 sec'),
(1, 2, 11, 3, '53.50 sec'),
(2, 3, 12, 2, '10.15 sec'),
(3, 4, 13, 1, '3:0'),
(4, 5, 14, 3, '87:91'),
(5, 6, 15, 2, '4-6 7-5 3-6'),
(1, 1, 16, 1, '51.80 sec');

-- =========================
-- НЕПРАВИЛЬНЫЕ ДАННЫЕ
-- ДЛЯ ДЕМОНСТРАЦИИ ОГРАНИЧЕНИЙ
-- =========================


-- 1. Нарушение PRIMARY KEY / UNIQUE
-- Страна с уже существующим кодом

INSERT INTO countries (country_code, name)
VALUES ('USA', 'Another USA');


-- 2. Нарушение CHECK (sport_type)

INSERT INTO sports (sport_name, sport_type)
VALUES ('Boxing', 'mixed');


-- 3. Нарушение CHECK (возраст < 18)

INSERT INTO participants (
    country_code,
    sport_id,
    full_name,
    birth_date
)
VALUES
('USA', 1, 'Young Athlete', CURRENT_DATE - INTERVAL '10 years');


-- 4. Нарушение FOREIGN KEY
-- Несуществующая страна

INSERT INTO participants (
    country_code,
    sport_id,
    full_name,
    birth_date
)
VALUES
('ZZZ', 1, 'Unknown Athlete', '1995-01-01');


-- 5. Нарушение CHECK (время окончания раньше начала)

INSERT INTO event_schedule (
    sport_id,
    start_date,
    start_time,
    end_time,
    venue_id
)
VALUES
(1, '2026-07-15', '15:00', '14:00', 1);


-- 6. Нарушение UNIQUE (participant_id, start_id)

INSERT INTO results (
    sport_id,
    participant_id,
    start_id,
    place,
    result
)
VALUES
(1, 1, 1, 2, '54.00 sec');


-- 7. Демонстрация работы триггера
-- Попытка провести Tennis на Basketball Arena
-- Basketball Arena разрешает только sport_id = 4

INSERT INTO event_schedule (
    sport_id,
    start_date,
    start_time,
    end_time,
    venue_id
)
VALUES
(5, '2026-07-20', '10:00', '12:00', 3);

--==============================================================================

SELECT * FROM vw_country_medals;
SELECT * FROM vw_athlete_results;
SELECT * FROM vw_average_age_by_sport;
SELECT * FROM vw_schedule_by_venue WHERE start_date = '2026-07-10';