# DOCUMENTATION

import logging
import logging.config
import json
import os
import sys

default = {
    'version': 1,
    'formatters': { 
        'standard': {
            'format': '%(asctime)s [%(process)s] [%(levelname)s] %(message)s',
            'datefmt': '%Y-%m-%d - %H:%M:%S' },
    },
    'handlers': {
        'console':  {'class': 'logging.StreamHandler', 
                     'formatter': "standard", 
                     'level': os.environ['PYTHON_LOGLEVEL'], 
                     'stream': sys.stdout},
        'file':     {'class': 'logging.FileHandler', 
                     'formatter': "standard", 
                     'level': os.environ['PYTHON_LOGLEVEL'], 
                     'filename': os.environ['PYTHON_LOGFILE']} 
    },
    'loggers': { 
        __name__:   {'level': 'INFO', 
                     'handlers': ['console', 'file'], 
                     'propagate': False },
    }
}

logging.config.dictConfig(default)
log = logging.getLogger(__name__)


def invalidMessage(rk, properties):
    log.warning("Invalid message received {}. Message will be nacked and sent to dead letter.".format(rk))
    log.debug("{}".format(json.dumps(properties.__dict__)))

def processedSuccessfully(rk, properties):
    log.info("Successfully processed {}".format(rk))
    log.debug("{}".format(json.dumps(properties.__dict__)))

def processedUnsuccessfully(rk, properties):
    log.warn("Message processed successfully but operation failed. {}".format(rk))
    log.debug("{}".format(json.dumps(properties.__dict__)))

def receivedMessage(rk, properties):
    log.debug("Processing message {}".format(rk))
    log.debug("{}".format(json.dumps(properties.__dict__)))

def publishResponse(rk, properties):
    log.info("Published response {}".format(rk))
    log.debug("{}".format(json.dumps(properties.__dict__)))

def debug(message):
    log.debug(message)

def info(message):
    log.info(message)
