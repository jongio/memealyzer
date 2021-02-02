import requests
import os
import time
import json

from azure.identity import (
    ManagedIdentityCredential,
    AzureCliCredential
)
from azure.storage.queue import QueueServiceClient
from azure.ai.formrecognizer import FormRecognizerClient
from azure.ai.textanalytics import TextAnalyticsClient
from azure.cosmos import CosmosClient
from azure.keyvault.secrets import SecretClient

from smart_getenv import getenv
from dotmap import DotMap
from dotenv import load_dotenv, find_dotenv

load_dotenv(find_dotenv())

sleep = getenv("AZURE_STORAGE_QUEUE_RECEIVE_SLEEP", type=bool, default=1)

credential = ChainedTokenCredential(
    ManagedIdentityCredential(), AzureCliCredential()
)

queue_service_client = QueueServiceClient(
    account_url=getenv("AZURE_STORAGE_QUEUE_ENDPOINT"), credential=credential
)

queue_client = queue_service_client.get_queue_client(
    queue=getenv("AZURE_MESSAGES_QUEUE_NAME", default="messages")
)

client_sync_queue_client = queue_service_client.get_queue_client(
    queue=getenv("AZURE_CLIENT_SYNC_QUEUE_NAME", default="sync")
)

fr_client = FormRecognizerClient(
    endpoint=getenv("AZURE_FORM_RECOGNIZER_ENDPOINT"), credential=credential
)

ta_client = TextAnalyticsClient(
    endpoint=getenv("AZURE_TEXT_ANALYTICS_ENDPOINT"), credential=credential
)

secret_client = SecretClient(
    vault_url=getenv("AZURE_KEYVAULT_ENDPOINT"), credential=credential
)

secret = secret_client.get_secret(getenv("AZURE_COSMOS_KEY_SECRET_NAME"))

cosmos_client = CosmosClient(
    url=getenv("AZURE_COSMOS_ENDPOINT"), credential=secret.value
)

cosmos_database_client = cosmos_client.get_database_client(
    getenv("AZURE_COSMOS_DB", default="memealyzer")
)

cosmos_container_client = cosmos_database_client.get_container_client(
    getenv("AZURE_COSMOS_COLLECTION", default="images")
)

while True:

    print("Receiving messages from queue")
    
    batches = queue_client.receive_messages(
        messages_per_page=getenv("AZURE_STORAGE_QUEUE_MSG_COUNT", default="10")
    )

    for batch in batches.by_page():
        for message in batch:
            message_json = DotMap(json.loads(message.content))

            print("Message received: " + message_json.id)

            print("Extracting text from image")
            fr_poller = fr_client.begin_recognize_content_from_url(message_json.url)
            fr_result = fr_poller.result()

            message_json.text = " ".join([line.text for page in fr_result for line in page.lines])

            print("Image text: " + message_json.text)

            if message_json.text:
                print("Analyzing text sentiment")
                ta_response = ta_client.analyze_sentiment([message_json.text])

                for doc in ta_response:
                    print("Image text sentiment: " + doc.sentiment)
                    message_json.sentiment = doc.sentiment
            else:
                print("No text found in image");

            print("Saving document")
            cosmos_container_client.upsert_item(message_json.toDict())

            
            print("Deleting message from queue")    
            queue_client.delete_message(message)

            print("Send message to client sync queue")    
            client_sync_queue_client.send_message(message)

            print(message_json)

    time.sleep(sleep)

