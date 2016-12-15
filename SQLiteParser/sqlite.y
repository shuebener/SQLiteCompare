
%namespace SQLiteParser

%union { 	
    public SQLiteDdlStatement			DdlStatement;
    public SQLiteCreateTableStatement	CreateTableStatement;
    public SQLiteCreateViewStatement	CreateViewStatement;
    public SQLiteCreateTriggerStatement	CreateTriggerStatement;
    public SQLiteCreateIndexStatement	CreateIndexStatement;
    
    public List<SQLiteColumnStatement>	ColumnsList;
    public List<SQLiteTableConstraint>  TableConstraintsList;
    public SQLiteColumnStatement		ColumnStatement;
    public SQLiteColumnType				ColumnType;
    public SQLiteColumnConstraint		ColumnConstraint;
    public List<SQLiteColumnConstraint> ColumnConstraintsList;
    public SQLiteTerm					Term;
    public SQLiteExpression				Expression;
    public SQLiteSortOrder				SortOrder;
    public List<SQLiteIndexedColumn>	IndexedColumnsList;
    public SQLiteReferenceAction		ReferenceAction;
    public SQLiteReferenceHandler		ReferenceHandler;
    public List<SQLiteReferenceHandler>	ReferenceHandlersList;
    public SQLiteDeferType				DeferType;
    public SQLiteDeferColumnConstraint	DeferColumnConstraint;
    public SQLiteTableConstraint		TableConstraint;
    public SQLiteResolveAction			ResolveAction;
    public SQLiteTimeFunction			TimeFunction;
    public SQLiteLikeOperator			LikeOperator;
    public SQLiteLike					Like;
    public List<SQLiteExpression>		ExpressionsList;
    public SQLiteDistinct				Distinct;
    public List<SQLiteSelectColumn>		SelectColumnsList;
    public SQLiteSelectStatement		SelectStatement;
    public SQLiteJoinOperator			JoinOperator;
    public SQLiteFromClause				FromClause;
    public SQLiteFromIndexed			FromIndexed;
    public SQLiteObjectName				ObjectName;
    public List<string>					TextList;
    public SQLiteFromInternalTable		FromInternalTable;
    public SQLiteSortItem				SortItem;
    public List<SQLiteSortItem>			SortItemsList;
    public SQLiteLimitClause			LimitClause;
    public SQLiteSingleSelectStatement	SingleSelectStatement;
    public SQLiteSelectOperator			SelectOperator;
    public List<SQLiteCaseItem>			CaseItemsList;
    public SQLiteTriggerTime			TriggerTime;
    public SQLiteTriggerEventClause		TriggerEventClause;
    public SQLiteStatement				Statement;
    public List<SQLiteStatement>		StatementsList;
    public List<SQLiteUpdateItem>		UpdateItemsList;
    public SQLiteInsertPrefix			InsertPrefix;
        
	public string						Text;
	public bool							Bool;	
	public double						Number;
	public byte[]						Blob;
}

%start main

%token AS
%token <Number> NUMBER
%token LP
%token RP
%token CREATE
%token UNIQUE
%token INDEX
%token IF
%token NOT
%token EXISTS
%token ON
%token COLLATE
%token ASC
%token DESC
%token COMMA
%token <Text> ID
%token <Text> DQUOTED
%token <Text> SQUOTED
%token <Text> BQUOTED
%token <Text> TQUOTED
%token TABLE
%token PRIMARY
%token KEY
%token CONFLICT
%token AUTOINCR
%token CONSTRAINT
%token ROLLBACK
%token ABORT
%token FAIL
%token IGNORE
%token REPLACE
%token <Like> LIKE_KW
%token ISNULL
%token NOTNULL
%token NE
%token EQ
%token BETWEEN
%token MATCH
%token IS
%token GT
%token LE
%token LT
%token GE
%token ESCAPE
%token BITAND
%token BITOR
%token LSHIFT
%token RSHIFT
%token PLUS
%token MINUS
%token STAR
%token SLASH
%token REM
%token CONCAT
%token BITNOT
%token IN
%token <JoinOperator> JOIN_KW
%token DOT
%token DEFAULT
%token NULL
%token REFERENCES
%token DEFERRABLE
%token CASCADE
%token RESTRICT
%token INITIALLY
%token DEFERRED
%token IMMEDIATE
%token FOREIGN
%token OR
%token REGISTER
%token <Text> VARIABLE
%token CAST
%token CASE
%token RAISE
%token BEGIN
%token END
%token WHEN
%token THEN
%token <Blob> BLOB
%token <TimeFunction> CTIME_KW
%token TRIGGER
%token BEFORE
%token AFTER
%token INSTEAD
%token OF
%token LIMIT
%token OFFSET
%token DELETE
%token INSERT
%token UPDATE
%token FOR
%token EACH
%token ROW
%token SEMI
%token ELSE
%token SET
%token VALUES
%token INTO
%token FROM
%token UNION
%token ALL
%token EXCEPT
%token INTERSECT
%token SELECT
%token DISTINCT
%token JOIN
%token INDEXED
%token BY
%token USING
%token ORDER
%token GROUP
%token HAVING
%token CHECK
%token VIEW
%token WHERE
%token TEMP
%token AND
%token TEMPORARY

