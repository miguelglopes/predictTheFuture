#TODO DOCUMENTATION

import pika
import LogMessages
import os
import jsend

_amqpUrl=os.environ['RABBITMQ_URL']
_exchange=os.environ['RABBITMQ_EXCHANGE']


class MRabbit():

    def __init__(self, queue):
        self._queue=queue
        self._parameters=pika.URLParameters(_amqpUrl)
        self._connection = pika.BlockingConnection(self._parameters)
        self._channel = self._connection.channel()
        LogMessages.info("Successfully connected to RabbitMQ.")

    def _publish(self, routingKey, data, properties, basic_deliver, type, ack=True):

        #message properties
        newProps = pika.spec.BasicProperties(message_id=properties.message_id)

        #jsend 
        if type == jsend.successStr:
            body = jsend.success(data)
        elif type == jsend.failStr:
            body = jsend.fail(data)
        elif type == jsend.errorStr:
            body = jsend.error(data)

        #publish
        self._channel.basic_publish(exchange=_exchange, routing_key=routingKey, body = body, properties = newProps)
        LogMessages.debug("Produced message {} with routingKey {}.".format(properties.message_id, routingKey))

        #acknowledge
        if ack:
            self.ackMessage(basic_deliver, properties)

    def publishSuccess(self, routingKey, message, properties, basic_deliver, ack=True):
        self._publish(routingKey, message, properties, basic_deliver, jsend.successStr, ack)
        LogMessages.publishResponse(basic_deliver.routing_key, properties)

    def publishFail(self, routingKey, errorMessage, properties, basic_deliver, ack=True):
        self._publish(routingKey, errorMessage, properties, basic_deliver, jsend.failStr, ack)
        LogMessages.processedUnsuccessfully(basic_deliver.routing_key, properties)

    def consume(self, callbackFunction):
        self._channel.basic_consume(queue=self._queue, auto_ack=False, on_message_callback=callbackFunction)
        LogMessages.info("Consuming messages on queue {}".format(self._queue))
        self._channel.start_consuming()

    def nackMessage(self, basic_deliver, properties):
        self._channel.basic_nack(basic_deliver.delivery_tag, requeue=False)
        LogMessages.invalidMessage(basic_deliver.routing_key, properties)

    def ackMessage(self, basic_deliver, properties):
        self._channel.basic_ack(basic_deliver.delivery_tag)
        LogMessages.processedSuccessfully(basic_deliver.routing_key, properties)

