-- ============================================
-- Создание базы данных
-- ============================================

CREATE DATABASE zaderzhania
    WITH
    OWNER = postgres
    ENCODING = 'UTF8';

-- ============================================
-- Справочники
-- ============================================

CREATE TABLE positions (
    position_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    salary NUMERIC(12,2)
);

CREATE TABLE offense_types (
    offense_type_id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL UNIQUE,
    description TEXT
);

-- ============================================
-- Полицейские
-- ============================================

CREATE TABLE police_officers (
    officer_id SERIAL PRIMARY KEY,
    badge_number VARCHAR(30) NOT NULL UNIQUE,
    last_name VARCHAR(100) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    middle_name VARCHAR(100),
    position_id INTEGER REFERENCES positions(position_id),
    rank_name VARCHAR(100),
    hire_date DATE,
    phone VARCHAR(30),
    email VARCHAR(200),
    active BOOLEAN DEFAULT TRUE
);

-- ============================================
-- Задержанные
-- ============================================

CREATE TABLE detainees (
    detainee_id SERIAL PRIMARY KEY,
    last_name VARCHAR(100) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    middle_name VARCHAR(100),
    birth_date DATE,
    passport_number VARCHAR(50),
    address TEXT,
    phone VARCHAR(30),
    status VARCHAR(20) NOT NULL DEFAULT 'В камере'
        CHECK (
            status IN (
                'В камере',
                'Отпущен',
                'Переведён'
            )
        ),
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- Свидетели
-- ============================================

CREATE TABLE witnesses (
    witness_id SERIAL PRIMARY KEY,
    last_name VARCHAR(100) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    middle_name VARCHAR(100),
    phone VARCHAR(30),
    address TEXT,
    statement TEXT
);

-- ============================================
-- Заявления о правонарушениях
-- ============================================

CREATE TABLE violation_reports (
    report_id SERIAL PRIMARY KEY,
    report_number VARCHAR(50) UNIQUE NOT NULL,
    report_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    witness_id INTEGER REFERENCES witnesses(witness_id),
    offense_type_id INTEGER REFERENCES offense_types(offense_type_id),
    location TEXT,
    description TEXT NOT NULL
);

-- ============================================
-- Принятые меры
-- ============================================

CREATE TABLE measures (
    measure_id SERIAL PRIMARY KEY,

    case_id INTEGER NOT NULL
        REFERENCES cases(case_id)
        ON DELETE CASCADE,

    detainee_id INTEGER NOT NULL
        REFERENCES detainees(detainee_id),

    measure_type VARCHAR(50) NOT NULL
        CHECK (
            measure_type IN (
                'Штраф',
                'Предупреждение',
                'Административный арест',
                'Устное замечание',
                'Обязательные работы',
                'Конфискация',
                'Передача материалов в суд',
                'Передача материалов следствию',
                'Освобождение без санкций'
            )
        ),

    description TEXT,
    duration_days INTEGER,

    issued_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- Дела о правонарушениях
-- ============================================

CREATE TABLE cases (
    case_id SERIAL PRIMARY KEY,
    case_number VARCHAR(50) UNIQUE NOT NULL,

    report_id INTEGER
        REFERENCES violation_reports(report_id)
        ON DELETE SET NULL,

    offense_type_id INTEGER
        REFERENCES offense_types(offense_type_id),

    opened_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    closed_at TIMESTAMP,

    status VARCHAR(30) NOT NULL DEFAULT 'Открыто'
        CHECK (
            status IN (
                'Открыто',
                'В расследовании',
                'Закрыто',
                'Передано в суд'
            )
        ),

    description TEXT
);

-- ============================================
-- Полицейские, участвующие в деле
-- ============================================

CREATE TABLE case_officers (
    case_id INTEGER NOT NULL
        REFERENCES cases(case_id)
        ON DELETE CASCADE,

    officer_id INTEGER NOT NULL
        REFERENCES police_officers(officer_id),

    role_in_case VARCHAR(100),

    PRIMARY KEY (case_id, officer_id)
);

-- ============================================
-- Задержанные, участвующие в деле
-- ============================================

CREATE TABLE case_detainees (
    case_id INTEGER NOT NULL
        REFERENCES cases(case_id)
        ON DELETE CASCADE,

    detainee_id INTEGER NOT NULL
        REFERENCES detainees(detainee_id),

    detention_datetime TIMESTAMP,

    PRIMARY KEY (case_id, detainee_id)
);

-- ============================================
-- свидетили, участвующие в деле
-- ============================================

CREATE TABLE case_witnesses (
    case_id INTEGER NOT NULL
        REFERENCES cases(case_id)
        ON DELETE CASCADE,

    witness_id INTEGER NOT NULL
        REFERENCES witnesses(witness_id),

    testimony_notes TEXT,

    PRIMARY KEY (case_id, witness_id)
);
-- ============================================
-- Камеры
-- ============================================

CREATE TABLE detention_cells (
    cell_id SERIAL PRIMARY KEY,

    cell_number VARCHAR(20) NOT NULL UNIQUE,

    capacity INTEGER NOT NULL
        CHECK (capacity > 0),

    notes TEXT
);

-- ============================================
-- История размещения в камерах
-- Дополнительная таблица для аудита
-- ============================================

CREATE TABLE cell_history (
    history_id SERIAL PRIMARY KEY,

    detainee_id INTEGER NOT NULL
        REFERENCES detainees(detainee_id),

    cell_id INTEGER NOT NULL
        REFERENCES detention_cells(cell_id),

    placed_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    released_at TIMESTAMP
);

-- ============================================================
-- Отчёт 1: Сводка по делам
-- Объединяет: cases, offense_types, violation_reports,
--             case_officers, case_detainees, case_witnesses,
--             measures
-- Показывает по каждому делу: тип нарушения, статус,
-- количество участников, количество и сумму мер
-- ============================================================

CREATE OR REPLACE VIEW report_case_summary AS
SELECT
    c.case_id,
    c.case_number,
    ot.name                                   AS offense_type,
    c.status                                  AS case_status,
    vr.report_number,
    c.opened_at,
    c.closed_at,

    -- количество участников дела
    (SELECT COUNT(*) FROM case_officers co  WHERE co.case_id = c.case_id) AS officers_count,
    (SELECT COUNT(*) FROM case_detainees cd WHERE cd.case_id = c.case_id) AS detainees_count,
    (SELECT COUNT(*) FROM case_witnesses cw WHERE cw.case_id = c.case_id) AS witnesses_count,

    -- сводка по мерам
    (SELECT COUNT(*) FROM measures m WHERE m.case_id = c.case_id)         AS measures_count,
    (SELECT STRING_AGG(DISTINCT m.measure_type, ', ')
       FROM measures m WHERE m.case_id = c.case_id)                       AS measure_types,

    c.description

FROM cases c
LEFT JOIN offense_types     ot ON ot.offense_type_id = c.offense_type_id
LEFT JOIN violation_reports vr ON vr.report_id        = c.report_id
ORDER BY c.opened_at;

-- Пример использования отчёта:
-- SELECT * FROM report_case_summary;
-- SELECT * FROM report_case_summary WHERE case_status = 'В расследовании';


-- ============================================================
-- Отчёт 2: Нагрузка сотрудников полиции по делам
-- Объединяет: police_officers, positions, case_officers,
--             cases, measures, case_detainees
-- Показывает по каждому сотруднику: должность, звание,
-- количество дел, их роли и связанные меры
-- ============================================================

CREATE OR REPLACE VIEW report_officer_workload AS
SELECT
    po.officer_id,
    po.badge_number,
    po.last_name || ' ' || po.first_name || ' ' || COALESCE(po.middle_name, '') AS full_name,
    pos.name                                   AS position_name,
    po.rank_name,
    po.active,

    -- количество дел и роли
    COUNT(DISTINCT co.case_id)                 AS cases_count,
    STRING_AGG(DISTINCT co.role_in_case, '; ') AS roles,

    -- сколько задержанных прошло через дела этого сотрудника
    (SELECT COUNT(DISTINCT cd.detainee_id)
       FROM case_detainees cd
       WHERE cd.case_id IN (
           SELECT case_id FROM case_officers WHERE officer_id = po.officer_id
       )
    )                                           AS detainees_handled,

    -- сколько мер применено в делах этого сотрудника
    (SELECT COUNT(*)
       FROM measures m
       WHERE m.case_id IN (
           SELECT case_id FROM case_officers WHERE officer_id = po.officer_id
       )
    )                                           AS measures_in_cases

FROM police_officers po
LEFT JOIN positions     pos ON pos.position_id = po.position_id
LEFT JOIN case_officers co  ON co.officer_id    = po.officer_id
GROUP BY po.officer_id, po.badge_number, po.last_name, po.first_name,
         po.middle_name, pos.name, po.rank_name, po.active
ORDER BY cases_count DESC;

-- Пример использования отчёта:
-- SELECT * FROM report_officer_workload;
-- SELECT * FROM report_officer_workload WHERE cases_count > 0;