%type <Text> nm
%type <Text> dbnm
%type <Bool> temp
%type <Bool> ifnotexists
%type <Text> ids
%type <ObjectName> columnid
%type <Text> typename
%type <Number> number
%type <Number> signed
%type <Number> minus_num
%type <Number> plus_num
%type <Bool> autoinc
%type <Bool> in_op
%type <Bool> between_op
%type <Bool> uniqueflag
%type <Bool> foreach_clause
%type <Text> as
%type <Text> id
%type <Text> exid
%type <Text> collate

%type <DdlStatement> main
%type <CreateTableStatement> create_table
%type <CreateIndexStatement> create_index
%type <CreateTriggerStatement> create_trigger
%type <CreateViewStatement> create_view

%type <CreateTriggerStatement> trigger_decl
%type <CreateTableStatement> create_table_args
%type <ColumnsList> columnlist
%type <TableConstraintsList> conslist_opt
%type <TableConstraintsList> conslist
%type <ColumnStatement> column
%type <ColumnType> typetoken
%type <ColumnType> type
%type <ColumnConstraint> carg
%type <ColumnConstraint> ccons
%type <ColumnConstraintsList> carglist
%type <SortOrder> sortorder
%type <IndexedColumnsList> idxlist
%type <IndexedColumnsList> idxlist_opt
%type <ReferenceAction> refact
%type <ReferenceHandler> refarg
%type <ReferenceHandlersList> refargs
%type <DeferType> init_deferred_pred_opt
%type <DeferColumnConstraint> defer_subclause
%type <DeferColumnConstraint> defer_subclause_opt
%type <TableConstraint> tcons
%type <ResolveAction> onconf
%type <ResolveAction> orconf
%type <ResolveAction> raisetype
%type <ResolveAction> resolvetype
%type <LikeOperator> likeop
%type <Expression> escape
%type <ExpressionsList> exprlist
%type <ExpressionsList> nexprlist
%type <Distinct> distinct
%type <SelectColumnsList> selcollist
%type <SelectColumnsList> sclp
%type <SingleSelectStatement> oneselect
%type <SelectStatement> select
%type <JoinOperator> joinop
%type <FromClause> stl_prefix
%type <FromClause> seltablist
%type <FromClause> from
%type <FromIndexed> indexed_opt
%type <ObjectName> fullname
%type <Expression> on_opt
%type <TextList> inscollist
%type <TextList> inscollist_opt
%type <TextList> using_opt
%type <FromInternalTable> seltablist_paren
%type <Expression> where_opt
%type <ExpressionsList> groupby_opt
%type <Expression> having_opt
%type <Expression> sortitem
%type <SortItemsList> sortlist
%type <SortItemsList> orderby_opt
%type <LimitClause> limit_opt
%type <SelectOperator> multiselect_op
%type <Expression> case_operand
%type <CaseItemsList> case_exprlist
%type <Expression> case_else
%type <TriggerTime> trigger_time
%type <TriggerEventClause> trigger_event
%type <Expression> when_clause
%type <Statement> trigger_cmd
%type <StatementsList> trigger_cmd_list
%type <UpdateItemsList> setlist
%type <InsertPrefix> insert_cmd
%type <ExpressionsList> itemlist

%type <Term> term
%type <Expression> expr

%left OR
%left AND
%right NOT
%left IS MATCH LIKE_KW BETWEEN IN ISNULL NOTNULL NE EQ
%left GT LE LT GE
%right ESCAPE
%left BITAND BITOR LSHIFT RSHIFT
%left PLUS MINUS
%left STAR SLASH REM
%left CONCAT
%left COLLATE
%right UMINUS UPLUS BITNOT

%%

