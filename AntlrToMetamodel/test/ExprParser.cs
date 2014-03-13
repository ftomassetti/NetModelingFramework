//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.1-SNAPSHOT
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Expr.g4 by ANTLR 4.1-SNAPSHOT
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.Collections.Generic;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.1-SNAPSHOT")]
public partial class ExprParser : Parser {
	public const int
		T__5=1, T__4=2, T__3=3, T__2=4, T__1=5, T__0=6, NUMBER=7, WS=8;
	public static readonly string[] tokenNames = {
		"<INVALID>", "')'", "'+'", "'*'", "'-'", "'('", "'/'", "NUMBER", "WS"
	};
	public const int
		RULE_root = 0, RULE_expr = 1;
	public static readonly string[] ruleNames = {
		"root", "expr"
	};

	public override string GrammarFileName { get { return "Expr.g4"; } }

	public override string[] TokenNames { get { return tokenNames; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public ExprParser(ITokenStream input)
		: base(input)
	{
		_interp = new ParserATNSimulator(this,_ATN);
	}
	public partial class RootContext : ParserRuleContext {
		public ExprContext expr() {
			return GetRuleContext<ExprContext>(0);
		}
		public RootContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int GetRuleIndex() { return RULE_root; }
		public override void EnterRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.EnterRoot(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.ExitRoot(this);
		}
	}

	[RuleVersion(0)]
	public RootContext root() {
		RootContext _localctx = new RootContext(_ctx, State);
		EnterRule(_localctx, 0, RULE_root);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 4; expr(0);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.ReportError(this, re);
			_errHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ExprContext : ParserRuleContext {
		public ExprContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int GetRuleIndex() { return RULE_expr; }
	 
		public ExprContext() { }
		public virtual void CopyFrom(ExprContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class MultContext : ExprContext {
		public ExprContext left;
		public IToken @operator;
		public ExprContext right;
		public ExprContext[] expr() {
			return GetRuleContexts<ExprContext>();
		}
		public ExprContext expr(int i) {
			return GetRuleContext<ExprContext>(i);
		}
		public MultContext(ExprContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.EnterMult(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.ExitMult(this);
		}
	}
	public partial class MinusContext : ExprContext {
		public ExprContext expr() {
			return GetRuleContext<ExprContext>(0);
		}
		public MinusContext(ExprContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.EnterMinus(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.ExitMinus(this);
		}
	}
	public partial class AddContext : ExprContext {
		public ExprContext left;
		public IToken @operator;
		public ExprContext right;
		public ExprContext[] expr() {
			return GetRuleContexts<ExprContext>();
		}
		public ExprContext expr(int i) {
			return GetRuleContext<ExprContext>(i);
		}
		public AddContext(ExprContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.EnterAdd(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.ExitAdd(this);
		}
	}
	public partial class SubContext : ExprContext {
		public ExprContext left;
		public IToken @operator;
		public ExprContext right;
		public ExprContext[] expr() {
			return GetRuleContexts<ExprContext>();
		}
		public ExprContext expr(int i) {
			return GetRuleContext<ExprContext>(i);
		}
		public SubContext(ExprContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.EnterSub(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.ExitSub(this);
		}
	}
	public partial class IntContext : ExprContext {
		public ITerminalNode NUMBER() { return GetToken(ExprParser.NUMBER, 0); }
		public IntContext(ExprContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.EnterInt(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.ExitInt(this);
		}
	}
	public partial class ParenContext : ExprContext {
		public ExprContext expr() {
			return GetRuleContext<ExprContext>(0);
		}
		public ParenContext(ExprContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.EnterParen(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.ExitParen(this);
		}
	}
	public partial class DivContext : ExprContext {
		public ExprContext left;
		public IToken @operator;
		public ExprContext right;
		public ExprContext[] expr() {
			return GetRuleContexts<ExprContext>();
		}
		public ExprContext expr(int i) {
			return GetRuleContext<ExprContext>(i);
		}
		public DivContext(ExprContext context) { CopyFrom(context); }
		public override void EnterRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.EnterDiv(this);
		}
		public override void ExitRule(IParseTreeListener listener) {
			IExprListener typedListener = listener as IExprListener;
			if (typedListener != null) typedListener.ExitDiv(this);
		}
	}

	[RuleVersion(0)]
	public ExprContext expr(int _p) {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = State;
		ExprContext _localctx = new ExprContext(_ctx, _parentState);
		ExprContext _prevctx = _localctx;
		int _startState = 2;
		EnterRecursionRule(_localctx, RULE_expr, _p);
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 14;
			switch (_input.La(1)) {
			case 4:
				{
				_localctx = new MinusContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;

				State = 7; Match(4);
				State = 8; expr(3);
				}
				break;
			case NUMBER:
				{
				_localctx = new IntContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				State = 9; Match(NUMBER);
				}
				break;
			case 5:
				{
				_localctx = new ParenContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				State = 10; Match(5);
				State = 11; expr(0);
				State = 12; Match(1);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			_ctx.stop = _input.Lt(-1);
			State = 30;
			_errHandler.Sync(this);
			_alt = Interpreter.AdaptivePredict(_input,2,_ctx);
			while ( _alt!=2 && _alt!=-1 ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) TriggerExitRuleEvent();
					_prevctx = _localctx;
					{
					State = 28;
					switch ( Interpreter.AdaptivePredict(_input,1,_ctx) ) {
					case 1:
						{
						_localctx = new MultContext(new ExprContext(_parentctx, _parentState));
						((MultContext)_localctx).left = _prevctx;
						PushNewRecursionContext(_localctx, _startState, RULE_expr);
						State = 16;
						if (!(Precpred(_ctx, 7))) throw new FailedPredicateException(this, "Precpred(_ctx, 7)");
						State = 17; ((MultContext)_localctx).@operator = Match(3);
						State = 18; ((MultContext)_localctx).right = expr(8);
						}
						break;

					case 2:
						{
						_localctx = new DivContext(new ExprContext(_parentctx, _parentState));
						((DivContext)_localctx).left = _prevctx;
						PushNewRecursionContext(_localctx, _startState, RULE_expr);
						State = 19;
						if (!(Precpred(_ctx, 6))) throw new FailedPredicateException(this, "Precpred(_ctx, 6)");
						State = 20; ((DivContext)_localctx).@operator = Match(6);
						State = 21; ((DivContext)_localctx).right = expr(7);
						}
						break;

					case 3:
						{
						_localctx = new AddContext(new ExprContext(_parentctx, _parentState));
						((AddContext)_localctx).left = _prevctx;
						PushNewRecursionContext(_localctx, _startState, RULE_expr);
						State = 22;
						if (!(Precpred(_ctx, 5))) throw new FailedPredicateException(this, "Precpred(_ctx, 5)");
						State = 23; ((AddContext)_localctx).@operator = Match(2);
						State = 24; ((AddContext)_localctx).right = expr(6);
						}
						break;

					case 4:
						{
						_localctx = new SubContext(new ExprContext(_parentctx, _parentState));
						((SubContext)_localctx).left = _prevctx;
						PushNewRecursionContext(_localctx, _startState, RULE_expr);
						State = 25;
						if (!(Precpred(_ctx, 4))) throw new FailedPredicateException(this, "Precpred(_ctx, 4)");
						State = 26; ((SubContext)_localctx).@operator = Match(4);
						State = 27; ((SubContext)_localctx).right = expr(5);
						}
						break;
					}
					} 
				}
				State = 32;
				_errHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(_input,2,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.ReportError(this, re);
			_errHandler.Recover(this, re);
		}
		finally {
			UnrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public override bool Sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 1: return expr_sempred((ExprContext)_localctx, predIndex);
		}
		return true;
	}
	private bool expr_sempred(ExprContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0: return Precpred(_ctx, 7);

		case 1: return Precpred(_ctx, 6);

		case 2: return Precpred(_ctx, 5);

		case 3: return Precpred(_ctx, 4);
		}
		return true;
	}

	public static readonly string _serializedATN =
		"\x3\xB6D5\x5D61\xF22C\xAD89\x44D2\xDF97\x846A\xE419\x3\n$\x4\x2\t\x2\x4"+
		"\x3\t\x3\x3\x2\x3\x2\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x5"+
		"\x3\x11\n\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3\x3"+
		"\x3\x3\x3\x3\x3\a\x3\x1F\n\x3\f\x3\xE\x3\"\v\x3\x3\x3\x2\x2\x3\x4\x4\x2"+
		"\x2\x4\x2\x2\x2\'\x2\x6\x3\x2\x2\x2\x4\x10\x3\x2\x2\x2\x6\a\x5\x4\x3\x2"+
		"\a\x3\x3\x2\x2\x2\b\t\b\x3\x1\x2\t\n\a\x6\x2\x2\n\x11\x5\x4\x3\x5\v\x11"+
		"\a\t\x2\x2\f\r\a\a\x2\x2\r\xE\x5\x4\x3\x2\xE\xF\a\x3\x2\x2\xF\x11\x3\x2"+
		"\x2\x2\x10\b\x3\x2\x2\x2\x10\v\x3\x2\x2\x2\x10\f\x3\x2\x2\x2\x11 \x3\x2"+
		"\x2\x2\x12\x13\f\t\x2\x2\x13\x14\a\x5\x2\x2\x14\x1F\x5\x4\x3\n\x15\x16"+
		"\f\b\x2\x2\x16\x17\a\b\x2\x2\x17\x1F\x5\x4\x3\t\x18\x19\f\a\x2\x2\x19"+
		"\x1A\a\x4\x2\x2\x1A\x1F\x5\x4\x3\b\x1B\x1C\f\x6\x2\x2\x1C\x1D\a\x6\x2"+
		"\x2\x1D\x1F\x5\x4\x3\a\x1E\x12\x3\x2\x2\x2\x1E\x15\x3\x2\x2\x2\x1E\x18"+
		"\x3\x2\x2\x2\x1E\x1B\x3\x2\x2\x2\x1F\"\x3\x2\x2\x2 \x1E\x3\x2\x2\x2 !"+
		"\x3\x2\x2\x2!\x5\x3\x2\x2\x2\" \x3\x2\x2\x2\x5\x10\x1E ";
	public static readonly ATN _ATN =
		ATNSimulator.Deserialize(_serializedATN.ToCharArray());
}