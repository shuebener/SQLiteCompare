%namespace SQLiteParser

blob [xX]\'([0-9a-fA-F][0-9a-fA-F])*\'
variable (\?[0-9]+)|(\?)|([\:\@][a-zA-Z_][a-zA-Z_0-9\$]*)|(\$([a-zA-Z0-9_:]+(?:\([^\)]*\))?)|\$[a-zA-Z0-9_:]+(?:\([^\)]*\))?)
dquoted \"([^\"]|(\"\"))*\"
bquoted \[([^\]]|(\\\]))*\]
squoted \'([^\']|(\'\'))*\'
tquoted \`([^\`]|(\`\`))*\`
id [a-zA-Z_][a-zA-Z_0-9\$]*
number [0-9]*\.?[0-9]+([eE][\-\+]?[0-9]+)?
%%

{blob} yylval.Blob = SQLiteTerm.CreateBlob(yytext); return (int)Tokens.BLOB;
{dquoted} yylval.Text = yytext; return (int)Tokens.DQUOTED;
{bquoted} yylval.Text = yytext; return (int)Tokens.BQUOTED;
{squoted} yylval.Text = yytext; return (int)Tokens.SQUOTED;
{tquoted} yylval.Text = yytext; return (int)Tokens.TQUOTED;
{number} yylval.Number = double.Parse(yytext); return (int)Tokens.NUMBER;
{id} yylval.Text = yytext; return ParseId(yytext);
";" return (int)Tokens.SEMI;
"(" return (int)Tokens.LP;
")" return (int)Tokens.RP;
"," return (int)Tokens.COMMA;
"!="|"<>" return (int)Tokens.NE;
"=="|"=" return (int)Tokens.EQ;
"<<" return (int)Tokens.LSHIFT;
">>" return (int)Tokens.RSHIFT;
"<=" return (int)Tokens.LE;
">=" return (int)Tokens.GE;
">" return (int)Tokens.GT;
"<" return (int)Tokens.LT;
"&" return (int)Tokens.BITAND;
"||" return (int)Tokens.CONCAT;
"|" return (int)Tokens.BITOR;
"+" return (int)Tokens.PLUS;
"-" return (int)Tokens.MINUS;
"*" return (int)Tokens.STAR;
"/" return (int)Tokens.SLASH;
"%" return (int)Tokens.REM;
"~" return (int)Tokens.BITNOT;
"." return (int)Tokens.DOT;
{variable} yylval.Text = yytext; return (int)Tokens.VARIABLE;
%%

public int ParseId(string val)
{
	string uval = val.ToUpper();
	switch(uval)
	{
		case "CREATE":
			return (int)Tokens.CREATE;
		case "UNIQUE":
			return (int)Tokens.UNIQUE;
		case "INDEX":
			return (int)Tokens.INDEX;
		case "IF":
			return (int)Tokens.IF;
		case "NOT":
			return (int)Tokens.NOT;
		case "EXISTS":
			return (int)Tokens.EXISTS;
		case "ON":
			return (int)Tokens.ON;
		case "COLLATE":
			return (int)Tokens.COLLATE;
		case "ASC":
			return (int)Tokens.ASC;
		case "DESC":
			return (int)Tokens.DESC;
		case "TABLE":
			return (int)Tokens.TABLE;
		case "PRIMARY":
			return (int)Tokens.PRIMARY;
		case "KEY":
			return (int)Tokens.KEY;
		case "CONFLICT":
			return (int)Tokens.CONFLICT;
		case "AUTOINCREMENT":
			return (int)Tokens.AUTOINCR;
		case "CONSTRAINT":
			return (int)Tokens.CONSTRAINT;
		case "ROLLBACK":
			return (int)Tokens.ROLLBACK;
		case "ABORT":
			return (int)Tokens.ABORT;
		case "FAIL":
			return (int)Tokens.FAIL;
		case "IGNORE":
			return (int)Tokens.IGNORE;
		case "REPLACE":
			return (int)Tokens.REPLACE;
		case "AS":
			return (int)Tokens.AS;
		case "LIKE":
			yylval.Like = SQLiteLike.Like;
			return (int)Tokens.LIKE_KW;
		case "GLOB":
			yylval.Like = SQLiteLike.Glob;
			return (int)Tokens.LIKE_KW;
		case "REGEXP":
			yylval.Like = SQLiteLike.Regexp;
			return (int)Tokens.LIKE_KW;
		case "ISNULL":
			return (int)Tokens.ISNULL;
		case "NOTNULL":
			return (int)Tokens.NOTNULL;
		case "BETWEEN":
			return (int)Tokens.BETWEEN;
		case "MATCH":
			return (int)Tokens.MATCH;
		case "IS":
			return (int)Tokens.IS;
		case "ESCAPE":
			return (int)Tokens.ESCAPE;
		case "IN":
			return (int)Tokens.IN;
		case "NATURAL":
			yylval.JoinOperator = SQLiteJoinOperator.Natural;
			return (int)Tokens.JOIN_KW;
		case "OUTER":
			yylval.JoinOperator = SQLiteJoinOperator.Outer;
			return (int)Tokens.JOIN_KW;
		case "CROSS":
			yylval.JoinOperator = SQLiteJoinOperator.Cross;
			return (int)Tokens.JOIN_KW;
		case "INNER":
			yylval.JoinOperator = SQLiteJoinOperator.Inner;
			return (int)Tokens.JOIN_KW;		
		case "LEFT":
			yylval.JoinOperator = SQLiteJoinOperator.Left;
			return (int)Tokens.JOIN_KW;
		case "DEFAULT":
			return (int)Tokens.DEFAULT;
		case "NULL":
			return (int)Tokens.NULL;
		case "REFERENCES":
			return (int)Tokens.REFERENCES;
		case "DEFERRABLE":
			return (int)Tokens.DEFERRABLE;
		case "CASCADE":
			return (int)Tokens.CASCADE;
		case "RESTRICT":
			return (int)Tokens.RESTRICT;
		case "INITIALLY":
			return (int)Tokens.INITIALLY;
		case "DEFERRED":
			return (int)Tokens.DEFERRED;
		case "IMMEDIATE":
			return (int)Tokens.IMMEDIATE;
		case "FOREIGN":
			return (int)Tokens.FOREIGN;
		case "OR":
			return (int)Tokens.OR;		
		case "CAST":
			return (int)Tokens.CAST;	
		case "CASE":
			return (int)Tokens.CASE;
		case "RAISE":
			return (int)Tokens.RAISE;			
		case "BEGIN":
			return (int)Tokens.BEGIN;
		case "END":
			return (int)Tokens.END;
		case "WHEN":
			return (int)Tokens.WHEN;
		case "THEN":
			return (int)Tokens.THEN;			
		case "TRIGGER":
			return (int)Tokens.TRIGGER;
		case "BEFORE":
			return (int)Tokens.BEFORE;
		case "AFTER":
			return (int)Tokens.AFTER;
		case "INSTEAD":
			return (int)Tokens.INSTEAD;
		case "OF":
			return (int)Tokens.OF;
		case "LIMIT":
			return (int)Tokens.LIMIT;
		case "OFFSET":
			return (int)Tokens.OFFSET;
		case "DELETE":
			return (int)Tokens.DELETE;
		case "INSERT":
			return (int)Tokens.INSERT;
		case "UPDATE":
			return (int)Tokens.UPDATE;
		case "FOR":
			return (int)Tokens.FOR;
		case "EACH":
			return (int)Tokens.EACH;
		case "ROW":
			return (int)Tokens.ROW;									
		case "ELSE":
			return (int)Tokens.ELSE;
		case "SET":
			return (int)Tokens.SET;
		case "VALUES":
			return (int)Tokens.VALUES;
		case "INTO":
			return (int)Tokens.INTO;
		case "FROM":
			return (int)Tokens.FROM;
		case "UNION":
			return (int)Tokens.UNION;
		case "ALL":
			return (int)Tokens.ALL;
		case "EXCEPT":
			return (int)Tokens.EXCEPT;
		case "INTERSECT":
			return (int)Tokens.INTERSECT;
		case "SELECT":
			return (int)Tokens.SELECT;
		case "DISTINCT":
			return (int)Tokens.DISTINCT;
		case "JOIN":
			return (int)Tokens.JOIN;
		case "INDEXED":
			return (int)Tokens.INDEXED;
		case "BY":
			return (int)Tokens.BY;
		case "USING":
			return (int)Tokens.USING;
		case "GROUP":
			return (int)Tokens.GROUP;
		case "ORDER":
			return (int)Tokens.ORDER;
		case "HAVING":
			return (int)Tokens.HAVING;
		case "CURRENT_TIME":
			yylval.TimeFunction = SQLiteTimeFunction.CurrentTime;
			return (int)Tokens.CTIME_KW;
		case "CURRENT_DATE":
			yylval.TimeFunction = SQLiteTimeFunction.CurrentDate;
			return (int)Tokens.CTIME_KW;
		case "CURRENT_TIMESTAMP":
			yylval.TimeFunction = SQLiteTimeFunction.CurrentTimestamp;
			return (int)Tokens.CTIME_KW;
		case "CHECK":
			return (int)Tokens.CHECK;
		case "VIEW":
			return (int)Tokens.VIEW;
		case "WHERE":
			return (int)Tokens.WHERE;
		case "TEMP":
			return (int)Tokens.TEMP;
		case "AND":
			return (int)Tokens.AND;
		case "TEMPORARY":
			return (int)Tokens.TEMPORARY;

		default:
			return (int)Tokens.ID;
	} // switch
}

