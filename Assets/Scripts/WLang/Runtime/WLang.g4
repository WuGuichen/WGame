grammar WLang;

file    :   (statement | ifStatement | whileStatement | forStatement | waitStatement)*
        ;
        
fsmBuilder      : fileCode? fsmBlock*
;

fileCode        : CODE OPENBRACE file CLOSEBRACE
;

fsmBlock        :   
                STATE OPENBRACE fsmContent* CLOSEBRACE                  #FSMState
                | TRIGGER OPENBRACE fsmTrigger* CLOSEBRACE         #FSMTrigger
                | CONDITION OPENBRACE fsmCondition* CLOSEBRACE         #FSMCondition
                | TRIGGER_TIME OPENBRACE fsmTriggerTime* CLOSEBRACE    #FSMTriggerTime
                | '->' s=ID    #FSMStart
                | '<-' s=ID    #FSMExit
;

fsmContent      :   i=ID
                |   i=ID ':' p=parametersMethodRef
                |   i=ID '->' f=ID
;

parametersMethodRef :   exprMethodRef (',' exprMethodRef)*
;

fsmTransition   :   f=ID '->' t=ID
;

fsmTrigger      : t=fsmTransition ':' i=ID exprMethodRef?
;

fsmCondition      : t=fsmTransition ':' c=exprMethodRef
;

fsmTriggerTime  :   t=fsmTransition ':' n=numParam exprMethodRef?
;
        
bTreeBuilder    :   fileCode? treeBlock+
;

treeBlock       :   SELECTOR treeContent    #TreeSelector
                |   SEQUENCE treeContent    #TreeSequence
                |   DO block           #TreeDo
                |   WAITTIME numParam       #TreeWaitTime
                |   WAIT numParam           #TreeWait
//                |   DECORATOR ID treeContent #TreeDecorator  有需要再实现
;

treeContent     :   numParam? OPENBRACE treeBlock+ CLOSEBRACE
;

numParam       :   s=(INT|FLOAT|ID)
;
        
parameters  : (expr | exprID) (',' (expr | exprID))*;

parametersDef  : i=ID (',' ID)*;
        
expr:
    o='-' expr    #ExprUnary
    |   o='!' expr    #ExprUnary
    |   expr o=('*'|'/') expr   #ExprBinary
    |   expr o=('*='|'/=') expr   #ExprBinary
    |   expr o=('+'|'-') expr   #ExprBinary
    |   expr o=('+='|'-=') expr   #ExprBinary
    |   expr o=('=='|'!='|'>'|'>='|'<'|'<=') expr #ExprBinary
    |   expr o=(AND|OR) expr #ExprBinary
    |   l=ID point+  #ExprPoint
    |   o=primary #ExprPrimary
    |   '('expr ')' #ExprGroup
    |   l=exprList        #ExprTable
    |   exprLambda       #ExprLambdaRef
    |   m=exprMethod    #ExprCommand
    ;
    
//exprRight   :
//            expr                #ExprExpr
//            | l=exprList        #ExprTable
//            |  exprLambda       #ExprLambdaRef
//            |   m=exprMethod    #ExprCommand
//;

exprLambda  : '(' parametersDef? ')' '=>' b=block
;
    
exprMethod  : 
            e=exprID ':' d=ID '(' parameters? ')'
            |d=ID '(' parameters? ')'
;

exprID    : '@' i=ID
;

exprMethodRef : i=ID         #ExprMethodRefID
                | NULL      #ExprMethodRefNull
                | l=exprLambda #ExprMethodRefLambda
;

exprList    :   OPENBRACK expr? (',' expr)* CLOSEBRACK
            |   exprInt ':' exprInt (':' exprInt)?
;

exprInt  :    i=ID    #ExprIntID
            | i=INT   #ExprIntINT
            ;

primary:    i=ID      #PrimaryID
    |       i=INT     #PrimaryINT
    |       i=FLOAT   #PrimaryFLOAT
    |       i=CHAR    #PrimaryCHAR
    |       i=STRING  #PrimarySTRING
    |       i=BOOLEAN #PrimaryBOOL
    |       i=NULL    #PrimaryNULL
    ;

statement  :
              k=expr '=' r=expr     #StatAssign
            | PASS                  #StatPass
            | RETURN r=expr         #StatReturn
            | IMPORT f=ID ('.' ID)* #StatImport
            | DEFINE f=ID OPENPAREN p=parametersDef? CLOSEPAREN b=block #StatMethod
            | exprMethod    #StatCommand
            ;
            
waitStatement   :   WAIT t=(INT|FLOAT|ID) b=block
;
            
ifStatement :   ifStat (elseIfStat)* elseStat?
;

ifStat      :   IF e=expr b=block
;

elseIfStat  :   ELSEIF e=expr b=block
;

elseStat    :   ELSE b=block
;

whileStatement  :   WHILE e=expr b=block
;

forStatement    :   FOR i=ID IN (exprList | ID) b=block
;

block   : OPENBRACE f=file CLOSEBRACE
;

point : SHARP exprInt
;

BOOLEAN: 'true' | 'false';
NULL : 'nil';
PASS    :   'pass';
RETURN  :   'return';
IMPORT  :   'import';
DEFINE  :   'def';
CACHE_DEFINE  :   'cDef';

WAIT    :   'WAIT';
DO      :   'DO';
AND     :   'and';
OR      :   'or';
WAITTIME:   'WAIT_TIME';
SELECTOR:   'SELECTOR';
SEQUENCE:   'SEQUENCE';
TRIGGER :   'TRIGGER';
CONDITION :   'CONDITION';
TRIGGER_TIME :   'TRIGGER_TIME';
TRANS_TO_ANY    : 'TRANS_TO_ANY';
STATE   :   'STATE';
CODE    :   'CODE';
BTREE   :   'BTREE';
DECORATOR : 'DECORATOR';

AT      :   '@';
SHARP   :   '#';
OP_EQUAL:   '=';
OP_ADD  :   '+';
OP_SUB  :   '-';
OP_MUL  :   '*';
OP_DIV  :   '/';

IF      :   'if';
ELSEIF  :   'elif';
ELSE    :   'else';
IN      :   'in';
WHILE   :   'while';
FOR     :   'for';

OPENBRACE   : '{';
CLOSEBRACE  : '}';
OPENBRACK   : '[';
CLOSEBRACK  : ']';
OPENPAREN   : '(';
CLOSEPAREN  : ')';

ID  :   LETTER (LETTER | '_' | [0-9])* ;

fragment
LETTER : [a-zA-Z] ;

INT :   [0-9]+ ;
FLOAT : INT '.' INT ;
WS  :   [ \t\n\r]+ -> skip ;
CHAR :  '\'' . '\'' ;
STRING: '"' ~( '"' | '\r' | '\n' )* '"'; 

SLCOMMENT
    :   '//' .*? '\n' -> skip
    ;

COMMNET
    : '/*' .*? '*/' -> skip
    ;
    
TITLE
    : '##' .*? '\n' -> channel(HIDDEN)
;