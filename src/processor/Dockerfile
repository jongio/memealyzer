FROM maven:3.5.3-jdk-8-alpine AS build-env
WORKDIR /usr/app/
COPY pom.xml .
RUN mvn dependency:go-offline

COPY src/ /usr/app/src
RUN mvn package -Dmaven.test.skip=true assembly:single

# Build runtime image
FROM openkbs/jre-mvn-py3

RUN pip3 install --user azure-cli 
ENV PATH /home/developer/.local/bin/:$PATH

WORKDIR /usr/app/
COPY --from=build-env /usr/app/ .
ENTRYPOINT ["java", "-jar", "target/processor-1.0-SNAPSHOT.jar"]