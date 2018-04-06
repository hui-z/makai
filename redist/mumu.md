adb connect 127.0.0.1:7555
adb -s 127.0.0.1:7555 shell am start -a android.intent.action.MAIN -n com.android.settings/.wifi.WifiSettings
