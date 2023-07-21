#export EZYFOX_SERVER_HOME=
mvn -pl . clean install
mvn -pl free-server-common -Pexport clean install
mvn -pl free-server-app-api -Pexport clean install
mvn -pl free-server-app-entry -Pexport clean install
mvn -pl free-server-plugin -Pexport clean install
cp free-server-zone-settings.xml $EZYFOX_SERVER_HOME/settings/zones/
