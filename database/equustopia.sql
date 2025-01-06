CREATE SCHEMA analytics;
CREATE TABLE analytics."pagesViews"(
    id SERIAL PRIMARY KEY,
    "userId" INTEGER,
    "pageId" INTEGER NOT NULL,
    "pageType" VARCHAR(50) CHECK ("pageType" IN ('equestrianCentre', 'horse', 'trade', 'serviceProvider')) NOT NULL,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "ipAddress" VARCHAR(45),
    FOREIGN KEY ("userId") REFERENCES main."userData"(id) ON DELETE SET NULL
);
ALTER TABLE analytics."pagesViews" ADD CONSTRAINT chk_page_type_length CHECK (LENGTH("pageType") BETWEEN 2 AND 50);
ALTER TABLE analytics."pagesViews" ADD CONSTRAINT chk_ip_address_length CHECK (LENGTH("ipAddress") BETWEEN 2 AND 45);

--usuwanie z pagesViews stron, które już nie istnieją
CREATE OR REPLACE FUNCTION analytics.delete_page_views()
RETURNS TRIGGER AS $$
BEGIN
    DELETE FROM analytics."pagesViews"
    WHERE "pageId" = OLD.id AND "pageType" = tg_table_name;
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER horse_delete_trigger2 AFTER DELETE ON main.horse FOR EACH ROW EXECUTE FUNCTION analytics.delete_page_views();
CREATE TRIGGER equestrian_centre_delete_trigger2 AFTER DELETE ON main."equestrianCentre" FOR EACH ROW EXECUTE FUNCTION analytics.delete_page_views();
CREATE TRIGGER trade_delete_trigger2 AFTER DELETE ON main."trade" FOR EACH ROW EXECUTE FUNCTION analytics.delete_page_views();
CREATE TRIGGER service_provider_delete_trigger2 AFTER DELETE ON main."serviceProvider" FOR EACH ROW EXECUTE FUNCTION analytics.delete_page_views();

CREATE VIEW analytics."mostViewedPages" AS
    SELECT "pageId", "pageType",
           COUNT(*) AS "viewsCount",
           RANK() OVER (ORDER BY COUNT(*) DESC) AS "position"
FROM analytics."pagesViews"
GROUP BY "pageId", "pageType"
ORDER BY "viewsCount" DESC
LIMIT 10;

CREATE OR REPLACE FUNCTION analytics.get_last_viewed_pages(user_id INTEGER)
RETURNS TABLE (
    "pageId" INTEGER,
    "pageType" VARCHAR(50),
    "timestamp" TIMESTAMP
) AS $$
BEGIN
    RETURN QUERY
    SELECT "pageId", "pageType", "timestamp"
    FROM analytics."pagesViews"
    WHERE "userId" = user_id
    ORDER BY "timestamp" DESC
    LIMIT 10;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION analytics.get_centre_views_by_date(
    startDate TIMESTAMP,
    endDate TIMESTAMP,
    centreId INTEGER
)
RETURNS TABLE ("date" DATE, "viewsCount" INTEGER) AS $$
BEGIN
    RETURN QUERY
    SELECT timestamp::DATE AS "date", COUNT(*)::INTEGER AS "viewsCount"
    FROM analytics."pagesViews"
    WHERE "pageType" = 'equestrianCentre'
      AND "pageId" = centreId
      AND timestamp BETWEEN startDate AND endDate
    GROUP BY timestamp::DATE
    ORDER BY "date";
END;
$$ LANGUAGE plpgsql;

SELECT * FROM analytics.get_centre_views_by_date(
    '2024-01-01 00:00:00'::TIMESTAMP,
    CURRENT_TIMESTAMP::TIMESTAMP,
    6
);



