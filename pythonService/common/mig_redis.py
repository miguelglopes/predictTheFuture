#TODO DOCUMENTATION
#TODO LOGGING!

import redis
import time

_lockModel='lockLatestModel'
_model='latestModel'

class MigRedis(redis.Redis):

    def __init__(self):
        super().__init__(host='localhost', port=6379, db=0) #TODO meter nas env vars

    def getLatestModel(self):
        return self.get(_model)

    def setLatestModel(self, model):
        return self.set(_model, model)

    def lockLatestModel(self, timeout=10):
        endLock = float(time.time() + timeout)
        self.set(_lockModel, endLock)
        return endLock

    def unlockLatestModel(self, previousEndLock):
        if self.exists(_lockModel):
            newEndLock=float(self.get(_lockModel)) #TODO IF THIS GIVES ERROR SOMETHING VERY STRANGE HAPPENED
            if newEndLock==previousEndLock:
                #it was my lock, delete it
                self.delete(_lockModel)
                return True
            elif newEndLock<previousEndLock:
                raise #SOMETHING VERY STRANGE HAPPENED
            elif newEndLock>previousEndLock:
                #someone else has the lock
                return True
        else:
            #autoexpired
            return False

    #returns false or time when lock ends
    def isLatestModelLocked(self):
        if self.exists(_lockModel):
            endLock=float(self.get(_lockModel))
            if endLock <= time.time():
                #lock ended, not locked
                return False
            else:
                return True
        else:
            return False