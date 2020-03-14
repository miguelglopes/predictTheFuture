#TODO DOCUMENTATION

from generalService import GeneralService #local
import arimaModel #local
import logMessages #local
import exceptions #local
import json
import pickle
import time
import os

responseRK=os.environ['RABBITMQ_RK_RESPONSEFIT']
requestQueue=os.environ['RABBITMQ_QUEUE_REQUESTFIT']


class FitService(GeneralService):

    def __init__(self):
        super().__init__(requestQueue, responseRK)
    
    def onMessage(self, channel, basic_deliver, properties, body):

        logMessages.receivedMessage(basic_deliver.routing_key, properties)

        #check for invalid requests. These should not be aknowledge since they are not our responsability. They are not requed.        
        try:
            X = json.loads(body)["data"]
            assert(properties)
            assert(properties.message_id)
        except:
            self.rabbit.nackMessage(basic_deliver, properties)
            return

        try:
            response = dict()

            #lock model while creating new one
            lock = self.redis.lockLatestModel()

            #create model
            try:
                model=arimaModel.fit_model(X)
                serializedModel = pickle.dumps(model)
                response["message"] = "Successfully fitted and persisted model."
            except:
                raise exceptions.ModelFitError()

            #TODO order on these? this should be one single transaction on the database. Use pipe with Redis?
            self.redis.unlockLatestModel(lock) #raises LockExpiredError
            self.redis.setLatestModel(serializedModel) #raises UnableToSaveModel

            self.rabbit.publishSuccess(responseRK, response, properties, basic_deliver, ack=True)

        except exceptions.LockExpiredError as e:
            self.rabbit.publishFail(responseRK, e.message, properties, basic_deliver, ack=True)
        except exceptions.ModelFitError as e:
            self.rabbit.publishFail(responseRK, e.message, properties, basic_deliver, ack=True)
        except exceptions.UnableToSaveModel as e:
            self.rabbit.publishFail(responseRK, e.message, properties, basic_deliver, ack=True)


def main():
    FitService().run()

if __name__ == '__main__':
    main()