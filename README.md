# Free-MMORPG
## Foreword
This project uses free resources for the server and client technology 
without the project probably wouldn't be possible, 
so my special thanks go to the guys from [Young Monkeys](https://youngmonkeys.org/ "Young Monkeys") for their great preliminary work 
and sharing their work with the community. 
Please check out our Resources Section what technologies we use and support their projects 
in some way such as donation or code so these guys can keep doing what they like, 
write good code and a great framework. 

For the game client we are using [Unity](https://unity.com "Unity") as game engine.

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


Add/Create this file to your Free-Server Java project:

``Location: Free-Server/free-server-common/src/main/resources/free-server-common-config.properties``

Insert the following values for your database and change it to your needs. 

````
database.mongo.uri=mongodb://root:123456@127.0.0.1:27017/free
database.mongo.database=free
database.mongo.collection.naming.case=UNDERSCORE
database.mongo.collection.naming.ignored_suffix=Entity
````
In this example file we use:

user: root

password 123456

database: free

### Setup Mail
#### Enable or disable server sending mail via smtp
First if you don`t have a mail server or some mail provider with an e-mail address who accept sending mails via smtp or want sending e-mails like for local development, find the file: 

``Location: Free-MMORPG\Free-Server\free-server-app-api\src\main\java\com.assambra.app\ServerVariables``

and change the value to false, SERVER_CAN_SEND_MAIL = true

Hint! ``Forgot password sending an plain password back to the client if SERVER_CAN_SEND_MAIL = false, it`s a security issue and more for local development but keep in mind set it to true and setup the mail values ``

#### Setup mail values
The same file as  like this one you used for your database settings and add additional to this file the following values and change to your needs:

``Location: Free-Server/free-server-common/src/main/resources/free-server-common-config.properties``

````
mail.host=mail.example.com
mail.port.tls=587
mail.port.ssl=465
mail.authentication=true
mail.username=account@example.com
mail.password=123456
mail.tls=true
mail.ssl=false
mail.mail.address=account@example.com
mail.mail.sender.name=YourCompany Account Management
mail.use.replay.address=false;
mail.mail.replay.address=account@example.com
mail.charset=UTF-8
mail.content.type.primary=text/HTML
mail.content.type.sub=charset=UTF-8
mail.format=flowed
mail.content.transfer.encoding=8bit
````

``Hint! Do not use SSL and TSL at the same time where both are true, there is no check and only one should be true.``

Most should be self-explanatory, if not go to the class: SMTP_EMail and look at the variables comments.

