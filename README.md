# Memealyzer

Meme + Analyzer = Memealyzer

The repo demonstrates Azure SDK usage via a complete application.  This application takes in an image, uploads it to Blog Storage and enqueues a message into a Queue.  A process receives that message and uses Form Recognizer to extract the text from the image, then uses Text Analytics to get the sentiment of the text, and then stores the results in Cosmos DB or Azure Table Storage.

If the text in the image is positive then the border color will change to green, if neutral then black, and if negative it will change to red.

Download the [Azure SDK Releases](https://aka.ms/azsdk)

![](assets/hero.png)

## .NET Architecture
![](assets/arch.png)

> This is the current .NET architecture - we are in the process of developing for other languages and architectures as well.

## Pre-reqs

The following are required to run this application.

1. A Terminal - WSL, GitBash, PowerShell. The Terraform deployment will not work with Windows Command Prompt because it uses some shell scripts to fill Terraform gaps. You will need to run all the commands below in your selected terminal.
1. [Install Azure CLI](https://aka.ms/azcliget)
1. [Install Terraform](https://terraform.io)
1. [Install Git](https://git-scm.com/downloads) 
1. [Install VS Code](https://code.visualstudio.com/)
1. [Install Docker](https://docs.docker.com/get-docker/)
1. [Azure Subscription](https://azure.microsoft.com/free/)

## Azure Resources

The following Azure resources will be deployed with the Terraform script.

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


## Code Setup

1. Open Git Bash or WSL - The same terminal you used to install the pre-reqs above.
1. Clone Repo
   `git clone https://github.com/jongio/memealyzer`

## Azure Setup

1. Azure CLI Login
   `az login`
1. Select Azure Subscription - If you have more than one subscription, make sure you have the right one selected.
   `az account set -s {SUBSCRIPTION_NAME}`
1. Set Terraform Variables
   1. Open `.env` file in the root of this project. Set the `TF_VAR_basename` setting to something unique
1. Create Azure Resources with Terraform
   1. CD to `iac/terraform`
   1. Terraform init: `terraform init`
   1. Terraform plan: 
      1. For dev: `./plan.sh`
      1. For prod: `./plan.sh prod` - You can pass any value as the first parameter. You just need a matching `.env.{workspace}` file in the root of the repo. 
   1. Terraform apply: `./apply.sh` - This will deploy the above resources to Azure.

   > If you get this error: `Error validating token: IDX10223`, then run `az logout`, `az login`, and then run `./plan.sh` and `./apply.sh` again.
1. Update `.env` file
   1. Copy and paste the Terraform output values to the `.env` file in the root of this repo.
      > NOTE: .env files do not allow spaces around the `=`, so please remove any spaces after you copy and paste.

## Permissions Setup
This app uses the Azure CLI login to connect to Azure resources for local development. You need to run the following script to assign the appropriate roles to the Azure CLI user.

1. CD to `iac/terraform`
1. Run `./azcliuserperms.sh {basename}`
   > You need to replace `{basename}` with the basename you set in your `TF_VAR_basename` setting used above, such as `memealyzerdev` or `memealyzerprod`.

## .NET Local Machine Setup

If you are running the .NET versino of this project locally, then you will need to install the following tools.
1. [.NET Core SDK](https://dotnet.microsoft.com/download) - v3.1.402 minimum
1. [Azure Functions Core Tools](https://docs.microsoft.com/azure/azure-functions/functions-run-local) - v3.0.2881 minimum

## Configuration

### Data Provider

You can configure which store the app uses to store your image metadata, either Cosmos DB or Azure Table Storage.

1. Open `.env` file
1. Find the `AZURE_STORAGE_TYPE` setting
1. Set it to one of the following values:
   - `COSMOS_SQL` - This will instruct the app to use Cosmos DB.
   - `STORAGE_TABLE` - This will instruct the app to use Azure Storage Tables.  As of 8/31/2020, this option uses a dev build of the Azure Tables client library, which will go to preview soon.

### Border Style

You can configure the image border style with the Azure App Configuration service.  It will default to `solid`, but you can change it to any valid [CSS border style](https://www.w3schools.com/css/css_border.asp).  You can either do this in the Azure Portal, or via the Azure CLI with this command:

```bash
az appconfig kv set -y -n {basename}appconfig --key borderStyle --value dashed
```

> Replace {basename} with the basename you used when you created your Azure resources above.

After you change the setting, reload the WebApp to see the new style take effect.

## Run Application

### Local Docker Compose
1. CD to the `src` folder for the language you would like to run, i.e. for .NET, cd to `src/net` for Python, cd to `src/python`
1. Run Docker Compose to start the API, WebApp, Service, and Azurite containers.
   1. Linux: `./run.sh`
1. Start Azure Function
   - Run `./func.sh`
1. Navigate to http://localhost:1080
1. Add a Meme
   1. Enter meme url into the text box and click "+".
   1. Or to enter a random meme, just click "+".
   1. The image will be added to the grid. Wait for the service to pick it up. You will eventually see the text and the image border color will change indicating the image text sentiment.

### Local Kubernetes
1. In Docker Desktop settings, Enable Kubernetes and setup to use WSL 2 as backend. [Docker Desktop WSL 2 backend](https://docs.docker.com/docker-for-windows/wsl/)
1. CD to `pac/net/k8s/local`.
1. Run `./run.sh`
1. Navigate to http://localhost:31389

### Azure Kubernetes Service

1. **Terraform Workspace**: It is recommended that you create another Azure deployment in a new Terraform workspace, so you can have a dev backend and a prod backend. i.e. `iac/terraform/plan.sh prod`, where `prod` is the name of the workspace and the name of the env file, i.e. `env.prod`.

   > The `{basename}` value is pulled from your .env file: `TF_VAR_basename`.

1. **AKS Credentials**: Run the following command to get the AKS cluster credentials locally:
   
   `az aks get-credentials --resource-group {basename}rg --name {basename}aks`

   > Replace `{basename}` with the basename you used when you created your Azure resources.
1. **K8S Context**: Make sure the `K8S_CONTEXT` setting in your .env file is set to the desired value, which will be `{basename}aks`.
1. **Nginx Ingress Controller Install**
   1. Install [Helm](https://helm.sh/) - This will be used for an [nginx ingress controller](https://github.com/kubernetes/ingress-nginx/tree/master/charts/ingress-nginx) that will expose a Public IP for our cluster and handle routing.
   1. Run `./scripts/nginx.sh` to install the nginx-ingress controller to your AKS cluster.
1. **AKS Cluster IP Address**: Run `az network public-ip list -g memealyzerdevaksnodes --query '[0].ipAddress' --output tsv` to find the AKS cluster's public IP address.
1. **Update .env File**: Open `./.env` and change.  (Use `./.env.prod` for production environment)
    1. `API_ENDPOINT` value to the Public IP address URI, i.e., `https://52.250.2.137`
    1. `FUNCTIONS_ENDPOINT` to the URI of your functions endpoint, i.e. `https://memealyzerdevfunction.azurewebsites.net` - this was outputted by your Terraform run and can be found in your `.env` file.
1. **Deploy**: CD to `/pac/net/k8s/aks` and run `./deploy.sh` - this will build containers, push them to ACR, apply K8S files, and deploy the Azure Function.
1. **Run**: Open a browser and go to the AKS cluster's Public IP.

