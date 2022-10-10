# DrunkNet

## How to setup local dev environment

1. Add 127.0.0.1 drunkapp.net to your hosts-file
2. Add dotnet user-secrets for the missing configuration keys (see appsettings.json)  
This is for DrunkNet.API -folder  
Set the following configurations:  
*LiteDb:DbLocation  
Auth0:Domain  
Auth0:ClientSecret  
Auth0:ClientId  
Auth0:Audience*
3. Start DrunkNet.sln (Rider/VS) and run it
4. Build the Angular app. See README.md under the Angular app for more info
5. Open https://drunkapp.net and enjoy


## dotnet user-secrets

set new values example  
*dotnet user-secrets set "Movies:ServiceApiKey" "12345"*

list all set user-secrets  
*dotnet user-secrets list*
