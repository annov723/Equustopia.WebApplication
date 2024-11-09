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