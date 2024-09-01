BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "Sentence" (
	"OID"	TEXT NOT NULL UNIQUE,
	"Author"	TEXT NOT NULL,
	"Date"	TEXT NOT NULL,
	"Text"	TEXT NOT NULL,
	PRIMARY KEY("OID")
);
CREATE TABLE IF NOT EXISTS "WordStatistic" (
	"Name"	TEXT NOT NULL UNIQUE,
	"Appearances"	INTEGER NOT NULL,
	PRIMARY KEY("Name")
);
CREATE TABLE IF NOT EXISTS "UserPermission" (
	"Author"	TEXT NOT NULL,
	"OriginScope"	TEXT,
	"AllowedScope"	TEXT,
	PRIMARY KEY("Author","OriginScope")
);
CREATE VIEW SentenceAfterLinkWithPermission As
SELECT s.OID, s.Author, s.Date, s.Text, up.AllowedScope
FROM Sentence s
INNER JOIN UserPermission up
ON s.Author = up.Author
AND (
    up.OriginScope IS NULL 
    OR INSTR(s.OID, up.OriginScope) = 1
)
WHERE LENGTH(COALESCE(up.OriginScope, '')) = (
    SELECT MAX(LENGTH(COALESCE(up2.OriginScope, '')))
    FROM UserPermission up2
    WHERE s.Author = up2.Author
    AND (
        up2.OriginScope IS NULL 
        OR INSTR(s.OID, up2.OriginScope) = 1
    )
);
COMMIT;
