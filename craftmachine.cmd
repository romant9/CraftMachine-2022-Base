REG ADD "HKEY_CLASSES_ROOT\simple.oauth" /v "uri" /t REG_SZ /d %1 /f
start "" "G:\Unity Projects\TWD\EpicGames\CM_win_2022.3_Base\craftmachine.exe" %1
