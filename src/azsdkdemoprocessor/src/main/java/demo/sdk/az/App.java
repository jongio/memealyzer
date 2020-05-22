package demo.sdk.az;

import ch.qos.logback.classic.Logger;
import com.azure.identity.DefaultAzureCredential;
import com.azure.identity.DefaultAzureCredentialBuilder;
import com.azure.storage.queue.QueueAsyncClient;
import com.azure.storage.queue.QueueClientBuilder;
import com.microsoft.applicationinsights.logback.ApplicationInsightsAppender;
import io.github.cdimascio.dotenv.Dotenv;
import java.io.IOException;
import java.util.Collections;
import org.slf4j.LoggerFactory;

public class App {
  private static final Logger LOGGER = (Logger) LoggerFactory.getLogger("root");
  private static final Dotenv ENV = Dotenv
    .configure()
    .ignoreIfMissing()
    .systemProperties()
    .directory("../../")
    .load();

  public static void main(String[] args) {
    try {
      App app = new App();
      app.setInstrumentationKey();
      app.start();
      Thread.currentThread().join();
    } catch (Exception e) {
      LOGGER.error("Program Execution Error", e);
    }
  }

  private void setInstrumentationKey() {
    ApplicationInsightsAppender appender = (ApplicationInsightsAppender) LOGGER.getAppender(
      "aiAppender"
    );
    appender.setInstrumentationKey(ENV.get("APPINSIGHTS_INSTRUMENTATIONKEY"));
  }

  private void start() {
    DefaultAzureCredential credential = new DefaultAzureCredentialBuilder()
    .build();

    QueueAsyncClient queueAsyncClient = new QueueClientBuilder()
      .endpoint(ENV.get("AZURE_STORAGE_QUEUE_URI"))
      .queueName(ENV.get("AZURE_STORAGE_QUEUE_NAME"))
      .credential(credential)
      .buildAsyncClient();

    queueAsyncClient
      .createWithResponse(Collections.singletonMap("queue", "metadataMap"))
      .doOnSuccess(
        response -> LOGGER.info("Queue created: {}", response.getStatusCode())
      )
      .flatMapMany(
        response ->
          queueAsyncClient
            .receiveMessages(
              Integer.parseInt(ENV.get("AZURE_STORAGE_QUEUE_MSG_COUNT", "10"))
            )
            .repeat()
      )
      .flatMap(
        message -> {
          String msgId = message.getMessageId();
          LOGGER.info(
            "Message Received: {}, {}",
            msgId,
            message.getMessageText()
          );
          return queueAsyncClient.deleteMessageWithResponse(
            msgId,
            message.getPopReceipt()
          );
        }
      )
      .log()
      .subscribe(
        response -> {
          LOGGER.info("Deleted message: {}", response.getStatusCode());
        },
        error -> LOGGER.error("Error: {}", error)
      );
  }
}
