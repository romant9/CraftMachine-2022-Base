using System;
using System.IO;
using UnityEngine;

#if UNITY_STANDALONE_WIN
using System.Runtime.InteropServices;
#endif

public static class UniversalClipboardManager
{
#if UNITY_STANDALONE_WIN
	// ---- НА ТИВНЫЕ ИМПОРТЫ ДЛЯ WINDOWS (Win32 API) ----
	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool OpenClipboard(IntPtr hWndNewOwner);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool EmptyClipboard();

	[DllImport("user32.dll", SetLastError = true)]
	private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool CloseClipboard();

	[DllImport("user32.dll", SetLastError = true)]
	private static extern uint RegisterClipboardFormat(string lpszFormat);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr GlobalLock(IntPtr hMem);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool GlobalUnlock(IntPtr hMem);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr GlobalFree(IntPtr hMem);

	private const uint GMEM_MOVEABLE = 0x0002;
	private const uint CF_DIB = 8;
#endif

	/// <summary>
	/// Главный универсальный метод. Принимает базовую Texture (RenderTexture, WebCamTexture и т.д.),
	/// конвертирует её в Texture2D с сохранением альфа-канала и отправляет в буфер текущей ОС.
	/// </summary>
	public static void CopyToClipboard(Texture sourceTexture)
	{
		if (sourceTexture == null)
		{
			Debug.LogError("[ClipboardManager] Исходная текстура пуста (null)!");
			return;
		}

		// 1. Конвертируем любую текстуру в читаемую Texture2D с поддержкой альфы
		Texture2D readableTex = ConvertToTexture2D(sourceTexture);

		if (readableTex == null) return;

		// 2. Отправляем в буфер обмена в зависимости от платформы
#if UNITY_STANDALONE_WIN
		CopyWindows(readableTex);
#elif UNITY_ANDROID
        CopyAndroid(readableTex);
#else
        Debug.LogWarning("[ClipboardManager] Данная платформа не поддерживается текущим скриптом.");
#endif

		// 3. Освобождаем память, если была создана временная копия
		if (readableTex != sourceTexture)
		{
			UnityEngine.Object.Destroy(readableTex);
		}
	}

	private static Texture2D ConvertToTexture2D(Texture sourceTexture)
	{
		if (sourceTexture is Texture2D tex2D && tex2D.isReadable)
		{
			return tex2D;
		}

		RenderTexture tempRT = RenderTexture.GetTemporary(
			sourceTexture.width,
			sourceTexture.height,
			0,
			RenderTextureFormat.ARGB32
		);

		RenderTexture previousRT = RenderTexture.active;
		RenderTexture.active = tempRT;
		GL.Clear(true, true, Color.clear); // Очищаем буфер в прозрачный цвет

		Graphics.Blit(sourceTexture, tempRT);

		Texture2D resultTex = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, false);
		resultTex.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
		resultTex.Apply();

		RenderTexture.active = previousRT;
		RenderTexture.ReleaseTemporary(tempRT);

		return resultTex;
	}

#if UNITY_STANDALONE_WIN
	private static void CopyWindows(Texture2D texture)
	{
		byte[] pngBytes = texture.EncodeToPNG();
		uint pngFormatCode = RegisterClipboardFormat("PNG");

		Color32[] pixels = texture.GetPixels32();
		int width = texture.width;
		int height = texture.height;

		int bmiHeaderSize = 40;
		int pixelDataSize = width * height * 4;
		byte[] dibBytes = new byte[bmiHeaderSize + pixelDataSize];

		BitConverter.GetBytes(bmiHeaderSize).CopyTo(dibBytes, 0);
		BitConverter.GetBytes(width).CopyTo(dibBytes, 4);
		BitConverter.GetBytes(height).CopyTo(dibBytes, 8);
		BitConverter.GetBytes((short)1).CopyTo(dibBytes, 12);
		BitConverter.GetBytes((short)32).CopyTo(dibBytes, 14);
		BitConverter.GetBytes(0).CopyTo(dibBytes, 18);
		BitConverter.GetBytes(pixelDataSize).CopyTo(dibBytes, 20);

		int index = bmiHeaderSize;
		for (int i = 0; i < pixels.Length; i++)
		{
			dibBytes[index] = pixels[i].b;
			dibBytes[index + 1] = pixels[i].g;
			dibBytes[index + 2] = pixels[i].r;
			dibBytes[index + 3] = pixels[i].a;
			index += 4;
		}

		if (OpenClipboard(IntPtr.Zero))
		{
			try
			{
				EmptyClipboard();

				// Формат 1: PNG для Photoshop/Telegram/Discord
				if (pngFormatCode != 0)
				{
					IntPtr hGlobalPng = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)pngBytes.Length);
					IntPtr pTargetPng = GlobalLock(hGlobalPng);
					Marshal.Copy(pngBytes, 0, pTargetPng, pngBytes.Length);
					GlobalUnlock(hGlobalPng);

					if (SetClipboardData(pngFormatCode, hGlobalPng) == IntPtr.Zero)
						GlobalFree(hGlobalPng);
				}

				// Формат 2: DIB для MS Paint / MS Office
				IntPtr hGlobalDib = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)dibBytes.Length);
				IntPtr pTargetDib = GlobalLock(hGlobalDib);
				Marshal.Copy(dibBytes, 0, pTargetDib, dibBytes.Length);
				GlobalUnlock(hGlobalDib);

				if (SetClipboardData(CF_DIB, hGlobalDib) == IntPtr.Zero)
					GlobalFree(hGlobalDib);

				Debug.Log("[ClipboardManager] Текстура успешно скопирована на Windows!");
			}
			finally
			{
				CloseClipboard();
			}
		}
	}
