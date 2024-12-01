:: set EZYFOX_SERVER_HOME=
mvn -pl . clean install & ^
mvn -pl master-server-common -Pexport clean install & ^
mvn -pl master-server-app-api -Pexport clean install & ^
mvn -pl master-server-app-entry -Pexport clean install & ^
mvn -pl master-server-plugin -Pexport clean install & ^
copy master-server-zone-settings.xml %EZYFOX_SERVER_HOME%/settings/zones/
