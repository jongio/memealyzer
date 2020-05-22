FROM maven AS build-env
WORKDIR /usr/app/

COPY src/azsdkdemoprocessor/ ./
RUN mvn package -Dmaven.test.skip=true assembly:single

# Build runtime image
FROM java:8-jdk-alpine
WORKDIR /usr/app/
COPY --from=build-env /usr/app/ .
ENTRYPOINT ["java", "-jar", "target/processor-1.0-SNAPSHOT.jar"]
