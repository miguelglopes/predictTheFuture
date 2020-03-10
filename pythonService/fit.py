from GeneralService import GeneralService
import arimaModel
import json
import pickle
import time
import LogMessages

responseRK="response.fit"
requestQueue="requestQueueFit"#TODO FROM ENV VARS

class FitService(GeneralService):

    def __init__(self):
        super().__init__(requestQueue, responseRK)
    
    def onMessage(self, channel, basic_deliver, properties, body):

        LogMessages.receivedMessage(basic_deliver.routing_key, properties)

        #check for invalid requests. These should not be aknowledge. They are not requed        
        try:
            X = json.loads(body)["data"] #TODO FROM ENV VARS?
            assert(properties)
            assert(properties.message_id)
        except:
            self.rabbit.nackMessage(basic_deliver, properties)
            return

        try:
            lock = self.redis.lockLatestModel()
            model=arimaModel.fit_model(X)
            serializedModel = pickle.dumps(model)
            self.redis.setLatestModel(serializedModel)
            self.redis.unlockLatestModel(lock) #TODO what to do if lock already expired?
            self.rabbit.publish(responseRK, serializedModel, properties, basic_deliver, ack=True)
        except:
            self.rabbit.publishError(responseRK, "unexpected error", properties, basic_deliver, ack=True) #TODO IMPROVE ERROR PUBLISH


def main():
    FitService().run()

if __name__ == '__main__':
    main()