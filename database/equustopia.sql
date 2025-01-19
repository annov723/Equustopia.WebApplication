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
ALTER TABLE analytics."pagesViews" ADD CONSTRAINT chk_ip_address_length CHECK ("ipAddress" IS NULL OR LENGTH("ipAddress") BETWEEN 2 AND 45);

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
    startDate timestamp with time zone, endDate timestamp with time zone, centreId INTEGER)
RETURNS TABLE ("date" DATE, "viewsCount" INTEGER) AS $$
BEGIN
    RETURN QUERY
    WITH date_series AS (
        SELECT generate_series(startDate::DATE, endDate::DATE, '1 day') AS "date"
    ),
    views AS (
        SELECT
            timestamp::DATE AS "date",
            COUNT(*)::INTEGER AS "viewsCount"
        FROM analytics."pagesViews"
        WHERE "pageType" = 'equestrianCentre'
          AND "pageId" = centreId
          AND timestamp BETWEEN startDate AND endDate
        GROUP BY timestamp::DATE
    )
    SELECT
        d."date"::DATE,
        COALESCE(v."viewsCount", 0)::INTEGER AS "viewsCount"
    FROM date_series d
    LEFT JOIN views v ON d."date" = v."date"
    ORDER BY d."date";
END;
$$ LANGUAGE plpgsql;

SELECT * FROM analytics.get_centre_views_by_date(
    '2025-01-06 00:00:00'::TIMESTAMP,
    CURRENT_TIMESTAMP::TIMESTAMP,
    5
);

CREATE OR REPLACE FUNCTION analytics.get_centre_views_by_hour(centre_id INTEGER)
RETURNS TABLE(date TIMESTAMP, "viewsCount" INTEGER) AS $$
BEGIN
    RETURN QUERY
    WITH hours AS (
        SELECT generate_series(
            date_trunc('hour', NOW() - INTERVAL '23 hours')::TIMESTAMP,
            date_trunc('hour', NOW())::TIMESTAMP,
            '1 hour'
        ) AS date
    ),
    views AS (
        SELECT
            date_trunc('hour', timestamp)::TIMESTAMP AS date,
            COUNT(*)::INTEGER AS "viewsCount"
        FROM analytics."pagesViews"
        WHERE "pageType" = 'equestrianCentre'
          AND "pageId" = centre_id
          AND timestamp >= NOW() - INTERVAL '24 hours'
        GROUP BY date_trunc('hour', timestamp)::TIMESTAMP
    )
    SELECT h.date, COALESCE(v."viewsCount", 0)::INTEGER AS "viewsCount"
    FROM hours h
    LEFT JOIN views v ON h.date = v.date
    ORDER BY h.date;
END;
$$ LANGUAGE plpgsql;

SELECT * FROM analytics.get_centre_views_by_hour(5);



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
CREATE TABLE badges(
    id SERIAL PRIMARY KEY,
    name VARCHAR(250) NOT NULL UNIQUE,
    description VARCHAR(1000) NOT NULL,
    iconPath VARCHAR(1000) NOT NULL UNIQUE
);
ALTER TABLE reference."badges" ADD CONSTRAINT chk_badges_name_length CHECK (LENGTH("name") BETWEEN 2 AND 250);
ALTER TABLE reference."badges" ADD CONSTRAINT chk_badges_description_length CHECK (LENGTH("description") BETWEEN 2 AND 1000);
ALTER TABLE reference."badges" ADD CONSTRAINT chk_badges_icon_path_length CHECK (LENGTH("iconPath") BETWEEN 2 AND 1000);

CREATE TABLE "centreServices"(
    id SERIAL PRIMARY KEY,
    name VARCHAR(250) NOT NULL UNIQUE,
    description VARCHAR(1000) NOT NULL
);
ALTER TABLE reference."centreServices" ADD CONSTRAINT chk_centre_services_name_length CHECK (LENGTH("name") BETWEEN 2 AND 250);
ALTER TABLE reference."centreServices" ADD CONSTRAINT chk_centre_services_description_length CHECK (LENGTH("description") BETWEEN 2 AND 1000);

