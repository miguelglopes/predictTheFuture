FROM rabbitmq:3.8

WORKDIR /

#enable management plugin
RUN rabbitmq-plugins enable rabbitmq_management

#copy configuration
ADD rabbitmq.conf /etc/rabbitmq/
ADD definitions.json /etc/rabbitmq/

#adjust permissions
RUN chown rabbitmq:rabbitmq /etc/rabbitmq/rabbitmq.conf /etc/rabbitmq/definitions.json

ENTRYPOINT ["rabbitmq-server"]
