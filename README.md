Architecture:
Clean arch: api -> application -> domain; api -> infrastructure -> domain

Auth:
Jwt auth scheme.
Jwt auth for SignalR.

Error handling:
*TODO: Consider to add exceptio resolvers.

API layer:
Using minimal APIs.
*TODO: Consider to add some attributes.

Data access layer:
Mock database based on Singleton pattern. All apis for it are with lock (threadsafe).
*TODO: Consider to change it on some thread-safe collection like ConcurrentDictionary.
*TODO: Consider to add json file to work with for retaining users info (instead of db).

Repository methods are using delegates/expressions as a parameters.
*TODO: Consider to partially change it with regular parameters in order to have more incapsulation.

Technologies used:
Standart JWT auth. 


Next steps:
Put the registration of services in order.
Add auth registration.
Add Swagger.
Test it all.
Add OAuth 2 for Google.
