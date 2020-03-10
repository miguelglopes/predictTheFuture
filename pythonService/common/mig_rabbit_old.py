import pika


class Messenger():
    def __init__(self, amqpUrl):
        self._amqpUrl = amqpUrl
        self._connection = None
        self._channel = None

class Consumer(Messenger):

    def __init__(self, amqpUrl, queueDic):
        super().__init__(amqpUrl)
        self.queueDic = queueDic
        
    def _connect(self):
        print('Connecting to {}'.format(self._amqpUrl))
        return pika.SelectConnection(
            parameters=pika.URLParameters(self._amqpUrl),
            on_open_callback=self._onConnectionOpen)

    def _onConnectionOpen(self, _unused_connection):
        print('Connection opened')
        self._connection.channel(on_open_callback=self._onChannelOpen)
        
    def _onChannelOpen(self, channel):
        print('Channel opened')
        self._channel = channel
        for key in self.queueDic:
            self._channel.basic_consume(key, self.queueDic[key])
      
    def run(self):
        self._connection = self._connect()
        self._connection.ioloop.start()


class Producer(Messenger):

    def __init__(self, amqpUrl, exchange, routingKey):
        super().__init__(amqpUrl)
        self.exchange = exchange
        self.routingKey = routingKey
        
    def _connect(self):
        print('Connecting to {}'.format(self._amqpUrl))
        self._connection = pika.BlockingConnection(parameters=pika.URLParameters(self._amqpUrl))
        self._channel = self._connection.channel()
     
    def publish(self, message, properties):
        if self._channel is None:
            self._connect()
        props = pika.spec.BasicProperties()
        props.headers = properties.headers
        self._channel.basic_publish(self.exchange, self.routingKey, message, props)

    def publishError(self, message, properties):
        if self._channel is None:
            self._connect()
        props = pika.spec.BasicProperties()
        props.headers = properties.headers
        self._channel.basic_publish(self.exchange, self.routingKey, "error", props)