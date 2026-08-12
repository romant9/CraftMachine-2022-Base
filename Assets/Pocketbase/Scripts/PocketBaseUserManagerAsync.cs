using PocketBaseSdk;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Pocketbase.TWD
{
    public class PocketBaseUserManagerAsync : MonoBehaviour
    {
        private string baseUrl = "http://127.0.0";

        [Serializable]
        private class PocketBaseListResponse
        {
            public List<CMUser> items;
        }

        /// <summary>
        /// Синхронизирует данные пользователя (Upsert). 
        /// Возвращает актуальные данные CMUser, полученные с сервера.
        /// </summary>
        public async Task<CMUser> SyncUserAsync(string userUID, CMUser localData)
        {
            localData.userID = userUID;

            // ШАГ 1: Проверяем существование записи по UID
            string filterUrl = $"{baseUrl}?filter=(UID='{userUID}')";

            using (UnityWebRequest getRequest = UnityWebRequest.Get(filterUrl))
            {
                // Ожидаем завершения веб-запроса в асинхронном режиме
                await getRequest.SendWebRequest();

                if (getRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Ошибка проверки пользователя: {getRequest.error}");
                    return null;
                }

                string jsonResult = getRequest.downloadHandler.text;
                var response = JsonUtility.FromJson<PocketBaseListResponse>(jsonResult);

                if (response.items != null && response.items.Count > 0)
                {
                    // ШАГ 2А: Запись НАЙДЕНА -> ОБНОВЛЯЕМ (PATCH)
                    CMUser serverData = response.items[0];
                    string recordId = serverData.userID;

                    Debug.Log($"Пользователь найден (ID записи: {recordId}). Обновляем данные...");

                    localData.TimesRun = serverData.TimesRun + 1;
                    localData.TimesConnect = serverData.TimesConnect + 1;

                    return await UpdateUserAsync(recordId, localData);
                }
                else
                {
                    // ШАГ 2Б: Запись НЕ НАЙДЕНА -> СОЗДАЕМ (POST)
                    Debug.Log("Пользователь не найден в таблице cm_users. Создаем новую запись...");

                    localData.TimesRun = 1;
                    localData.TimesConnect = 1;

                    return await CreateUserAsync(localData);
                }
            }
        }

        // Вспомогательный метод для отправки POST (Создание)
        private async Task<CMUser> CreateUserAsync(CMUser data)
        {
            string jsonPayload = JsonUtility.ToJson(data);

            using (UnityWebRequest request = new UnityWebRequest(baseUrl, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Запись успешно СОЗДАНА в cm_users!");
                    return JsonUtility.FromJson<CMUser>(request.downloadHandler.text);
                }

                Debug.LogError($"Ошибка создания записи: {request.error}\nДетали: {request.downloadHandler.text}");
                return null;
            }
        }

        // Вспомогательный метод для отправки PATCH (Обновление)
        private async Task<CMUser> UpdateUserAsync(string recordId, CMUser data)
        {
            string updateUrl = $"{baseUrl}/{recordId}";
            string jsonPayload = JsonUtility.ToJson(data);

            using (UnityWebRequest request = new UnityWebRequest(updateUrl, "PATCH"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Запись успешно ОБНОВЛЕНА в cm_users!");
                    return JsonUtility.FromJson<CMUser>(request.downloadHandler.text);
                }

                Debug.LogError($"Ошибка обновления записи: {request.error}\nДетали: {request.downloadHandler.text}");
                return null;
            }
        }
    }
}
