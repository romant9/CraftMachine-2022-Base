using System;

namespace TWDModel
{
	[Serializable]
	public class SteamStoreConfig
	{
		public string ProductID;

		public string Des_en;

		public string Des_zh_CN;

		public string Des_zh_TW;

		public string Des_ja;

		public string Des_ko;

		public string Des_de;

		public string Des_fr;

		public string Des_es;

		public string Des_it;

		public string Des_pt_BR;

		public string Des_ru;

		public string Des_tr;

		public string Title_en;

		public string Title_zh_CN;

		public string Title_zh_TW;

		public string Title_ja;

		public string Title_ko;

		public string Title_de;

		public string Title_fr;

		public string Title_es;

		public string Title_it;

		public string Title_pt_BR;

		public string Title_ru;

		public string Title_tr;

		public int USD;

		public int AUD;

		public int BRL;

		public int CAD;

		public int CLP;

		public int CNY;

		public int COP;

		public int CRC;

		public int EUR;

		public int HKD;

		public int INR;

		public int IDR;

		public int ILS;

		public int JPY;

		public int KZT;

		public int MYR;

		public int MXN;

		public int NZD;

		public int NOK;

		public int PEN;

		public int PHP;

		public int PLN;

		public int QAR;

		public int RUB;

		public int SAR;

		public int SGD;

		public int ZAR;

		public int KRW;

		public int CHF;

		public int TWD;

		public int THB;

		public int AED;

		public int GBP;

		public int VND;

		public object GetFieldValue(string field)
		{
			return GetType().GetField(field)?.GetValue(this);
		}
	}
}
