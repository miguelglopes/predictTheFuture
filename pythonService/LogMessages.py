#TODO DOCUMENTATION

import logging
import json

#TODO AQUI
logging.basicConfig(level=logging.INFO)
#logging.basicConfig(format='%(asctime)s - %(message)s', level=logging.INFO)


def invalidMessage(rk, properties):
    logging.warning("Invalid message received. Message will be nacked and sent to dead letter. {}".format(json.dumps(properties.__dict__)))

def processedSuccessfully(rk, properties):
    logging.info("Successfully processed. {}".format(json.dumps(properties.__dict__)))

def processedUnsuccessfully(rk, properties):
    logging.info("Unsuccessfully processed. {}".format(json.dumps(properties.__dict__)))

def receivedMessage(rk, properties):
    logging.debug("Processing message. {}".format(json.dumps(properties.__dict__)))


def debug(message):
    logging.debug(message)