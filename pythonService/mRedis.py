#TODO DOCUMENTATION

import exceptions #local
import redis
import time
import os
import logMessages

_lockModel='lockLatestModel'
_model='latestModel'
_host=os.environ["REDIS_HOST"]
_port=os.environ["REDIS_PORT"]
_timeout=os.environ["REDIS_LOCK_TIMEOUT"]


class MRedis(redis.Redis):

    def __init__(self):
        super().__init__(host=_host, port=_port, db=0)
        logMessages.info("Successfully connected to Redis.")

    def getLatestModel(self):
        model = self.get(_model)
        if model is None:
            raise exceptions.ModelNotFoundError()
        return model

    def setLatestModel(self, model):
        try:
            return self.set(_model, model)
        except:
            raise exceptions.UnableToSaveModel()

    def lockLatestModel(self, timeout=_timeout):
        endLock = float(time.time() + int(timeout))
        self.set(_lockModel, endLock)
        return endLock

    def unlockLatestModel(self, previousEndLock):
        if self.exists(_lockModel):
            newEndLock=float(self.get(_lockModel)) #IF THIS GIVES ERROR SOMETHING VERY STRANGE HAPPENED
            if newEndLock==previousEndLock:
                #it was my lock, delete it
                self.delete(_lockModel)
                return True
            elif newEndLock<previousEndLock:
                raise #SOMETHING VERY STRANGE HAPPENED
            elif newEndLock>previousEndLock:
                #someone else has the lock. It's ok cause it remains locked.
                return True
        else:
            raise exceptions.LockExpiredError()

    #returns false or time when lock ends
    def isLatestModelLocked(self):
        if self.exists(_lockModel):
            endLock=float(self.get(_lockModel))
            if endLock <= time.time():
                #lock ended, not locked
                return False
            else:
                logMessages.debug("Model locked.")
                return True
        else:
            return False
