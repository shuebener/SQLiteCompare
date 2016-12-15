create table [$test12\ 2]
(
	key integer  NOT NULL PRIMARY KEY ASC on conflict ignore AUTOINCREMENT,
	firstname text constraint "check1" CHECK (firstname LIKE 'mr.%'),
	birthdate datetime NOT NULL constraint "default" DEFAULT (CURRENT_DATE),
	`my %name` NULL COLLATE nocase DEFAULT NULL,
	balance float(50,3) DEFAULT 0 NOT NULL constraint "check balance" CHECK (balance >= 0.0),
	deathdate datetime NULL on conflict rollback DEFAULT NULL,
	picture blob NULL on conflict abort DEFAULT X'123456',
	constraint fk1 FOREIGN KEY (key) REFERENCES "test1" (tid) ON DELETE CASCADE ON UPDATE CASCADE ON INSERT CASCADE NOT DEFERRABLE INITIALLY IMMEDIATE
);

create table test1
(
	tid bigint NOT NULL,
	name varchar(100) NOT NULL DEFAULT '' COLLATE rtrim,
	double double NOT NULL DEFAULT 0.3 CHECK (double > 0.1),
	"null" null default null CHECK ("null" = null),
	[primary key] int8 NOT NULL PRIMARY KEY ON CONFLICT ABORT,
	"index" integer not null default 0
);

create view "test view1" as select name,double,[primary key],"null",key,firstname from [$test12\ 2],test1 where key = tid
create view `test view2` as select picture,deathdate,double,[index] from [$test12\ 2],test1 where firstname = name

create trigger "trigger" before delete on test1 
begin 
	update test1 set "null" = 'deleted' where tid = old.tid; 
	insert into test1 (1,'liron',4.55555,NULL,44,2);
end

CREATE TRIGGER [primary] UPDATE OF double,[null]
ON test1
FOR EACH ROW
WHEN double = 5
BEGIN
    INSERT INTO [$test12\ 2]
        VALUES (1,'liron',CURRENT_DATE,4,45.5,NULL,X'1212');
    DELETE FROM test1
        WHERE tid = old.tid;
    SELECT test1.tid+3*(4.3/2+test1.double) AS [89],(([primary key] + 1) * 2.3) AS pkey
        FROM  test1 ,  [$test12\ 2] AS [2t]
        WHERE tid = [2t].[key];
    UPDATE test1
        SET [null] = NULL
        WHERE tid = old.tid;
END

create trigger test_select update of [null]
ON test1
FOR EACH ROW
WHEN double = 10 AND tid < 10000 AND "null" = null
BEGIN
	SELECT t1.tid*100 as MyTID, (t2.balance-10+t1."index")*t1.[primary key] as MyKey
		FROM test1 as t1, [$test12\ 2] as t2
		WHERE t1.double+5.3<(t2.balance-3.8)*t2.key+t1."index" AND t1.[primary key]*7 BETWEEN t2.key AND t2.key*1000
		ORDER BY t2.name ASC, t1.tid DESC;
	SELECT DISTINCT MAX(t1.tid)
		FROM test1 as t1
		GROUP BY name COLLATE RTRIM, "index"
		HAVING [primary key] BETWEEN 5 AND 10000
		LIMIT 1000 OFFSET 0
	UNION ALL
    SELECT CAST(COUNT(*) AS integer) AS ID
        FROM test1
        WHERE tid > 10000 AND "index" < 500 AND "null" <> X'121212' AND double <> round(double);
END

create trigger test_insert update of [null]
ON test1
BEGIN
	INSERT INTO test1
		SELECT key as tid, firstname as name, balance as double, picture as "null", `my %name` as [primary key], 4 as "index"
		FROM [$test12\ 2] as t2
		WHERE t2.key > old.tid
		ORDER BY key ASC;
	INSERT OR REPLACE INTO test1 (double) 
		VALUES ((SELECT COUNT(*) FROM test2 WHERE balance < 1000));
	REPLACE INTO test2 (firstname) 
        SELECT MAX(name) FROM test1
        WHERE tid = old.tid;        
    INSERT OR ROLLBACK INTO test1 VALUES (1,'liron',-5.3E-5, NULL, 34, -111);
END

create trigger empty_trigger update of [null]
ON test1
BEGIN
END