CREATE TABLE "horseQualities"(
    id SERIAL PRIMARY KEY,
    name VARCHAR(250) NOT NULL UNIQUE,
    description VARCHAR(1000) NOT NULL
);
ALTER TABLE reference."horseQualities" ADD CONSTRAINT chk_horse_qualities_name_length CHECK (LENGTH("name") BETWEEN 2 AND 250);
ALTER TABLE reference."horseQualities" ADD CONSTRAINT chk_horse_qualities_description_length CHECK (LENGTH("description") BETWEEN 2 AND 1000);

CREATE TABLE "serviceTypes"(
    id SERIAL PRIMARY KEY,
    name VARCHAR(250) NOT NULL UNIQUE,
    description VARCHAR(1000) NOT NULL
);
ALTER TABLE reference."serviceTypes" ADD CONSTRAINT chk_service_types_name_length CHECK (LENGTH("name") BETWEEN 2 AND 250);
ALTER TABLE reference."serviceTypes" ADD CONSTRAINT chk_service_types_description_length CHECK (LENGTH("description") BETWEEN 2 AND 1000);

CREATE TABLE reference."workerChores"(
    id SERIAL PRIMARY KEY,
    name VARCHAR(250) NOT NULL UNIQUE
);
ALTER TABLE reference."workerChores" ADD CONSTRAINT chk_worker_chores_name_length CHECK (LENGTH("name") BETWEEN 2 AND 250);





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
ALTER TABLE main."userData" ADD COLUMN "isPrivate" BOOLEAN DEFAULT TRUE;

INSERT INTO main."userData" (name, email, password) VALUES ('User2', 'user2@gmail.com', 'haslohaslo');
SELECT * FROM main."userData";

ALTER TABLE main."userData" ADD CONSTRAINT chk_name_length CHECK (LENGTH(name) BETWEEN 2 AND 50);
ALTER TABLE main."userData" ADD CONSTRAINT chk_email_length CHECK (LENGTH(email) BETWEEN 5 AND 255);
ALTER TABLE main."userData" ADD CONSTRAINT chk_password_length CHECK (LENGTH(password) BETWEEN 4 AND 60);
ALTER TABLE main."userData" ADD CONSTRAINT chk_birth_date CHECK ("birthDate" IS NULL OR "birthDate" <= CURRENT_DATE);
ALTER TABLE main."userData" ADD CONSTRAINT chk_profile_photo_length CHECK ("profilePhoto" IS NULL OR LENGTH("profilePhoto") BETWEEN 2 AND 1000);

CREATE VIEW main."publicUsers" AS
SELECT id, name, email, "birthDate", "profilePhoto"
FROM main."userData"
WHERE "isPrivate" = FALSE;



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
ALTER TABLE main.horse ADD COLUMN "isPrivate" BOOLEAN DEFAULT TRUE;
ALTER TABLE main.horse ADD COLUMN "photo" VARCHAR(1000);
ALTER TABLE main.horse ADD COLUMN "height" DOUBLE PRECISION;
ALTER TABLE main.horse ADD COLUMN "feedingSchedule" JSONB;

INSERT INTO main.horse (name, "userId") VALUES ('Blobiczek', 36);
INSERT INTO main.horse (name, "userId") VALUES ('Juglaś', 36);

ALTER TABLE main.horse ADD CONSTRAINT chk_name_length CHECK (LENGTH(name) BETWEEN 2 AND 50);
ALTER TABLE main.horse ADD CONSTRAINT chk_breed_length CHECK (breed IS NULL OR LENGTH(breed) BETWEEN 2 AND 100);
ALTER TABLE main.horse ADD CONSTRAINT chk_birth_date CHECK ("birthDate" IS NULL OR "birthDate" <= CURRENT_DATE);
ALTER TABLE main.horse ADD CONSTRAINT chk_photo_length CHECK (photo IS NULL OR LENGTH(photo) BETWEEN 2 AND 1000);
ALTER TABLE main.horse ADD CONSTRAINT check_height_value CHECK (height IS NULL OR height > 0);

