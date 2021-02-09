msbuild.exe ..\..\orderline.sln /p:Configuration=Release /t:Clean
msbuild.exe ..\..\orderline.droid\orderline.droid.csproj /p:Configuration=Release /t:PackageForAndroid
"d:\bin\Android\sdk\build-tools\30.0.3\zipalign.exe" -p 4 ..\..\orderline.droid\bin\Release\orderline.droid.apk ..\..\orderline.droid\bin\Release\orderline.droid-signed.apk
java.exe -jar "d:\bin\android\sdk\build-tools\30.0.3\lib\apksigner.jar" sign --ks ..\..\orderline.droid\Published\simetrix.keystore --ks-pass pass:Tikmonca13 --ks-key-alias simetrix --key-pass pass:Tikmonca13 --min-sdk-version 23 --max-sdk-version 30  ..\..\orderline.droid\bin\Release\orderline.droid-signed.apk
copy ..\..\orderline.droid\bin\Release\orderline.droid-signed.apk ..\..\orderline.droid\Published\orderline.droid.000.apk