main : create_table		{ $$ = $1; SQLiteDdlMain.CreateTable = $1; }
	| create_trigger	{ $$ = $1; SQLiteDdlMain.CreateTrigger = $1; }
	| create_index		{ $$ = $1; SQLiteDdlMain.CreateIndex = $1; }
	| create_view		{ $$ = $1; SQLiteDdlMain.CreateView = $1; }
    ;
     
create_table : CREATE TABLE ifnotexists nm dbnm create_table_args					
{ 
	$$ = $6;
	$$.IfNotExists = $3;
	$$.ObjectName = new SQLiteObjectName($4, $5);
}
	;
	
create_index : CREATE uniqueflag INDEX ifnotexists nm dbnm ON nm LP idxlist RP		
{ 
	SQLiteObjectName idxName = new SQLiteObjectName($5, $6);
	$$ = new SQLiteCreateIndexStatement($2, idxName, $4, $8, $10); 
}
	;	
	
create_trigger : CREATE trigger_decl BEGIN trigger_cmd_list END						{ $$ = $2; $$.StatementsList = $4;}
	;	
	
create_view : CREATE temp VIEW ifnotexists nm dbnm AS select						
{ 
	SQLiteObjectName viewName = new SQLiteObjectName($5, $6);
	$$ = new SQLiteCreateViewStatement($2, $4, viewName, $8);
}
	;	
	
create_table_args : LP columnlist conslist_opt RP									{ $$ = new SQLiteCreateTableStatement($2, $3); }
	;
	
columnlist : columnlist COMMA column												{ $1.Add($3); $$ = $1; }
	| column																		{ $$ = new List<SQLiteColumnStatement>(); $$.Add($1); }
	;
	
column : columnid type carglist														{ $$ = new SQLiteColumnStatement($1, $2, $3); }
	;
	
columnid : nm																		{ $$ = new SQLiteObjectName($1); }
	;
	
id : ID
	;	
	
ifnotexists :								{ $$ = false; }
	| IF NOT EXISTS							{ $$ = true; }
	;
	
ids : ID
    | KEY
    | IF
    | ASC
    | DESC
    | CONFLICT
    | ABORT
    | FAIL
    | IGNORE
    | REPLACE
    | LIKE_KW
    | MATCH
    | JOIN_KW
    | CASCADE
    | RESTRICT
    | INITIALLY
    | DEFERRED
    | IMMEDIATE
    | CAST
    | RAISE
    | BEGIN
    | END
    | TRIGGER
    | BEFORE
    | AFTER
    | INSTEAD
    | OF
    | OFFSET
    | FOR
    | EACH
    | ROW
    | INDEXED
    | CTIME_KW
    | VIEW
    | TEMP
    | TEMPORARY
	| DQUOTED
	| SQUOTED
	| BQUOTED
	| TQUOTED
	;
	
nm : ID
    | KEY
    | IF
    | ASC
    | DESC
    | CONFLICT
    | ABORT
    | FAIL
    | IGNORE
    | REPLACE
    | LIKE_KW
    | MATCH
    | JOIN_KW
    | CASCADE
    | RESTRICT
    | INITIALLY
    | DEFERRED
    | IMMEDIATE
    | CAST
    | RAISE
    | BEGIN
    | END
    | TRIGGER
    | BEFORE
    | AFTER
    | INSTEAD
    | OF
    | OFFSET
    | FOR
    | EACH
    | ROW
    | INDEXED
    | CTIME_KW
    | VIEW
    | TEMP
    | TEMPORARY
	| DQUOTED
	| SQUOTED
	| BQUOTED
	| TQUOTED
	| JOIN_KW								{ $$ = Enum.GetName(typeof(SQLiteJoinOperator), $1); }
	;
	
exid : ID
    | KEY
    | IF
    | ASC
    | DESC
    | CONFLICT
    | ABORT
    | FAIL
    | IGNORE
    | REPLACE
    | LIKE_KW
    | MATCH
    | JOIN_KW
    | CASCADE
    | RESTRICT
    | INITIALLY
    | DEFERRED
    | IMMEDIATE
    | CAST
    | RAISE
    | BEGIN
    | END
    | TRIGGER
    | BEFORE
    | AFTER
    | INSTEAD
    | OF
    | OFFSET
    | FOR
    | EACH
    | ROW
    | INDEXED
    | CTIME_KW
    | VIEW
    | TEMP
    | TEMPORARY
    ;

type :										{ $$ = new SQLiteColumnType(string.Empty, -1, -1); }					
	| typetoken							
	;
	