#endif

#if UNITY_ANDROID
    private static void CopyAndroid(Texture2D texture)
    {
        //try
        //{
        //    byte[] bytes = texture.EncodeToPNG();
        //    string fileName = "shared_clip_image.png";
        //    string filePath = Path.Combine(Application.temporaryCachePath, fileName);
        //    File.WriteAllBytes(filePath, bytes);

        //    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        //    using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        //    {
        //        using (AndroidJavaClass fileProvider = new AndroidJavaClass("androidx.core.content.FileProvider"))
        //        {
        //            string packageName = currentActivity.Call<string>("getPackageName");
        //            string authority = packageName + ".fileprovider";

        //            using (AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", filePath))
        //            {
        //                using (AndroidJavaObject uriObject = fileProvider.CallStatic<AndroidJavaObject>("getUriForFile", currentActivity, authority, fileObject))
        //                {
        //                    using (AndroidJavaObject clipboardService = currentActivity.Call<AndroidJavaObject>("getSystemService", "clipboard"))
        //                    {
        //                        using (AndroidJavaClass clipDataClass = new AndroidJavaClass("android.content.ClipData"))
        //                        {
        //                            using (AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver"))
        //                            {
        //                                using (AndroidJavaObject clipData = clipDataClass.CallStatic<AndroidJavaObject>("newUri", contentResolver, "Image", uriObject))
        //                                {
        //                                    clipboardService.Call("setPrimaryClip", clipData);
        //                                    Debug.Log("[ClipboardManager] Текстура успешно скопирована на Android!");
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError($"[ClipboardManager] Ошибка на Android: {ex.Message}");
        //}

        try
        {
            // 1. Конвертируем текстуру в PNG (сохраняет прозрачность)
            byte[] pngBytes = texture.EncodeToPNG();

            // 2. Записываем во временный файл в кэш приложения
            //string cachePath = Path.Combine(Application.temporaryCachePath, "shared_clip_img.png");
            string cachePath;
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext"))
            using (var internalCacheDir = context.Call<AndroidJavaObject>("getCacheDir")) // Гарантирует внутреннюю память
            {
                string baseCachePath = internalCacheDir.Call<string>("getAbsolutePath");
                cachePath = Path.Combine(baseCachePath, "shared_clip_img.png");
            }
            File.WriteAllBytes(cachePath, pngBytes);

            // 3. Вызываем нативный Android код
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var clipboardHelper = new AndroidJavaObject("android.content.Intent")) // Используем встроенные классы для отправки
            {
                // Получаем контекст приложения
                using (var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext"))
                {
                    // Создаем Java File объект
                    using (var file = new AndroidJavaObject("java.io.File", cachePath))
                    {
                        // Получаем URI через FileProvider (замените "your.package.name" на ваш Bundle ID)
                        string authority = context.Call<string>("getPackageName") + ".fileprovider";
                        using (var fileProvider = new AndroidJavaClass("androidx.core.content.FileProvider"))
                        using (var uri = fileProvider.CallStatic<AndroidJavaObject>("getUriForFile", context, authority, file))
                        {
                            // Получаем ClipboardManager
                            using (var clipboardService = context.Call<AndroidJavaObject>("getSystemService", "clipboard"))
                            {
                                // Создаем ClipData с MIME-типом image/png
                                using (var clipDataClass = new AndroidJavaClass("android.content.ClipData"))
                                using (var clipData = clipDataClass.CallStatic<AndroidJavaObject>("newUri", context.Call<AndroidJavaObject>("getContentResolver"), "image", uri))
                                {
                                    // Устанавливаем данные в буфер обмена
                                    clipboardService.Call("setPrimaryClip", clipData);
                                    Debug.Log("[Clipboard] Изображение с прозрачностью успешно скопировано!");
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[Clipboard] Ошибка при копировании: {e.Message}");
        }
    }
#endif

#if TEMP
	private Texture2D ConvertToTexture2D(Texture sourceTexture)
	{
		if (sourceTexture == null) return null;

		// Шаг 1: Создаем временную RenderTexture. 
		// Формат ОБЯЗАТЕЛЬНО должен быть ARGB32 (или в новых версиях Unity: GraphicsFormat.R8G8B8A8_SRGB), 
		// так как обычные форматы вроде RGB24 альфу стирают.
		RenderTexture tempRT = RenderTexture.GetTemporary(
			sourceTexture.width,
			sourceTexture.height,
			0,
			RenderTextureFormat.ARGB32 // <-- Проверяем наличие 'A' (Alpha)
		);

		// Шаг 2: Копируем данные. 
		// Чтобы прозрачность не смешивалась с черным фоном, очищаем RenderTexture перед копированием
		RenderTexture previousRT = RenderTexture.active;
		RenderTexture.active = tempRT;
		GL.Clear(true, true, Color.clear); // Очищает буфер в полностью прозрачный цвет (0,0,0,0)

		// Копируем исходную текстуру
		Graphics.Blit(sourceTexture, tempRT);

		// Шаг 3: Создаем новую Texture2D.
		// Формат ОБЯЗАТЕЛЬНО должен поддерживать альфу, например, RGBA32.
		Texture2D resultTex = new Texture2D(
			sourceTexture.width,
			sourceTexture.height,
			TextureFormat.RGBA32, // <-- Проверяем наличие 'A' (Alpha)
			false
		);

		// Читаем пиксели из активной RenderTexture
		resultTex.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
		resultTex.Apply();

		// Восстанавливаем старую RenderTexture и освобождаем память временной
		RenderTexture.active = previousRT;
		RenderTexture.ReleaseTemporary(tempRT);

		return resultTex;
	}

	public void CopyAnyTextureToBuffer(Texture generalTexture)
	{
		// 1. Конвертируем в Texture2D
		Texture2D readableTex = ConvertToTexture2D(generalTexture);

		if (readableTex != null)
		{
			// 2. Отправляем в буфер обмена (код из предыдущего ответа)
			CopyTextureWithAlphaNew(readableTex);

			// 3. Важно! Удаляем временную текстуру из памяти
			// Если вы передавали оригинальную Texture2D (без создания копии), проверьте это:
			if (readableTex != generalTexture)
			{
				UnityEngine.Object.Destroy(readableTex);
			}
		}
	}

	public static void CopyTextureWithAlpha(Texture2D texture)
	{
		if (texture == null) return;

		// 1. Получаем сырые байты PNG (с альфа-каналом)
		byte[] imageBytes = texture.EncodeToPNG();

		// 2. Регистрируем в Windows стандартный формат "PNG"
		uint pngFormatCode = RegisterClipboardFormat("PNG");
		if (pngFormatCode == 0)
		{
			Debug.LogError("Не удалось зарегистрировать формат PNG в Windows.");
			return;
		}

		// 3. Выделяем неконтролируемую (глобальную) память под размер нашего массива байт
		IntPtr hGlobal = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)imageBytes.Length);
		if (hGlobal == IntPtr.Zero)
		{
			Debug.LogError("Ошибка выделения глобальной памяти.");
			return;
		}

		// 4. Блокируем память, чтобы скопировать туда байты текстуры
		IntPtr pTarget = GlobalLock(hGlobal);
		if (pTarget == IntPtr.Zero)
		{
			GlobalFree(hGlobal);
			Debug.LogError("Не удалось заблокировать память.");
			return;
		}

		// Копируем данные из управляемого массива байт C# в выделенную память Windows
		Marshal.Copy(imageBytes, 0, pTarget, imageBytes.Length);
		GlobalUnlock(hGlobal);

		// 5. Открываем буфер обмена Windows и записываем данные
		if (OpenClipboard(IntPtr.Zero))
		{
			try
			{
				EmptyClipboard(); // Очищаем старое содержимое буфера

				// Передаем указатель на память в буфер обмена под зарегистрированным форматом "PNG"
				IntPtr hResult = SetClipboardData(pngFormatCode, hGlobal);

				if (hResult == IntPtr.Zero)
				{
					Debug.LogError($"Не удалось записать данные в буфер. Ошибка: {Marshal.GetLastWin32Error()}");
					GlobalFree(hGlobal); // Если запись не удалась, освобождаем память сами
				}
				else
				{
					Debug.Log("Текстура с альфа-каналом успешно скопирована через Native Win32 API!");
					// При успешном вызове SetClipboardData Windows сама забирает на себя 
					// управление этой памятью, вызывать GlobalFree(hGlobal) НЕЛЬЗЯ.
				}
			}
			finally
			{
				CloseClipboard(); // Обязательно закрываем буфер обмена, чтобы не подвесить систему
			}
		}
		else
		{
			GlobalFree(hGlobal);
			Debug.LogError("Не удалось открыть буфер обмена Windows.");
		}
	}

	public static void CopyTextureWithAlphaold(Texture2D texture)
	{
		if (texture == null) return;

		// 1. Получаем байты PNG с альфа-каналом
		byte[] imageBytes = texture.EncodeToPNG();

		// 2. Создаем MemoryStream БЕЗ конструкции using.
		// Он должен пережить метод SetDataObject, иначе GDI+ выдаст ошибку InvalidParameter.
		MemoryStream pngStream = new MemoryStream(imageBytes);

		try
		{
			DataObject dataObject = new DataObject();

			// 3. Регистрируем официальный формат "PNG" в Windows
			// Это гарантирует, что ОС поймет тип данных
			string pngFormatStr = DataFormats.GetFormat("PNG").Name;

			// 4. Записываем поток в DataObject
			// Флаг false означает, что данные не нужно преобразовывать в другие типы
			dataObject.SetData(pngFormatStr, false, pngStream);

			// 5. Очищаем буфер обмена перед записью
			Clipboard.Clear();

			// 6. Записываем объект. 
			// ВТОРОЙ аргумент (copy) ставим в FALSE. 
			// Если поставить true, Unity попытается принудительно сериализовать данные через GDI+, что вызовет ошибку.
			Clipboard.SetDataObject(dataObject, false);

			UnityEngine.Debug.Log("Текстура с прозрачностью успешно скопирована в буфер!");
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogError($"Ошибка Clipboard: {ex.Message}");
			// В случае ошибки обязательно чистим память вручную
			pngStream.Dispose();
		}
		// В случае успеха поток pngStream будет автоматически освобожден операционной системой Windows 
		// после того, как данные окончательно попадут в буфер обмена.
	}

	public static void CopyTexture(Texture2D texture)
	{
		if (texture == null) return;

		// Кодируем текстуру в массив байтов (PNG или JPG)
		byte[] imageBytes = texture.EncodeToPNG();

		File.WriteAllBytes(@"g:\Unity Projects\TWD\EpicGames\postgre\image.png", imageBytes);

		// Передаем в поток памяти и сохраняем в буфер Windows через System.Drawing
		using (MemoryStream ms = new MemoryStream(imageBytes))
		{
			// 3. Создаем контейнер данных для буфера Windows
			DataObject dataObject = new DataObject();

			// Записываем PNG поток напрямую. Имя формата "PNG" является общесистемным стандартом
			dataObject.SetData("PNG", false, ms);

			// 4. Дополнительно (для совместимости со старыми приложениями, которые не знают про PNG):
			// Создаем обычную картинку без альфы. Если целевой софт (например, Paint) не поймет PNG,
			// он возьмет эту обычную картинку. Photoshop/Telegram/Discord возьмут PNG с альфой.
			using (System.Drawing.Image normalImg = System.Drawing.Image.FromStream(ms))
			{
				dataObject.SetImage(normalImg);
			}

			// 5. Очищаем старый буфер и записываем наш прокачанный объект
			Clipboard.Clear();
			Clipboard.SetDataObject(dataObject, true);
		}
	}
#endif
}