CREATE TABLE analytics.favorites(
    id SERIAL PRIMARY KEY,
    "userId" INTEGER NOT NULL,
    "pageId" INTEGER NOT NULL,
    "pageType" VARCHAR(50) CHECK ("pageType" IN ('equestrianCentre', 'horse', 'trade', 'serviceProvider')) NOT NULL,
    "createdAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE analytics."favorites" ADD CONSTRAINT chk_page_type_length CHECK (LENGTH("pageType") BETWEEN 2 AND 50);
ALTER TABLE analytics.favorites ADD CONSTRAINT fk_favorites_userid FOREIGN KEY ("userId") REFERENCES main."userData" (id) ON DELETE CASCADE;

CREATE OR REPLACE FUNCTION analytics.get_favorites(user_id INTEGER)
RETURNS TABLE(
    pageId INTEGER,
    pageType VARCHAR,
    createdAt TIMESTAMP
) AS $$
BEGIN
    RETURN QUERY
    SELECT "pageId", "pageType", "createdAt"
    FROM analytics.favorites
    WHERE "userId" = user_id
    ORDER BY "createdAt" DESC;
END;
$$ LANGUAGE plpgsql;

--usuwanie z favorites stron, które już nie istnieją
CREATE OR REPLACE FUNCTION analytics.delete_favorites()
RETURNS TRIGGER AS $$
BEGIN
    DELETE FROM analytics.favorites
    WHERE "pageId" = OLD.id AND "pageType" = tg_table_name;
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER horse_delete_trigger AFTER DELETE ON main.horse FOR EACH ROW EXECUTE FUNCTION analytics.delete_favorites();
CREATE TRIGGER equestrian_centre_delete_trigger AFTER DELETE ON main."equestrianCentre" FOR EACH ROW EXECUTE FUNCTION analytics.delete_favorites();
CREATE TRIGGER trade_delete_trigger AFTER DELETE ON main."trade" FOR EACH ROW EXECUTE FUNCTION analytics.delete_favorites();
CREATE TRIGGER service_provider_delete_trigger AFTER DELETE ON main."serviceProvider" FOR EACH ROW EXECUTE FUNCTION analytics.delete_favorites();

CREATE VIEW analytics."mostFavorites" AS
SELECT "pageId", "pageType",
    COUNT(*) AS "favoritesCount",
    RANK() OVER (ORDER BY COUNT(*) DESC) AS "position"
FROM analytics."favorites"
GROUP BY "pageId", "pageType"
ORDER BY "favoritesCount" DESC
LIMIT 10;





CREATE SCHEMA reference;
CREATE TYPE reference."requestStatus" AS ENUM ('new', 'in progress', 'approved', 'declined');

CREATE TABLE badges(
    id SERIAL PRIMARY KEY,
    name VARCHAR(250) NOT NULL UNIQUE,
    description VARCHAR(1000) NOT NULL,
    iconPath VARCHAR(1000) NOT NULL UNIQUE
);

CREATE TABLE "centreServices"(
    id SERIAL PRIMARY KEY,
    name VARCHAR(250) NOT NULL UNIQUE,
    description VARCHAR(1000) NOT NULL
);

CREATE TABLE "horseQualities"(
    id SERIAL PRIMARY KEY,
    name VARCHAR(250) NOT NULL UNIQUE,
    description VARCHAR(1000) NOT NULL
);

CREATE TABLE "serviceTypes"(
    id SERIAL PRIMARY KEY,
    name VARCHAR(250) NOT NULL UNIQUE,
    description VARCHAR(1000) NOT NULL
);

CREATE TABLE reference."workerChores"(
    id SERIAL PRIMARY KEY,
    name VARCHAR(250) NOT NULL UNIQUE
);





CREATE SCHEMA main;
CREATE TABLE main."userData"(
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(60) NOT NULL
);
ALTER TABLE main."userData" ADD COLUMN "birthDate" DATE;
ALTER TABLE main."userData" ADD COLUMN "profilePhoto" VARCHAR(1000);
ALTER TABLE main."userData" ADD COLUMN "moderator" BOOLEAN DEFAULT FALSE;
ALTER TABLE main."userData" ADD COLUMN "private" BOOLEAN DEFAULT TRUE;

INSERT INTO main."userData" (name, email, password) VALUES ('User2', 'user2@gmail.com', 'haslohaslo');
SELECT * FROM main."userData";

ALTER TABLE main."userData" ADD CONSTRAINT chk_name_length CHECK (LENGTH(name) BETWEEN 2 AND 50);
ALTER TABLE main."userData" ADD CONSTRAINT chk_email_length CHECK (LENGTH(email) BETWEEN 5 AND 255);
ALTER TABLE main."userData" ADD CONSTRAINT chk_password_length CHECK (LENGTH(password) BETWEEN 4 AND 60);
ALTER TABLE main."userData" ADD CONSTRAINT chk_birth_date CHECK ("birthDate" <= CURRENT_DATE);

CREATE VIEW main."publicUsers" AS
SELECT id, name, email, "birthDate", "profilePhoto"
FROM main."userData"
WHERE "private" = FALSE;



CREATE TABLE main.horse(
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    "userId" INTEGER NOT NULL,
    "centreId" INTEGER,
    FOREIGN KEY ("userId") REFERENCES main."userData"(id) ON DELETE CASCADE,
    FOREIGN KEY ("centreId") REFERENCES main."equestrianCentre"(id) ON DELETE SET NULL
);
ALTER TABLE main.horse ADD COLUMN "birthDate" DATE;
ALTER TABLE main.horse ADD COLUMN "breed" VARCHAR(100);
ALTER TABLE main.horse ADD COLUMN "private" BOOLEAN DEFAULT TRUE;
ALTER TABLE main.horse ADD COLUMN "photo" VARCHAR(1000);
ALTER TABLE main.horse ADD COLUMN "height" DOUBLE PRECISION;
ALTER TABLE main.horse ADD COLUMN "feedingSchedule" JSONB;

INSERT INTO main.horse (name, "userId") VALUES ('Blobiczek', 36);
INSERT INTO main.horse (name, "userId") VALUES ('Juglaś', 36);

ALTER TABLE main.horse ADD CONSTRAINT chk_name_length CHECK (LENGTH(name) BETWEEN 2 AND 50);
ALTER TABLE main.horse ADD CONSTRAINT chk_breed_length CHECK (LENGTH(breed) BETWEEN 2 AND 100);
ALTER TABLE main.horse ADD CONSTRAINT chk_birth_date CHECK ("birthDate" <= CURRENT_DATE);

CREATE VIEW main."publicHorses" AS
SELECT id, name, "userId", "birthDate", "breed", "photo", "height"
FROM main.horse
WHERE "private" = FALSE;



CREATE TABLE main."equestrianCentre"(
    id SERIAL PRIMARY KEY,
    name VARCHAR(250) NOT NULL,
    "userId" INTEGER NOT NULL,
    latitude DOUBLE PRECISION,
    longitude DOUBLE PRECISION,
    address VARCHAR(250),
    FOREIGN KEY ("userId") REFERENCES main."userData"(id) ON DELETE CASCADE
);
ALTER TABLE main."equestrianCentre" ADD COLUMN "openHours" JSONB;
ALTER TABLE main."equestrianCentre" ADD COLUMN "contactInformation" JSONB;
ALTER TABLE main."equestrianCentre" ADD COLUMN approved BOOLEAN DEFAULT false;

INSERT INTO main."equestrianCentre" (name, "userId") VALUES ('Jazda Konna Blobi', 32);

ALTER TABLE main."equestrianCentre" ADD CONSTRAINT chk_name_length CHECK (LENGTH(name) BETWEEN 2 AND 250);
ALTER TABLE main."equestrianCentre" ADD CONSTRAINT chk_address_length CHECK (LENGTH(address) BETWEEN 2 AND 250);
ALTER TABLE main."equestrianCentre" ADD CONSTRAINT unique_latitude_longitude UNIQUE (latitude, longitude);

CREATE OR REPLACE FUNCTION main.get_horse_counts_by_age_group(centreId INTEGER)
RETURNS TABLE(age_group VARCHAR, horse_count INTEGER) AS $$
BEGIN
    RETURN QUERY
    SELECT
        CAST(
            CASE
                WHEN EXTRACT(YEAR FROM AGE(horse."birthDate")) <= 3 THEN '0-3'
                WHEN EXTRACT(YEAR FROM AGE(horse."birthDate")) <= 10 THEN '3-10'
                WHEN EXTRACT(YEAR FROM AGE(horse."birthDate")) <= 19 THEN '10-19'
                ELSE '19+'
            END AS VARCHAR
        ) AS age_group,
        COUNT(*)::INTEGER AS horse_count
    FROM main.horse AS horse
    WHERE horse."centreId" = centreId
    GROUP BY age_group
    ORDER BY age_group;
END;
$$ LANGUAGE plpgsql;

SELECT * FROM main.get_horse_counts_by_age_group(1);

CREATE OR REPLACE FUNCTION main.get_horses_by_centre(centreId INTEGER)
RETURNS TABLE(horse_id INTEGER, horse_name VARCHAR, birth_date DATE, breed VARCHAR, photo VARCHAR, height DOUBLE PRECISION, feeding_schedule JSONB) AS $$
BEGIN
    RETURN QUERY
    SELECT
        horse.id AS horse_id,
        horse.name AS horse_name,
        horse."birthDate" AS birth_date,
        horse.breed,
        horse.photo,
        horse.height,
        horse."feedingSchedule"
    FROM main.horse AS horse
    WHERE horse."centreId" = centreId;
END;
$$ LANGUAGE plpgsql;



CREATE TABLE main.trade(
    id SERIAL PRIMARY KEY,
    "horseId" INTEGER NOT NULL,
    price DOUBLE PRECISION,
    location VARCHAR(250) NOT NULL,
    description VARCHAR(1000) NOT NULL,
    "contactInformation" JSONB NOT NULL,
    "createAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY ("horseId") REFERENCES main."horse"(id) ON DELETE CASCADE
);









CREATE TABLE main."userBadge"(
    id SERIAL PRIMARY KEY,
    "userId" INTEGER NOT NULL,
    FOREIGN KEY ("userId") REFERENCES main."userData"(id) ON DELETE CASCADE,
    "badgeId" INTEGER NOT NULL,
    FOREIGN KEY ("badgeId") REFERENCES reference."badges"(id) ON DELETE CASCADE,
    "approvedBy" INTEGER,
    FOREIGN KEY ("approvedBy") REFERENCES main."userData"(id),
    "centreId" INTEGER,
    FOREIGN KEY ("centreId") REFERENCES main."equestrianCentre"(id),
    "approvedAt" TIMESTAMP
);
ALTER TABLE main."userBadge" DROP CONSTRAINT "userBadge_approvedBy_fkey";
ALTER TABLE main."userBadge" DROP CONSTRAINT "userBadge_centreId_fkey";
ALTER TABLE main."userBadge"
ADD CONSTRAINT "userBadge_approvedBy_fkey"
FOREIGN KEY ("approvedBy") REFERENCES main."worker"(id) ON DELETE SET NULL;
ALTER TABLE main."userBadge"
ADD CONSTRAINT "userBadge_centreId_fkey"
FOREIGN KEY ("centreId") REFERENCES main."equestrianCentre"(id) ON DELETE SET NULL;

CREATE TABLE main."centreServicesMapping"(
    "centreId" INTEGER NOT NULL,
    "serviceId" INTEGER NOT NULL,
    PRIMARY KEY ("centreId", "serviceId"),
    FOREIGN KEY ("serviceId") REFERENCES reference."centreServices"(id) ON DELETE CASCADE,
    FOREIGN KEY ("centreId") REFERENCES main."equestrianCentre"(id) ON DELETE CASCADE
);

CREATE TABLE main."horseQualitiesMapping"(
    "tradeId" INTEGER NOT NULL,
    "qualityId" INTEGER NOT NULL,
    PRIMARY KEY ("tradeId", "qualityId"),
    FOREIGN KEY ("qualityId") REFERENCES reference."horseQualities"(id) ON DELETE CASCADE,
    FOREIGN KEY ("tradeId") REFERENCES "trade"(id) ON DELETE CASCADE
);



CREATE TABLE main."serviceProvider"(
    id SERIAL PRIMARY KEY,
    "userId" INTEGER NOT NULL,
    FOREIGN KEY ("userId") REFERENCES "userData"(id) ON DELETE CASCADE,
    "typeId" INTEGER NOT NULL,
    FOREIGN KEY ("typeId") REFERENCES reference."serviceTypes"(id) ON DELETE CASCADE,
    description VARCHAR(1000) NOT NULL,
    "contactInformation" JSONB NOT NULL,
    location VARCHAR(250) NOT NULL,
    "createAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE main."worker"(
    id SERIAL PRIMARY KEY,
    "userId" INTEGER NOT NULL,
    FOREIGN KEY ("userId") REFERENCES "userData"(id) ON DELETE CASCADE,
    "centreId" INTEGER NOT NULL,
    FOREIGN KEY ("centreId") REFERENCES "equestrianCentre"(id) ON DELETE CASCADE,
    "jobTitle" VARCHAR(250),
    salary DOUBLE PRECISION,
    "hireDate" DATE DEFAULT CURRENT_DATE,
    "contactInformation" JSONB NOT NULL
);

CREATE TABLE main."workerChoresMapping"(
    "workerId" INTEGER NOT NULL,
    "choreId" INTEGER NOT NULL,
    PRIMARY KEY ("workerId", "choreId"),
    FOREIGN KEY ("choreId") REFERENCES reference."workerChores"(id) ON DELETE CASCADE,
    FOREIGN KEY ("workerId") REFERENCES worker (id) ON DELETE CASCADE
);

CREATE TABLE main."schedule"(
    id SERIAL PRIMARY KEY,
    "centreId" INTEGER NOT NULL,
    FOREIGN KEY ("centreId") REFERENCES "equestrianCentre"(id) ON DELETE CASCADE,
    "workerId" INTEGER,
    FOREIGN KEY ("workerId") REFERENCES worker (id) ON DELETE SET NULL,
    "name" VARCHAR (250) NOT NULL,
    "startDateTime" TIMESTAMP NOT NULL,
    "endDateTime" TIMESTAMP NOT NULL,
    "choreId" INTEGER,
    FOREIGN KEY ("choreId") REFERENCES reference."workerChores"(id) ON DELETE SET NULL,
    price DOUBLE PRECISION
);
ALTER TABLE main."schedule" ADD COLUMN "numberOfParticipants" INTEGER;

CREATE TABLE main."centreCreateRequest"(
    id SERIAL PRIMARY KEY,
    status reference."requestStatus" NOT NULL DEFAULT 'new',
    "createdAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE main."centreCreateRequest"
ADD COLUMN "centreId" INTEGER NOT NULL,
ADD CONSTRAINT fk_centre
    FOREIGN KEY ("centreId")
    REFERENCES "equestrianCentre"("id")
    ON DELETE CASCADE;

ALTER TABLE main."equestrianCentre" ADD COLUMN "approved" BOOLEAN DEFAULT false;
ALTER TABLE main."worker" ADD COLUMN "certifiedInstructor" BOOLEAN DEFAULT false;

CREATE TABLE main."badgeApprovalRequest"(
    id SERIAL PRIMARY KEY,
    "receiverId" INTEGER NOT NULL,
    FOREIGN KEY ("receiverId") REFERENCES "worker"(id) ON DELETE CASCADE,
    "badgeId" INTEGER NOT NULL,
    FOREIGN KEY ("badgeId") REFERENCES "userBadge"(id) ON DELETE CASCADE,
    status reference."requestStatus" NOT NULL DEFAULT 'new',
    "createdAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE main."bookedActivities" (
    "userId" INTEGER NOT NULL,
    "scheduleId" INTEGER,
    participated BOOLEAN DEFAULT FALSE,
    PRIMARY KEY ("userId", "scheduleId"),
    FOREIGN KEY ("userId") REFERENCES main."userData" (id) ON DELETE CASCADE,
    FOREIGN KEY ("scheduleId") REFERENCES main."schedule" (id) ON DELETE CASCADE
);