typetoken : typename						{ $$ = new SQLiteColumnType($1, -1, -1); }
	| typename LP signed RP					{ $$ = new SQLiteColumnType($1, (int)$3, -1); }
	| typename LP signed COMMA signed RP	{ $$ = new SQLiteColumnType($1, (int)$3, (int)$5); }
	;
	
typename : ids
	| typename ids							{ $$ = $1 +" "+ $2; }
	;
	
signed : plus_num
	| minus_num
	;
	
plus_num : plus_opt number					{ $$ = $2; }
	;
	
plus_opt :
	| PLUS
	;	
	
minus_num : MINUS number					{ $$ = -$2; }
	;
	
number : NUMBER
	;
	
carglist : 
	| carglist carg							
{ 
	if ($1 == null)
		$$ = new List<SQLiteColumnConstraint>();
	else
		$$ = $1;		
	$$.Add($2);
}
	;
	
carg : CONSTRAINT nm ccons					{ $3.ConstraintName = $2; $$ = $3; }
	| ccons
	;

ccons : DEFAULT term						{ $$ = new SQLiteDefaultColumnConstraint(null, false, $2); }
    | DEFAULT NULL                          { $$ = new SQLiteDefaultColumnConstraint(null, new SQLiteNullExpression(), false); }
	| DEFAULT LP expr RP					{ $$ = new SQLiteDefaultColumnConstraint(null, $3); }
	| DEFAULT PLUS term						{ $$ = new SQLiteDefaultColumnConstraint(null, false, $3); }
	| DEFAULT MINUS term					{ $$ = new SQLiteDefaultColumnConstraint(null, true, $3); }
	| DEFAULT id							{ $$ = new SQLiteDefaultColumnConstraint(null, $2); }
	| NULL onconf							{ $$ = new SQLiteNullColumnConstraint(null, true, $2); }
	| NOT NULL onconf						{ $$ = new SQLiteNullColumnConstraint(null, false, $3); }
	| PRIMARY KEY sortorder onconf autoinc	{ $$ = new SQLitePrimaryKeyColumnConstraint(null, $3, $4, $5); }
	| UNIQUE onconf							{ $$ = new SQLiteUniqueColumnConstraint(null, $2); }
	| CHECK LP expr RP						{ $$ = new SQLiteCheckColumnConstraint(null, $3); }
	| REFERENCES nm idxlist_opt refargs		{ $$ = new SQLiteReferencesColumnConstraint(null, $2, $3, $4); }
	| defer_subclause						{ $$ = $1; }
	| COLLATE ids							{ $$ = new SQLiteCollateColumnConstraint(null, $2); }
	;

autoinc : 									{ $$ = false; }
	| AUTOINCR								{ $$ = true; }
	;

refargs : 
	| refargs refarg						
{
	if ($1 == null)
		$$ = new List<SQLiteReferenceHandler>();
	else
		$$ = $1;
	$$.Add($2);
}
	;

refarg : MATCH nm							{ $$ = new SQLiteReferenceHandler($2); }
	| ON DELETE refact						{ $$ = new SQLiteReferenceHandler(SQLiteReferenceTrigger.OnDelete, $3); }
	| ON UPDATE refact						{ $$ = new SQLiteReferenceHandler(SQLiteReferenceTrigger.OnUpdate, $3); }
	| ON INSERT refact						{ $$ = new SQLiteReferenceHandler(SQLiteReferenceTrigger.OnInsert, $3); }
	;

refact : SET NULL							{ $$ = SQLiteReferenceAction.SetNull; }
	| SET DEFAULT							{ $$ = SQLiteReferenceAction.SetDefault; }
	| CASCADE								{ $$ = SQLiteReferenceAction.Cascade; }
	| RESTRICT								{ $$ = SQLiteReferenceAction.Restrict; }
	;

defer_subclause : NOT DEFERRABLE init_deferred_pred_opt
{
	$$ = new SQLiteDeferColumnConstraint(null, false, $3);
}
	| DEFERRABLE init_deferred_pred_opt
{
	$$ = new SQLiteDeferColumnConstraint(null, true, $2);
}
	;

init_deferred_pred_opt :					{ $$ = SQLiteDeferType.None; }
	| INITIALLY DEFERRED					{ $$ = SQLiteDeferType.InitiallyDeferred; }
	| INITIALLY IMMEDIATE					{ $$ = SQLiteDeferType.InitiallyImmediate; }
	;

