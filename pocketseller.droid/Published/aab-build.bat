msbuild.exe ..\..\pocketseller.sln /p:Configuration=Release /t:Clean
msbuild.exe ..\..\pocketseller.droid\pocketseller.droid.csproj /p:Configuration=Release /t:PackageForAndroid
"d:\bin\Android\sdk\build-tools\30.0.3\zipalign.exe" -p 4 ..\..\pocketseller.droid\bin\Release\ch.pocketseller.aab ..\..\pocketseller.droid\bin\Release\ch.pocketseller-signed.aab
java.exe -jar "d:\bin\android\sdk\build-tools\30.0.3\lib\apksigner.jar" sign --ks ..\..\pocketseller.droid\Published\simetrix.keystore --ks-pass pass:Tikmonca13 --ks-key-alias simetrix --key-pass pass:Tikmonca13 --min-sdk-version 23 --max-sdk-version 30  ..\..\pocketseller.droid\bin\Release\ch.pocketseller-signed.aab
copy ..\..\pocketseller.droid\bin\Release\ch.pocketseller-signed.aab ..\..\pocketseller.droid\Published\pocketseller.000.aab


