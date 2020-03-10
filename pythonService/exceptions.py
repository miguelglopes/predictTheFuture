#TODO DOCUMENTATION

class ModelNotFoundError(Exception):
    def __init__(self):
        super().__init__("Fitted Model not found on backend. Request fit_model first.")

class ModelForecastError(Exception):
    def __init__(self):
        super().__init__("Unable to forecast data.")

class ModelFitError(Exception):
    def __init__(self):
        super().__init__("Unable to fit model.")

class LockExpiredError(Exception):
    def __init__(self):
        super().__init__("Model took too long to be created. Creation failed.")

class UnableToSaveModel(Exception):
    def __init__(self):
        super().__init__("Model was created but not persisted. Request failed.")