using System.Reflection;
using System.Text;
using TWDModel;

public class JsonUtils
{
	private static int GetCharacterCount(string s, char c)
	{
		int num = 0;
		char[] array = s.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == c)
			{
				num++;
			}
		}
		return num;
	}

	public static string CreateJSONBreakdown<T>(MessageSerializer serializer, T obj) where T : new()
	{
		string text = serializer.SerializeObject(new T());
		int length = text.Length;
		int characterCount = GetCharacterCount(text, ':');
		int characterCount2 = GetCharacterCount(text, '[');
		int characterCount3 = GetCharacterCount(text, '{');
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Characters, Properties, Arrays, Scopes, Name");
		FieldInfo[] fields = typeof(T).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			T val = new T();
			FieldInfo field = typeof(T).GetField(fieldInfo.Name);
			if (field.IsPublic)
			{
				field.SetValue(val, field.GetValue(obj));
				string text2 = serializer.SerializeObject(val);
				int num = text2.Length - length;
				int num2 = GetCharacterCount(text2, ':') - characterCount;
				int num3 = GetCharacterCount(text2, '[') - characterCount2;
				int num4 = GetCharacterCount(text2, '{') - characterCount3;
				stringBuilder.AppendLine(num + "," + num2 + "," + num3 + "," + num4 + "," + field.Name);
			}
		}
		return stringBuilder.ToString();
	}
}
