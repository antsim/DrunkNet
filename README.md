# DrunkNet
A webapp to track consumed alcohol and blood alcohol level. Uses Auth0 for authentication and requires auth0 account and app configuration. 
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
4. Create a file 'auth.environment.ts' in the environments-folder.  
Add auth configuration values in it as follows (see app.module.ts on how they are used):

    export const authConfiguration = {
        authDomain: '',
        authAudience: '',
        clientId: '',
        apiDomain: ''
    };    

5. Build the Angular app. See README.md under the Angular app for more info
6. Open https://drunkapp.net and enjoy


## dotnet user-secrets

set new values example  
*dotnet user-secrets set "Movies:ServiceApiKey" "12345"*

list all set user-secrets  
*dotnet user-secrets list*
