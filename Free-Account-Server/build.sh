export EZYFOX_SERVER_HOME=D:/ezyfox-server
mvn -pl . clean install
mvn -pl free-account-server-common -Pexport clean install
mvn -pl free-account-server-app-api -Pexport clean install
mvn -pl free-account-server-app-entry -Pexport clean install
mvn -pl free-account-server-plugin -Pexport clean install
cp free-account-server-zone-settings.xml $EZYFOX_SERVER_HOME/settings/zones/
