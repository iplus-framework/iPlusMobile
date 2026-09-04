Remove-Item -Recurse -Force bin, obj
$env:MY_KEYSTORE_PASS="yourpass"
$env:MY_KEYSTORE_PATH="C:\Users\ivanh\AppData\Local\Xamarin\Mono for Android\Keystore\iplusmobile\iplusmobile.keystore"
dotnet publish -f net10.0-android -c ReleaseZebra
Read-Host "Press ENTER to exit"