# ShoppingList

Blazor with a serverless backend running on Azure Functions

# Build

Use Visual Studio or Visual Studio Code

# How to run

You will need an Azure Application Registration either using Azure AD or Azure AD B2C
Fill the information into the ```appsettings.json``` file of the ShoppingList.Web project.
Optionally deploy Azure SignalR and add the connection string to the config as well

Afterwards deploy the function code (ShoppingList.Function folder) to an Azure Function App and fill the Function URL into the ```appsettings.json``` file under 
```"FunctionHost" : ""```

