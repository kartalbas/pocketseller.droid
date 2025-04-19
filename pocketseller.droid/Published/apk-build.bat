msbuild.exe ..\..\pocketseller.sln /p:Configuration=Release /t:Clean
msbuild.exe ..\..\pocketseller.droid\pocketseller.droid.csproj /p:Configuration=Release /t:PackageForAndroid
"C:\Program Files (x86)\Android\android-sdk\build-tools\32.0.0\zipalign.exe" -p 4 ..\..\pocketseller.droid\bin\Release\ch.pocketseller.apk ..\..\pocketseller.droid\bin\Release\ch.pocketseller-signed.apk
java.exe -jar "C:\Program Files (x86)\Android\android-sdk\build-tools\32.0.0\lib\apksigner.jar" sign --ks ..\..\pocketseller.droid\Published\kartalbas.keystore --ks-pass pass:kartalbas --ks-key-alias kartalbas --key-pass pass:kartalbas --min-sdk-version 23 --max-sdk-version 30  ..\..\pocketseller.droid\bin\Release\ch.pocketseller-signed.apk
copy ..\..\pocketseller.droid\bin\Release\ch.pocketseller-signed.apk ..\..\pocketseller.droid\Published\pocketseller.000.apk


