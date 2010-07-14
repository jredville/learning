// Copyright (c) Microsoft Corporation.  All rights reserved.
// This code is licensed by Microsoft Corporation under the terms
// of the MICROSOFT REACTIVE EXTENSIONS FOR JAVASCRIPT AND .NET LIBRARIES License.
// See http://go.microsoft.com/fwlink/?LinkId=186234.

(function()
{
    var root;
    if (typeof ProvideCustomRxRootObject == "undefined")
    {
        root = this.Rx;
    }
    else
    {
        root = ProvideCustomRxRootObject();
    }
    var observable = root.Observable;

    var fromMooToolsEvent = observable.FromMooToolsEvent = function(mooToolsObject, eventType)
    {
        return observable.Create(function(observer)
        {
            var handler = function(eventObject)
            {
                observer.OnNext(eventObject);
            };
            var handle = mooToolsObject.addEvent(eventType, handler);
            return function()
            {
                mooToolsObject.removeEvent(eventType, handler);
            };
        });
    };

    var mooToolsRequest = observable.MooToolsRequest = function(options)
    {
        var newOptions = {};
        for (var k in options)
        {
            newOptions[k] = options[k];
        }

        var subject = new root.AsyncSubject();
        newOptions.onSuccess = function(responseText, responseXML)
        {
            subject.OnNext({ responseText: responseText, responseXML: responseXML });
            subject.OnCompleted();
        };

        newOptions.onFailure = function(xhr)
        {
            subject.OnError({ kind: "failure", xhr: xhr });
        };


        newOptions.onException = function(headerName, value)
        {
            subject.OnError({ kind: "exception", headerName: headerName, value: value });
        };

        var request = new Request(newOptions);
        request.send();
        return subject;
    };

    observable.MooToolsJSONRequest = function(options)
    {
        return observable.MooToolsRequest(options).Select(function(result)
        {    
            return JSON.decode(result.responseText, options.secure);
        });
    }
})();