CREATE VIEW main."publicHorses" AS
SELECT id, name, "userId", "centreId", "birthDate", "breed", "photo", "height"
FROM main.horse
WHERE "isPrivate" = FALSE;



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
ALTER TABLE main."equestrianCentre" ADD CONSTRAINT chk_address_length CHECK (address IS NULL OR LENGTH(address) BETWEEN 2 AND 250);
ALTER TABLE main."equestrianCentre" ADD CONSTRAINT unique_latitude_longitude UNIQUE (latitude, longitude);

CREATE OR REPLACE FUNCTION main.get_horse_counts_by_age_group(centreId INTEGER)
RETURNS TABLE(age_group VARCHAR, horse_count INTEGER) AS $$
BEGIN
    RETURN QUERY
    SELECT
        CAST(
            CASE
                WHEN EXTRACT(YEAR FROM AGE(horse."birthDate")) <= 3 THEN '0_3'
                WHEN EXTRACT(YEAR FROM AGE(horse."birthDate")) <= 10 THEN '3_10'
                WHEN EXTRACT(YEAR FROM AGE(horse."birthDate")) <= 19 THEN '10_19'
                ELSE '19_'
            END AS VARCHAR
        ) AS age_group,
        COUNT(*)::INTEGER AS horse_count
    FROM main.horse AS horse
    WHERE horse."centreId" = centreId
      AND horse."birthDate" IS NOT NULL
    GROUP BY age_group
    ORDER BY age_group;
END;
$$ LANGUAGE plpgsql;

SELECT * FROM main.get_horse_counts_by_age_group(5);

CREATE OR REPLACE FUNCTION main.get_horses_from_centre(centre_id INTEGER)
RETURNS TABLE("id" INTEGER, "name" VARCHAR, "userId" INTEGER, "centreId" INTEGER, "birthDate" DATE, "breed" VARCHAR, "isPrivate" BOOLEAN, "photo" VARCHAR, "height" DOUBLE PRECISION, "feedingSchedule" JSONB) AS $$
BEGIN
    RETURN QUERY
    SELECT * FROM main.horse AS h
    WHERE h."centreId" = centre_id;
END;
$$ LANGUAGE plpgsql;

SELECT * FROM main.get_horses_from_centre(5);

