grammar WLang;

file    :   (bTreeBuilder | statement | ifStatement | whileStatement | forStatement | waitStatement)*
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
;

treeContent     :   numParam? OPENBRACE treeBlock+ CLOSEBRACE
;

numParam       :   s=(INT|FLOAT|ID)
;
        
parameters  : (exprRight | exprID) (',' (exprRight | exprID))*;

parametersDef  : ID (',' ID)*;
        
expr:
    o='-' exprRight    #ExprUnary
    |   o='!' exprRight    #ExprUnary
    |   expr o=('*'|'/') exprRight   #ExprBinary
    |   expr o=('*='|'/=') exprRight   #ExprBinary
    |   expr o=('+'|'-') exprRight   #ExprBinary
    |   expr o=('+='|'-=') exprRight   #ExprBinary
    |   expr o=('=='|'!='|'>'|'>='|'<'|'<=') exprRight #ExprBinary
    |   expr o=(AND|OR) exprRight #ExprBinary
    |   l=ID point+  #ExprPoint
    |   o=primary #ExprPrimary
    |   '('expr ')' #ExprGroup
    ;
    
exprRight   :
            expr                #ExprExpr
            | l=exprList        #ExprTable
            |  exprLambda       #ExprLambdaRef
            |   m=exprMethod    #ExprCommand
;

exprLambda  : '(' parametersDef? ')' '=>' b=block
;
    
exprMethod  : 
            e=exprID ':' d=ID '(' parameters? ')'
            |d=ID '(' parameters? ')'
;

exprID    : '@' ID
;

exprMethodRef : i=ID         #ExprMethodRefID
                | NULL      #ExprMethodRefNull
                | l=exprLambda #ExprMethodRefLambda
;

exprList    :   OPENBRACK exprRight? (',' exprRight)* CLOSEBRACK
            |   exprInt ':' exprInt (':' exprInt)?
;

exprInt  :    ID    #ExprIntID
            | INT   #ExprIntINT
            ;

primary:    ID      #PrimaryID
    |       INT     #PrimaryINT
    |       FLOAT   #PrimaryFLOAT
    |       CHAR    #PrimaryCHAR
    |       STRING  #PrimarySTRING
    |       BOOLEAN #PrimaryBOOL
    |       NULL    #PrimaryNULL
    ;

statement  :
              k=expr '=' exprRight     #StatAssign
            | PASS                  #StatPass
            | RETURN r=exprRight         #StatReturn
            | IMPORT f=ID ('.' ID)* #StatImport
            | DEFINE f=ID OPENPAREN parametersDef? CLOSEPAREN b=block #StatMethod
            | exprMethod    #StatCommand
            ;
            
waitStatement   :   WAIT t=(INT|FLOAT|ID) b=block
;
            
ifStatement :   ifStat (elseIfStat)* elseStat?
;

ifStat      :   IF e=exprRight b=block
;

elseIfStat  :   ELSEIF e=exprRight b=block
;

elseStat    :   ELSE b=block
;

whileStatement  :   WHILE e=exprRight b=block
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
WAITTIME:   'WAITTIME';
SELECTOR:   'SELECTOR';
SEQUENCE:   'SEQUENCE';
TRIGGER :   'TRIGGER';
CONDITION :   'CONDITION';
TRIGGER_TIME :   'TRIGGER_TIME';
TRANS_TO_ANY    : 'TRANS_TO_ANY';
STATE   :   'STATE';
CODE    :   'CODE';
BTREE   :   'BTREE';

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