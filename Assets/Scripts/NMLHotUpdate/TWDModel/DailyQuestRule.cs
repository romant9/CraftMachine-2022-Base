using System.Collections.Generic;

namespace TWDModel
{
	public class DailyQuestRule
	{
		private class Tokenizer
		{
			public enum TokenType
			{
				None = 0,
				Identifier = 1,
				Number = 2,
				Punctuation = 3,
				Literal = 4
			}

			private string str;

			private int index;

			private TokenType tokenType;

			private int punctuationIndex;

			private string currentToken;

			private int tokenIndex;

			private static string[] punctuations = new string[14]
			{
				"==", "<=", ">=", "&&", "||", "<", ">", "{", "}", ",",
				"(", ")", "!", "%%"
			};

			private static QuestDefinitionOperator.Op[] punctuationOperations = new QuestDefinitionOperator.Op[14]
			{
				QuestDefinitionOperator.Op.Equal,
				QuestDefinitionOperator.Op.LessEqual,
				QuestDefinitionOperator.Op.GreaterEqual,
				QuestDefinitionOperator.Op.And,
				QuestDefinitionOperator.Op.Or,
				QuestDefinitionOperator.Op.Less,
				QuestDefinitionOperator.Op.Greater,
				QuestDefinitionOperator.Op.Group,
				QuestDefinitionOperator.Op.Invalid,
				QuestDefinitionOperator.Op.Invalid,
				QuestDefinitionOperator.Op.Bracket,
				QuestDefinitionOperator.Op.Invalid,
				QuestDefinitionOperator.Op.Neg,
				QuestDefinitionOperator.Op.IN
			};

			private QuestDefinitionOperator.Op operation;

			public Tokenizer(string str)
			{
				this.str = str;
				index = 0;
				tokenType = TokenType.None;
				punctuationIndex = -1;
			}

			public string ReadToken()
			{
				if (str == null)
				{
					return null;
				}
				while (index < str.Length && char.IsWhiteSpace(str[index]))
				{
					index++;
				}
				int num = index;
				TokenType tokenType = TokenType.None;
				for (punctuationIndex = -1; index < str.Length; index++)
				{
					char c = str[index];
					if (char.IsWhiteSpace(c))
					{
						break;
					}
					if (c >= '0' && c <= '9')
					{
						if (tokenType == TokenType.None)
						{
							tokenType = TokenType.Number;
						}
						if (tokenType != TokenType.Number && tokenType != TokenType.Identifier)
						{
							break;
						}
					}
					else if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_')
					{
						if (tokenType == TokenType.None)
						{
							tokenType = TokenType.Identifier;
						}
						if (tokenType != TokenType.Identifier)
						{
							break;
						}
					}
					else
					{
						if (tokenType == TokenType.None && c == '"')
						{
							num = index + 1;
							while (++index < str.Length && str[index] != '"')
							{
							}
							tokenType = TokenType.Literal;
							break;
						}
						if (tokenType != TokenType.None && tokenType != TokenType.Punctuation)
						{
							break;
						}
						for (int i = 0; i < punctuations.Length; i++)
						{
							string text = punctuations[i];
							int num2 = index - num + 1;
							int j = 0;
							bool flag = false;
							for (; j < text.Length && j < num2; j++)
							{
								if (text[j] != str[num + j])
								{
									flag = true;
									break;
								}
								tokenType = TokenType.Punctuation;
							}
							if (!flag && j < text.Length)
							{
								break;
							}
							if (j == text.Length)
							{
								punctuationIndex = i;
								index++;
								break;
							}
						}
						if (punctuationIndex >= 0)
						{
							break;
						}
					}
				}
				if (tokenType == TokenType.Punctuation && punctuationIndex < 0)
				{
					tokenType = TokenType.None;
				}
				if (index == num)
				{
					return null;
				}
				string text2 = str.Substring(num, index - num);
				this.tokenType = tokenType;
				if (this.tokenType == TokenType.Identifier)
				{
					if (text2 == "IN")
					{
						operation = QuestDefinitionOperator.Op.IN;
					}
					else
					{
						operation = QuestDefinitionOperator.Op.ContextField;
					}
				}
				else if (this.tokenType == TokenType.Number)
				{
					operation = QuestDefinitionOperator.Op.Value;
				}
				else if (this.tokenType == TokenType.Punctuation)
				{
					operation = QuestDefinitionOperator.Op.Invalid;
					if (punctuationIndex < punctuationOperations.Length)
					{
						operation = punctuationOperations[punctuationIndex];
					}
				}
				else if (this.tokenType == TokenType.Literal)
				{
					index++;
				}
				else
				{
					operation = QuestDefinitionOperator.Op.Invalid;
				}
				currentToken = text2;
				tokenIndex = num;
				return text2;
			}

			public QuestDefinitionOperator.Op GetOperation()
			{
				return operation;
			}

			public TokenType GetTokenType()
			{
				return tokenType;
			}

			public string GetCurrentToken()
			{
				return currentToken;
			}

			public int GetTokenIndex()
			{
				return tokenIndex;
			}

			public int GetPrecedence()
			{
				return (int)GetOperation();
			}
		}

		public DailyQuestDefinition Definition;

		public QuestDefinitionOperator CountsTowardsCompletionRuleCheck;

		public QuestDefinitionOperator IsAvailableRuleCheck;

