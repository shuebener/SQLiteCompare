SQLite Compare V2.5
---------------------

SQLite Compare is a database comparison and merge utility for Windows 2000/XP/2003


Features: 

  - Blazing fast in-memory schema comparisons.

  - Optimized data comparison engine capable of 
    comparing even huge tables with millions of rows.

  - WinMerge-like schema difference view and operations.

  - SQL change script generation - useful for supporting 
    database upgrades in existing installations (available 
    only with a commercial license).

  - Intuitive data difference view for comparing individual field values.

  - Open Source and Free !!


Known limitations:

1. SQLite Compare supports only the newer SQLite 3 file format. SQLite version 2 files are incompatible with the newer file format and are therefore not supported. If you really want to compare older SQLite file then you can convert them to the newer file format by following method outlined in http://www.sqlite.org/formatchng.html

2. Currently it is not impossible to rename a column in the schema diff view. This is a limitation caused by the inability to determine if a new column was added or an existing column was renamed. The workaround is to do such renames in another SQL management tool and than run the comparison if necessary.

3. BLOB field comparisons tend to take more time to execute, especially when 
big BLOBs are involved. When BLOB fields are not part of a table primary key - 
they are compared using a fast algorithm that does not load the entire BLOB into memory before doing the comparison. However - when BLOB fields are part of a table primary key (should be rare but possible), there is no way to avoid loading them into main memory before doing comparisons (SQLite limitation).

4. Currently it is not possible to compare tables that have mismatching primary keys. For example - it is not possible to compare two tables where table A has a primary key composed from two integers while table B has a primary key composed of a single integer.


Enjoy :-)
Liron Levi
