# Free-MMORPG

## Server
### Setup Database

1. install mongoDB
2. open your mongosh
3. Create your Database

    ```use free```

4. Create a new user and password and give it access to the created database 
     
    ```db.createUser({user: "root", pwd: "123456",roles: [{role: "readWrite", db:"free" }] })```
 
5. Create two new collections: 

    ```db.createCollection("account", { collation: { locale: 'en_US', strength: 2 } } )```
    ```db.account.createIndex( { username: 1 } ) ```
 
    ```db.createCollection("character", { collation: { locale: 'en_US', strength: 2 } } )```
    ```db.character.createIndex( { charactername: 1 } ) ```

#### We use for local development

DB user: root

DB password: 123456

DB name: free


For production or live change the earlier created database values also in the server project you need update some values

Database values in the server project:


    Free-MMORPG\Free-Server\free-server-common\src\mail\resources\free-server-common-config.properties

E-Mail (Sending e-mails not implemented at the moment), forgot password send plain password back to the client if SERVER_CAN_SEND_MAIL = false, it`s a security issue and more for local development but keep in mind set it to true.
        
    Free-MMORPG\Free-Server\free-server-app-api\src\main\java\com.assambra.app\ServerVariables

