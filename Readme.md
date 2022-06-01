De hieronder beschreven worker ‘Messaging’ is verantwoordelijk voor alle communicatie omtrent het versturen en ophalen van berichten. 

Deze applicatie gebruikt de volgende configuratie
Secrets:
- RabbitMQHostname
- RabbitMQUsername
- RabbitMQPassword
- GOOGLE_CLIENT_ID
- MongoDBConnectionString
CORS:
Cors is ingesteld op allow all, dit is omdat de gateway verantwoordelijk is voor het verifieren van deze policy.
Prometheus stuurt deze headers mee en daardoor gaan de aanvragen anders fout.

Input
HTTP endpoints
GET: /
Hiermee worden alle berichten van een specifieke gebruiker opgehaald.
Input: 
-	string username

POST /send
Hiermee wordt een message opgeslagen in de database.
Input:
-	[FromHeader]string token
-	[FromBody] Message msg


Background Service
UserService
De userservice is verantwoordelijk voor het synchroniseren van relevante user gegevens naar de database van de messaging applicatie.

Opgeslagen data
Binnen de applicatie worden de volgende gegevens opgeslagen
User
-	Email (Benodigd omdat dit de enige waarde is die vanuit google geverifieerd kan worden)
-	Username
-	UserId (voor als er een eigen Authenticatie server wordt geimplementeerd)

Message
-	SenderId
-	Id
-	Text
-	CreationDate
