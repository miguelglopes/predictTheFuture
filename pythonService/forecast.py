#TODO DOCUMENTATION

from GeneralService import GeneralService
import arimaModel
import json
import pickle
import time
import LogMessages

responseRK="response.forecast" 
requestQueue="requestQueueForecast" #TODO FROM ENV VARS

class ForecastService(GeneralService):

    def __init__(self):
        super().__init__(requestQueue, responseRK)
    
    def onMessage(self, channel, basic_deliver, properties, body):
        
        LogMessages.receivedMessage(basic_deliver.routing_key, properties)

        #check for invalid requests. These should not be aknowledge. They are not requed        
        try:
            numSteps = json.loads(body)["num_steps"] #TODO FROM ENV VARS?
            assert(properties)
            assert(properties.message_id)
        except:
            self.rabbit.nackMessage(basic_deliver, properties)
            return

        try:
            while self.redis.isLatestModelLocked():
                time.sleep(2) #sleep 5s

            model = pickle.loads(self.redis.getLatestModel())
            forecast=arimaModel.forecast(None, model, numSteps)
            serializedForecast = json.dumps(forecast.tolist())
            self.rabbit.publish(responseRK, serializedForecast, properties, basic_deliver, ack=True)
        except:
            self.rabbit.publishError(responseRK, "unexpected error", properties, basic_deliver, ack=True) #TODO IMPROVE ERROR PUBLISH

def main():
    ForecastService().run()

if __name__ == '__main__':
    main()