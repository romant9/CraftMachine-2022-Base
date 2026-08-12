using Newtonsoft.Json;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using System;
using System.IO;
using UnityEngine;

public class SupabaseSessionHandler : IGotrueSessionPersistence<Session>
{
    private readonly string _sessionFilePath = "";

    public SupabaseSessionHandler()
    {
        var _sessionFileDir = Application.persistentDataPath + "\\Supabase\\";
        if (!Directory.Exists(_sessionFileDir)) Directory.CreateDirectory(_sessionFileDir);
        _sessionFilePath = _sessionFileDir + "supabase_session.json";
    }

    public void SaveSession(Session session)
    {
        var json = JsonConvert.SerializeObject(session);
        File.WriteAllText(_sessionFilePath, json);
    }

    // Удаление сессии при выходе (SignOut)
    public void DestroySession()
    {
        if (File.Exists(_sessionFilePath))
        {
            File.Delete(_sessionFilePath);
        }
    }

    // Загрузка сессии при старте приложения
    public Session LoadSession()
    {
        if (!File.Exists(_sessionFilePath)) return null;

        try
        {
            var json = File.ReadAllText(_sessionFilePath);

            var session = JsonConvert.DeserializeObject<Session>(json);

            if (session != null && !string.IsNullOrEmpty(session.RefreshToken))
            {
                // КОСТЫЛЬ ДЛЯ Unity/C# SDK: 
                // Искусственно сдвигаем локальное время истечения токена в будущее.
                // Это заставит SDK пропустить внутреннюю ошибку "Expired" и 
                // передать refresh_token на сервер Supabase в методе RetrieveSessionAsync().

                // Задаем дату создания — прямо сейчас в UTC
                session.CreatedAt = DateTime.UtcNow.ToLocalTime();

                // Говорим, что access_token будет жить еще час
                session.ExpiresIn = 3600;
            }
            return session;
        }
        catch
        {
            return null;
        }
    }
}
