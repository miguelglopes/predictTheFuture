#implementation of jsend - https://github.com/omniti-labs/jsend

import json

errorStr="error"
failStr="fail"
successStr="success"

def success(data={}):
    if not isinstance(data, dict):
        raise ValueError('data must be the dict type')
    return json.dumps({'status': 'success', 'data': data})


def fail(message=""):
    result = {}
    result["message"]=message
    return json.dumps({'status': 'fail', 'data': result})


def error(message='', code=None, data=None):
    ret = {}
    if (not isinstance(message, str)) and (not isinstance(message, bytes)):
        raise ValueError('message must be the str type')
    if code:
        if not isinstance(code, int):
            raise ValueError('code must be the int type')
        ret['code'] = code
    if data:
        if not isinstance(data, dict):
            raise ValueError('data must be the dict type')
        ret['data'] = data

    ret['status'] = errorStr
    ret['message'] = message
    return json.dumps(ret)