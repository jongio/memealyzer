package demo.sdk.az;

import ch.qos.logback.classic.Logger;
import com.azure.identity.*;
import com.azure.storage.queue.*;
import com.microsoft.applicationinsights.logback.ApplicationInsightsAppender;
import io.github.cdimascio.dotenv.*;
import java.util.*;
import org.slf4j.LoggerFactory;

public class App {
  static final Logger logger = (Logger) LoggerFactory.getLogger("root");
  static final Dotenv env = Dotenv
    .configure()
    .ignoreIfMissing()
    .systemProperties()
    .directory("../../")
    .load();

  public static void main(String[] args) {
    try {
      App app = new App();
      app.setInstrumentationKey();
      app.process();
    } catch (Exception e) {
      logger.error("Program Execution Error", e);
    }
  }

  private void setInstrumentationKey() {
    ApplicationInsightsAppender appender = (ApplicationInsightsAppender) logger.getAppender(
      "aiAppender"
    );
    appender.setInstrumentationKey(env.get("APPINSIGHTS_INSTRUMENTATIONKEY"));
  }

  private synchronized void process() {
    DefaultAzureCredential cred = new DefaultAzureCredentialBuilder().build();

    QueueAsyncClient queueClient = new QueueClientBuilder()
      .endpoint(env.get("AZURE_STORAGE_QUEUE_URI"))
      .queueName(env.get("AZURE_STORAGE_QUEUE_NAME"))
      .credential(cred)
      .buildAsyncClient();

    queueClient
      .createWithResponse(Collections.singletonMap("queue", "metadataMap"))
      .subscribe(
        response -> logger.info("Queue Created: {}", response.getStatusCode()),
        error -> System.err.print(error.toString())
      );

    while (true) {
      logger.trace("Receive Msgs Started");
      queueClient
        .receiveMessages(
          Integer.parseInt(env.get("AZURE_STORAGE_QUEUE_MSG_COUNT", "10"))
        )
        .subscribe(
          message -> {
            String msgId = message.getMessageId();

            logger.info(
              "Msg Received: {}, {}",
              msgId,
              message.getMessageText()
            );

            queueClient
              .deleteMessageWithResponse(msgId, message.getPopReceipt())
              .subscribe(
                response ->
                  logger.info(
                    "Deleting Msg: {}, {}",
                    msgId,
                    response.getStatusCode()
                  ),
                deleteError -> System.err.print(deleteError.toString()),
                () -> logger.info("Deleted Msg: {}", msgId)
              );
          },
          error -> logger.error(error.toString()),
          () -> logger.trace("Receive Msgs Complete")
        );

      try {
        this.wait(5000);
      } catch (InterruptedException e) {
        logger.error("Error while waiting", e);
      }
    }
  }
}
