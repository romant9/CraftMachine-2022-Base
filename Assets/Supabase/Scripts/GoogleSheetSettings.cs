using UnityEngine;

namespace Supabase.TWD
{
	[CreateAssetMenu(fileName = "Supabase", menuName = "Supabase/Google Sheet Settings", order = 1)]
	public class GoogleSheetSettings : ScriptableObject
	{
		// id таблицы (из адреса url)
		public string SpreadsheetId = null!;
		// имя таблицы
		public string ApplicationName = null!;
        // развертывание appscript
#if UNITY_EDITOR
        // new - https://script.google.com/macros/s/AKfycbwBidLA9FRpXDZnfqlKulZCteJq42um7zEyq_DYHiKuSINP6f1_VfkIv2_6ePktMFwH/exec
        // old - https://script.google.com/macros/s/AKfycbz2R1wrUbHAeQs3Nym7-SLQCBcVDfKJLm10R_XhwfIznHjGiAqfiCRtnPfCjeABu9jB/exec
#endif
        public string WebAppUrl = null!;
	}
}