CREATE OR REPLACE FUNCTION main.get_horse_count_by_breed(centreId INTEGER)
RETURNS TABLE(
    breed VARCHAR,
    horse_count INTEGER
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        COALESCE(horse.breed, 'unknown') AS breed,
        COUNT(*)::INTEGER AS horse_count
    FROM main.horse AS horse
    WHERE horse."centreId" = centreId
    GROUP BY horse.breed
    ORDER BY horse_count DESC;
END;
$$ LANGUAGE plpgsql;

SELECT * FROM main.get_horse_count_by_breed(5);

CREATE OR REPLACE FUNCTION main.get_public_horses_from_centre(centre_id INTEGER)
RETURNS TABLE("id" INTEGER, "name" VARCHAR, "userId" INTEGER, "birthDate" DATE, "breed" VARCHAR, "photo" VARCHAR, "height" DOUBLE PRECISION) AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM main."publicHorses" as ph
    WHERE ph."id" IN (
        SELECT h.id
        FROM main.horse AS h
        WHERE h."centreId" = centre_id
    );
END;
$$ LANGUAGE plpgsql;

SELECT * FROM main.get_public_horses_from_centre(5);



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
CREATE TABLE main."workerChoresMapping"(
    "workerId" INTEGER NOT NULL,
    "choreId" INTEGER NOT NULL,
    PRIMARY KEY ("workerId", "choreId"),
    FOREIGN KEY ("choreId") REFERENCES reference."workerChores"(id) ON DELETE CASCADE,
    FOREIGN KEY ("workerId") REFERENCES worker (id) ON DELETE CASCADE
);



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

ALTER TABLE main."trade" ADD CONSTRAINT chk_location_length CHECK (LENGTH(location) BETWEEN 2 AND 250);
ALTER TABLE main."trade" ADD CONSTRAINT chk_description_length CHECK (LENGTH(description) BETWEEN 5 AND 1000);

INSERT INTO main.trade ("horseId", price, location, description, "contactInformation") VALUES(
    13,
    2500.00,
    'Sunrise Equestrian Centre, Springfield',
    'A beautiful 5-year-old stallion trained in show jumping.',
    '{"phone": "555-123-4567", "email": "owner@example.com", "socialMediaAccount": "@sunrise_horses"}'::JSONB
);
INSERT INTO main.trade ("horseId", price, location, description, "contactInformation") VALUES(
    15,
    3200.00,
    'Meadowbrook Stables, Maplewood',
    'A gentle mare with excellent pedigree and a sweet temperament.',
    '{"phone": "555-987-6543", "email": "seller@example.com", "socialMediaAccount": "@meadowbrook_horses"}'::JSONB
);

CREATE VIEW main."horsesForSaleInfo" AS
SELECT trade.id, horse.id AS "horseId", price, location, description, "contactInformation", "createdAt", "updatedAt", name, "userId", "birthDate", "breed", "photo", "height"
FROM main.trade, main.horse
WHERE trade."horseId" = horse.id;

CREATE OR REPLACE FUNCTION main.get_horses_for_sale_by_quality(quality_id INTEGER)
RETURNS SETOF main."horsesForSaleInfo" AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM main."horsesForSaleInfo" AS hfsi
    INNER JOIN main."horseQualitiesMapping" AS hqm
    ON hfsi.id = hqm."tradeId"
    WHERE hqm."qualityId" = quality_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION main.get_horse_qualities(horse_id INTEGER)
RETURNS TABLE(quality_name VARCHAR, quality_description VARCHAR) AS $$
BEGIN
    RETURN QUERY
    SELECT hq.name, hq.description
    FROM reference."horseQualities" AS hq
    INNER JOIN main."horseQualitiesMapping" AS hqm
    ON hq.id = hqm."qualityId"
    INNER JOIN main.horse AS h
    ON hqm."tradeId" = h.id
    WHERE h.id = horse_id;
END;
$$ LANGUAGE plpgsql;



CREATE TABLE main."centreCreateRequest"(
    id SERIAL PRIMARY KEY,
    "createdAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "updatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
ALTER TABLE main."centreCreateRequest" ADD COLUMN "status" INTEGER NOT NULL;
ALTER TABLE main."centreCreateRequest" ADD CONSTRAINT chk_request_status_range CHECK (status BETWEEN 0 AND 3);
ALTER TABLE main."centreCreateRequest" ADD COLUMN "centreId" INTEGER NOT NULL,
ADD CONSTRAINT fk_centre FOREIGN KEY ("centreId") REFERENCES "equestrianCentre"("id") ON DELETE CASCADE;

CREATE OR REPLACE FUNCTION main.get_user_equestrian_centre_requests(user_id INTEGER)
RETURNS SETOF main."centreCreateRequest"  AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM main."centreCreateRequest" r
    INNER JOIN main."equestrianCentre" c ON r."centreId" = c.id
    WHERE c."userId" = user_id;
END;
$$ LANGUAGE plpgsql;



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
ALTER TABLE main."serviceProvider" ADD CONSTRAINT chk_location_length CHECK (LENGTH(location) BETWEEN 2 AND 250);
ALTER TABLE main."serviceProvider" ADD CONSTRAINT chk_description_length CHECK (LENGTH(description) BETWEEN 5 AND 1000);

CREATE OR REPLACE FUNCTION main.get_service_providers_by_type(service_type_id INTEGER)
RETURNS TABLE(id INTEGER, "userId" INTEGER, description VARCHAR(1000), "contactInformation" JSONB, location VARCHAR(250), "createAt" TIMESTAMP, "updatedAt" TIMESTAMP) AS $$
BEGIN
    RETURN QUERY
    SELECT sp.id, sp."userId",  sp.description, sp."contactInformation", sp.location, sp."createAt", sp."updatedAt"
    FROM main."serviceProvider" sp
    WHERE sp."typeId" = service_type_id;
END;
$$ LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION main.get_services_for_centre(centre_id INTEGER)
RETURNS TABLE(service_name VARCHAR, service_description VARCHAR) AS $$
BEGIN
    RETURN QUERY
    SELECT
        se.name AS service_name,
        se.description AS service_description
    FROM main."centreServicesMapping" AS csm
    INNER JOIN reference."centreServices" AS se
        ON csm."serviceId" = se.id
    WHERE csm."centreId" = centre_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION main.get_centres_by_service(service_id INTEGER)
RETURNS SETOF main."equestrianCentre" AS $$
BEGIN
    RETURN QUERY
    SELECT *
    FROM main."equestrianCentre" AS ec
    INNER JOIN main."centreServicesMapping" AS csm
        ON csm."centreId" = ec.id
    WHERE csm."serviceId" = service_id;
END;
$$ LANGUAGE plpgsql;

CREATE VIEW main."approvedEquestrianCentres" AS
SELECT id, name, "userId", latitude, longitude, address, "openHours", "contactInformation"
FROM main."equestrianCentre"
WHERE approved = TRUE;




CREATE OR REPLACE FUNCTION main.remove_page_views_on_private_horse_change()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW."isPrivate" = true AND OLD."isPrivate" = false THEN
        DELETE FROM analytics."pagesViews"
        WHERE "pageType" = 'horse' AND "pageId" = OLD.id;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_remove_page_views_on_private_horse_change
AFTER UPDATE OF "isPrivate" ON main.horse
FOR EACH ROW
EXECUTE FUNCTION main.remove_page_views_on_private_horse_change();

CREATE OR REPLACE FUNCTION main.remove_page_views_on_unapproved_centre_change()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW."approved" = false AND OLD."approved" = true THEN
        DELETE FROM analytics."pagesViews"
        WHERE "pageType" = 'equestrianCentre' AND "pageId" = OLD.id;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_remove_page_views_on_unapproved_centre_change
AFTER UPDATE OF "approved" ON main."equestrianCentre"
FOR EACH ROW
EXECUTE FUNCTION main.remove_page_views_on_unapproved_centre_change();



CREATE OR REPLACE FUNCTION main.remove_horses_centre_on_unapproved_centre_change()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW."approved" = false AND OLD."approved" = true THEN
        UPDATE main.horse
        SET "centreId" = NULL
        WHERE "centreId" = OLD.id;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_remove_horses_centre_on_unapproved_centre_change
AFTER UPDATE OF "approved" ON main."equestrianCentre"
FOR EACH ROW
EXECUTE FUNCTION main.remove_horses_centre_on_unapproved_centre_change();



-- 100 rows of fake data - analytics."pagesViews"
DO $$
BEGIN
    FOR i IN 1..100 LOOP
        INSERT INTO analytics."pagesViews" ("userId", "pageId", "pageType", timestamp, "ipAddress")
        VALUES (
            (SELECT id FROM main."userData" ORDER BY RANDOM() LIMIT 1),
            CASE WHEN RANDOM() < 0.5 THEN 5 ELSE 8 END,
            'equestrianCentre',
            NOW() - (RANDOM() * INTERVAL '30 days'),
            FORMAT('%s.%s.%s.%s',
                FLOOR(RANDOM() * 256),
                FLOOR(RANDOM() * 256),
                FLOOR(RANDOM() * 256),
                FLOOR(RANDOM() * 256)
            )
        );
    END LOOP;
END $$;
























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
ALTER TABLE main."userBadge" ADD CONSTRAINT "userBadge_approvedBy_fkey" FOREIGN KEY ("approvedBy") REFERENCES main."worker"(id) ON DELETE SET NULL;
ALTER TABLE main."userBadge" ADD CONSTRAINT "userBadge_centreId_fkey" FOREIGN KEY ("centreId") REFERENCES main."equestrianCentre"(id) ON DELETE SET NULL;

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