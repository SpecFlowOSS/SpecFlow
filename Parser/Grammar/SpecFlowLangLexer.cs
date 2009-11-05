// $ANTLR 3.1.2 SpecFlowLangLexer.g 2009-11-05 16:13:46

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162
namespace  TechTalk.SpecFlow.Parser.Grammar 
{

using System;
using Antlr.Runtime;
using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;

using IDictionary	= System.Collections.IDictionary;
using Hashtable 	= System.Collections.Hashtable;

public partial class SpecFlowLangLexer : Lexer {
    public const int NEWLINECHR = 6;
    public const int T_BACKGROUND = 9;
    public const int T_THEN = 15;
    public const int T_SCENARIO_OUTLINE = 11;
    public const int MLTEXT = 18;
    public const int WSCHAR = 4;
    public const int NONNLCHR = 7;
    public const int EOF = -1;
    public const int T_AND = 16;
    public const int T_GIVEN = 13;
    public const int AT = 20;
    public const int WORDCHAR = 24;
    public const int T_BUT = 17;
    public const int WS = 22;
    public const int NEWLINE = 23;
    public const int T_WHEN = 14;
    public const int T_FEATURE = 8;
    public const int T_EXAMPLES = 12;
    public const int NONWCHR = 5;
    public const int T_SCENARIO = 10;
    public const int COMMENT = 21;
    public const int CELLSEP = 19;

    // delegates
    // delegators

    public SpecFlowLangLexer() 
    {
		InitializeCyclicDFAs();
    }
    public SpecFlowLangLexer(ICharStream input)
		: this(input, null) {
    }
    public SpecFlowLangLexer(ICharStream input, RecognizerSharedState state)
		: base(input, state) {
		InitializeCyclicDFAs(); 

    }
    
    override public string GrammarFileName
    {
    	get { return "SpecFlowLangLexer.g";} 
    }

    override public IToken NextToken() 
    {
        while (true) 
    	{
            if ( input.LA(1) == (int)CharStreamConstants.EOF ) 
    		{
                return Token.EOF_TOKEN;
            }

    	    state.token = null;
    		state.channel = Token.DEFAULT_CHANNEL;
            state.tokenStartCharIndex = input.Index();
            state.tokenStartCharPositionInLine = input.CharPositionInLine;
            state.tokenStartLine = input.Line;
    	    state.text = null;
            try 
    		{
                int m = input.Mark();
                state.backtracking = 1; 
                state.failed = false;
                mTokens();
                state.backtracking = 0;
                if ( state.failed ) 
    			{
    	            input.Rewind(m);
                    input.Consume(); 
                }
                else 
    			{
    				Emit();
                    return state.token;
                }
            }
            catch (RecognitionException re) 
    		{
                // shouldn't happen in backtracking mode, but...
                ReportError(re);
                Recover(re);
            }
        }
    }

    override public void Memoize(IIntStream input, int ruleIndex, int ruleStartIndex)
    {
    	if ( state.backtracking > 1 ) 
    		base.Memoize(input, ruleIndex, ruleStartIndex);
    }

