using UnityEditor;

public static class CloudBuildTargetConfigurer
{
    // Этот метод мы укажем в настройках Android-конфигурации
    public static void ConfigureAndroidIL2CPP()
    {
        Debug.Log("[CloudBuildScript] Настройка Android: переключение на IL2CPP и ARM64...");

        BuildTargetGroup targetGroup = BuildTargetGroup.Android;

        // 1. Включаем IL2CPP
        PlayerSettings.SetScriptingBackend(targetGroup, ScriptingImplementation.IL2CPP);

        // 2. Включаем архитектуры ARMv7 и ARM64 (для Google Play)
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;

        Debug.Log("[CloudBuildScript] Настройка Android успешно завершена.");
    }

    // Этот метод можно вызвать для Windows, если хотите принудительно вернуть Mono
    public static void ConfigureWindowsMono()
    {
        Debug.Log("[CloudBuildScript] Настройка Windows: переключение на Mono...");

        BuildTargetGroup targetGroup = BuildTargetGroup.Standalone;

        // Включаем Mono для Windows
        PlayerSettings.SetScriptingBackend(targetGroup, ScriptingImplementation.Mono2x);

        Debug.Log("[CloudBuildScript] Настройка Windows успешно завершена.");
    }
}
