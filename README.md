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
   `git clone https://github.com/jongio/azsdkdemo`

## Azure Setup

1. Azure CLI Login
   `az login`
1. Create Azure Resources with Terraform
   1. CD to `iac/terraform`
   1. Terraform init: `terraform init`
   1. Terraform plan: `terraform plan -var="basename=azsdkdemo1" --out tf.plan`
      > Change the `basename` variable from `azsdkdemo1` to something that will be globally unique.  It will be used as part of Azure resource names, so keep it short, lowercase, and no special characters.
   1. Terraform apply: `terraform apply tf.plan`
1. Update `.env` file
   1. Copy and paste the Terraform output values to the `.env` file in the root of this repo.
      > NOTE: .env files do not allow spaces around the `=`, so please remove any spaces after you copy and paste.
   1. Set the AZURE_CONTAINER_REGISTRY_SERVER to your ACR LOGIN SERVER or DockerHub registry name.

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

## Permissions Setup
This app uses the Azure CLI login to connect to Azure resources for local development. You need to run the following script to assign the appropriate roles to the Azure CLI user.

1. CD to `iac/terraform`
1. Run `./azcliuserperms.sh {basename}`
   > You need to replace `{basename}` with the basename you used above, such as 'azsdkdemo1'.


## Run Application

### Local Docker Compose
1. CD to the `src` folder for the language you would like to run, i.e. for .NET, cd to `src/net` for Python, cd to `src/python`
1. Run Docker Compose
   1. Linux: `./docker-up.sh`
   1. Windows: `docker-compose -f docker-compose.windows.yml up --build`
1. Start Azure Function
   - Run `./startfunc.sh`
1. Navigate to http://localhost:1080
1. Add an Image
   1. Enter url into text box and click "Submit"
   1. Or click "Add Random Meme"
   1. The image will be added to the grid. Wait for the service to pick it up. You will eventually see the text and the image border color will change indicating the image text sentiment.

### Local Kubernetes
1. In Docker Desktop settings, Enable Kubernetes and setup to use WSL 2 as backend. [Docker Desktop WSL 2 backend](https://docs.docker.com/docker-for-windows/wsl/)
1. Copy the values outputted from the Terraform commands above (they should be in your `.env` file if you followed the [Code Setup](#Code-Setup) steps above) into the `pac/net/k8s/local/env-configmap.yaml` file.
1. CD to `pac/net/k8s/local`.
1. Run `./mount.sh` to mount your local `.azure` folder to the container, so we can use AzureCliCredential in Kubernetes.
1. Run `./deploy.sh`
1. Navigate to http://localhost:31389

### Azure Kubernetes Service

1. For AKS, we recommend doing another Terraform deployment in a new Terraform workspace. 
1. Copy the values outputted from Terraform to `env.prod` and `pac/net/k8s/aks/env-configmap.yaml` file.
1. Run the `az aks get-credentials` command that was outputted from the `terraform apply` command you ran earlier. It is something like `az aks get-credentials --resource-group azsdkdemo100rg --name azsdkdemo100aks`. Replace the resource group and cluster name with the one you created with Terraform earlier.
1. Install [Helm](https://helm.sh/) - This will be used for an [nginx ingress controller](https://github.com/kubernetes/ingress-nginx/tree/master/charts/ingress-nginx) that will expose a Public IP for our cluster and handle routing.
1. Run the following commands:
   ```
   helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
   helm repo add stable https://kubernetes-charts.storage.googleapis.com/
   helm repo update
   helm install nginx ingress-nginx/ingress-nginx

   # If you already have installed or get this error: Error: cannot re-use a name that is still in use

   helm upgrade --install nginx ingress-nginx/ingress-nginx
   ```
1. CD to `/pac/net/k8s/aks` and run `./deploy.sh` - this will build containers, push them to ACR, apply K8S files, and deploy the Azure Function.
1. Run `az network public-ip list -g azsdkdemo100aksnodes --query '[0].ipAddress' --output tsv` to find the AKS cluster's public IP address.
   > Note: Change the resource group to your `node_resource_group` name, this command is also outputted by the Terraform commands.
1. Open `/pac/net/k8s/aks/web-configmap.yaml` and change:
    1. `API_ENDPOINT` value to the Public IP address.
    1. `FUNCTIONS_ENDPOINT` to the URI of your functions endpoint, i.e. `https://azsdkdemoauthfunction.azurewebsites.net`
1. Rerun `./deploy.sh`
   > Note: We'll fix this soon so you won't have to run this twice.
1. Open a browser and go to the AKS cluster's Public IP.

