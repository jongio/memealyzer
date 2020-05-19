# Azure SDK Demo

This repo demonstrates Azure.Core HTTP Pipelines, custom policies and Azure.Identity DefaultAzureCredential in a local and cloud ASP.NET Core API project.

## Code Setup

### Clone repo

`git clone https://github.com/jongio/azsdkdemo`

### Open Projects

Open both `azsdkdemoapi` and `azsdkdemoconsole` in VS Code.

## Azure CLI

### Install Azure CLI

https://aka.ms/azcliget

### Login to Azure CLI

`az login`

## Azure Setup

### Set Active Subscription

`az account set -n {SUBSCRIPTION_NAME}`

Get subscription list with `az account list`

See current selected/default subscription with `az account show`

### Deploy Azure Resources with Terraform

We'll use Terraform to create the following Azure resources:

1. App Service Plan
1. App Service
1. Storage Account

It will also assign the "Storage Blob Data Contributor" role to the App Service Managed Identity and the to currently logged in Azure CLI user.

> See `/iac/terraform/main.tf` to see details of what is deployed with the Terraform script.

#### Install Terraform

Go to the ["Install Terraform"](https://learn.hashicorp.com/terraform/getting-started/install.html#install-terraform) to install Terraform.

#### Run Terraform

1. Open a terminal and navigate to the `terraform` folder.

1. Run the following command to setup the Terraform modules.

   `terraform init`

1. Run the following command to see what actions Terraform will take when the apply command is executed.

   `terraform plan`

1. Run the following command to create the Azure resources.

   `terraform apply`

### Deploy Azure Resources with ARM and PowerShell

1. Open a PowerShell Terminal
2. Open `/iac/arm/deployScriptExample.ps1` and change baseName, localUserName, and location to suit your needs.  localUserName is the name of the local user that is signed into PowerShell.
3. Run `/iac/arm/deployScriptExample.ps1`. Navigate to the arm folder and run `.\deployScriptExample.ps1`

## Console App Demo

### Get the Storage Aaccount Blob URI

Run the following Terraform command to retrive the storage primary endpoint.

`terraform output storage_uri`

### Set Storage Account Blob URI

1. Rename `/src/azsdkdemoconsole/.env.tmp` to `.env` and open it.
1. Set the `AZURE_STORAGE_BLOB_URI` variable to the full storage account blob url from the previous step.

### Azure CLI Login

1. Run `az login` so `DefaultAzureCredential` uses your Azure account to authenticate.

### Open the code and explain it

1. All the code can be found in `Program.cs`. Open it and explain the BlobClientOptions, HttpPipeline.

### Run the app

1. Hit F5 to debug the application and step through it with your audience.

## API Demo

### Set Storage Account Blob URI

1. Open `/src/azsdkdemoapi/.vscode/launch.json` and set:
 - `configuration/env/AZUREBLOBSTORAGE:SERVICEURI` to the full storage account blob url returned from the terraform deployment.
 - `configuration/env/APPINSIGHTS_INSTRUMENTATIONKEY` to the App Insights key returned from the terraform deployment.


### Run App Locally

1. Hit F5
1. View the Output in Browser.
1. View the Output in VS Code Output Console.
1. Show that Azure CLI Credential was used.

   You will see the following lines in the output:

   ```cmd
   info: Azure-Identity[1]
       AzureCliCredential.GetToken invoked. Scopes: [ https://storage.azure.com/.default ] ParentRequestId: c7e8b2b7-a86d-4710-a10e-99f44eedb8bc
   info: Azure-Identity[2]
       AzureCliCredential.GetToken succeeded. Scopes: [ https://storage.azure.com/.default ] ParentRequestId: c7e8b2b7-a86d-4710-a10e-99f44eedb8bc ExpiresOn: 2020-03-18T23:12:27.2820620+00:00
   info: Azure-Identity[2]
       DefaultAzureCredential.GetToken succeeded. Scopes: [ https://storage.azure.com/.default ] ParentRequestId: c7e8b2b7-a86d-4710-a10e-99f44eedb8bc ExpiresOn: 2020-03-18T23:12:27.2820620+00:00
   ```

1. Show the custom pipeline log entries.

   You will see the following lines in the output:

   ```cmd
   info: SimpleTracingPolicy[0]
     >> Response: 206 from GET https://azsdkdemostorage.blob.core.windows.net/blobs/blob.txt
   ```

### Deploy to Azure

1. Open the VS Code Azure Extension.
1. Under APP SERVICE, right-click on your app and select 'Deploy to Web App'.

### Start Log Streaming

1. Open the VS Code Azure Extension.
1. Under APP SERVICE, right-click on your app and select 'Start Streaming Logs'

### Run App on Azure

1. Open a browser and hit the web api URL: https://azsdkdemoapp.azurewebsites.net/api/blob or https://{APP_NAME}.azurewebsites.net/api/blob
1. View the output in the browser.

### View the Log Stream

1. Back to VS Code, go to the log stream and show that the policy logs are there and that Managed Identity was successfully used.

   You will see the following lines for the custom policy:

   ```cmd
   2020-03-18 22:48:13.757 +00:00 [Information] SimpleTracingPolicy: >> Response: 206 from GET https://azsdkdemostorage.blob.core.windows.net/blobs/blob.txt
   ```

   You will see the following lines for Managed Identity:

   ```cmd
   2020-03-18 22:48:13.295 +00:00 [Information] Azure-Identity: ManagedIdentityCredential.GetToken succeeded. Scopes: [ https://storage.azure.com/.default ] ParentRequestId: 1fce14b9-720a-45a0-af21-146098f3ec25 ExpiresOn: 2020-03-19T01:49:40.0000000+00:00

   2020-03-18 22:48:13.297 +00:00 [Information] Azure-Identity: DefaultAzureCredential.GetToken succeeded. Scopes: [ https://storage.azure.com/.default ] ParentRequestId: 1fce14b9-720a-45a0-af21-146098f3ec25 ExpiresOn: 2020-03-19T01:49:40.0000000+00:00
   ```

   If you do not see those lines, that means that you started the log streaming after the first request. Since tokens are now obtained at the app level, you'll only see those Identity related messages once per app start, not once per request.

### View the Logs in App Insights

1. Go to App Insights in the Portal, click Logs, and then click the Trace query.  You will see the Azure Identity logs.  Be patient as it could take a few minutes from when the API was hit to when the trace appears.