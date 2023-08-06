#export EZYFOX_SERVER_HOME=
mvn -pl . clean install
mvn -pl free-game-server-common -Pexport clean install
mvn -pl free-game-server-app-api -Pexport clean install
mvn -pl free-game-server-app-entry -Pexport clean install
mvn -pl free-game-server-plugin -Pexport clean install
cp free-game-server-zone-settings.xml $EZYFOX_SERVER_HOME/settings/zones/
