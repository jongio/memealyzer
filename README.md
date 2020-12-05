# Memealyzer

## Meme + Analyzer = Memealyzer

Memealyzer determines if a meme's sentiment is positive, negative, or neutral. 

For example, given this meme:

![](assets/meme.png)

Memealyzer will extract the text, analyize the sentiment of that text, and then change the border color to red for negative sentiment, green for positive, and yellow for neutral.

![](assets/hero.png)

Memealyzer is an app built to demonstrate some the latest and greatest Azure tech, including the new [Azure SDKs](https://aka.ms/azsdk), [Azure CLI](https://aka.ms/getazcli), Azure Functions, Azure SignalR Service, [Bicep](https://github.com/azure/bicep) or [Terraform](https://terraform.io) to provision Azure resources, [Project Tye](https://github.com/dotnet/tye/) or Docker Compose to dev, debug, and deploy, and Kubernetes.

## .NET Architecture

![](assets/arch.png)

> This is the current .NET architecture - we are in the process of developing for other languages and architectures as well.

## Azure Resources

The following Azure resources are used in this application:

1. [Resource Group](https://docs.microsoft.com/azure/azure-resource-manager/management/overview#resource-groups)
1. [Storage Account](https://docs.microsoft.com/azure/storage/common/storage-introduction)
1. [Cognitive Services Form Recognizer](https://docs.microsoft.com/azure/cognitive-services/form-recognizer/overview)
1. [Cognitive Services Text Analytics](https://azure.microsoft.com/services/cognitive-services/text-analytics/)
1. [Cosmos DB](https://docs.microsoft.com/azure/cosmos-db/introduction)
1. [Key Vault](https://azure.microsoft.com/services/key-vault/)
1. [Azure Kubernetes Service](https://docs.microsoft.com/azure/aks/)
1. [Application Insights](https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview)
1. [Azure SignalR Service](https://azure.microsoft.com/services/signalr-service/)
1. [Azure Functions](https://azure.microsoft.com/services/functions/)
1. [Azure Service Bus](https://azure.microsoft.com/services/service-bus/)


## Pre-reqs

The following are required to run this application.

1. A Terminal - WSL, GitBash, PowerShell. The Terraform deployment will not work with Windows Command Prompt because it uses shell scripts. You will need to run all the commands below in your selected terminal.
1. [Install Azure CLI](https://aka.ms/azcliget)
1. [Install Git](https://git-scm.com/downloads) 
1. [Install VS Code](https://code.visualstudio.com/)
1. [Install Docker](https://docs.docker.com/get-docker/)
1. [Azure Subscription](https://azure.microsoft.com/free/)

   ### .NET Pre-reqs

   If you are running the .NET version of this project locally, then you will need to install the following tools:

   1. [.NET SDK](https://dotnet.microsoft.com/download) - 5.0
   1. [Azure Functions Core Tools](https://docs.microsoft.com/azure/azure-functions/functions-run-local) - v3.0.2881 minimum
   1. [Project Tye](https://github.com/dotnet/tye/blob/master/docs/getting_started.md)
   1. [Bicep](https://github.com/Azure/bicep/blob/main/docs/installing.md)


## Code Setup

1. Open Terminal - The same terminal you used to install the pre-reqs above.
1. Clone Repo
   `git clone https://github.com/jongio/memealyzer`



## Quickstart

Here's how to quickly provision and deploy the app:
1. Install Pre-reqs and Code Setup above.
1. `az login` and `az account set -s {SUBSCRIPTION_NAME}`
1. Set Config: 
   1. Open `.env.prod` and change BASENAME to something unique.
1. Provision Azure Resources: 
   1. CD to `iac\bicep`
   1. Run `./deploy.sh prod cloud`
1. Deploy Application: 
   1. CD to `pac\net\tye`
   1. Run `./deploy.sh prod cloud`
1. Click on the IP ADDRESS that is outputted to the console to load Memealyzer.
1. Click + (single) or &#8734; (stream) buttons to analyze memes.


---

> The rest of this readme is further details that you can explore if you want to use Terraform instead of Bicep or Kubectl/Docker Compose instead of Project Tye.

## Azure Resource Provisioning

You have two options to provision your Azure resources:

1. [Bicep](https://github.com/azure/bicep) - A new ARM abstraction from Microsoft.
1. [Terraform](https://terraform.io) - A Azure provisioning system built on top of the Azure Go SDK.

Both are great and have their pros and cons - please research them both and decide which one you want to use.

Regardless of your choice, you need to run the Azure CLI Setup and Configuration steps below:

### Azure CLI Setup
1. Azure CLI Login

   `az login`
1. Select Azure Subscription - If you have more than one subscription, make sure you have the right one selected.

   `az account set -s {SUBSCRIPTION_NAME}`

### Configuration
1. Set BASENAME Variable
   1. Open `.env` file in the root of this project. Set the `BASENAME` setting to something unique
   1. You can also change your default `REGION` and `FAILOVER_REGION` (Used for Cosmos DB failover).



### Bicep

[Bicep](https://github.com/azure/bicep) is an Azure Resource Management (ARM) abstraction that allows you to compose JSON-like files, that get converted to ARM template files.

   1. CD to `iac\bicep`
   1. Run: `./deploy.sh {BASENAME}`, where {BASENAME} matches the .env file extension that you want to use. i.e. `./deploy.sh staging` to use the values found in the `.env.staging` file.

   Optionally, if you want to see what resources would be created if you run deploy, then you can first run `./plan.sh`

### Terraform

[Terraform](https://terraform.io) is a system that abstracts Azure resource creation and uses the Azure SDK for Go to generate the resources instead of ARM templates.

   1. [Install Terraform](https://terraform.io)
   1. CD to `iac/terraform`
   1. Terraform init: `terraform init`
   1. Terraform plan: 

      This will create a new Terraform workspace, activate it, and create a new plan file called `tf.plan`.

      1. For local dev: `./plan.sh`
      1. For staging: `./plan.sh staging`
      1. For prod: `./plan.sh prod`
      > You can pass any value as the first parameter. You just need a matching `.env.{workspace}` file in the root of the repo. 
   1. Terraform apply: `./apply.sh` - This will deploy the above resources to Azure. It will use the `tf.plan` that was generated from the previous step.

   > If you get this error: `Error validating token: IDX10223`, then run `az logout`, `az login`, and then run `./plan.sh` and `./apply.sh` again.

## Run Application

### Local

#### Project Tye
1. CD to `/pac/net/tye` and run `./run.sh`
1. Navigate to http://localhost:1080

#### Docker Compose
1. CD to the `pac/{lang}/docker` folder for the language you would like to run, i.e. for .NET, cd to `pac/net/docker`.
1. Open `.env` and set the following:
   1. `API_ENDPOINT=http://localhost:2080`
   1. `FUNCTIONS_ENDPOINT=http://localhost:3080`
1. Run Docker Compose to start the API, WebApp, Service, and Azurite containers.
   - Run: `./run.sh`
1. Start Azure Function
   - Run `./func.sh`
1. Navigate to http://localhost:1080

#### Local Kubernetes
1. In Docker Desktop settings, Enable Kubernetes and setup to use WSL 2 as backend. [Docker Desktop WSL 2 backend](https://docs.docker.com/docker-for-windows/wsl/)
1. CD to `pac/net/kubectl/local`.
1. Run `./run.sh`
1. Navigate to http://localhost:31389

### Azure Kubernetes Service (AKS)

When deploying to AKS you have two options: Project Tye or Kubectl.

If you chose Terraform to provision your Azure Resources:

   - It is recommended that you create another Azure deployment in a new Terraform workspace, so you can have a dev backend and a prod backend. i.e. `iac/terraform/plan.sh prod`, where `prod` is the name of the workspace and the name of the env file, i.e. `env.prod`.

      > See the "Azure Resource Provisioning" steps above for full Terraform deployment steps.

      > The `{basename}` value is pulled from your .env file: `TF_VAR_basename`.

      > You do not need to do this step if you are using Bicep.

#### Project Tye

- CD to `/pac/net/tye`
- Run `./deploy.sh {BASENAME} cloud`

   - This will build containers, push them to ACR, apply Kubernetes files, and deploy the Azure Function.

#### Kubectl

1. **AKS Credentials**
   - Run the following command to get the AKS cluster credentials locally:
   
   `az aks get-credentials --resource-group {basename}rg --name {basename}aks`

   > Replace `{basename}` with the basename you used when you created your Azure resources.
1. **Kubernetes Context**
   - Make sure the `K8S_CONTEXT` setting in your .env file is set to the desired value, which will be `{basename}aks`.
1. **Nginx Ingress Controller Install**
   1. Install [Helm](https://helm.sh/) - This will be used for an [nginx ingress controller](https://github.com/kubernetes/ingress-nginx/tree/master/charts/ingress-nginx) that will expose a Public IP for our cluster and handle routing.
   1. Set your kubectl context with: `kubectl config use-context {basename}aks`
   1. Run `./scripts/nginx.sh` to install the nginx-ingress controller to your AKS cluster.
1. **AKS Cluster IP Address**
   - Run `az network public-ip list -g memealyzerdevaksnodes --query '[0].ipAddress' --output tsv` to find the AKS cluster's public IP address.

1. **Deploy**: 
   - CD to `/pac/net/kubectl/aks`
   - Run `./deploy.sh {BASENAME} cloud`, where env is the name of the environment you want to deploy to, this will match your .env file in the project root. 
   
   - This will build containers, push them to ACR, apply Kubernetes files, and deploy the Azure Function.

### Add a Meme

#### Add a single meme:
1. Click on the "+" icon to add a random meme.
1. Memealyzer will analyize the sentiment of that meme and change the border color to red (negative), yellow (neutral), or green (positive) depending on the sentiment.

#### Start the memestream:
1. Click on the "&#8734;" icon to continuously add memes.


## Configuration

### Data Provider

You can configure which store the app uses to persist your image metadata, either Cosmos DB or Azure Table Storage.

1. Open `.env` file
1. Find or add the `AZURE_STORAGE_TYPE` setting
1. Set it to one of the following values:
   - `COSMOS_SQL` - This will instruct the app to use Cosmos DB.
   - `STORAGE_TABLE` - This will instruct the app to use Azure Storage Tables.

### Messaging Provider

You can configure which messaging service you want to use, either Service Bus Queue or Azure Storage Queue.

1. Open `.env` file
1. Find or add the `AZURE_MESSAGING_TYPE` setting
1. Set it to one of the following values:
   - `SERVICE_BUS_QUEUE` - This will instruct the app to use Service Bus Queue.
   - `STORAGE_QUEUE` - This will instruct the app to use Azure Storage Queue.

### Border Style

You can configure the image border style with the Azure App Configuration service. It will default to `solid`, but you can change it to any valid [CSS border style](https://www.w3schools.com/css/css_border.asp). You can either do this in the Azure Portal, or via the Azure CLI with this command:

```bash
az appconfig kv set -y -n {basename}appconfig --key borderStyle --value dashed
```

> Replace {basename} with the basename you used when you created your Azure resources above.

After you change the setting, reload the WebApp to see the new style take effect.

### All Environment Variables

You can add override any of the following environment variables to suit your needs. Memealyzer chooses smart defaults that match what is created when you deploy the app with Terraform.

|Name |Default Value|Values|
|---|---|---|
|BASENAME|This is the only variable that you are required to set.||
|AZURE_COSMOS_ENDPOINT|https://${BASENAME}cosmosaccount.documents.azure.com:443||
|AZURE_FORM_RECOGNIZER_ENDPOINT|https://${BASENAME}fr.cognitiveservices.azure.com/||
|AZURE_KEYVAULT_ENDPOINT|https://${BASENAME}kv.vault.azure.net/||
|AZURE_STORAGE_ACCOUNT_NAME|${BASENAME}storage||
|AZURE_STORAGE_BLOB_ENDPOINT|https://${BASENAME}storage.blob.core.windows.net/||
|AZURE_STORAGE_QUEUE_ENDPOINT|https://${BASENAME}storage.queue.core.windows.net/||
|AZURE_STORAGE_TABLE_ENDPOINT|https://${BASENAME}storage.table.core.windows.net/||
|AZURE_TEXT_ANALYTICS_ENDPOINT|https://${BASENAME}ta.cognitiveservices.azure.com/||
|AZURE_APP_CONFIG_ENDPOINT|https://${BASENAME}appconfig.azconfig.io||
|AZURE_CONTAINER_REGISTRY_SERVER|${BASENAME}acr.azurecr.io||
|AZURE_STORAGE_BLOB_CONTAINER_NAME|blobs||
|AZURE_MESSAGES_QUEUE_NAME|messages||
|AZURE_STORAGE_QUEUE_MSG_COUNT|10||
|AZURE_STORAGE_QUEUE_RECEIVE_SLEEP|1 second||
|AZURE_STORAGE_TABLE_NAME|images||
|AZURE_COSMOS_DB|memealyzer||
|AZURE_COSMOS_COLLECTION|images||
|AZURE_COSMOS_KEY_SECRET_NAME|CosmosKey||
|AZURE_STORAGE_TYPE|COSMOS_SQL|COSMOS_SQL, STORAGE_TABLE|
|AZURE_MESSAGING_TYPE|SERVICE_BUS_QUEUE|SERVICE_BUS_QUEUE, STORAGE_QUEUE|
|AZURE_STORAGE_KEY_SECRET_NAME|StorageKey||
|AZURE_CLIENT_SYNC_QUEUE_NAME|sync||
|AZURE_SIGNALR_CONNECTION_STRING_SECRET_NAME|SignalRConnectionString||
|AZURE_STORAGE_CONNECTION_STRING_SECRET_NAME|StorageConnectionString||
|AZURE_SERVICE_BUS_CONNECTION_STRING_SECRET_NAME|ServiceBusConnectionString||
|MEME_ENDPOINT|https://meme-api.herokuapp.com/gimme/wholesomememes||
|AZURITE_ACCOUNT_KEY|Default value in .env files||
|AZURE_COSMOS_KEY|Default value in .env files||