    override public bool AlreadyParsedRule(IIntStream input, int ruleIndex)
    {
    	if ( state.backtracking>1 ) 
    		return base.AlreadyParsedRule(input, ruleIndex);
    	return false;
    }// $ANTLR start "WSCHAR"
    public void mWSCHAR() // throws RecognitionException [2]
    {
    		try
    		{
            // SpecFlowLangLexer.g:10:21: ( ( ' ' | '\\t' ) )
            // SpecFlowLangLexer.g:10:23: ( ' ' | '\\t' )
            {
            	if ( input.LA(1) == '\t' || input.LA(1) == ' ' ) 
            	{
            	    input.Consume();
            	state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "WSCHAR"

    // $ANTLR start "NONWCHR"
    public void mNONWCHR() // throws RecognitionException [2]
    {
    		try
    		{
            // SpecFlowLangLexer.g:11:21: ( ( ' ' | '\\t' | '\\r' | '\\n' | '#' | '@' ) )
            // SpecFlowLangLexer.g:11:23: ( ' ' | '\\t' | '\\r' | '\\n' | '#' | '@' )
            {
            	if ( (input.LA(1) >= '\t' && input.LA(1) <= '\n') || input.LA(1) == '\r' || input.LA(1) == ' ' || input.LA(1) == '#' || input.LA(1) == '@' ) 
            	{
            	    input.Consume();
            	state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "NONWCHR"

    // $ANTLR start "NEWLINECHR"
    public void mNEWLINECHR() // throws RecognitionException [2]
    {
    		try
    		{
            // SpecFlowLangLexer.g:12:21: ( ( '\\r' | '\\n' ) )
            // SpecFlowLangLexer.g:12:23: ( '\\r' | '\\n' )
            {
            	if ( input.LA(1) == '\n' || input.LA(1) == '\r' ) 
            	{
            	    input.Consume();
            	state.failed = false;
            	}
            	else 
            	{
            	    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "NEWLINECHR"

    // $ANTLR start "NONNLCHR"
    public void mNONNLCHR() // throws RecognitionException [2]
    {
    		try
    		{
            // SpecFlowLangLexer.g:13:21: ( ( '\\u0000' .. '\\t' ) | ( '\\u000B' .. '\\f' ) | ( '\\u000E' .. '\\uFFFF' ) )
            int alt1 = 3;
            int LA1_0 = input.LA(1);

            if ( ((LA1_0 >= '\u0000' && LA1_0 <= '\t')) )
            {
                alt1 = 1;
            }
            else if ( ((LA1_0 >= '\u000B' && LA1_0 <= '\f')) )
            {
                alt1 = 2;
            }
            else if ( ((LA1_0 >= '\u000E' && LA1_0 <= '\uFFFF')) )
            {
                alt1 = 3;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d1s0 =
                    new NoViableAltException("", 1, 0, input);

                throw nvae_d1s0;
            }
            switch (alt1) 
            {
                case 1 :
                    // SpecFlowLangLexer.g:13:23: ( '\\u0000' .. '\\t' )
                    {
                    	// SpecFlowLangLexer.g:13:23: ( '\\u0000' .. '\\t' )
                    	// SpecFlowLangLexer.g:13:24: '\\u0000' .. '\\t'
                    	{
                    		MatchRange('\u0000','\t'); if (state.failed) return ;

                    	}


                    }
                    break;
                case 2 :
                    // SpecFlowLangLexer.g:13:40: ( '\\u000B' .. '\\f' )
                    {
                    	// SpecFlowLangLexer.g:13:40: ( '\\u000B' .. '\\f' )
                    	// SpecFlowLangLexer.g:13:41: '\\u000B' .. '\\f'
                    	{
                    		MatchRange('\u000B','\f'); if (state.failed) return ;

                    	}


                    }
                    break;
                case 3 :
                    // SpecFlowLangLexer.g:13:57: ( '\\u000E' .. '\\uFFFF' )
                    {
                    	// SpecFlowLangLexer.g:13:57: ( '\\u000E' .. '\\uFFFF' )
                    	// SpecFlowLangLexer.g:13:58: '\\u000E' .. '\\uFFFF'
                    	{
                    		MatchRange('\u000E','\uFFFF'); if (state.failed) return ;

                    	}


                    }
                    break;

            }
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NONNLCHR"

    // $ANTLR start "T_FEATURE"
    public void mT_FEATURE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T_FEATURE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:16:11: ( 'Feature:' )
            // SpecFlowLangLexer.g:16:13: 'Feature:'
            {
            	Match("Feature:"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T_FEATURE"

    // $ANTLR start "T_BACKGROUND"
    public void mT_BACKGROUND() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T_BACKGROUND;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:17:14: ( 'Background:' )
            // SpecFlowLangLexer.g:17:16: 'Background:'
            {
            	Match("Background:"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T_BACKGROUND"

    // $ANTLR start "T_SCENARIO"
    public void mT_SCENARIO() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T_SCENARIO;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:18:12: ( 'Scenario:' )
            // SpecFlowLangLexer.g:18:14: 'Scenario:'
            {
            	Match("Scenario:"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T_SCENARIO"

    // $ANTLR start "T_SCENARIO_OUTLINE"
    public void mT_SCENARIO_OUTLINE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T_SCENARIO_OUTLINE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:19:20: ( 'Scenario Outline:' )
            // SpecFlowLangLexer.g:19:22: 'Scenario Outline:'
            {
            	Match("Scenario Outline:"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T_SCENARIO_OUTLINE"

    // $ANTLR start "T_EXAMPLES"
    public void mT_EXAMPLES() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T_EXAMPLES;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:20:12: ( 'Examples:' | 'Scenarios:' )
            int alt2 = 2;
            int LA2_0 = input.LA(1);

            if ( (LA2_0 == 'E') )
            {
                alt2 = 1;
            }
            else if ( (LA2_0 == 'S') )
            {
                alt2 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d2s0 =
                    new NoViableAltException("", 2, 0, input);

                throw nvae_d2s0;
            }
            switch (alt2) 
            {
                case 1 :
                    // SpecFlowLangLexer.g:20:14: 'Examples:'
                    {
                    	Match("Examples:"); if (state.failed) return ;


                    }
                    break;
                case 2 :
                    // SpecFlowLangLexer.g:20:26: 'Scenarios:'
                    {
                    	Match("Scenarios:"); if (state.failed) return ;


                    }
                    break;

            }
            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T_EXAMPLES"

    // $ANTLR start "T_GIVEN"
    public void mT_GIVEN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T_GIVEN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:21:9: ( 'Given' )
            // SpecFlowLangLexer.g:21:11: 'Given'
            {
            	Match("Given"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T_GIVEN"

    // $ANTLR start "T_WHEN"
    public void mT_WHEN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T_WHEN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:22:8: ( 'When' )
            // SpecFlowLangLexer.g:22:10: 'When'
            {
            	Match("When"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T_WHEN"

    // $ANTLR start "T_THEN"
    public void mT_THEN() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T_THEN;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:23:8: ( 'Then' )
            // SpecFlowLangLexer.g:23:10: 'Then'
            {
            	Match("Then"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T_THEN"

    // $ANTLR start "T_AND"
    public void mT_AND() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T_AND;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:24:7: ( 'And' )
            // SpecFlowLangLexer.g:24:9: 'And'
            {
            	Match("And"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T_AND"

    // $ANTLR start "T_BUT"
    public void mT_BUT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T_BUT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:25:7: ( 'But' )
            // SpecFlowLangLexer.g:25:9: 'But'
            {
            	Match("But"); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T_BUT"

    // $ANTLR start "MLTEXT"
    public void mMLTEXT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = MLTEXT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:27:9: ( '\"\"\"' )
            // SpecFlowLangLexer.g:27:11: '\"\"\"'
            {
            	Match("\"\"\""); if (state.failed) return ;


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "MLTEXT"

    // $ANTLR start "CELLSEP"
    public void mCELLSEP() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = CELLSEP;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:28:10: ( '|' )
            // SpecFlowLangLexer.g:28:12: '|'
            {
            	Match('|'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "CELLSEP"

    // $ANTLR start "AT"
    public void mAT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = AT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:29:13: ( '@' )
            // SpecFlowLangLexer.g:29:15: '@'
            {
            	Match('@'); if (state.failed) return ;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "AT"

    // $ANTLR start "COMMENT"
    public void mCOMMENT() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = COMMENT;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:30:13: ( ( WSCHAR )* '#' ( NONNLCHR )* )
            // SpecFlowLangLexer.g:30:15: ( WSCHAR )* '#' ( NONNLCHR )*
            {
            	// SpecFlowLangLexer.g:30:15: ( WSCHAR )*
            	do 
            	{
            	    int alt3 = 2;
            	    int LA3_0 = input.LA(1);

            	    if ( (LA3_0 == '\t' || LA3_0 == ' ') )
            	    {
            	        alt3 = 1;
            	    }


            	    switch (alt3) 
            		{
            			case 1 :
            			    // SpecFlowLangLexer.g:30:15: WSCHAR
            			    {
            			    	mWSCHAR(); if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop3;
            	    }
            	} while (true);

            	loop3:
            		;	// Stops C# compiler whining that label 'loop3' has no statements

            	Match('#'); if (state.failed) return ;
            	// SpecFlowLangLexer.g:30:27: ( NONNLCHR )*
            	do 
            	{
            	    int alt4 = 2;
            	    int LA4_0 = input.LA(1);

            	    if ( ((LA4_0 >= '\u0000' && LA4_0 <= '\t') || (LA4_0 >= '\u000B' && LA4_0 <= '\f') || (LA4_0 >= '\u000E' && LA4_0 <= '\uFFFF')) )
            	    {
            	        alt4 = 1;
            	    }


            	    switch (alt4) 
            		{
            			case 1 :
            			    // SpecFlowLangLexer.g:30:27: NONNLCHR
            			    {
            			    	mNONNLCHR(); if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    goto loop4;
            	    }
            	} while (true);

            	loop4:
            		;	// Stops C# compiler whining that label 'loop4' has no statements

            	if ( (state.backtracking == 1) )
            	{
            	   _channel = Token.HIDDEN_CHANNEL; 
            	}

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "COMMENT"

    // $ANTLR start "WS"
    public void mWS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:31:13: ( ( WSCHAR )+ )
            // SpecFlowLangLexer.g:31:15: ( WSCHAR )+
            {
            	// SpecFlowLangLexer.g:31:15: ( WSCHAR )+
            	int cnt5 = 0;
            	do 
            	{
            	    int alt5 = 2;
            	    int LA5_0 = input.LA(1);

            	    if ( (LA5_0 == '\t' || LA5_0 == ' ') )
            	    {
            	        alt5 = 1;
            	    }


            	    switch (alt5) 
            		{
            			case 1 :
            			    // SpecFlowLangLexer.g:31:15: WSCHAR
            			    {
            			    	mWSCHAR(); if (state.failed) return ;

            			    }
            			    break;

            			default:
            			    if ( cnt5 >= 1 ) goto loop5;
            			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            		            EarlyExitException eee5 =
            		                new EarlyExitException(5, input);
            		            throw eee5;
            	    }
            	    cnt5++;
            	} while (true);

            	loop5:
            		;	// Stops C# compiler whinging that label 'loop5' has no statements


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WS"

    // $ANTLR start "NEWLINE"
    public void mNEWLINE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = NEWLINE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:32:13: ( '\\r\\n' | '\\n' )
            int alt6 = 2;
            int LA6_0 = input.LA(1);

            if ( (LA6_0 == '\r') )
            {
                alt6 = 1;
            }
            else if ( (LA6_0 == '\n') )
            {
                alt6 = 2;
            }
            else 
            {
                if ( state.backtracking > 0 ) {state.failed = true; return ;}
                NoViableAltException nvae_d6s0 =
                    new NoViableAltException("", 6, 0, input);

                throw nvae_d6s0;
            }
            switch (alt6) 
            {
                case 1 :
                    // SpecFlowLangLexer.g:32:15: '\\r\\n'
                    {
                    	Match("\r\n"); if (state.failed) return ;


                    }
                    break;
                case 2 :
                    // SpecFlowLangLexer.g:32:24: '\\n'
                    {
                    	Match('\n'); if (state.failed) return ;

                    }
                    break;

            }
            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "NEWLINE"

    // $ANTLR start "WORDCHAR"
    public void mWORDCHAR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WORDCHAR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // SpecFlowLangLexer.g:33:13: ( ( ( '\\u0000' .. '\\b' ) | ( '\\u000B' .. '\\f' ) | ( '\\u000E' .. '\\u001F' ) | ( '!' .. '\"' ) | ( '$' .. '?' ) | ( 'A' .. '\\uFFFF' ) )+ )
            // SpecFlowLangLexer.g:33:15: ( ( '\\u0000' .. '\\b' ) | ( '\\u000B' .. '\\f' ) | ( '\\u000E' .. '\\u001F' ) | ( '!' .. '\"' ) | ( '$' .. '?' ) | ( 'A' .. '\\uFFFF' ) )+
            {
            	// SpecFlowLangLexer.g:33:15: ( ( '\\u0000' .. '\\b' ) | ( '\\u000B' .. '\\f' ) | ( '\\u000E' .. '\\u001F' ) | ( '!' .. '\"' ) | ( '$' .. '?' ) | ( 'A' .. '\\uFFFF' ) )+
            	int cnt7 = 0;
            	do 
            	{
            	    int alt7 = 7;
            	    int LA7_0 = input.LA(1);

            	    if ( ((LA7_0 >= '\u0000' && LA7_0 <= '\b')) )
            	    {
            	        alt7 = 1;
            	    }
            	    else if ( ((LA7_0 >= '\u000B' && LA7_0 <= '\f')) )
            	    {
            	        alt7 = 2;
            	    }
            	    else if ( ((LA7_0 >= '\u000E' && LA7_0 <= '\u001F')) )
            	    {
            	        alt7 = 3;
            	    }
            	    else if ( ((LA7_0 >= '!' && LA7_0 <= '\"')) )
            	    {
            	        alt7 = 4;
            	    }
            	    else if ( ((LA7_0 >= '$' && LA7_0 <= '?')) )
            	    {
            	        alt7 = 5;
            	    }
            	    else if ( ((LA7_0 >= 'A' && LA7_0 <= '\uFFFF')) )
            	    {
            	        alt7 = 6;
            	    }


            	    switch (alt7) 
            		{
            			case 1 :
            			    // SpecFlowLangLexer.g:33:16: ( '\\u0000' .. '\\b' )
            			    {
            			    	// SpecFlowLangLexer.g:33:16: ( '\\u0000' .. '\\b' )
            			    	// SpecFlowLangLexer.g:33:17: '\\u0000' .. '\\b'
            			    	{
            			    		MatchRange('\u0000','\b'); if (state.failed) return ;

            			    	}


            			    }
            			    break;
            			case 2 :
            			    // SpecFlowLangLexer.g:34:7: ( '\\u000B' .. '\\f' )
            			    {
            			    	// SpecFlowLangLexer.g:34:7: ( '\\u000B' .. '\\f' )
            			    	// SpecFlowLangLexer.g:34:8: '\\u000B' .. '\\f'
            			    	{
            			    		MatchRange('\u000B','\f'); if (state.failed) return ;

            			    	}


            			    }
            			    break;
            			case 3 :
            			    // SpecFlowLangLexer.g:35:7: ( '\\u000E' .. '\\u001F' )
            			    {
            			    	// SpecFlowLangLexer.g:35:7: ( '\\u000E' .. '\\u001F' )
            			    	// SpecFlowLangLexer.g:35:8: '\\u000E' .. '\\u001F'
            			    	{
            			    		MatchRange('\u000E','\u001F'); if (state.failed) return ;

            			    	}


            			    }
            			    break;
            			case 4 :
            			    // SpecFlowLangLexer.g:36:7: ( '!' .. '\"' )
            			    {
            			    	// SpecFlowLangLexer.g:36:7: ( '!' .. '\"' )
            			    	// SpecFlowLangLexer.g:36:8: '!' .. '\"'
            			    	{
            			    		MatchRange('!','\"'); if (state.failed) return ;

            			    	}


            			    }
            			    break;
            			case 5 :
            			    // SpecFlowLangLexer.g:37:7: ( '$' .. '?' )
            			    {
            			    	// SpecFlowLangLexer.g:37:7: ( '$' .. '?' )
            			    	// SpecFlowLangLexer.g:37:8: '$' .. '?'
            			    	{
            			    		MatchRange('$','?'); if (state.failed) return ;

            			    	}


            			    }
            			    break;
            			case 6 :
            			    // SpecFlowLangLexer.g:38:7: ( 'A' .. '\\uFFFF' )
            			    {
            			    	// SpecFlowLangLexer.g:38:7: ( 'A' .. '\\uFFFF' )
            			    	// SpecFlowLangLexer.g:38:8: 'A' .. '\\uFFFF'
            			    	{
            			    		MatchRange('A','\uFFFF'); if (state.failed) return ;

            			    	}


            			    }
            			    break;

            			default:
            			    if ( cnt7 >= 1 ) goto loop7;
            			    if ( state.backtracking > 0 ) {state.failed = true; return ;}
            		            EarlyExitException eee7 =
            		                new EarlyExitException(7, input);
            		            throw eee7;
            	    }
            	    cnt7++;
            	} while (true);

            	loop7:
            		;	// Stops C# compiler whinging that label 'loop7' has no statements


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WORDCHAR"

    override public void mTokens() // throws RecognitionException 
    {
        // SpecFlowLangLexer.g:1:39: ( T_FEATURE | T_BACKGROUND | T_SCENARIO | T_SCENARIO_OUTLINE | T_EXAMPLES | T_GIVEN | T_WHEN | T_THEN | T_AND | T_BUT | MLTEXT | CELLSEP | AT | COMMENT | WS | NEWLINE | WORDCHAR )
        int alt8 = 17;
        alt8 = dfa8.Predict(input);
        switch (alt8) 
        {
            case 1 :
                // SpecFlowLangLexer.g:1:41: T_FEATURE
                {
                	mT_FEATURE(); if (state.failed) return ;

                }
                break;
            case 2 :
                // SpecFlowLangLexer.g:1:51: T_BACKGROUND
                {
                	mT_BACKGROUND(); if (state.failed) return ;

                }
                break;
            case 3 :
                // SpecFlowLangLexer.g:1:64: T_SCENARIO
                {
                	mT_SCENARIO(); if (state.failed) return ;

                }
                break;
            case 4 :
                // SpecFlowLangLexer.g:1:75: T_SCENARIO_OUTLINE
                {
                	mT_SCENARIO_OUTLINE(); if (state.failed) return ;

                }
                break;
            case 5 :
                // SpecFlowLangLexer.g:1:94: T_EXAMPLES
                {
                	mT_EXAMPLES(); if (state.failed) return ;

                }
                break;
            case 6 :
                // SpecFlowLangLexer.g:1:105: T_GIVEN
                {
                	mT_GIVEN(); if (state.failed) return ;

                }
                break;
            case 7 :
                // SpecFlowLangLexer.g:1:113: T_WHEN
                {
                	mT_WHEN(); if (state.failed) return ;

                }
                break;
            case 8 :
                // SpecFlowLangLexer.g:1:120: T_THEN
                {
                	mT_THEN(); if (state.failed) return ;

                }
                break;
            case 9 :
                // SpecFlowLangLexer.g:1:127: T_AND
                {
                	mT_AND(); if (state.failed) return ;

                }
                break;
            case 10 :
                // SpecFlowLangLexer.g:1:133: T_BUT
                {
                	mT_BUT(); if (state.failed) return ;

                }
                break;
            case 11 :
                // SpecFlowLangLexer.g:1:139: MLTEXT
                {
                	mMLTEXT(); if (state.failed) return ;

                }
                break;
            case 12 :
                // SpecFlowLangLexer.g:1:146: CELLSEP
                {
                	mCELLSEP(); if (state.failed) return ;

                }
                break;
            case 13 :
                // SpecFlowLangLexer.g:1:154: AT
                {
                	mAT(); if (state.failed) return ;

                }
                break;
            case 14 :
                // SpecFlowLangLexer.g:1:157: COMMENT
                {
                	mCOMMENT(); if (state.failed) return ;

                }
                break;
            case 15 :
                // SpecFlowLangLexer.g:1:165: WS
                {
                	mWS(); if (state.failed) return ;

                }
                break;
            case 16 :
                // SpecFlowLangLexer.g:1:168: NEWLINE
                {
                	mNEWLINE(); if (state.failed) return ;

                }
                break;
            case 17 :
                // SpecFlowLangLexer.g:1:176: WORDCHAR
                {
                	mWORDCHAR(); if (state.failed) return ;

                }
                break;

        }

    }

    // $ANTLR start "synpred1_SpecFlowLangLexer"
    public void synpred1_SpecFlowLangLexer_fragment() {
        // SpecFlowLangLexer.g:1:41: ( T_FEATURE )
        // SpecFlowLangLexer.g:1:41: T_FEATURE
        {
        	mT_FEATURE(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred1_SpecFlowLangLexer"

    // $ANTLR start "synpred2_SpecFlowLangLexer"
    public void synpred2_SpecFlowLangLexer_fragment() {
        // SpecFlowLangLexer.g:1:51: ( T_BACKGROUND )
        // SpecFlowLangLexer.g:1:51: T_BACKGROUND
        {
        	mT_BACKGROUND(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred2_SpecFlowLangLexer"

    // $ANTLR start "synpred3_SpecFlowLangLexer"
    public void synpred3_SpecFlowLangLexer_fragment() {
        // SpecFlowLangLexer.g:1:64: ( T_SCENARIO )
        // SpecFlowLangLexer.g:1:64: T_SCENARIO
        {
        	mT_SCENARIO(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred3_SpecFlowLangLexer"

    // $ANTLR start "synpred4_SpecFlowLangLexer"
    public void synpred4_SpecFlowLangLexer_fragment() {
        // SpecFlowLangLexer.g:1:75: ( T_SCENARIO_OUTLINE )
        // SpecFlowLangLexer.g:1:75: T_SCENARIO_OUTLINE
        {
        	mT_SCENARIO_OUTLINE(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred4_SpecFlowLangLexer"

    // $ANTLR start "synpred5_SpecFlowLangLexer"
    public void synpred5_SpecFlowLangLexer_fragment() {
        // SpecFlowLangLexer.g:1:94: ( T_EXAMPLES )
        // SpecFlowLangLexer.g:1:94: T_EXAMPLES
        {
        	mT_EXAMPLES(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred5_SpecFlowLangLexer"

    // $ANTLR start "synpred6_SpecFlowLangLexer"
    public void synpred6_SpecFlowLangLexer_fragment() {
        // SpecFlowLangLexer.g:1:105: ( T_GIVEN )
        // SpecFlowLangLexer.g:1:105: T_GIVEN
        {
        	mT_GIVEN(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred6_SpecFlowLangLexer"

    // $ANTLR start "synpred7_SpecFlowLangLexer"
    public void synpred7_SpecFlowLangLexer_fragment() {
        // SpecFlowLangLexer.g:1:113: ( T_WHEN )
        // SpecFlowLangLexer.g:1:113: T_WHEN
        {
        	mT_WHEN(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred7_SpecFlowLangLexer"

    // $ANTLR start "synpred8_SpecFlowLangLexer"
    public void synpred8_SpecFlowLangLexer_fragment() {
        // SpecFlowLangLexer.g:1:120: ( T_THEN )
        // SpecFlowLangLexer.g:1:120: T_THEN
        {
        	mT_THEN(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred8_SpecFlowLangLexer"

    // $ANTLR start "synpred9_SpecFlowLangLexer"
    public void synpred9_SpecFlowLangLexer_fragment() {
        // SpecFlowLangLexer.g:1:127: ( T_AND )
        // SpecFlowLangLexer.g:1:127: T_AND
        {
        	mT_AND(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred9_SpecFlowLangLexer"

    // $ANTLR start "synpred10_SpecFlowLangLexer"
    public void synpred10_SpecFlowLangLexer_fragment() {
        // SpecFlowLangLexer.g:1:133: ( T_BUT )
        // SpecFlowLangLexer.g:1:133: T_BUT
        {
        	mT_BUT(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred10_SpecFlowLangLexer"

    // $ANTLR start "synpred11_SpecFlowLangLexer"
    public void synpred11_SpecFlowLangLexer_fragment() {
        // SpecFlowLangLexer.g:1:139: ( MLTEXT )
        // SpecFlowLangLexer.g:1:139: MLTEXT
        {
        	mMLTEXT(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred11_SpecFlowLangLexer"

    // $ANTLR start "synpred12_SpecFlowLangLexer"
    public void synpred12_SpecFlowLangLexer_fragment() {
        // SpecFlowLangLexer.g:1:146: ( CELLSEP )
        // SpecFlowLangLexer.g:1:146: CELLSEP
        {
        	mCELLSEP(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred12_SpecFlowLangLexer"

    // $ANTLR start "synpred14_SpecFlowLangLexer"
    public void synpred14_SpecFlowLangLexer_fragment() {
        // SpecFlowLangLexer.g:1:157: ( COMMENT )
        // SpecFlowLangLexer.g:1:157: COMMENT
        {
        	mCOMMENT(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred14_SpecFlowLangLexer"

    // $ANTLR start "synpred15_SpecFlowLangLexer"
    public void synpred15_SpecFlowLangLexer_fragment() {
        // SpecFlowLangLexer.g:1:165: ( WS )
        // SpecFlowLangLexer.g:1:165: WS
        {
        	mWS(); if (state.failed) return ;

        }
    }
    // $ANTLR end "synpred15_SpecFlowLangLexer"

   	public bool synpred6_SpecFlowLangLexer() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred6_SpecFlowLangLexer_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred3_SpecFlowLangLexer() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred3_SpecFlowLangLexer_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred12_SpecFlowLangLexer() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred12_SpecFlowLangLexer_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred10_SpecFlowLangLexer() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred10_SpecFlowLangLexer_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred7_SpecFlowLangLexer() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred7_SpecFlowLangLexer_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred1_SpecFlowLangLexer() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred1_SpecFlowLangLexer_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred11_SpecFlowLangLexer() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred11_SpecFlowLangLexer_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred4_SpecFlowLangLexer() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred4_SpecFlowLangLexer_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred9_SpecFlowLangLexer() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred9_SpecFlowLangLexer_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred15_SpecFlowLangLexer() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred15_SpecFlowLangLexer_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred2_SpecFlowLangLexer() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred2_SpecFlowLangLexer_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred8_SpecFlowLangLexer() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred8_SpecFlowLangLexer_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred14_SpecFlowLangLexer() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred14_SpecFlowLangLexer_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}
   	public bool synpred5_SpecFlowLangLexer() 
   	{
   	    state.backtracking++;
   	    int start = input.Mark();
   	    try 
   	    {
   	        synpred5_SpecFlowLangLexer_fragment(); // can never throw exception
   	    }
   	    catch (RecognitionException re) 
   	    {
   	        Console.Error.WriteLine("impossible: "+re);
   	    }
   	    bool success = !state.failed;
   	    input.Rewind(start);
   	    state.backtracking--;
   	    state.failed = false;
   	    return success;
   	}


    protected DFA8 dfa8;
	private void InitializeCyclicDFAs()
	{
	    this.dfa8 = new DFA8(this);
	    this.dfa8.specialStateTransitionHandler = new DFA.SpecialStateTransitionHandler(DFA8_SpecialStateTransition);
	}

    const string DFA8_eotS =
        "\x1d\uffff";
    const string DFA8_eofS =
        "\x1d\uffff";
    const string DFA8_minS =
        "\x01\x00\x01\uffff\x01\x00\x01\uffff\x01\x00\x02\uffff\x01\x00"+
        "\x01\uffff\x01\x00\x01\uffff\x01\x00\x01\uffff\x01\x00\x03\uffff"+
        "\x01\x00\x01\uffff\x01\x00\x02\uffff\x01\x00\x01\uffff\x01\x00\x02"+
        "\uffff\x01\x00\x01\uffff";
    const string DFA8_maxS =
        "\x01\uffff\x01\uffff\x01\x00\x01\uffff\x01\x00\x02\uffff\x01\x00"+
        "\x01\uffff\x01\x00\x01\uffff\x01\x00\x01\uffff\x01\x00\x03\uffff"+
        "\x01\x00\x01\uffff\x01\x00\x02\uffff\x01\x00\x01\uffff\x01\x00\x02"+
        "\uffff\x01\x00\x01\uffff";
    const string DFA8_acceptS =
        "\x01\uffff\x01\x11\x01\uffff\x01\x01\x01\uffff\x01\x06\x01\x0d"+
        "\x01\uffff\x01\x05\x01\uffff\x01\x09\x01\uffff\x01\x0b\x01\uffff"+
        "\x01\x0c\x01\x10\x01\x0e\x01\uffff\x01\x07\x01\uffff\x01\x02\x01"+
        "\x0a\x01\uffff\x01\x08\x01\uffff\x01\x03\x01\x04\x01\uffff\x01\x0f";
    const string DFA8_specialS =
        "\x01\x00\x01\uffff\x01\x01\x01\uffff\x01\x02\x02\uffff\x01\x03"+
        "\x01\uffff\x01\x04\x01\uffff\x01\x05\x01\uffff\x01\x06\x03\uffff"+
        "\x01\x07\x01\uffff\x01\x08\x02\uffff\x01\x09\x01\uffff\x01\x0a\x02"+
        "\uffff\x01\x0b\x01\uffff}>";
    static readonly string[] DFA8_transitionS = {
            "\x09\x01\x01\x1b\x01\x0f\x02\x01\x01\x0f\x12\x01\x01\x1b\x01"+
            "\x01\x01\x0b\x01\x10\x1c\x01\x01\x06\x01\x09\x01\x13\x02\x01"+
            "\x01\x07\x01\x02\x01\x04\x0b\x01\x01\x18\x01\x16\x02\x01\x01"+
            "\x11\x24\x01\x01\x0d\uff83\x01",
            "",
            "\x01\uffff",
            "",
            "\x01\uffff",
            "",
            "",
            "\x01\uffff",
            "",
            "\x01\uffff",
            "",
            "\x01\uffff",
            "",
            "\x01\uffff",
            "",
            "",
            "",
            "\x01\uffff",
            "",
            "\x01\uffff",
            "",
            "",
            "\x01\uffff",
            "",
            "\x01\uffff",
            "",
            "",
            "\x01\uffff",
            ""
    };

    static readonly short[] DFA8_eot = DFA.UnpackEncodedString(DFA8_eotS);
    static readonly short[] DFA8_eof = DFA.UnpackEncodedString(DFA8_eofS);
    static readonly char[] DFA8_min = DFA.UnpackEncodedStringToUnsignedChars(DFA8_minS);
    static readonly char[] DFA8_max = DFA.UnpackEncodedStringToUnsignedChars(DFA8_maxS);
    static readonly short[] DFA8_accept = DFA.UnpackEncodedString(DFA8_acceptS);
    static readonly short[] DFA8_special = DFA.UnpackEncodedString(DFA8_specialS);
    static readonly short[][] DFA8_transition = DFA.UnpackEncodedStringArray(DFA8_transitionS);

    protected class DFA8 : DFA
    {
        public DFA8(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 8;
            this.eot = DFA8_eot;
            this.eof = DFA8_eof;
            this.min = DFA8_min;
            this.max = DFA8_max;
            this.accept = DFA8_accept;
            this.special = DFA8_special;
            this.transition = DFA8_transition;

        }

        override public string Description
        {
            get { return "1:1: Tokens options {k=1; backtrack=true; } : ( T_FEATURE | T_BACKGROUND | T_SCENARIO | T_SCENARIO_OUTLINE | T_EXAMPLES | T_GIVEN | T_WHEN | T_THEN | T_AND | T_BUT | MLTEXT | CELLSEP | AT | COMMENT | WS | NEWLINE | WORDCHAR );"; }
        }

    }


    protected internal int DFA8_SpecialStateTransition(DFA dfa, int s, IIntStream _input) //throws NoViableAltException
    {
            IIntStream input = _input;
    	int _s = s;
        switch ( s )
        {
               	case 0 : 
                   	int LA8_0 = input.LA(1);

                   	s = -1;
                   	if ( ((LA8_0 >= '\u0000' && LA8_0 <= '\b') || (LA8_0 >= '\u000B' && LA8_0 <= '\f') || (LA8_0 >= '\u000E' && LA8_0 <= '\u001F') || LA8_0 == '!' || (LA8_0 >= '$' && LA8_0 <= '?') || (LA8_0 >= 'C' && LA8_0 <= 'D') || (LA8_0 >= 'H' && LA8_0 <= 'R') || (LA8_0 >= 'U' && LA8_0 <= 'V') || (LA8_0 >= 'X' && LA8_0 <= '{') || (LA8_0 >= '}' && LA8_0 <= '\uFFFF')) ) { s = 1; }

                   	else if ( (LA8_0 == 'F') ) { s = 2; }

                   	else if ( (LA8_0 == 'G') ) { s = 4; }

                   	else if ( (LA8_0 == '@') ) { s = 6; }

                   	else if ( (LA8_0 == 'E') ) { s = 7; }

                   	else if ( (LA8_0 == 'A') ) { s = 9; }

                   	else if ( (LA8_0 == '\"') ) { s = 11; }

                   	else if ( (LA8_0 == '|') ) { s = 13; }

                   	else if ( (LA8_0 == '\n' || LA8_0 == '\r') ) { s = 15; }

                   	else if ( (LA8_0 == '#') ) { s = 16; }

                   	else if ( (LA8_0 == 'W') ) { s = 17; }

                   	else if ( (LA8_0 == 'B') ) { s = 19; }

                   	else if ( (LA8_0 == 'T') ) { s = 22; }

                   	else if ( (LA8_0 == 'S') ) { s = 24; }

                   	else if ( (LA8_0 == '\t' || LA8_0 == ' ') ) { s = 27; }

                   	if ( s >= 0 ) return s;
                   	break;
               	case 1 : 
                   	int LA8_2 = input.LA(1);

                   	 
                   	int index8_2 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred1_SpecFlowLangLexer()) ) { s = 3; }

                   	else if ( (true) ) { s = 1; }

                   	 
                   	input.Seek(index8_2);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 2 : 
                   	int LA8_4 = input.LA(1);

                   	 
                   	int index8_4 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred6_SpecFlowLangLexer()) ) { s = 5; }

                   	else if ( (true) ) { s = 1; }

                   	 
                   	input.Seek(index8_4);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 3 : 
                   	int LA8_7 = input.LA(1);

                   	 
                   	int index8_7 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred5_SpecFlowLangLexer()) ) { s = 8; }

                   	else if ( (true) ) { s = 1; }

                   	 
                   	input.Seek(index8_7);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 4 : 
                   	int LA8_9 = input.LA(1);

                   	 
                   	int index8_9 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred9_SpecFlowLangLexer()) ) { s = 10; }

                   	else if ( (true) ) { s = 1; }

                   	 
                   	input.Seek(index8_9);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 5 : 
                   	int LA8_11 = input.LA(1);

                   	 
                   	int index8_11 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred11_SpecFlowLangLexer()) ) { s = 12; }

                   	else if ( (true) ) { s = 1; }

                   	 
                   	input.Seek(index8_11);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 6 : 
                   	int LA8_13 = input.LA(1);

                   	 
                   	int index8_13 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred12_SpecFlowLangLexer()) ) { s = 14; }

                   	else if ( (true) ) { s = 1; }

                   	 
                   	input.Seek(index8_13);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 7 : 
                   	int LA8_17 = input.LA(1);

                   	 
                   	int index8_17 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred7_SpecFlowLangLexer()) ) { s = 18; }

                   	else if ( (true) ) { s = 1; }

                   	 
                   	input.Seek(index8_17);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 8 : 
                   	int LA8_19 = input.LA(1);

                   	 
                   	int index8_19 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred2_SpecFlowLangLexer()) ) { s = 20; }

                   	else if ( (synpred10_SpecFlowLangLexer()) ) { s = 21; }

                   	else if ( (true) ) { s = 1; }

                   	 
                   	input.Seek(index8_19);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 9 : 
                   	int LA8_22 = input.LA(1);

                   	 
                   	int index8_22 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred8_SpecFlowLangLexer()) ) { s = 23; }

                   	else if ( (true) ) { s = 1; }

                   	 
                   	input.Seek(index8_22);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 10 : 
                   	int LA8_24 = input.LA(1);

                   	 
                   	int index8_24 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred3_SpecFlowLangLexer()) ) { s = 25; }

                   	else if ( (synpred4_SpecFlowLangLexer()) ) { s = 26; }

                   	else if ( (synpred5_SpecFlowLangLexer()) ) { s = 8; }

                   	else if ( (true) ) { s = 1; }

                   	 
                   	input.Seek(index8_24);
                   	if ( s >= 0 ) return s;
                   	break;
               	case 11 : 
                   	int LA8_27 = input.LA(1);

                   	 
                   	int index8_27 = input.Index();
                   	input.Rewind();
                   	s = -1;
                   	if ( (synpred14_SpecFlowLangLexer()) ) { s = 16; }

                   	else if ( (synpred15_SpecFlowLangLexer()) ) { s = 28; }

                   	 
                   	input.Seek(index8_27);
                   	if ( s >= 0 ) return s;
                   	break;
        }
        if (state.backtracking > 0) {state.failed = true; return -1;}
        NoViableAltException nvae8 =
            new NoViableAltException(dfa.Description, 8, _s, input);
        dfa.Error(nvae8);
        throw nvae8;
    }
 
    
}
}