#TODO DOCUMENTATION

from common.mig_rabbit import MigRabbit
from common.mig_redis import MigRedis
import time
from abc import ABC, abstractmethod

class GeneralService(ABC):

    def __init__(self, requestQueue, responseRK):

        time.sleep(10)# Delays to make sure rabbit and redis is up

        self.requestQueue=requestQueue
        self.responseRK=responseRK
        self.rabbit=MigRabbit(self.requestQueue)
        self.redis=MigRedis()

    @abstractmethod
    def onMessage(self,channel, basic_deliver, properties, body):
        pass

    def run(self):
        self.rabbit.consume(self.onMessage)