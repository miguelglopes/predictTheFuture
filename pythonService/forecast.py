#TODO DOCUMENTATION

from generalService import GeneralService #local
import arimaModel #local
import exceptions #local
import logMessages #local
import json
import pickle
import time
import os

responseRK=os.environ['RABBITMQ_RK_RESPONSEFORECAST']
requestQueue=os.environ['RABBITMQ_QUEUE_REQUESTFORECAST']

class ForecastService(GeneralService):

    def __init__(self):
        super().__init__(requestQueue, responseRK)
    
    def onMessage(self, channel, basic_deliver, properties, body):
        
        logMessages.receivedMessage(basic_deliver.routing_key, properties)

        #check for invalid requests. These should not be aknowledge since they are not our responsability. They are not requed.        
        try:
            numSteps = json.loads(body)["num_steps"]
            assert(properties)
            assert(properties.message_id)
        except:
            self.rabbit.nackMessage(basic_deliver, properties)
            return

        try:

            response = dict()

            #check lock and wait
            while self.redis.isLatestModelLocked():
                time.sleep(2)

            #get latest model
            latestModel = self.redis.getLatestModel() # raises ModelNotFoundError
            model = pickle.loads(latestModel)

            #forecast
            try:
                forecast=arimaModel.forecast(None, model, numSteps)
                response["forecast"]=forecast.tolist()
            except:
                raise exceptions.ModelForecastError()
            
            #publish success
            self.rabbit.publishSuccess(responseRK, response, properties, basic_deliver, ack=True)

        except exceptions.ModelNotFoundError as e:
            self.rabbit.publishFail(responseRK, e.message, properties, basic_deliver, ack=True)
        except exceptions.ModelForecastError as e:
            self.rabbit.publishFail(responseRK, e.message, properties, basic_deliver, ack=True)



def main():
    ForecastService().run()

if __name__ == '__main__':
    main()