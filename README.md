# Azure SDK Demo - Azure Image Analyzer

The repo demonstrates Azure SDK usage via a complete application.  This application takes in an image, uploads it to Blog Storage and enqueues a message into a Queue.  A process receives that message and uses Form Recognizer to extract the text from the image, then uses Text Analytics to get the sentiment of the text, and then stores the results in a Cosmos DB.

![](assets/hero.png)

## Pre-reqs

The following are required to run this application.

1. A Terminal - We tested this with Git Bash and WSL - it will not currently work with PowerShell or Windows Command Prompt. The Terraform deployment currently only works on Linux compatible systems. You will need to run all the commands below in your selected terminal.
1. [Install Azure CLI](https://aka.ms/azcliget)
1. [Install Terraform](https://terraform.io)
1. [Install Git](https://git-scm.com/downloads) 
1. [Install VS Code](https://code.visualstudio.com/)
1. [Install Docker](https://docs.docker.com/get-docker/)
1. An [Azure Subscription](https://azure.microsoft.com/free/)

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

## Code Setup

1. Open Git Bash or WSL - The same terminal you used to install the pre-reqs above.
1. Clone Repo
   `git clone https://github.com/jongio/azsdkdemo`
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

## Run Application

### Docker Compose
1. CD to the `src` folder for the language you would like to run, i.e. for .NET, cd to `src/net` for Python, cd to `src/python`
1. Run `docker-compose up --build`
1. Navigate to http://localhost:1080
1. Add an Image
   1. Enter url into text box and click "Submit"
   1. Or click "Add Random Meme"
   1. The image will be added to the grid. Wait for the service to pick it up. You will eventually see the text and the image border color will change indicating the image text sentiment.

### Local Kubernetes
1. Copy the values outputted from the Terraform commands above (they should be in your `.env` file if you followed the [Code Setup](#Code-Setup) steps above) into the `pac/net/k8s/local/env-configmap.yaml` file.
1. CD to the `src` folder for the language you would like to run, i.e. for .NET, cd to `src/net` for Python, cd to `src/python`.
1. Run `docker-compose build` to build the containers locally.
1. CD to `pac/net/k8s/local`.
1. Run `./mount.sh` to mount your local `.azure` folder to the container, so we can use AzureCliCredential in Kubernetes.
1. Run `kubectl apply -f .`
1. Navigate to http://localhost:31389

### Azure Kubernetes Service

1. Copy the values outputted from the Terraform commands above (they should be in your `.env` file if you followed the Code Setup steps above) into the `pac/net/k8s/aks/env-configmap.yaml` file.
1. Run the `az aks get-credentials` command that was outputted from the `terraform apply` command you ran earlier. It is something like `az aks get-credentials --resource-group azsdkdemo100rg --name azsdkdemo100aks`. Replace the resource group and cluster name with the one you created with Terraform earlier.
1. Install [Helm](https://helm.sh/) - This will be used for an nginx ingress controller that will expose a Public IP for our cluster and handle routing.
1. Run the following commands:
   ```
   helm repo add stable https://kubernetes-charts.storage.googleapis.com/
   helm install nginx stable/nginx-ingress
   ```
1. CD to the `src` folder for the language you would like to run, i.e. for .NET, cd to `src/net` for Python, cd to `src/python`.
1. Login to your container registry. `docker login` or `az acr login`.
1. Search the entire project for image names that start with `jongio/` and replace with the name of your container registry.
   > Note: This experience will be improved with Helm or Kustomize soon.
1. Run `docker-compose push` to push the containers to your container registry of choice. 
1. Run `az network public-ip list -g azsdkdemo100aksnodes --query '[0].ipAddress' --output tsv` to find the AKS cluster's public IP address.
   > Note: Change the resource group to your `node_resource_group` name, this command is also outputted by the Terraform commands.
1. Open `/pac/net/k8s/aks/web-configmap.yaml` and change the `API_ENDPOINT` value to the Public IP address.
1. CD to `/pac/net/k8s/aks` and run `kubectl apply -f .`
1. Open a browser and go to that Public IP.