		private QuestDefinitionOperator Parse(QuestCompleteContext context, Tokenizer tokenizer)
		{
			Stack<QuestDefinitionOperator> stack = new Stack<QuestDefinitionOperator>();
			string text = null;
			while ((text = tokenizer.ReadToken()) != null)
			{
				QuestDefinitionOperator.Op operation = tokenizer.GetOperation();
				if (text == ")" || text == ",")
				{
					break;
				}
				QuestDefinitionOperator questDefinitionOperator = new QuestDefinitionOperator(Definition.Id);
				questDefinitionOperator.Operation = operation;
				switch (operation)
				{
				case QuestDefinitionOperator.Op.Group:
					questDefinitionOperator.GroupValues = new List<long>();
					while ((text = tokenizer.ReadToken()) != null && !(text == "}"))
					{
						if (text == ",")
						{
							continue;
						}
						long result;
						switch (tokenizer.GetTokenType())
						{
						case Tokenizer.TokenType.Identifier:
							if (!context.ValueMap.ContainsKey(text))
							{
								result = context.StringValues.Count;
								context.StringValues.Add(text);
								context.ValueMap.Add(text, result);
							}
							else
							{
								result = context.ValueMap[text];
							}
							questDefinitionOperator.GroupValues.Add(result);
							break;
						case Tokenizer.TokenType.Number:
							long.TryParse(text, out result);
							questDefinitionOperator.GroupValues.Add(result);
							break;
						}
					}
					if (stack.Count > 0)
					{
						stack.Peek().Right = questDefinitionOperator;
					}
					stack.Push(questDefinitionOperator);
					continue;
				case QuestDefinitionOperator.Op.Bracket:
				{
					bool flag = false;
					if (stack.Count > 0 && stack.Peek().Operation == QuestDefinitionOperator.Op.Value)
					{
						QuestDefinitionOperator questDefinitionOperator2 = stack.Peek();
						flag = true;
						do
						{
							QuestDefinitionOperator questDefinitionOperator3 = Parse(context, tokenizer);
							if (questDefinitionOperator3 != null)
							{
								if (questDefinitionOperator2.Arguments == null)
								{
									questDefinitionOperator2.Arguments = new List<QuestDefinitionOperator>();
								}
								questDefinitionOperator2.Arguments.Add(questDefinitionOperator3);
							}
							questDefinitionOperator2.Operation = QuestDefinitionOperator.Op.FunctionCall;
						}
						while (!(tokenizer.GetCurrentToken() != ","));
					}
					if (!flag)
					{
						QuestDefinitionOperator questDefinitionOperator4 = Parse(context, tokenizer);
						if (stack.Count > 0)
						{
							stack.Peek().Right = questDefinitionOperator4;
						}
						stack.Push(questDefinitionOperator4);
					}
					continue;
				}
				}
				switch (tokenizer.GetTokenType())
				{
				case Tokenizer.TokenType.Identifier:
				case Tokenizer.TokenType.Literal:
				{
					long num = QuestDefinitionOperator.MapIdentifierToContextField(text);
					if (num >= 0)
					{
						questDefinitionOperator.Operation = QuestDefinitionOperator.Op.ContextField;
						questDefinitionOperator.Value = num;
						break;
					}
					questDefinitionOperator.Operation = QuestDefinitionOperator.Op.Value;
					if (!context.ValueMap.ContainsKey(text))
					{
						questDefinitionOperator.Value = context.StringValues.Count;
						context.StringValues.Add(text);
						context.ValueMap.Add(text, questDefinitionOperator.Value);
					}
					else
					{
						questDefinitionOperator.Value = context.ValueMap[text];
					}
					break;
				}
				case Tokenizer.TokenType.Number:
					questDefinitionOperator.Operation = QuestDefinitionOperator.Op.Value;
					long.TryParse(text, out questDefinitionOperator.Value);
					break;
				}
				QuestDefinitionOperator questDefinitionOperator5 = null;
				while (stack.Count > 0)
				{
					questDefinitionOperator5 = stack.Peek();
					if (questDefinitionOperator5.Precedence >= questDefinitionOperator.Precedence)
					{
						break;
					}
					stack.Pop();
					questDefinitionOperator5.Right = questDefinitionOperator.Left;
					questDefinitionOperator.Left = questDefinitionOperator5;
					if (stack.Count > 0)
					{
						stack.Peek().Right = questDefinitionOperator;
					}
				}
				if (stack.Count > 0)
				{
					stack.Peek().Right = questDefinitionOperator;
				}
				stack.Push(questDefinitionOperator);
			}
			QuestDefinitionOperator result2 = ((stack.Count > 0) ? stack.Pop() : null);
			while (stack.Count > 0)
			{
				result2 = stack.Pop();
			}
			return result2;
		}

		public bool LoadRule(DailyQuestDefinition definition, QuestCompleteContext context)
		{
			Definition = definition;
			Tokenizer tokenizer = new Tokenizer(Definition.Rule);
			CountsTowardsCompletionRuleCheck = Parse(context, tokenizer);
			tokenizer = new Tokenizer(definition.IsAvailableRule);
			IsAvailableRuleCheck = Parse(context, tokenizer);
			if (CountsTowardsCompletionRuleCheck != null)
			{
				if (!string.IsNullOrEmpty(definition.IsAvailableRule))
				{
					return IsAvailableRuleCheck != null;
				}
				return true;
			}
			return false;
		}
	}
}
