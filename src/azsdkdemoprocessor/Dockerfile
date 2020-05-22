FROM java:8-jdk-alpine
COPY ./target/processor-1.0-SNAPSHOT.jar /usr/app/
WORKDIR /usr/app
ENTRYPOINT ["java", "-jar", "processor-1.0-SNAPSHOT.jar"]