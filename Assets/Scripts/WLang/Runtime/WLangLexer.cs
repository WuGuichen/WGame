//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from D:/UnityProject/WGame/Client/Assets/Scripts/WLang/Runtime/WLang.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public partial class WLangLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, T__13=14, T__14=15, T__15=16, T__16=17, 
		BOOLEAN=18, NULL=19, PASS=20, RETURN=21, IMPORT=22, DEFINE=23, CACHE_DEFINE=24, 
		WAIT=25, DO=26, AND=27, NOT=28, OR=29, WAITTIME=30, SELECTOR=31, SEQUENCE=32, 
		TRIGGER=33, CONDITION=34, TRIGGER_TIME=35, TRANS_TO_ANY=36, STATE=37, 
		CODE=38, BTREE=39, DECORATOR=40, AT=41, SHARP=42, OP_EQUAL=43, OP_ADD=44, 
		OP_SUB=45, OP_MUL=46, OP_DIV=47, IF=48, ELSEIF=49, ELSE=50, IN=51, WHILE=52, 
		FOR=53, OPENBRACE=54, CLOSEBRACE=55, OPENBRACK=56, CLOSEBRACK=57, OPENPAREN=58, 
		CLOSEPAREN=59, COMMENTHEADER=60, ID=61, INT=62, FLOAT=63, WS=64, CHAR=65, 
		STRING=66, SLCOMMENT=67, COMMNET=68, TITLE=69;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
		"T__9", "T__10", "T__11", "T__12", "T__13", "T__14", "T__15", "T__16", 
		"BOOLEAN", "NULL", "PASS", "RETURN", "IMPORT", "DEFINE", "CACHE_DEFINE", 
		"WAIT", "DO", "AND", "NOT", "OR", "WAITTIME", "SELECTOR", "SEQUENCE", 
		"TRIGGER", "CONDITION", "TRIGGER_TIME", "TRANS_TO_ANY", "STATE", "CODE", 
		"BTREE", "DECORATOR", "AT", "SHARP", "OP_EQUAL", "OP_ADD", "OP_SUB", "OP_MUL", 
		"OP_DIV", "IF", "ELSEIF", "ELSE", "IN", "WHILE", "FOR", "OPENBRACE", "CLOSEBRACE", 
		"OPENBRACK", "CLOSEBRACK", "OPENPAREN", "CLOSEPAREN", "COMMENTHEADER", 
		"ID", "LETTER", "INT", "FLOAT", "WS", "CHAR", "STRING", "SLCOMMENT", "COMMNET", 
		"TITLE"
	};


	public WLangLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public WLangLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'->'", "'<-'", "':'", "','", "'!'", "'*='", "'/='", "'+='", "'-='", 
		"'=='", "'!='", "'>'", "'>='", "'<'", "'<='", "'=>'", "'.'", null, "'nil'", 
		"'pass'", "'return'", "'import'", "'def'", "'cDef'", "'WAIT'", "'DO'", 
		"'and'", "'not'", "'or'", "'WAIT_TIME'", "'SELECTOR'", "'SEQUENCE'", "'TRIGGER'", 
		"'CONDITION'", "'TRIGGER_TIME'", "'TRANS_TO_ANY'", "'STATE'", "'CODE'", 
		"'BTREE'", "'DECORATOR'", "'@'", "'#'", "'='", "'+'", "'-'", "'*'", "'/'", 
		"'if'", "'elif'", "'else'", "'in'", "'while'", "'for'", "'{'", "'}'", 
		"'['", "']'", "'('", "')'", "'--'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, "BOOLEAN", "NULL", "PASS", "RETURN", 
		"IMPORT", "DEFINE", "CACHE_DEFINE", "WAIT", "DO", "AND", "NOT", "OR", 
		"WAITTIME", "SELECTOR", "SEQUENCE", "TRIGGER", "CONDITION", "TRIGGER_TIME", 
		"TRANS_TO_ANY", "STATE", "CODE", "BTREE", "DECORATOR", "AT", "SHARP", 
		"OP_EQUAL", "OP_ADD", "OP_SUB", "OP_MUL", "OP_DIV", "IF", "ELSEIF", "ELSE", 
		"IN", "WHILE", "FOR", "OPENBRACE", "CLOSEBRACE", "OPENBRACK", "CLOSEBRACK", 
		"OPENPAREN", "CLOSEPAREN", "COMMENTHEADER", "ID", "INT", "FLOAT", "WS", 
		"CHAR", "STRING", "SLCOMMENT", "COMMNET", "TITLE"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "WLang.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static WLangLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static int[] _serializedATN = {
		4,0,69,479,6,-1,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,
		6,2,7,7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,2,14,
		7,14,2,15,7,15,2,16,7,16,2,17,7,17,2,18,7,18,2,19,7,19,2,20,7,20,2,21,
		7,21,2,22,7,22,2,23,7,23,2,24,7,24,2,25,7,25,2,26,7,26,2,27,7,27,2,28,
		7,28,2,29,7,29,2,30,7,30,2,31,7,31,2,32,7,32,2,33,7,33,2,34,7,34,2,35,
		7,35,2,36,7,36,2,37,7,37,2,38,7,38,2,39,7,39,2,40,7,40,2,41,7,41,2,42,
		7,42,2,43,7,43,2,44,7,44,2,45,7,45,2,46,7,46,2,47,7,47,2,48,7,48,2,49,
		7,49,2,50,7,50,2,51,7,51,2,52,7,52,2,53,7,53,2,54,7,54,2,55,7,55,2,56,
		7,56,2,57,7,57,2,58,7,58,2,59,7,59,2,60,7,60,2,61,7,61,2,62,7,62,2,63,
		7,63,2,64,7,64,2,65,7,65,2,66,7,66,2,67,7,67,2,68,7,68,2,69,7,69,1,0,1,
		0,1,0,1,1,1,1,1,1,1,2,1,2,1,3,1,3,1,4,1,4,1,5,1,5,1,5,1,6,1,6,1,6,1,7,
		1,7,1,7,1,8,1,8,1,8,1,9,1,9,1,9,1,10,1,10,1,10,1,11,1,11,1,12,1,12,1,12,
		1,13,1,13,1,14,1,14,1,14,1,15,1,15,1,15,1,16,1,16,1,17,1,17,1,17,1,17,
		1,17,1,17,1,17,1,17,1,17,3,17,196,8,17,1,18,1,18,1,18,1,18,1,19,1,19,1,
		19,1,19,1,19,1,20,1,20,1,20,1,20,1,20,1,20,1,20,1,21,1,21,1,21,1,21,1,
		21,1,21,1,21,1,22,1,22,1,22,1,22,1,23,1,23,1,23,1,23,1,23,1,24,1,24,1,
		24,1,24,1,24,1,25,1,25,1,25,1,26,1,26,1,26,1,26,1,27,1,27,1,27,1,27,1,
		28,1,28,1,28,1,29,1,29,1,29,1,29,1,29,1,29,1,29,1,29,1,29,1,29,1,30,1,
		30,1,30,1,30,1,30,1,30,1,30,1,30,1,30,1,31,1,31,1,31,1,31,1,31,1,31,1,
		31,1,31,1,31,1,32,1,32,1,32,1,32,1,32,1,32,1,32,1,32,1,33,1,33,1,33,1,
		33,1,33,1,33,1,33,1,33,1,33,1,33,1,34,1,34,1,34,1,34,1,34,1,34,1,34,1,
		34,1,34,1,34,1,34,1,34,1,34,1,35,1,35,1,35,1,35,1,35,1,35,1,35,1,35,1,
		35,1,35,1,35,1,35,1,35,1,36,1,36,1,36,1,36,1,36,1,36,1,37,1,37,1,37,1,
		37,1,37,1,38,1,38,1,38,1,38,1,38,1,38,1,39,1,39,1,39,1,39,1,39,1,39,1,
		39,1,39,1,39,1,39,1,40,1,40,1,41,1,41,1,42,1,42,1,43,1,43,1,44,1,44,1,
		45,1,45,1,46,1,46,1,47,1,47,1,47,1,48,1,48,1,48,1,48,1,48,1,49,1,49,1,
		49,1,49,1,49,1,50,1,50,1,50,1,51,1,51,1,51,1,51,1,51,1,51,1,52,1,52,1,
		52,1,52,1,53,1,53,1,54,1,54,1,55,1,55,1,56,1,56,1,57,1,57,1,58,1,58,1,
		59,1,59,1,59,1,60,1,60,1,60,5,60,406,8,60,10,60,12,60,409,9,60,1,61,1,
		61,1,62,4,62,414,8,62,11,62,12,62,415,1,63,1,63,1,63,1,63,1,64,4,64,423,
		8,64,11,64,12,64,424,1,64,1,64,1,65,1,65,1,65,1,65,1,66,1,66,5,66,435,
		8,66,10,66,12,66,438,9,66,1,66,1,66,1,67,1,67,5,67,444,8,67,10,67,12,67,
		447,9,67,1,67,1,67,1,67,1,67,1,68,1,68,1,68,1,68,5,68,457,8,68,10,68,12,
		68,460,9,68,1,68,1,68,1,68,1,68,1,68,1,69,1,69,1,69,1,69,5,69,471,8,69,
		10,69,12,69,474,9,69,1,69,1,69,1,69,1,69,3,445,458,472,0,70,1,1,3,2,5,
		3,7,4,9,5,11,6,13,7,15,8,17,9,19,10,21,11,23,12,25,13,27,14,29,15,31,16,
		33,17,35,18,37,19,39,20,41,21,43,22,45,23,47,24,49,25,51,26,53,27,55,28,
		57,29,59,30,61,31,63,32,65,33,67,34,69,35,71,36,73,37,75,38,77,39,79,40,
		81,41,83,42,85,43,87,44,89,45,91,46,93,47,95,48,97,49,99,50,101,51,103,
		52,105,53,107,54,109,55,111,56,113,57,115,58,117,59,119,60,121,61,123,
		0,125,62,127,63,129,64,131,65,133,66,135,67,137,68,139,69,1,0,5,2,0,48,
		57,95,95,2,0,65,90,97,122,1,0,48,57,3,0,9,10,13,13,32,32,3,0,10,10,13,
		13,34,34,486,0,1,1,0,0,0,0,3,1,0,0,0,0,5,1,0,0,0,0,7,1,0,0,0,0,9,1,0,0,
		0,0,11,1,0,0,0,0,13,1,0,0,0,0,15,1,0,0,0,0,17,1,0,0,0,0,19,1,0,0,0,0,21,
		1,0,0,0,0,23,1,0,0,0,0,25,1,0,0,0,0,27,1,0,0,0,0,29,1,0,0,0,0,31,1,0,0,
		0,0,33,1,0,0,0,0,35,1,0,0,0,0,37,1,0,0,0,0,39,1,0,0,0,0,41,1,0,0,0,0,43,
		1,0,0,0,0,45,1,0,0,0,0,47,1,0,0,0,0,49,1,0,0,0,0,51,1,0,0,0,0,53,1,0,0,
		0,0,55,1,0,0,0,0,57,1,0,0,0,0,59,1,0,0,0,0,61,1,0,0,0,0,63,1,0,0,0,0,65,
		1,0,0,0,0,67,1,0,0,0,0,69,1,0,0,0,0,71,1,0,0,0,0,73,1,0,0,0,0,75,1,0,0,
		0,0,77,1,0,0,0,0,79,1,0,0,0,0,81,1,0,0,0,0,83,1,0,0,0,0,85,1,0,0,0,0,87,
		1,0,0,0,0,89,1,0,0,0,0,91,1,0,0,0,0,93,1,0,0,0,0,95,1,0,0,0,0,97,1,0,0,
		0,0,99,1,0,0,0,0,101,1,0,0,0,0,103,1,0,0,0,0,105,1,0,0,0,0,107,1,0,0,0,
		0,109,1,0,0,0,0,111,1,0,0,0,0,113,1,0,0,0,0,115,1,0,0,0,0,117,1,0,0,0,
		0,119,1,0,0,0,0,121,1,0,0,0,0,125,1,0,0,0,0,127,1,0,0,0,0,129,1,0,0,0,
		0,131,1,0,0,0,0,133,1,0,0,0,0,135,1,0,0,0,0,137,1,0,0,0,0,139,1,0,0,0,
		1,141,1,0,0,0,3,144,1,0,0,0,5,147,1,0,0,0,7,149,1,0,0,0,9,151,1,0,0,0,
		11,153,1,0,0,0,13,156,1,0,0,0,15,159,1,0,0,0,17,162,1,0,0,0,19,165,1,0,
		0,0,21,168,1,0,0,0,23,171,1,0,0,0,25,173,1,0,0,0,27,176,1,0,0,0,29,178,
		1,0,0,0,31,181,1,0,0,0,33,184,1,0,0,0,35,195,1,0,0,0,37,197,1,0,0,0,39,
		201,1,0,0,0,41,206,1,0,0,0,43,213,1,0,0,0,45,220,1,0,0,0,47,224,1,0,0,
		0,49,229,1,0,0,0,51,234,1,0,0,0,53,237,1,0,0,0,55,241,1,0,0,0,57,245,1,
		0,0,0,59,248,1,0,0,0,61,258,1,0,0,0,63,267,1,0,0,0,65,276,1,0,0,0,67,284,
		1,0,0,0,69,294,1,0,0,0,71,307,1,0,0,0,73,320,1,0,0,0,75,326,1,0,0,0,77,
		331,1,0,0,0,79,337,1,0,0,0,81,347,1,0,0,0,83,349,1,0,0,0,85,351,1,0,0,
		0,87,353,1,0,0,0,89,355,1,0,0,0,91,357,1,0,0,0,93,359,1,0,0,0,95,361,1,
		0,0,0,97,364,1,0,0,0,99,369,1,0,0,0,101,374,1,0,0,0,103,377,1,0,0,0,105,
		383,1,0,0,0,107,387,1,0,0,0,109,389,1,0,0,0,111,391,1,0,0,0,113,393,1,
		0,0,0,115,395,1,0,0,0,117,397,1,0,0,0,119,399,1,0,0,0,121,402,1,0,0,0,
		123,410,1,0,0,0,125,413,1,0,0,0,127,417,1,0,0,0,129,422,1,0,0,0,131,428,
		1,0,0,0,133,432,1,0,0,0,135,441,1,0,0,0,137,452,1,0,0,0,139,466,1,0,0,
		0,141,142,5,45,0,0,142,143,5,62,0,0,143,2,1,0,0,0,144,145,5,60,0,0,145,
		146,5,45,0,0,146,4,1,0,0,0,147,148,5,58,0,0,148,6,1,0,0,0,149,150,5,44,
		0,0,150,8,1,0,0,0,151,152,5,33,0,0,152,10,1,0,0,0,153,154,5,42,0,0,154,
		155,5,61,0,0,155,12,1,0,0,0,156,157,5,47,0,0,157,158,5,61,0,0,158,14,1,
		0,0,0,159,160,5,43,0,0,160,161,5,61,0,0,161,16,1,0,0,0,162,163,5,45,0,
		0,163,164,5,61,0,0,164,18,1,0,0,0,165,166,5,61,0,0,166,167,5,61,0,0,167,
		20,1,0,0,0,168,169,5,33,0,0,169,170,5,61,0,0,170,22,1,0,0,0,171,172,5,
		62,0,0,172,24,1,0,0,0,173,174,5,62,0,0,174,175,5,61,0,0,175,26,1,0,0,0,
		176,177,5,60,0,0,177,28,1,0,0,0,178,179,5,60,0,0,179,180,5,61,0,0,180,
		30,1,0,0,0,181,182,5,61,0,0,182,183,5,62,0,0,183,32,1,0,0,0,184,185,5,
		46,0,0,185,34,1,0,0,0,186,187,5,116,0,0,187,188,5,114,0,0,188,189,5,117,
		0,0,189,196,5,101,0,0,190,191,5,102,0,0,191,192,5,97,0,0,192,193,5,108,
		0,0,193,194,5,115,0,0,194,196,5,101,0,0,195,186,1,0,0,0,195,190,1,0,0,
		0,196,36,1,0,0,0,197,198,5,110,0,0,198,199,5,105,0,0,199,200,5,108,0,0,
		200,38,1,0,0,0,201,202,5,112,0,0,202,203,5,97,0,0,203,204,5,115,0,0,204,
		205,5,115,0,0,205,40,1,0,0,0,206,207,5,114,0,0,207,208,5,101,0,0,208,209,
		5,116,0,0,209,210,5,117,0,0,210,211,5,114,0,0,211,212,5,110,0,0,212,42,
		1,0,0,0,213,214,5,105,0,0,214,215,5,109,0,0,215,216,5,112,0,0,216,217,
		5,111,0,0,217,218,5,114,0,0,218,219,5,116,0,0,219,44,1,0,0,0,220,221,5,
		100,0,0,221,222,5,101,0,0,222,223,5,102,0,0,223,46,1,0,0,0,224,225,5,99,
		0,0,225,226,5,68,0,0,226,227,5,101,0,0,227,228,5,102,0,0,228,48,1,0,0,
		0,229,230,5,87,0,0,230,231,5,65,0,0,231,232,5,73,0,0,232,233,5,84,0,0,
		233,50,1,0,0,0,234,235,5,68,0,0,235,236,5,79,0,0,236,52,1,0,0,0,237,238,
		5,97,0,0,238,239,5,110,0,0,239,240,5,100,0,0,240,54,1,0,0,0,241,242,5,
		110,0,0,242,243,5,111,0,0,243,244,5,116,0,0,244,56,1,0,0,0,245,246,5,111,
		0,0,246,247,5,114,0,0,247,58,1,0,0,0,248,249,5,87,0,0,249,250,5,65,0,0,
		250,251,5,73,0,0,251,252,5,84,0,0,252,253,5,95,0,0,253,254,5,84,0,0,254,
		255,5,73,0,0,255,256,5,77,0,0,256,257,5,69,0,0,257,60,1,0,0,0,258,259,
		5,83,0,0,259,260,5,69,0,0,260,261,5,76,0,0,261,262,5,69,0,0,262,263,5,
		67,0,0,263,264,5,84,0,0,264,265,5,79,0,0,265,266,5,82,0,0,266,62,1,0,0,
		0,267,268,5,83,0,0,268,269,5,69,0,0,269,270,5,81,0,0,270,271,5,85,0,0,
		271,272,5,69,0,0,272,273,5,78,0,0,273,274,5,67,0,0,274,275,5,69,0,0,275,
		64,1,0,0,0,276,277,5,84,0,0,277,278,5,82,0,0,278,279,5,73,0,0,279,280,
		5,71,0,0,280,281,5,71,0,0,281,282,5,69,0,0,282,283,5,82,0,0,283,66,1,0,
		0,0,284,285,5,67,0,0,285,286,5,79,0,0,286,287,5,78,0,0,287,288,5,68,0,
		0,288,289,5,73,0,0,289,290,5,84,0,0,290,291,5,73,0,0,291,292,5,79,0,0,
		292,293,5,78,0,0,293,68,1,0,0,0,294,295,5,84,0,0,295,296,5,82,0,0,296,
		297,5,73,0,0,297,298,5,71,0,0,298,299,5,71,0,0,299,300,5,69,0,0,300,301,
		5,82,0,0,301,302,5,95,0,0,302,303,5,84,0,0,303,304,5,73,0,0,304,305,5,
		77,0,0,305,306,5,69,0,0,306,70,1,0,0,0,307,308,5,84,0,0,308,309,5,82,0,
		0,309,310,5,65,0,0,310,311,5,78,0,0,311,312,5,83,0,0,312,313,5,95,0,0,
		313,314,5,84,0,0,314,315,5,79,0,0,315,316,5,95,0,0,316,317,5,65,0,0,317,
		318,5,78,0,0,318,319,5,89,0,0,319,72,1,0,0,0,320,321,5,83,0,0,321,322,
		5,84,0,0,322,323,5,65,0,0,323,324,5,84,0,0,324,325,5,69,0,0,325,74,1,0,
		0,0,326,327,5,67,0,0,327,328,5,79,0,0,328,329,5,68,0,0,329,330,5,69,0,
		0,330,76,1,0,0,0,331,332,5,66,0,0,332,333,5,84,0,0,333,334,5,82,0,0,334,
		335,5,69,0,0,335,336,5,69,0,0,336,78,1,0,0,0,337,338,5,68,0,0,338,339,
		5,69,0,0,339,340,5,67,0,0,340,341,5,79,0,0,341,342,5,82,0,0,342,343,5,
		65,0,0,343,344,5,84,0,0,344,345,5,79,0,0,345,346,5,82,0,0,346,80,1,0,0,
		0,347,348,5,64,0,0,348,82,1,0,0,0,349,350,5,35,0,0,350,84,1,0,0,0,351,
		352,5,61,0,0,352,86,1,0,0,0,353,354,5,43,0,0,354,88,1,0,0,0,355,356,5,
		45,0,0,356,90,1,0,0,0,357,358,5,42,0,0,358,92,1,0,0,0,359,360,5,47,0,0,
		360,94,1,0,0,0,361,362,5,105,0,0,362,363,5,102,0,0,363,96,1,0,0,0,364,
		365,5,101,0,0,365,366,5,108,0,0,366,367,5,105,0,0,367,368,5,102,0,0,368,
		98,1,0,0,0,369,370,5,101,0,0,370,371,5,108,0,0,371,372,5,115,0,0,372,373,
		5,101,0,0,373,100,1,0,0,0,374,375,5,105,0,0,375,376,5,110,0,0,376,102,
		1,0,0,0,377,378,5,119,0,0,378,379,5,104,0,0,379,380,5,105,0,0,380,381,
		5,108,0,0,381,382,5,101,0,0,382,104,1,0,0,0,383,384,5,102,0,0,384,385,
		5,111,0,0,385,386,5,114,0,0,386,106,1,0,0,0,387,388,5,123,0,0,388,108,
		1,0,0,0,389,390,5,125,0,0,390,110,1,0,0,0,391,392,5,91,0,0,392,112,1,0,
		0,0,393,394,5,93,0,0,394,114,1,0,0,0,395,396,5,40,0,0,396,116,1,0,0,0,
		397,398,5,41,0,0,398,118,1,0,0,0,399,400,5,45,0,0,400,401,5,45,0,0,401,
		120,1,0,0,0,402,407,3,123,61,0,403,406,3,123,61,0,404,406,7,0,0,0,405,
		403,1,0,0,0,405,404,1,0,0,0,406,409,1,0,0,0,407,405,1,0,0,0,407,408,1,
		0,0,0,408,122,1,0,0,0,409,407,1,0,0,0,410,411,7,1,0,0,411,124,1,0,0,0,
		412,414,7,2,0,0,413,412,1,0,0,0,414,415,1,0,0,0,415,413,1,0,0,0,415,416,
		1,0,0,0,416,126,1,0,0,0,417,418,3,125,62,0,418,419,5,46,0,0,419,420,3,
		125,62,0,420,128,1,0,0,0,421,423,7,3,0,0,422,421,1,0,0,0,423,424,1,0,0,
		0,424,422,1,0,0,0,424,425,1,0,0,0,425,426,1,0,0,0,426,427,6,64,0,0,427,
		130,1,0,0,0,428,429,5,39,0,0,429,430,9,0,0,0,430,431,5,39,0,0,431,132,
		1,0,0,0,432,436,5,34,0,0,433,435,8,4,0,0,434,433,1,0,0,0,435,438,1,0,0,
		0,436,434,1,0,0,0,436,437,1,0,0,0,437,439,1,0,0,0,438,436,1,0,0,0,439,
		440,5,34,0,0,440,134,1,0,0,0,441,445,3,119,59,0,442,444,9,0,0,0,443,442,
		1,0,0,0,444,447,1,0,0,0,445,446,1,0,0,0,445,443,1,0,0,0,446,448,1,0,0,
		0,447,445,1,0,0,0,448,449,5,10,0,0,449,450,1,0,0,0,450,451,6,67,0,0,451,
		136,1,0,0,0,452,453,5,47,0,0,453,454,5,42,0,0,454,458,1,0,0,0,455,457,
		9,0,0,0,456,455,1,0,0,0,457,460,1,0,0,0,458,459,1,0,0,0,458,456,1,0,0,
		0,459,461,1,0,0,0,460,458,1,0,0,0,461,462,5,42,0,0,462,463,5,47,0,0,463,
		464,1,0,0,0,464,465,6,68,0,0,465,138,1,0,0,0,466,467,5,35,0,0,467,468,
		5,35,0,0,468,472,1,0,0,0,469,471,9,0,0,0,470,469,1,0,0,0,471,474,1,0,0,
		0,472,473,1,0,0,0,472,470,1,0,0,0,473,475,1,0,0,0,474,472,1,0,0,0,475,
		476,5,10,0,0,476,477,1,0,0,0,477,478,6,69,1,0,478,140,1,0,0,0,10,0,195,
		405,407,415,424,436,445,458,472,2,6,0,0,0,1,0
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