conslist_opt :
	| COMMA conslist						{ $$ = $2; }
	;
	
conslist : conslist COMMA tcons
{
	if ($1 == null)
		$$ = new List<SQLiteTableConstraint>();
	else
		$$ = $1;
	$$.Add($3);
}
	| conslist tcons
{
	if ($1 == null)
		$$ = new List<SQLiteTableConstraint>();
	else
		$$ = $1;
	$$.Add($2);	
}
	| tcons
{
	$$ = new List<SQLiteTableConstraint>();
	$$.Add($1);	
}
	;

tcons : CONSTRAINT nm						{ $$ = new SQLiteTableConstraint($2); }
	| PRIMARY KEY LP idxlist autoinc RP onconf { $$ = new SQLitePrimaryKeyTableConstraint(null, $4, $5, $7); }
	| UNIQUE LP idxlist RP onconf				{ $$ = new SQLiteUniqueTableConstraint(null, $3, $5); }
	| CHECK LP expr RP onconf					{ $$ = new SQLiteCheckTableConstraint(null, $3, $5); }
	| FOREIGN KEY LP idxlist RP REFERENCES nm idxlist_opt refargs defer_subclause_opt
{
	$$ = new SQLiteForeignKeyTableConstraint(null, $4, $7, $8, $9, $10);
}
	;

defer_subclause_opt :
	| defer_subclause
	;
	
onconf :						{ $$ = SQLiteResolveAction.None; }
	| ON CONFLICT resolvetype	{ $$ = $3; }
	;

orconf :
	| OR resolvetype
	;
	
resolvetype : raisetype
	| IGNORE					{ $$ = SQLiteResolveAction.Ignore; }						
	| REPLACE					{ $$ = SQLiteResolveAction.Replace; }
	;
	
raisetype : ROLLBACK			{ $$ = SQLiteResolveAction.Rollback; }
	| ABORT						{ $$ = SQLiteResolveAction.Abort; }
	| FAIL						{ $$ = SQLiteResolveAction.Fail; }
	;
		
dbnm :
	| DOT nm								{ $$ = $2; }
	;

expr : term											{ $$ = new SQLiteTermExpression($1); }							
	| LP expr RP									{ $$ = $2; }
	| NULL											{ $$ = new SQLiteNullExpression(); }
	| exid											{ $$ = new SQLiteIdExpression($1); }
	| JOIN_KW										{ $$ = new SQLiteIdExpression(Enum.GetName(typeof(SQLiteJoinOperator), $1).ToUpper()); }
	| nm DOT nm										{ $$ = new SQLiteNameExpression(new SQLiteObjectName($1, $3)); }
	| nm DOT nm DOT nm								{ $$ = new SQLiteNameExpression(new SQLiteObjectName($1, $3, $5)); }
	| VARIABLE										{ $$ = new SQLiteVariableExpression($1); }
	| expr likeop expr escape %prec LIKE_KW			{ $$ = new SQLiteLikeExpression($1, $2, $3, $4); }
	| expr COLLATE ids								{ $$ = new SQLiteCollateExpression($1, $3); }
	| CAST LP expr AS typetoken RP					{ $$ = new SQLiteCastExpression($3, $5); }
	| ID LP distinct exprlist RP					{ $$ = new SQLiteFunctionExpression($1, $3, $4); }
	| ID LP STAR RP									{ $$ = new SQLiteFunctionExpression($1); }
	| expr between_op expr AND expr %prec BETWEEN	{ $$ = new SQLiteBetweenExpression($1, $2, $3, $5); }
	| expr AND expr									{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.And, $3); }
	| expr OR expr									{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Or, $3); }
	| expr LT expr									{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Lt, $3); }
	| expr GT expr									{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Gt, $3); }
	| expr GE expr									{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Ge, $3); }
	| expr LE expr									{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Le, $3); }
	| expr EQ expr									{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Eq, $3); }
	| expr NE expr									{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Ne, $3); }
	| expr BITAND expr								{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.BitAnd, $3); }
	| expr BITOR expr								{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.BitOr, $3); }
	| expr LSHIFT expr								{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Lshift, $3); }
	| expr RSHIFT expr								{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Rshift, $3); }
	| expr PLUS expr								{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Plus, $3); }
	| expr MINUS expr								{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Minus, $3); }
	| expr STAR expr								{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Star, $3); }
	| expr SLASH expr								{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Slash, $3); }
	| expr REM expr									{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Rem, $3); }
	| expr CONCAT expr								{ $$ = new SQLiteBinaryExpression($1, SQLiteOperator.Concat, $3); }
	| expr ISNULL									{ $$ = new SQLiteUnaryExpression(SQLiteOperator.IsNull, $1); }
	| expr NOTNULL									{ $$ = new SQLiteUnaryExpression(SQLiteOperator.NotNull, $1); }
	| expr IS NULL									{ $$ = new SQLiteUnaryExpression(SQLiteOperator.Is_Null, $1); }
	| expr NOT NULL									{ $$ = new SQLiteUnaryExpression(SQLiteOperator.Not_Null, $1); }
	| expr IS NOT NULL								{ $$ = new SQLiteUnaryExpression(SQLiteOperator.Is_Not_Null, $1); }
	| NOT expr										{ $$ = new SQLiteUnaryExpression(SQLiteOperator.Not, $2); }
	| BITNOT expr									{ $$ = new SQLiteUnaryExpression(SQLiteOperator.BitNot, $2); }
	| MINUS expr %prec UMINUS						{ $$ = new SQLiteUnaryExpression(SQLiteOperator.Minus, $2); }
	| PLUS expr %prec UPLUS							{ $$ = new SQLiteUnaryExpression(SQLiteOperator.Plus, $2); }
	| expr in_op LP exprlist RP %prec IN			{ $$ = new SQLiteInExpression($1, $2, $4); }
	| LP select RP									{ $$ = new SQLiteSelectExpression($2); }
	| expr in_op LP select RP %prec IN				{ $$ = new SQLiteInExpression($1, $2, $4); }
	| expr in_op nm dbnm %prec IN					{ $$ = new SQLiteInExpression($1, $2, new SQLiteObjectName($3, $4)); }
	| EXISTS LP select RP							{ $$ = new SQLiteExistsExpression($3); }
	| CASE case_operand case_exprlist case_else END	{ $$ = new SQLiteCaseExpression($2, $3, $4); }
	| RAISE LP IGNORE RP							{ $$ = new SQLiteRaiseExpression(); }
	| RAISE LP raisetype COMMA nm RP				{ $$ = new SQLiteRaiseExpression($3, $5); }
	;		
		
