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
CREATE TABLE IF NOT EXISTS "Author" (
	"ID"	TEXT NOT NULL UNIQUE,
	"Name"	TEXT NOT NULL,
	PRIMARY KEY("ID")
);
CREATE TABLE IF NOT EXISTS "Location" (
	"ID"	TEXT NOT NULL UNIQUE,
	"Name"	TEXT NOT NULL,
	PRIMARY KEY("ID")
);
CREATE TABLE IF NOT EXISTS "AuthorPermission" (
	"Author"	TEXT NOT NULL,
	"QueryScope"	TEXT,
	"AllowedScope"	TEXT,
	PRIMARY KEY("Author","QueryScope")
);
CREATE VIEW IF NOT EXISTS SentenceAfterLinkWithPermission As
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
CREATE TABLE IF NOT EXISTS "AuthorGroup" (
	"ID"	TEXT NOT NULL UNIQUE,
	"Name"	TEXT NOT NULL,
	PRIMARY KEY("ID")
);
CREATE TABLE IF NOT EXISTS "AuthorGroupPermission" (
	"ID"	TEXT NOT NULL,
	"Author"	TEXT NOT NULL,
	"Permissions"	TEXT,
	FOREIGN KEY("ID") REFERENCES "AuthorGroup"("ID") ON DELETE CASCADE,
	PRIMARY KEY("ID","Author")
);
CREATE TABLE IF NOT EXISTS "AuthorGroupRequest" (
	"ID"	TEXT NOT NULL,
	"Author"	TEXT NOT NULL,
	"Permissions"	TEXT,
	FOREIGN KEY("ID") REFERENCES "AuthorGroup"("ID") ON DELETE CASCADE,
	PRIMARY KEY("ID","Author")
);
CREATE TABLE IF NOT EXISTS "AuthorSetting" (
	"Author" TEXT NOT NULL,
	"Location" TEXT NOT NULL,
	"DisplayOption" TEXT NOT NULL,
	"FilterOIDs" TEXT NOT NULL,
	"FilterAuthors" TEXT NOT NULL,
	"AuthorGroup" TEXT,
	"LocationGroup" TEXT,
	"Keyword" TEXT,
	"FirstWord" TEXT,
	PRIMARY KEY("Author","Location")
);
COMMIT;
