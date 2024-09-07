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
CREATE TABLE IF NOT EXISTS "AuthorPermission" (
	"Author"	TEXT NOT NULL,
	"QueryScope"	TEXT,
	"AllowedScope"	TEXT,
	PRIMARY KEY("Author","QueryScope")
);
CREATE VIEW SentenceAfterLinkWithPermission As
SELECT s.OID, s.Author, s.Date, s.Text, up.AllowedScope
FROM Sentence s
INNER JOIN AuthorPermission up
ON s.Author = up.Author
AND (
    up.QueryScope IS NULL 
    OR INSTR(s.OID, up.QueryScope) = 1
)
WHERE LENGTH(COALESCE(up.QueryScope, '')) = (
    SELECT MAX(LENGTH(COALESCE(up2.QueryScope, '')))
    FROM AuthorPermission up2
    WHERE s.Author = up2.Author
    AND (
        up2.QueryScope IS NULL 
        OR INSTR(s.OID, up2.QueryScope) = 1
    )
);
COMMIT;