case_exprlist : case_exprlist WHEN expr THEN expr	{ $$ = $1; $$.Add(new SQLiteCaseItem($3, $5)); }
	| WHEN expr THEN expr							{ $$ = new List<SQLiteCaseItem>(); $$.Add(new SQLiteCaseItem($2, $4)); }
	;
	
exprlist :			
	| nexprlist		
	;
	
nexprlist : nexprlist COMMA expr
{
	if ($1 == null)
		$$ = new List<SQLiteExpression>();
	else
		$$ = $1;
	$$.Add($3);
}
	| expr	
{
	$$ = new List<SQLiteExpression>();
	$$.Add($1);
}								
	;
	
case_else :									{ $$ = null; }
	| ELSE expr								{ $$ = $2; }					
	;

case_operand :								{ $$ = null; } 
	| expr									{ $$ = $1; }
	;
		
in_op : IN									{ $$ = true; }
	| NOT IN								{ $$ = false; }
	;
				
term : NUMBER								{ $$ = new SQLiteTerm($1); }
	| BLOB									{ $$ = new SQLiteTerm($1); }
	| DQUOTED								{ $$ = new SQLiteTerm($1); }
	| SQUOTED								{ $$ = new SQLiteTerm($1); }
	| BQUOTED								{ $$ = new SQLiteTerm($1); }
	| TQUOTED								{ $$ = new SQLiteTerm($1); }
	| CTIME_KW								{ $$ = new SQLiteTerm($1); }
	;

likeop : LIKE_KW							{ $$ = new SQLiteLikeOperator($1, false); }
	| NOT LIKE_KW							{ $$ = new SQLiteLikeOperator($2, true); }
	| MATCH									{ $$ = new SQLiteLikeOperator(SQLiteLike.Match, false); }
	| NOT MATCH								{ $$ = new SQLiteLikeOperator(SQLiteLike.Match, true); }
	;

escape : %prec ESCAPE
	| ESCAPE expr %prec ESCAPE				
	;
	
between_op : BETWEEN						{ $$ = true; }
	| NOT BETWEEN							{ $$ = false; }
	;

uniqueflag :								{ $$ = false; }
	| UNIQUE								{ $$ = true; }
	;
	
idxlist_opt :
	| LP idxlist RP							{ $$ = $2; }
	;
	
