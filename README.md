# Azure SDK Demo - Azure Image Analyzer

The repo demonstrates Azure SDK usage via a complete application.  This application takes in an image, uploads it to Blog Storage and enqueues a message into a Queue.  A process receives that message and uses Form Recognizer to extract the text from the image, then uses Text Analytics to get the sentiment of the text, and then stores the results in a Cosmos DB.

![](assets/hero.png)

## Pre-reqs

The following are required to run this application.

1. [Install Azure CLI](https://aka.ms/azcliget)
1. [Install Terraform](https://terraform.io)
1. [Install Git](https://git-scm.com/downloads) - Make sure you install Git Bash or have WSL
1. [Install VS Code](https://code.visualstudio.com/)
1. [Install Docker](https://docs.docker.com/get-docker/)
1. An Azure Subscription

## Azure Resources

The following Azure resources will be deployed with the Terraform script.

1. Resource Group
1. Storage Account
1. Cognitive Services Form Recognizer
1. Cognitive Services Text Analytics
1. Cosmos DB
1. App Service: Plan
1. App Service: Website
1. App Service: API
1. Application Insights

## Code Setup

1. Open Git Bash or WSL
1. Clone Repo
   `git clone https://github.com/jongio/azsdkdemo`
1. Azure CLI Login
   `az login`
1. Create Azure Resources with Terraform
   1. CD to `iac/terraform`
   1. Open `variables.tf` and change the `basename` variable from `azsdkdemo1` to something that will be globally unique.  It will be used as part of Azure resource names, so keep it short, lowercase, and no special characters.
   1. Terraform init: `terraform init`
   1. Terraform plan: `terraform plan --out tf.plan`
   1. Terraform apply: `terraform apply tf.plan`
1. Update `.env` file
   1. Rename `.env.tmp` to `.env`
   1. Copy and paste the terraform output values to the .env file

## Run Application

1. CD to application root
1. `Run docker-compose up --build`
1. Navigate to http://localhost:1080
1. Add an Image
   1. Enter url into text box and click "Submit"
   1. Or click "Add Random Meme"
   1. The image will be added to the grid. Wait for the service to pick it up. You will eventually see the text and the image border color will change indicating the image text sentiment.