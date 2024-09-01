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
COMMIT;