idxlist : idxlist COMMA nm collate sortorder	 
{
	if ($1 == null)
		$$ = new List<SQLiteIndexedColumn>();
	else
		$$ = $1;
	$$.Add(new SQLiteIndexedColumn($3, $4, $5));
}
	| nm collate sortorder						{ $$ = new List<SQLiteIndexedColumn>(); $$.Add(new SQLiteIndexedColumn($1, $2, $3)); }
	;
	
collate :
	| COLLATE ids							{ $$ = $2; }							
	;
	
trigger_decl : temp TRIGGER ifnotexists nm dbnm trigger_time trigger_event ON fullname foreach_clause when_clause
{
	$$ = new SQLiteCreateTriggerStatement($1, $3, new SQLiteObjectName($4, $5), $6, $7, $9, $10, $11);
}
	;
	
temp : { $$ = false; }
	| TEMP { $$ = true; }
	| TEMPORARY { $$ = true; }
	;
	
trigger_time :				{ $$ = SQLiteTriggerTime.None; }							
	| BEFORE				{ $$ = SQLiteTriggerTime.Before; }
	| AFTER					{ $$ = SQLiteTriggerTime.After; }
	| INSTEAD OF			{ $$ = SQLiteTriggerTime.InsteadOf; }
	;
	
trigger_event : DELETE						{ $$ = new SQLiteTriggerEventClause(SQLiteTriggerEvent.Delete); }				
	| INSERT								{ $$ = new SQLiteTriggerEventClause(SQLiteTriggerEvent.Insert); }
	| UPDATE								{ $$ = new SQLiteTriggerEventClause(SQLiteTriggerEvent.Update); }
	| UPDATE OF inscollist					{ $$ = new SQLiteTriggerEventClause($3); }
	;
	
inscollist_opt :							{ $$ = null; }
	| LP inscollist RP						{ $$ = $2; }
	;
	
inscollist : inscollist COMMA nm			{ $$ = $1; $$.Add($3); }
	| nm									{ $$ = new List<string>(); $$.Add($1); }
	;
	
foreach_clause :							{ $$ = false; }
	| FOR EACH ROW							{ $$ = true; }
	;
	
when_clause :
	| WHEN expr								
	;

trigger_cmd_list : trigger_cmd_list trigger_cmd SEMI	{$$ = $1; $$.Add($2); }
	| trigger_cmd SEMI						{ $$ = new List<SQLiteStatement>(); $$.Add($1); }
	;
	
trigger_cmd : UPDATE orconf nm SET setlist where_opt { $$ = new SQLiteUpdateStatement($2, $3, $5, $6); }
	| insert_cmd INTO nm inscollist_opt VALUES LP itemlist RP { $$ = new SQLiteInsertStatement($1, $3, $4, $7); }
	| insert_cmd INTO nm inscollist_opt select	{ $$ = new SQLiteInsertStatement($1, $3, $4, $5); }
	| DELETE FROM nm where_opt				{ $$ = new SQLiteDeleteStatement($3, $4); }
	| select								{ $$ = $1; }
	;
	
itemlist : itemlist COMMA expr				{ $$ = $1; $$.Add($3); }			
	| expr									{ $$ = new List<SQLiteExpression>(); $$.Add($1); }
	;
		
insert_cmd : INSERT orconf					{ $$ = new SQLiteInsertPrefix($2); }
	| REPLACE								{ $$ = new SQLiteInsertPrefix(); }
	;	
	
where_opt :									{ $$ = null; }
	| WHERE expr							{ $$ = $2; }
	;	
	
setlist : setlist COMMA nm EQ expr			{ $$ = $1; $$.Add(new SQLiteUpdateItem($3, $5)); }		
	| nm EQ expr							{ $$ = new List<SQLiteUpdateItem>(); $$.Add(new SQLiteUpdateItem($1, $3)); }		
	;
	
select : oneselect						{ $$ = $1; }
	| select multiselect_op oneselect	{ $$ = new SQLiteMultiSelectStatement($1, $2, $3); }
	;
		
multiselect_op : UNION					{ $$ = SQLiteSelectOperator.Union; }		
	| UNION ALL							{ $$ = SQLiteSelectOperator.UnionAll; }
	| EXCEPT							{ $$ = SQLiteSelectOperator.Except; }
	| INTERSECT							{ $$ = SQLiteSelectOperator.Intersect; }
	;
	
oneselect : SELECT distinct selcollist from where_opt groupby_opt having_opt orderby_opt limit_opt
{
	$$ = new SQLiteSingleSelectStatement($2, $3, $4, $5, $6, $7, $8, $9);
}
	;

