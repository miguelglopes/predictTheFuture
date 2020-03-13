#TODO DOCUMENTATION

class ModelNotFoundError(Exception):
    def __init__(self):
        self.message="Fitted Model not found on backend. Request fit_model first."

class ModelForecastError(Exception):
    def __init__(self):
        self.message="Unable to forecast data."

class ModelFitError(Exception):
    def __init__(self):
        self.message="Unable to fit model."

class LockExpiredError(Exception):
    def __init__(self):
        self.message="Model took too long to be created. Creation failed."

class UnableToSaveModel(Exception):
    def __init__(self):
        self.message="Model was created but not persisted. Request failed."