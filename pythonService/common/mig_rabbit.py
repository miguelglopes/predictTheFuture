#TODO DOCUMENTATION

import pika
import LogMessages

#TODO
#_amqpUrl=os.environ['AMQPURL']
#_exchange=os.environ['EXCHANGE']

_amqpUrl="amqp://rabbitmq:rabbitmq@localhost:5672/app" #TODO ENV VARS
_exchange="predictFuture" #TODO ENV VARS


_parameters=pika.URLParameters(_amqpUrl)
_connection = pika.BlockingConnection(_parameters)
_channel = _connection.channel()

class MigRabbit():

    def __init__(self, queue):
        self.queue=queue

    def publish(self, routingKey, body, properties, basic_deliver, ack=True):
        newProps = pika.spec.BasicProperties(message_id=properties.messageId)
        _channel.basic_publish(exchange=_exchange, routing_key=routingKey, body=body, properties = newProps)
        LogMessages.debug("Produced message {} with routingKey {}.".format(properties.messageId, routingKey))
        if ack:
            _channel.ackMessage(basic_deliver, properties)

    def publishError(self, routingKey, errorMessage, properties, basic_deliver, ack=True):
        publish(routingKey, errorMessage, properties, basic_deliver, ack)
        LogMessages.processedUnsuccessfully(basic_deliver.routing_key, properties)

    def consume(self, callbackFunction):
        _channel.basic_consume(queue=self.queue, auto_ack=False, on_message_callback=callbackFunction)
        LogMessages.debug("Consuming messages on queue {}".format(self.queue))
        _channel.start_consuming()

    def nackMessage(self, basic_deliver, properties):
        _channel.basic_nack(basic_deliver.delivery_tag, requeue=False)
        LogMessages.invalidMessage(basic_deliver.routing_key, properties)

    def ackMessage(self, basic_deliver, properties):
        _channel.basic_ack(basic_deliver.delivery_tag)
        LogMessages.processedSuccessfully(basic_deliver.routing_key, properties)

