CREATE SCHEMA main;
CREATE SCHEMA analytics;

CREATE TABLE main."userData"(
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(60) NOT NULL
);
CREATE TABLE main."equestrianCentre"(
    id SERIAL PRIMARY KEY,
    name VARCHAR(250) NOT NULL,
    "userId" INTEGER NOT NULL,
    latitude DOUBLE PRECISION,
    longitude DOUBLE PRECISION,
    address VARCHAR(250),
    UNIQUE (latitude, longitude),
    FOREIGN KEY ("userId") REFERENCES main."userData"(id) ON DELETE CASCADE
);
CREATE TABLE main.horse(
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    "userId" INTEGER NOT NULL,
    "centreId" INTEGER,
    FOREIGN KEY ("userId") REFERENCES main."userData"(id) ON DELETE CASCADE,
    FOREIGN KEY ("centreId") REFERENCES main."equestrianCentre"(id) ON DELETE SET NULL
);

CREATE TABLE analytics."pagesViews"(
    id SERIAL PRIMARY KEY,
    "userId" INTEGER,
    "pageId" INTEGER NOT NULL,
    "pageType" VARCHAR(50) CHECK ("pageType" IN ('equestrianCentre', 'horse')) NOT NULL,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "ipAddress" VARCHAR(45),
    FOREIGN KEY ("userId") REFERENCES main."userData"(id)
);

CREATE VIEW analytics."mostViewedPages" AS
    SELECT "pageId", "pageType",
           COUNT(*) AS "viewsCount",
           RANK() OVER (ORDER BY COUNT(*) DESC) AS Pozycja
FROM analytics."pagesViews"
GROUP BY "pageId", "pageType"
ORDER BY "viewsCount" DESC
LIMIT 10;

INSERT INTO main."userData" (name, email, password)
VALUES ('User2', 'user2@gmail.com', 'haslohaslo');

SELECT * FROM main."userData";

ALTER TABLE main."userData" ADD CONSTRAINT chk_name_length CHECK (LENGTH(name) BETWEEN 2 AND 50);
ALTER TABLE main."userData" ADD CONSTRAINT chk_email_length CHECK (LENGTH(email) BETWEEN 5 AND 255);
ALTER TABLE main."userData" ADD CONSTRAINT chk_password_length CHECK (LENGTH(password) BETWEEN 4 AND 60);

INSERT INTO main.horse (name, "userId") VALUES ('Blobiczek', 36);
INSERT INTO main.horse (name, "userId") VALUES ('Juglaś', 36);

ALTER TABLE main.horse ADD COLUMN "birthDate" DATE;

ALTER TABLE main.horse ADD CONSTRAINT chk_name_length CHECK (LENGTH(name) BETWEEN 2 AND 50);


--15.11.2024
INSERT INTO main."equestrianCentre" (name, "userId") VALUES ('Jazda Konna Blobi', 32);

--19.11.2024
ALTER TABLE main."equestrianCentre" ADD CONSTRAINT chk_name_length CHECK (LENGTH(name) BETWEEN 2 AND 250);

--20.11.2024
--podział konisiów na wiek
SELECT name, "userId",
        CASE
            WHEN "birthDate" < (CURRENT_DATE - INTERVAL '15 years') THEN 'gen X'
            WHEN "birthDate" > (CURRENT_DATE - INTERVAL '15 years') AND "birthDate" < (CURRENT_DATE - INTERVAL '5 years') THEN 'millennials'
            WHEN "birthDate" > (CURRENT_DATE - INTERVAL '5 years') THEN 'gen Z'
            ELSE 'no birthdate'
        END
FROM main.horse;

--liczenie ile w danej stajni jest koni o danym wieku
SELECT e.name,
       SUM(CASE WHEN h."birthDate" < (CURRENT_DATE - INTERVAL '15 years') THEN 1 ELSE 0 END) as genX,
       SUM(CASE WHEN h."birthDate" > (CURRENT_DATE - INTERVAL '15 years') AND h."birthDate" < (CURRENT_DATE - INTERVAL '5 years') THEN 1 ELSE 0 END) as millennials,
       SUM(CASE WHEN h."birthDate" > (CURRENT_DATE - INTERVAL '5 years') THEN 1 ELSE 0 END) as genZ,
       SUM(CASE WHEN h."birthDate" is NULL THEN 1 ELSE 0 END) as empty
FROM main.horse h JOIN main."equestrianCentre" e on h."centreId" = e.id
GROUP BY e.name, h."centreId"
ORDER BY e.name;

--lista użytkowników, którzy mają konia starszego niż 10 lat jeśli mieszka w ośrodku 2
--konia starszego niż 5 lat jeśli mieszka w ośrodku 5
--konia starszego niż 1 rok jeśli mieszka w ośrodku 1
SELECT u.email, u.name, h.name, h."birthDate"
FROM main."userData" u JOIN main.horse h on u.id = h."userId"
WHERE h."birthDate" <
CASE
    WHEN h."centreId" IN (2) THEN (CURRENT_DATE - INTERVAL '10 years')
    WHEN h."centreId" IN (5) THEN (CURRENT_DATE - INTERVAL '5 years')
    WHEN h."centreId" IN (1) THEN (CURRENT_DATE - INTERVAL '1 years')
END
ORDER BY 4 DESC;

--ośrodki które mają więcej niż 1 konia
WITH statementCTE AS (SELECT "centreId", COUNT(*) AS num FROM main.horse GROUP BY "centreId")
SELECT e.name FROM statementCTE JOIN main."equestrianCentre" e on "centreId" = e.id WHERE num >= 2;