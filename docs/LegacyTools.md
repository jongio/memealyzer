# Memealyzer - Legacy Tools

This page is for historical purposes only.  It will not be kept up to date, because this project has fully moved to Bicep and Project Tye instead of Docker Compose and Terraform.

## Terraform to Provision Azure Resources

[Terraform](https://terraform.io) is a system that abstracts Azure resource creation and uses the Azure SDK for Go to generate the resources instead of ARM templates.

   1. [Install Terraform](https://terraform.io)
   1. CD to `iac/terraform`
   1. Terraform init: `terraform init`
   1. Terraform plan: 
       - `./plan.sh {BASENAME}`
         This will create a new Terraform workspace, activate it, and create a new plan file called `tf.plan`.
   1. Terraform apply: 
      - `./apply.sh`
         This will deploy the above resources to Azure. It will use the `tf.plan` that was generated from the previous step.

   > If you get this error: `Error validating token: IDX10223`, then run `az logout`, `az login`, and then run `./plan.sh` and `./apply.sh` again.

## Run Application

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

1. **AKS Credentials**
   - Run the following command to get the AKS cluster credentials locally:
   
   `az aks get-credentials --resource-group {BASENAME}rg --name {BASENAME}aks`

   > Replace `{BASENAME}` with the BASENAME you used when you created your Azure resources.
1. **Kubernetes Context**
   - Make sure the `K8S_CONTEXT` setting in your .env file is set to the desired value, which will be `{BASENAME}aks`.
1. **Nginx Ingress Controller Install**
   1. Install [Helm](https://helm.sh/) - This will be used for an [nginx ingress controller](https://github.com/kubernetes/ingress-nginx/tree/master/charts/ingress-nginx) that will expose a Public IP for our cluster and handle routing.
   1. Set your kubectl context with: `kubectl config use-context {BASENAME}aks`
   1. Run `./scripts/nginx.sh` to install the nginx-ingress controller to your AKS cluster.
1. **AKS Cluster IP Address**
   - Run `az network public-ip list -g memealyzerdevaksnodes --query '[0].ipAddress' --output tsv` to find the AKS cluster's public IP address.

1. **Deploy**: 
   - CD to `/pac/net/kubectl/aks`
   - Run `./deploy.sh {BASENAME} cloud`, where env is the name of the environment you want to deploy to, this will match your .env file in the project root. 
   
   - This will build containers, push them to ACR, apply Kubernetes files, and deploy the Azure Function.