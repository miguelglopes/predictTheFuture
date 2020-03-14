#TODO DOCUMENTATION

from mRabbit import MRabbit
from mRedis import MRedis
import time
from abc import ABC, abstractmethod

class GeneralService(ABC):

    def __init__(self, requestQueue, responseRK):

        self.requestQueue=requestQueue
        self.responseRK=responseRK
        self.rabbit=MRabbit(self.requestQueue)
        self.redis=MRedis()

    @abstractmethod
    def onMessage(self,channel, basic_deliver, properties, body):
        pass

    def run(self):
        self.rabbit.consume(self.onMessage)