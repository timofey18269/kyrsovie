CREATE TABLE countries (
    country_code VARCHAR(3) PRIMARY KEY,  -- ( 'RUS', 'USA'...)
    name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE sports (
    sport_id SERIAL PRIMARY KEY,
    sport_name VARCHAR(100) NOT NULL,
    sport_type VARCHAR(20) NOT NULL CHECK (sport_type IN ('team', 'individual')),
    CONSTRAINT unique_sport_type UNIQUE (sport_name, sport_type)
);

CREATE TABLE participants (
    participant_id SERIAL PRIMARY KEY,
    country_code VARCHAR(3) NOT NULL,
    sport_id INT NOT NULL,
    full_name VARCHAR(200) NOT NULL,
    birth_date DATE NOT NULL,
    CONSTRAINT fk_participants_countries FOREIGN KEY (country_code) 
        REFERENCES countries(country_code) ON DELETE RESTRICT,
    CONSTRAINT fk_participants_sports FOREIGN KEY (sport_id) 
        REFERENCES sports(sport_id) ON DELETE RESTRICT,
    CONSTRAINT check_age CHECK (EXTRACT(YEAR FROM AGE(CURRENT_DATE, birth_date)) >= 18)
);

CREATE TABLE venues (
    venue_id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    location VARCHAR(500) NOT NULL,
    possible_sports INT[] DEFAULT ARRAY[]::INT[]
);

CREATE TABLE event_schedule (
    start_id SERIAL PRIMARY KEY,
    sport_id INT NOT NULL,
    start_date DATE NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    venue_id INT NOT NULL,
    CONSTRAINT fk_event_schedule_sports FOREIGN KEY (sport_id) 
        REFERENCES sports(sport_id) ON DELETE RESTRICT,
    CONSTRAINT fk_event_schedule_venues FOREIGN KEY (venue_id) 
        REFERENCES venues(venue_id) ON DELETE RESTRICT,
    CONSTRAINT check_time CHECK (end_time > start_time)
);

CREATE TABLE results (
    result_id SERIAL PRIMARY KEY,
    sport_id INT NOT NULL,
    participant_id INT NOT NULL,
    start_id INT NOT NULL,
    place INT,
    result TEXT,
    CONSTRAINT fk_results_sports FOREIGN KEY (sport_id) 
        REFERENCES sports(sport_id) ON DELETE RESTRICT,
    CONSTRAINT fk_results_participants FOREIGN KEY (participant_id) 
        REFERENCES participants(participant_id) ON DELETE RESTRICT,
    CONSTRAINT fk_results_event_schedule FOREIGN KEY (start_id) 
        REFERENCES event_schedule(start_id) ON DELETE RESTRICT,
    CONSTRAINT unique_participant_start UNIQUE (participant_id, start_id)
);


CREATE OR REPLACE FUNCTION check_sport_on_venue()
RETURNS TRIGGER AS $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM venues 
        WHERE venue_id = NEW.venue_id 
        AND NEW.sport_id = ANY(possible_sports)
    ) THEN
        RAISE EXCEPTION 'Sport % is not allowed at venue %', NEW.sport_id, NEW.venue_id;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE TRIGGER trigger_check_sport_on_venue
    BEFORE INSERT OR UPDATE ON event_schedule
    FOR EACH ROW
    EXECUTE FUNCTION check_sport_on_venue();

CREATE INDEX idx_participants_country ON participants(country_code);
CREATE INDEX idx_participants_sport ON participants(sport_id);
CREATE INDEX idx_event_schedule_date ON event_schedule(start_date);
CREATE INDEX idx_event_schedule_venue ON event_schedule(venue_id);
CREATE INDEX idx_results_start ON results(start_id);
CREATE INDEX idx_results_participant ON results(participant_id);

-- ========================================================================================================

CREATE OR REPLACE VIEW vw_country_medals AS
SELECT
    c.country_code,
    c.name AS country_name,
    COUNT(CASE WHEN r.place BETWEEN 1 AND 3 THEN 1 END) AS total_medals,

    COUNT(CASE WHEN r.place = 1 THEN 1 END) AS gold_medals,
    COUNT(CASE WHEN r.place = 2 THEN 1 END) AS silver_medals,
    COUNT(CASE WHEN r.place = 3 THEN 1 END) AS bronze_medals

FROM countries c
LEFT JOIN participants p
    ON c.country_code = p.country_code
LEFT JOIN results r
    ON p.participant_id = r.participant_id
GROUP BY c.country_code, c.name
ORDER BY gold_medals DESC,
         silver_medals DESC,
         bronze_medals DESC;
		 

CREATE OR REPLACE VIEW vw_athlete_results AS
SELECT
    p.participant_id,
    p.full_name,
    c.name AS country,
    s.sport_name,
    es.start_date,
    es.start_time,
    v.name AS venue,
    r.place,
    r.result

FROM results r
JOIN participants p
    ON r.participant_id = p.participant_id
JOIN countries c
    ON p.country_code = c.country_code
JOIN sports s
    ON r.sport_id = s.sport_id
JOIN event_schedule es
    ON r.start_id = es.start_id
JOIN venues v
    ON es.venue_id = v.venue_id
ORDER BY p.full_name;


CREATE OR REPLACE VIEW vw_average_age_by_sport AS
SELECT
    s.sport_id,
    s.sport_name,
    ROUND(
        AVG(
            EXTRACT(YEAR FROM AGE(CURRENT_DATE, p.birth_date))
        ),
        2
    ) AS average_age

FROM sports s
LEFT JOIN participants p
    ON s.sport_id = p.sport_id
GROUP BY s.sport_id, s.sport_name
ORDER BY average_age DESC;


CREATE OR REPLACE VIEW vw_schedule_by_venue AS
SELECT
    es.start_date,
    v.name AS venue_name,
    s.sport_name,
    es.start_time,
    es.end_time

FROM event_schedule es
JOIN sports s
    ON es.sport_id = s.sport_id
JOIN venues v
    ON es.venue_id = v.venue_id
ORDER BY es.start_date,
         v.name,
         es.start_time;