distinct :			{ $$ = SQLiteDistinct.None; }							
	| DISTINCT		{ $$ = SQLiteDistinct.Distinct; }
	| ALL			{ $$ = SQLiteDistinct.All; }
	;
	
sclp : 
	| selcollist COMMA	{ $$ = $1; }
	;
		
selcollist : sclp expr as	
{
	if ($1 != null)
		$$ = $1;
	else
		$$ = new List<SQLiteSelectColumn>();
	$$.Add(new SQLiteSelectColumn($2, $3));		
}																																					
	| sclp STAR																			
{
	if ($1 != null)
		$$ = $1;
	else
		$$ = new List<SQLiteSelectColumn>();
	$$.Add(new SQLiteSelectColumn());	
}	
	| sclp nm DOT STAR				
{
	if ($1 != null)
		$$ = $1;
	else
		$$ = new List<SQLiteSelectColumn>();
	$$.Add(new SQLiteSelectColumn(new SQLiteObjectName($2)));	
}		
	;

as : 
	| AS nm									{ $$ = $2; }
	| ids									{ $$ = $1; }
	;
	
from :
	| FROM seltablist			{ $$ = $2; }			
	;
	
stl_prefix : 
	| seltablist joinop			{ $$ = $1.AddJoin($2); }				
	;
	
seltablist : stl_prefix nm dbnm as indexed_opt on_opt using_opt 
{
	if ($1 == null)
		$$ = new SQLiteFromClause();
	else
		$$ = $1;
	$$.AddTable(new SQLiteObjectName($2, $3), $4, $5, $6, $7);
}
	| stl_prefix LP seltablist_paren RP as on_opt using_opt 
{
	if ($1 == null)
		$$ = new SQLiteFromClause();
	else
		$$ = $1;
	$$.AddInternalTable($3, $5, $6, $7);
}
	;
  
seltablist_paren : select				{ $$ = new SQLiteFromInternalTable($1); }		
	| seltablist						{ $$ = new SQLiteFromInternalTable($1); }
	;

fullname : nm dbnm						{ $$ = new SQLiteObjectName($1, $2); }
	;

joinop : COMMA							{ $$ = SQLiteJoinOperator.Comma; }
	| JOIN								{ $$ = SQLiteJoinOperator.Join; }
	| JOIN_KW JOIN						{ $$ = $1 | SQLiteJoinOperator.Join; }
	| JOIN_KW JOIN_KW JOIN				{ $$ = $1 | $2 | SQLiteJoinOperator.Join; }
	| JOIN_KW JOIN_KW JOIN_KW JOIN		{ $$ = $1 | $2 | $3 | SQLiteJoinOperator.Join; }
	;

on_opt :								{ $$ = null; }
	| ON expr							{ $$ = $2; }
	;

indexed_opt :							{ $$ = null; }
	| INDEXED BY nm						{ $$ = new SQLiteFromIndexed($3); }
	| NOT INDEXED						{ $$ = new SQLiteFromIndexed(); }
	;

using_opt :								{ $$ = null; }
	| USING LP inscollist RP			{ $$ = $3; }
	;

orderby_opt :							{ $$ = null; }
	| ORDER BY sortlist					{ $$ = $3; }			
	;
	
sortlist : sortlist COMMA sortitem sortorder	{ $$ = $1; $$.Add(new SQLiteSortItem($3, $4)); }
	| sortitem sortorder				{ $$ = new List<SQLiteSortItem>(); $$.Add(new SQLiteSortItem($1, $2)); }
	;
	
sortitem : expr							{ $$ = $1; }
	;

sortorder :			{ $$ = SQLiteSortOrder.None; }							
	| ASC			{ $$ = SQLiteSortOrder.Ascending; }	
	| DESC			{ $$ = SQLiteSortOrder.Descending; }
	;

groupby_opt :							{ $$ = null; }
	| GROUP BY nexprlist				{ $$ = $3; }
	;

having_opt :							{ $$ = null; }
	| HAVING expr						{ $$ = $2; }
	;
	
limit_opt :								{ $$ = null; }
	| LIMIT expr						{ $$ = new SQLiteLimitClause($2); }
	| LIMIT expr OFFSET expr			{ $$ = new SQLiteLimitClause($2, $4); }
	| LIMIT expr COMMA expr				{ $$ = new SQLiteLimitClause($4, $2); }
	;

			
	
		
	
	



	
		
	
	